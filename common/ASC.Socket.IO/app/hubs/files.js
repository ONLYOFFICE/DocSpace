module.exports = (io) => {
  const logger = require("../log.js");
  const filesIO = io; //TODO: Restore .of("/files");

  filesIO.on("connection", (socket) => {
    const session = socket.handshake.session;

    if (!session) {
      logger.error("empty session");
      return;
    }

    if (!session.user) {
      logger.error("invalid session: unknown user");
      return;
    }

    if (!session.portal) {
      logger.error("invalid session: unknown portal");
      return;
    }

    const userId = session?.user?.id;
    const tenantId = session?.portal?.tenantId;

    getRoom = (roomPart) => {
      return `${tenantId}-${roomPart}`;
    };

    logger.info(
      `connect user='${userId}' on tenant='${tenantId}' socketId='${socket.id}'`
    );

    socket.on("disconnect", (reason) => {
      logger.info(
        `disconnect user='${userId}' on tenant='${tenantId}' socketId='${socket.id}' due to ${reason}`
      );
    });

    socket.on("subscribe", (roomParts) => {
      if (!roomParts) return;

      if (Array.isArray(roomParts)) {
        const rooms = roomParts.map((p) => getRoom(p));
        logger.info(`client ${socket.id} join rooms [${rooms.join(",")}]`);
        socket.join(rooms);
      } else {
        const room = getRoom(roomParts);
        logger.info(`client ${socket.id} join room ${room}`);
        socket.join(room);
      }
    });

    socket.on("unsubscribe", (roomParts) => {
      if (!roomParts) return;

      if (Array.isArray(roomParts)) {
        const rooms = roomParts.map((p) => getRoom(p));
        logger.info(`client ${socket.id} leave rooms [${rooms.join(",")}]`);
        socket.leave(rooms);
      } else {
        const room = getRoom(roomParts);
        logger.info(`client ${socket.id} leave room ${room}`);
        socket.leave(room);
      }
    });

    socket.on("c:refresh-folder", (folderId) => {
      const room = getRoom(folderId);
      logger.info(`refresh folder ${folderId} in room ${room}`);
      socket.to(room).emit("s:refresh-folder", folderId);
    });
  });

  function startEdit({ fileId, room } = {}) {
    logger.info(`start edit file ${fileId} in room ${room}`);
    filesIO.to(room).emit("s:start-edit-file", fileId);
  }

  function stopEdit({ fileId, room } = {}) {
    logger.info(`stop edit file ${fileId} in room ${room}`);
    filesIO.to(room).emit("s:stop-edit-file", fileId);
  }

  return { startEdit, stopEdit };
};
