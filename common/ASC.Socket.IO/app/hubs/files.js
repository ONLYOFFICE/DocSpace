module.exports = (io) => {
  const log = require("../log.js");
  const files = io; //TODO: Restore .of("/files");

  files.on("connection", (socket) => {
    const session = socket.handshake.session;

    if (!session || !session.user || !session.portal) {
      console.log("Invalid session");
      return;
    }

    const userId = session?.user?.id;
    const tenantId = session?.portal?.tenantId;

    console.log(
      `Connected user='${userId}' on tenant='${tenantId}' socketId='${socket.id}'`
    );

    const room = tenantId;

    if (room) {
      socket.join(room);
    }

    socket.on("c:start-edit-file", (fileId) => {
      console.log("Call of start-edit-file", fileId);
      socket.to(room).emit("s:start-edit-file", fileId);
    });

    socket.on("c:stop-edit-file", (fileId) => {
      console.log("Call of stop-edit-file", fileId);
      socket.to(room).emit("s:stop-edit-file", fileId);
    });

    socket.on("c:refresh-folder", (folderId) => {
      socket.to(room).emit("s:refresh-folder", folderId);
    });
  });

  function startEdit({ fileId, tenantId } = {}) {
    files.to(tenantId).emit("s:start-edit-file", fileId);
  }

  function stopEdit({ fileId, tenantId } = {}) {
    files.to(tenantId).emit("s:stop-edit-file", fileId);
  }

  return { startEdit, stopEdit };
};
