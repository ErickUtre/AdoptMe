const request = require('supertest');
const { crearAplicacion } = require('../../../src/http/aplicacion');
const { crearAutenticacionHttp } = require('../../../src/http/middlewares/autenticacion');
const { crearServicioTokens } = require('../../../src/infraestructura/seguridad/tokens');
const { crearAdopcionRutas } = require('../../../src/modulos/adopciones/adopcion.rutas');
const { crearAdopcionControlador } = require('../../../src/modulos/adopciones/adopcion.controlador');
const { ErrorProhibido } = require('../../../src/compartido/errores');
const { registroSilencioso } = require('../apoyo');

describe('aplicación HTTP', () => {
  const tokens = crearServicioTokens({ secreto: 'secreto', expiracion: '1h' });
  const adopcionServicio = {
    listarParaReporte: jest.fn().mockResolvedValue([{ AdopcionID: 1 }]),
    obtenerDetalle: jest.fn().mockResolvedValue({ AdopcionID: 3 }),
    eliminar: jest.fn().mockRejectedValue(new ErrorProhibido('La adopción no pertenece al usuario')),
    registrar: jest.fn().mockRejectedValue(new Error('fallo inesperado'))
  };
  const app = crearAplicacion({
    origenesPermitidos: '*',
    registro: registroSilencioso,
    modulosHttp: [{
      ruta: '/api/adopciones',
      enrutador: crearAdopcionRutas({
        adopcionControlador: crearAdopcionControlador({ adopcionServicio }),
        autenticacion: crearAutenticacionHttp({ tokens })
      })
    }]
  });
  const encabezado = (rol) => `Bearer ${tokens.emitir({ usuarioId: 1, rol })}`;

  test('responde el estado de salud sin autenticación', async () => {
    await request(app).get('/salud').expect(200, { estado: 'ok' });
  });

  test('exige un token válido', async () => {
    const respuesta = await request(app).get('/api/adopciones/3').expect(401);
    expect(respuesta.body.error).toBe('Token no proporcionado');
  });

  test('entrega el detalle con un token válido', async () => {
    await request(app).get('/api/adopciones/3').set('Authorization', encabezado('Usuario')).expect(200, { AdopcionID: 3 });
  });

  test('restringe los reportes a administradores', async () => {
    await request(app).get('/api/adopciones').set('Authorization', encabezado('Usuario')).expect(403);
    await request(app).get('/api/adopciones?estado=adoptada').set('Authorization', encabezado('Admin')).expect(200);
    expect(adopcionServicio.listarParaReporte).toHaveBeenCalledWith('adoptada');
  });

  test('traduce los errores de dominio y oculta los errores internos', async () => {
    const prohibido = await request(app).delete('/api/adopciones/3').set('Authorization', encabezado('Usuario')).expect(403);
    expect(prohibido.body.error).toBe('La adopción no pertenece al usuario');

    const interno = await request(app).post('/api/adopciones').set('Authorization', encabezado('Usuario')).send({}).expect(500);
    expect(interno.body.error).toBe('Ocurrió un error en el servidor');
  });

  test('valida identificadores y JSON mal formado', async () => {
    await request(app).get('/api/adopciones/abc').set('Authorization', encabezado('Usuario')).expect(400);
    await request(app).post('/api/adopciones').set('Authorization', encabezado('Usuario'))
      .set('Content-Type', 'application/json').send('{mal').expect(400);
  });

  test('responde 404 en rutas inexistentes', async () => {
    await request(app).get('/api/inexistente').expect(404);
  });
});
