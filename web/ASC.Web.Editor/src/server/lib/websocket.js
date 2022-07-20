const WebSocket = require("ws");
const pkg = require("../../../package.json");
const { socketPath } = pkg;

module.exports = (expressServer) => {
  const wss = new WebSocket.Server({
    noServer: true,
    path: socketPath,
  });

  expressServer.on("upgrade", (request, socket, head) => {
    wss.handleUpgrade(request, socket, head, (websocket) => {
      wss.emit("connection", websocket, request);
    });
  });

  wss.on("connection", function connection(ws) {
    ws.on("message", function (message) {
      wss.broadcast(message);
    });
  });

  wss.broadcast = function broadcast(msg) {
    wss.clients.forEach(function each(client) {
      client.send(msg);
    });
  };

  return wss;
};
