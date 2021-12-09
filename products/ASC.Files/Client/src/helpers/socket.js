import io from "socket.io-client";

const SOCKET_URL = "http://localhost:9899";
const socket = io(SOCKET_URL /* { withCredentials: true } */);

const socketInit = async () => {
  socket.on("connect", () => console.log("socket is connected"));
  socket.on("connect_error", (err) =>
    console.error("socket connect error", err)
  );
  socket.on("disconnect", () => console.log("socket is disconnected"));

  socket.on("subFileChanges", (file) => {
    console.log("subFileChanges File", file);
    file && console.log("subFileChanges File", JSON.parse(file)?.response);
  });

  socket.on("getFileCreation", (fileId) => {
    console.log("NEED UPDATE LIST OF FILES fileId=", fileId);
  });
};

const startEditingFile = (fileId) => {
  socket.emit("startFileEdit", fileId);
};

const reportFileCreation = (fileId) => {
  console.log("reportFileCreation");
  socket.emit("reportFileCreation", fileId);
};

export { socketInit, startEditingFile, reportFileCreation };
