const fs = require('node:fs/promises');
const os = require('node:os');
const path = require('node:path');
const { crearMultimediaServicio } = require('../../../src/modulos/multimedia/multimedia.servicio');
const { crearAlmacenamientoArchivos } = require('../../../src/infraestructura/almacenamiento/almacenamientoArchivos');
const { TIPOS_ARCHIVO } = require('../../../src/modulos/multimedia/tiposArchivo');
const { ErrorValidacion, ErrorProhibido } = require('../../../src/compartido/errores');
const { registroSilencioso, transaccionDirecta } = require('../apoyo');

async function* fragmentosDe(...partes) {
  for (const parte of partes) {
    yield Buffer.from(parte);
  }
}

describe('multimediaServicio.recibirArchivo', () => {
  let directorio;
  let almacenamiento;
  let multimediaRepositorio;
  let adopcionRepositorio;
  let servicio;

  beforeEach(async () => {
    directorio = await fs.mkdtemp(path.join(os.tmpdir(), 'adoptme-'));
    almacenamiento = crearAlmacenamientoArchivos({ directorioRaiz: directorio });
    multimediaRepositorio = {
      listarRutas: jest.fn().mockResolvedValue([]),
      reemplazar: jest.fn().mockResolvedValue()
    };
    adopcionRepositorio = { buscarPorMascota: jest.fn().mockResolvedValue({ PublicadorID: 1 }) };
    servicio = crearMultimediaServicio({
      multimediaRepositorio, adopcionRepositorio, almacenamiento, transaccion: transaccionDirecta, registro: registroSilencioso
    });
  });

  afterEach(() => fs.rm(directorio, { recursive: true, force: true }));

  test('guarda la foto de perfil del usuario autenticado aunque se envíe otro identificador', async () => {
    await servicio.recibirArchivo(TIPOS_ARCHIVO.FOTO_USUARIO, {
      sesion: { usuarioId: 1 },
      metadatos: { idReferencia: 999, nombreArchivo: 'perfil.PNG' },
      fragmentos: fragmentosDe('abc', 'def')
    });

    const [tipo, referenciaId, rutaRelativa] = multimediaRepositorio.reemplazar.mock.calls[0];
    expect(tipo).toBe(TIPOS_ARCHIVO.FOTO_USUARIO);
    expect(referenciaId).toBe(1);
    await expect(fs.readFile(path.join(directorio, rutaRelativa), 'utf8')).resolves.toBe('abcdef');
  });

  test('rechaza extensiones no permitidas', async () => {
    await expect(servicio.recibirArchivo(TIPOS_ARCHIVO.VIDEO_MASCOTA, {
      sesion: { usuarioId: 1 }, metadatos: { idReferencia: 3, nombreArchivo: 'video.avi' }, fragmentos: fragmentosDe('x')
    })).rejects.toBeInstanceOf(ErrorValidacion);
  });

  test('impide subir archivos de mascotas ajenas', async () => {
    adopcionRepositorio.buscarPorMascota.mockResolvedValue({ PublicadorID: 2 });
    await expect(servicio.recibirArchivo(TIPOS_ARCHIVO.FOTO_MASCOTA, {
      sesion: { usuarioId: 1 }, metadatos: { idReferencia: 3, nombreArchivo: 'foto.jpg' }, fragmentos: fragmentosDe('x')
    })).rejects.toBeInstanceOf(ErrorProhibido);
  });

  test('descarta el archivo parcial cuando excede el tamaño máximo', async () => {
    const tipoPequeno = { ...TIPOS_ARCHIVO.FOTO_USUARIO, tamanoMaximoBytes: 4 };
    await expect(servicio.recibirArchivo(tipoPequeno, {
      sesion: { usuarioId: 1 }, metadatos: { nombreArchivo: 'a.png' }, fragmentos: fragmentosDe('123', '456')
    })).rejects.toBeInstanceOf(ErrorValidacion);

    const carpeta = path.join(directorio, TIPOS_ARCHIVO.FOTO_USUARIO.carpeta);
    await expect(fs.readdir(carpeta)).resolves.toHaveLength(0);
    expect(multimediaRepositorio.reemplazar).not.toHaveBeenCalled();
  });

  test('elimina los archivos anteriores al reemplazarlos', async () => {
    const anterior = await almacenamiento.crearEscritura('fotos/usuarios', '.png');
    await new Promise((resolver) => anterior.flujo.end('viejo', resolver));
    multimediaRepositorio.listarRutas.mockResolvedValue([anterior.rutaRelativa]);

    await servicio.recibirArchivo(TIPOS_ARCHIVO.FOTO_USUARIO, {
      sesion: { usuarioId: 1 }, metadatos: { nombreArchivo: 'nuevo.png' }, fragmentos: fragmentosDe('nuevo')
    });

    await expect(almacenamiento.describir(anterior.rutaRelativa)).resolves.toBeNull();
  });
});
