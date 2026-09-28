const path = require('node:path');
const grpc = require('@grpc/grpc-js');
const protoLoader = require('@grpc/proto-loader');

const OPCIONES_CARGA = Object.freeze({ keepCase: false, longs: Number, enums: String, defaults: true, oneofs: true });

function cargarServicio(rutaProtos, { archivo, paquete, servicio }) {
  const definicion = protoLoader.loadSync(path.join(rutaProtos, archivo), OPCIONES_CARGA);
  return grpc.loadPackageDefinition(definicion)[paquete][servicio].service;
}

function crearServidorGrpc({ rutaProtos, servicios, registro }) {
  const servidor = new grpc.Server({ 'grpc.max_receive_message_length': 8 * 1024 * 1024 });

  for (const descripcion of servicios) {
    servidor.addService(cargarServicio(rutaProtos, descripcion), descripcion.implementacion);
  }

  function iniciar(puerto) {
    return new Promise((resolver, rechazar) => {
      servidor.bindAsync(`0.0.0.0:${puerto}`, grpc.ServerCredentials.createInsecure(), (error, puertoAsignado) => {
        if (error) {
          rechazar(error);
          return;
        }
        registro.info('Servidor gRPC escuchando', { puerto: puertoAsignado });
        resolver(puertoAsignado);
      });
    });
  }

  function detener() {
    return new Promise((resolver) => servidor.tryShutdown(() => resolver()));
  }

  return Object.freeze({ iniciar, detener });
}

module.exports = { crearServidorGrpc };
