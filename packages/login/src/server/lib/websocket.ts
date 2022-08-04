import { RawData, WebSocketServer, Server } from "ws";
import pkg from "../../../package.json";
const { socketPath } = pkg;

interface WSS extends Server {
  broadcast?: (message: RawData | string) => void;
}

export default (expressServer: any) => {
  const wss: WSS = new WebSocketServer({
    noServer: true,
    path: socketPath,
  });

  const broadcast = (message: RawData | string): void => {
    wss.clients.forEach(function each(client) {
      client.send(message);
    });
  };

  expressServer.on("upgrade", (request: any, socket: any, head: any) => {
    wss.handleUpgrade(request, socket, head, (websocket) => {
      wss.emit("connection", websocket, request);
    });
  });

  wss.on("connection", (ws) => {
    ws.on("message", (message) => {
      broadcast(message);
    });
  });

  wss.broadcast = broadcast;

  return wss;
};
