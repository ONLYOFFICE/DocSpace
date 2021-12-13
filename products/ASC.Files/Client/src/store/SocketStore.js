import { makeAutoObservable } from "mobx";
import io from "socket.io-client";

const SOCKET_URL = "http://localhost:9899";

const socket = io(SOCKET_URL, {
  withCredentials: true,
  transports: ["websocket", "polling"],
});

const fakeFile = {
  access: 0,
  canEdit: true,
  canOpenPlayer: false,
  canShare: true,
  comment: "Создано",
  contentLength: "6,78 Кб",
  contextOptions: (21)[
    ("edit",
    "preview",
    "separator0",
    "sharing-settings",
    "external-link",
    "link-for-portal-users",
    "send-by-email",
    "version",
    "finalize-version",
    "show-version-history",
    "block-unblock-version",
    "separator1",
    "mark-as-favorite",
    "download",
    "move",
    "move-to",
    "copy-to",
    "copy",
    "rename",
    "separator2",
    "delete")
  ],
  created: "2021-12-13T15:54:04.0000000+03:00",
  createdBy: "Proxy {…}",
  encrypted: undefined,
  fileExst: ".docx",
  fileStatus: 0,
  fileType: 7,
  filesCount: undefined,
  folderId: 6,
  folderUrl: null,
  foldersCount: undefined,
  href: "/products/files/doceditor?fileId=4390",
  icon: "/static/images/icons/24/docx.svg",
  id: 4390,
  isFolder: false,
  isThirdPartyFolder: undefined,
  locked: undefined,
  new: undefined,
  parentId: undefined,
  previewUrl: null,
  providerKey: undefined,
  pureContentLength: 6944,
  rootFolderId: 6,
  rootFolderType: 5,
  shared: false,
  thumbnailStatus: 1,
  thumbnailUrl:
    "http://localhost:8092/products/files/httphandlers/filehandler.ashx?action=thumb&fileid=4390&version=1",
  title: "Новый документ.docx",
  updated: "2021-12-13T15:54:04.0000000+03:00",
  updatedBy: "Proxy {…}",
  version: 1,
  versionGroup: 1,
  viewUrl:
    "http://localhost:8092/products/files/httphandlers/filehandler.ashx?action=download&fileid=4390",
  webUrl:
    "http://localhost:8092/products/files/doceditor?fileid=4390&version=1",
};

class SocketStore {
  filesStore;

  constructor(filesStore) {
    makeAutoObservable(this);
    this.filesStore = filesStore;
  }

  socketInit = () => {
    console.log("socketInit");

    socket.on("connect", () => console.log("socket is connected"));
    socket.on("connect_error", (err) =>
      console.error("socket connect error", err)
    );
    socket.on("disconnect", () => console.log("socket is disconnected"));

    //WAIT RESPONSE OF EDITING FILE
    socket.on("editFile", (file) => {
      if (file) this.filesStore.setFile(fakeFile);
      //if (file) this.filesStore.setFile(file);

      //console.log("editFile File", file);
      //file && console.log("subFileChanges File", JSON.parse(file)?.response);
    });

    socket.on("getFileCreation", (fileId) => {
      console.log("NEED UPDATE LIST OF FILES fileId=", fileId);
    });
  };

  //START EDIT FILE
  startEditingFile = (fileId) => {
    const newFakeFile = { ...fakeFile };
    newFakeFile.fileStatus = 1;
    this.filesStore.setFile(newFakeFile);
    socket.emit("editFile", fileId);
  };

  reportFileCreation = (fileId) => {
    console.log("reportFileCreation");
    socket.emit("reportFileCreation", fileId);
  };
}

export default SocketStore;
