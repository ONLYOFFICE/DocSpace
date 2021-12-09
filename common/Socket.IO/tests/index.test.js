const io = require("../index");
const Client = require("socket.io-client");

describe("Jest tests", () => {
  let clientSocket, serverSocket;

  beforeAll((done) => {
    const port = io.engine.port || 9899;
    clientSocket = new Client(`http://localhost:${port}`);
    io.on("connection", (socket) => {
      serverSocket = socket;
    });
    clientSocket.on("connect", done);
  });

  afterAll(() => {
    io.close();
    clientSocket.close();
  });

  test("Test socket connect", (done) => {
    expect(clientSocket.connected).toEqual(true);
    done();
  });

  test("Test startFileEdit", (done) => {
    if (clientSocket.connected) {
      const fileId = "12345";
      clientSocket.emit("reportFileCreation", fileId);

      clientSocket.on("getFileCreation", (file) => {
        expect(file).toEqual(fileId);
        done();
      });
    }
  });

  //TODO: Need test for authentication
});
