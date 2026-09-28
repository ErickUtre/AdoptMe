class ErrorAplicacion extends Error {
  constructor(mensaje, codigoHttp) {
    super(mensaje);
    this.name = this.constructor.name;
    this.codigoHttp = codigoHttp;
  }
}

class ErrorValidacion extends ErrorAplicacion {
  constructor(mensaje) {
    super(mensaje, 400);
  }
}

class ErrorNoAutenticado extends ErrorAplicacion {
  constructor(mensaje = 'Credenciales inválidas o ausentes') {
    super(mensaje, 401);
  }
}

class ErrorProhibido extends ErrorAplicacion {
  constructor(mensaje = 'No tienes permiso para realizar esta acción') {
    super(mensaje, 403);
  }
}

class ErrorNoEncontrado extends ErrorAplicacion {
  constructor(mensaje = 'Recurso no encontrado') {
    super(mensaje, 404);
  }
}

class ErrorConflicto extends ErrorAplicacion {
  constructor(mensaje) {
    super(mensaje, 409);
  }
}

class ErrorServicioNoDisponible extends ErrorAplicacion {
  constructor(mensaje = 'Servicio temporalmente no disponible') {
    super(mensaje, 503);
  }
}

module.exports = {
  ErrorAplicacion,
  ErrorValidacion,
  ErrorNoAutenticado,
  ErrorProhibido,
  ErrorNoEncontrado,
  ErrorConflicto,
  ErrorServicioNoDisponible
};
