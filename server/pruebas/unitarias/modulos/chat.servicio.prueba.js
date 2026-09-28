const { crearChatServicio } = require('../../../src/modulos/chat/chat.servicio');
const { ErrorValidacion, ErrorNoEncontrado } = require('../../../src/compartido/errores');

function crearDependencias({ existeDestinatario = true, mensajes = [] } = {}) {
  const chatRepositorio = {
    crear: jest.fn().mockImplementation(async (datos) => ({ ChatID: 1, ...datos })),
    listarDeUsuario: jest.fn().mockResolvedValue(mensajes),
    listarEntre: jest.fn().mockResolvedValue([])
  };
  const usuarioRepositorio = { existe: jest.fn().mockResolvedValue(existeDestinatario) };
  return { servicio: crearChatServicio({ chatRepositorio, usuarioRepositorio }), chatRepositorio };
}

describe('chatServicio', () => {
  test('envía un mensaje usando al usuario autenticado como remitente', async () => {
    const { servicio } = crearDependencias();
    const mensaje = await servicio.enviar(1, { DestinatarioID: 2, Contenido: '  Hola  ', RemitenteID: 99 });
    expect(mensaje).toMatchObject({ RemitenteID: 1, DestinatarioID: 2, Contenido: 'Hola' });
  });

  test('rechaza mensajes vacíos, a uno mismo o a usuarios inexistentes', async () => {
    await expect(crearDependencias().servicio.enviar(1, { DestinatarioID: 2, Contenido: ' ' })).rejects.toBeInstanceOf(ErrorValidacion);
    await expect(crearDependencias().servicio.enviar(1, { DestinatarioID: 1, Contenido: 'x' })).rejects.toBeInstanceOf(ErrorValidacion);
    await expect(crearDependencias({ existeDestinatario: false }).servicio.enviar(1, { DestinatarioID: 2, Contenido: 'x' }))
      .rejects.toBeInstanceOf(ErrorNoEncontrado);
  });

  test('agrupa las conversaciones por contraparte conservando el último mensaje', async () => {
    const ana = { UsuarioID: 1, Nombre: 'Ana' };
    const luis = { UsuarioID: 2, Nombre: 'Luis' };
    const eva = { UsuarioID: 3, Nombre: 'Eva' };
    const { servicio } = crearDependencias({
      mensajes: [
        { RemitenteID: 2, DestinatarioID: 1, Contenido: 'último de Luis', FechaEnvio: 't3', Remitente: luis, Destinatario: ana },
        { RemitenteID: 1, DestinatarioID: 3, Contenido: 'hola Eva', FechaEnvio: 't2', Remitente: ana, Destinatario: eva },
        { RemitenteID: 1, DestinatarioID: 2, Contenido: 'viejo', FechaEnvio: 't1', Remitente: ana, Destinatario: luis }
      ]
    });

    await expect(servicio.listarConversaciones(1)).resolves.toEqual([
      { UsuarioID: 2, Nombre: 'Luis', UltimoMensaje: 'último de Luis', Fecha: 't3' },
      { UsuarioID: 3, Nombre: 'Eva', UltimoMensaje: 'hola Eva', Fecha: 't2' }
    ]);
  });
});
