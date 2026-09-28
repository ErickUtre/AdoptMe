const request = require('supertest');
const { levantarEntorno, XALAPA } = require('./entornoPruebas');

describe('gestión de usuarios y acceso', () => {
  let entorno;
  let sesion;

  beforeAll(async () => {
    entorno = await levantarEntorno();
  });

  afterAll(() => entorno.cerrar());

  const registro = (correo, cambios = {}) => ({
    Nombre: 'Prueba',
    Telefono: '2281234567',
    Ubicacion: XALAPA,
    Acceso: { Correo: correo, Contrasena: 'Segura123' },
    ...cambios
  });

  test('registra un usuario y rechaza correos duplicados', async () => {
    await request(entorno.app).post('/api/usuarios').send(registro('prueba@correo.com')).expect(201);
    await request(entorno.app).post('/api/usuarios').send(registro('PRUEBA@correo.com')).expect(409);
  });

  test('rechaza coordenadas nulas o fuera de rango', async () => {
    await request(entorno.app).post('/api/usuarios')
      .send(registro('otro@correo.com', { Ubicacion: { Ciudad: 'Xalapa' } })).expect(400);
    await request(entorno.app).post('/api/usuarios')
      .send(registro('otro@correo.com', { Ubicacion: { Latitud: 190, Longitud: -196 } })).expect(400);
  });

  test('inicia sesión y rechaza credenciales incorrectas', async () => {
    const { body } = await request(entorno.app).post('/api/acceso/iniciar-sesion')
      .send({ Correo: 'prueba@correo.com', Contrasena: 'Segura123' }).expect(200);
    sesion = body;
    expect(body.esAdmin).toBe(false);
    expect(body.usuario).toMatchObject({ Nombre: 'Prueba', Ubicacion: { Ciudad: 'Xalapa' }, Acceso: { Correo: 'prueba@correo.com' } });
    expect(JSON.stringify(body)).not.toContain('ContrasenaHash');

    await request(entorno.app).post('/api/acceso/iniciar-sesion')
      .send({ Correo: 'prueba@correo.com', Contrasena: 'Incorrecta1' }).expect(401);
    await request(entorno.app).post('/api/acceso/iniciar-sesion')
      .send({ Correo: 'inexistente@correo.com', Contrasena: 'Segura123' }).expect(401);
  });

  test('el administrador inicial puede iniciar sesión', async () => {
    const { body } = await request(entorno.app).post('/api/acceso/iniciar-sesion')
      .send({ Correo: 'admin.pruebas@adoptme.com', Contrasena: 'Administrador123' }).expect(200);
    expect(body.esAdmin).toBe(true);
  });

  test('actualiza perfil, ubicación y correo del usuario autenticado', async () => {
    const autorizacion = `Bearer ${sesion.token}`;

    const perfil = await request(entorno.app).patch('/api/usuarios/yo').set('Authorization', autorizacion)
      .send({ Nombre: 'Pedro', Telefono: '9876543210' }).expect(200);
    expect(perfil.body).toMatchObject({ Nombre: 'Pedro', Telefono: '9876543210' });

    const ubicacion = await request(entorno.app).put('/api/usuarios/yo/ubicacion').set('Authorization', autorizacion)
      .send({ ...XALAPA, Ciudad: 'Coatepec' }).expect(200);
    expect(ubicacion.body.Ciudad).toBe('Coatepec');

    await request(entorno.app).patch('/api/acceso').set('Authorization', autorizacion)
      .send({ Correo: 'pedro@correo.com' }).expect(204);
    await request(entorno.app).post('/api/acceso/iniciar-sesion')
      .send({ Correo: 'pedro@correo.com', Contrasena: 'Segura123' }).expect(200);
  });

  test('exige un token válido en rutas protegidas', async () => {
    const sinToken = await request(entorno.app).put('/api/usuarios/yo/ubicacion').send(XALAPA).expect(401);
    expect(sinToken.body.error).toBe('Token no proporcionado');
    await request(entorno.app).get('/api/usuarios/yo').set('Authorization', 'Bearer token.invalido.1212').expect(401);
  });
});
