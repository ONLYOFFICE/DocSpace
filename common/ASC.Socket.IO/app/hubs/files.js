module.exports = (io) => {
  const logger = require("../log.js");
  const moment = require("moment");
  const filesIO = io; //TODO: Restore .of("/files");

  filesIO.on("connection", (socket) => {
    const session = socket.handshake.session;

    if (!session) {
      logger.error("empty session");
      return;
    }

    if (session.system) {
      logger.info(`connect system as socketId='${socket.id}'`);

      socket.on("ping", (date) => {
        logger.info(`ping (client ${socket.id}) at ${date}`);
        filesIO.to(socket.id).emit("pong", moment.utc());
      });

      socket.on("disconnect", (reason) => {
        logger.info(
          `disconnect system as socketId='${socket.id}' due to ${reason}`
        );
      });

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

    socket.on("refresh-folder", (folderId) => {
      const room = getRoom(`DIR-${folderId}`);
      logger.info(`refresh folder ${folderId} in room ${room}`);
      socket.to(room).emit("refresh-folder", folderId);
    });

    socket.on("restore-backup", () => {
      const room = getRoom("backup-restore");
      logger.info(`restore backup in room ${room}`);
      socket.to(room).emit("restore-backup");
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

  function modifyFolder(room, cmd, id, type, data) {
    filesIO.to(room).emit("s:modify-folder", { cmd, id, type, data });
  }

  function createFile({ fileId, room, data } = {}) {
    logger.info(`create new file ${fileId} in room ${room}`);
    modifyFolder(room, "create", fileId, "file", data);
  }

  function updateFile({ fileId, room, data } = {}) {
    logger.info(`update file ${fileId} in room ${room}`);
    modifyFolder(room, "update", fileId, "file", data);
  }

  function deleteFile({ fileId, room } = {}) {
    logger.info(`delete file ${fileId} in room ${room}`);
    modifyFolder(room, "delete", fileId, "file");
  }

  return { startEdit, stopEdit, createFile, deleteFile, updateFile };
};
