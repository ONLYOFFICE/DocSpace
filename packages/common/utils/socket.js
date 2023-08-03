import io from "socket.io-client";

let client = null;
let callbacks = [];

class SocketIOHelper {
  socketUrl = null;

  constructor(url, publicRoomKey) {
    if (!url) return;

    this.socketUrl = url;

    if (client) return;

    const origin = window.location.origin;

    const config = {
      withCredentials: true,
      transports: ["websocket", "polling"],
      eio: 4,
      path: url,
    };

    if (publicRoomKey) {
      config.query = {
        share: publicRoomKey,
      };
    }

    client = io(origin, config);

    client.on("connect", () => {
      console.log("socket is connected");
      if (callbacks?.length > 0) {
        callbacks.forEach(({ eventName, callback }) =>
          client.on(eventName, callback)
        );
        callbacks = [];
      }
    });
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
    if (!this.isEnabled) {
      callbacks.push({ eventName, callback });
      return;
    }

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
