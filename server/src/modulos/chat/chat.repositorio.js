const { Op } = require('sequelize');

function crearChatRepositorio({ Chat, Usuario }) {
  const inclusionParticipantes = [
    { model: Usuario, as: 'Remitente', attributes: ['UsuarioID', 'Nombre'] },
    { model: Usuario, as: 'Destinatario', attributes: ['UsuarioID', 'Nombre'] }
  ];

  return Object.freeze({
    crear: (datos) => Chat.create(datos),

    listarEntre: (usuarioA, usuarioB) => Chat.findAll({
      where: {
        [Op.or]: [
          { RemitenteID: usuarioA, DestinatarioID: usuarioB },
          { RemitenteID: usuarioB, DestinatarioID: usuarioA }
        ]
      },
      order: [['FechaEnvio', 'ASC'], ['ChatID', 'ASC']]
    }),

    listarDeUsuario: (usuarioId) => Chat.findAll({
      where: { [Op.or]: [{ RemitenteID: usuarioId }, { DestinatarioID: usuarioId }] },
      include: inclusionParticipantes,
      order: [['FechaEnvio', 'DESC'], ['ChatID', 'DESC']]
    })
  });
}

module.exports = { crearChatRepositorio };
