import io from "socket.io-client";

let client = null;

class SocketIOHelper {
  socketUrl = null;

  constructor(url) {
    if (!url) return;

    this.socketUrl = url;

    if (client) return;

    const origin = window.location.origin;

    client = io(origin, {
      withCredentials: true,
      transports: ["websocket", "polling"],
      eio: 4,
      path: url,
    });

    client.on("connect", () => console.log("socket is connected"));
    client.on("connect_error", (err) =>
      console.log("socket connect error", err)
    );
    client.on("disconnect", () => console.log("socket is disconnected"));
  }

  get isEnabled() {
    return this.socketUrl !== null;
  }

  emit = ({ command, data, room = null }) => {
    if (!this.isEnabled) return;

    if (!client.connected) {
      client.on("connect", () => {
        if (room !== null) {
          client.to(room).emit(command, data);
        } else {
          client.emit(command, data);
        }
      });
    } else {
      room ? client.to(room).emit(command, data) : client.emit(command, data);
    }
  };

  on = (eventName, callback) => {
    if (!this.isEnabled) return;

    if (!client.connected) {
      client.on("connect", () => {
        client.on(eventName, callback);
      });
    } else {
      client.on(eventName, callback);
    }
  };
}

export default SocketIOHelper;
