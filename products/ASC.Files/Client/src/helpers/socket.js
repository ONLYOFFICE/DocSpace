import io from "socket.io-client";

const SOCKET_URL = "http://localhost:9899";
const socket = io(SOCKET_URL, {
  withCredentials: true,
  transports: ["websocket", "polling"],
});

socket.on("connect", () => console.log("socket is connected"));
socket.on("connect_error", (err) => console.error("socket connect error", err));
socket.on("disconnect", () => console.log("socket is disconnected"));

export default socket;
