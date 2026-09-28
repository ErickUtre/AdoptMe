const MEGABYTE = 1024 * 1024;

const PROPIETARIOS = Object.freeze({ USUARIO: 'usuario', MASCOTA: 'mascota' });

const TIPOS_ARCHIVO = Object.freeze({
  FOTO_USUARIO: Object.freeze({
    clave: 'FOTO_USUARIO',
    propietario: PROPIETARIOS.USUARIO,
    carpeta: 'fotos/usuarios',
    extensiones: Object.freeze(['.jpg', '.jpeg', '.png']),
    tamanoMaximoBytes: 10 * MEGABYTE
  }),
  FOTO_MASCOTA: Object.freeze({
    clave: 'FOTO_MASCOTA',
    propietario: PROPIETARIOS.MASCOTA,
    carpeta: 'fotos/mascotas',
    extensiones: Object.freeze(['.jpg', '.jpeg', '.png']),
    tamanoMaximoBytes: 10 * MEGABYTE
  }),
  VIDEO_MASCOTA: Object.freeze({
    clave: 'VIDEO_MASCOTA',
    propietario: PROPIETARIOS.MASCOTA,
    carpeta: 'videos/mascotas',
    extensiones: Object.freeze(['.mp4']),
    tamanoMaximoBytes: 200 * MEGABYTE
  })
});

module.exports = { TIPOS_ARCHIVO, PROPIETARIOS };
