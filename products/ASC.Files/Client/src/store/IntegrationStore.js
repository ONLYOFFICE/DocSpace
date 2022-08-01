import { makeAutoObservable } from "mobx";
import { Events } from "../helpers/constants";
import { frameCallback } from "@appserver/common/utils";

class IntegrationStore {
  authStore;
  filesStore;
  settingsStore;
  filesActionsStore;

  constructor(authStore, filesStore, settingsStore, filesActionsStore) {
    makeAutoObservable(this);
    this.authStore = authStore;
    this.filesStore = filesStore;
    this.settingsStore = settingsStore;
    this.filesActionsStore = filesActionsStore;
  }

  handleMessage = async (e) => {
    const { settingsStore, userStore } = this.authStore;
    const { setFrameConfig } = settingsStore;
    const { user } = userStore;
    const {
      folders,
      files,
      selection,
      filesList,
      selectedFolderStore,
      createFile,
      createFolder,
      createRoom,
      refreshFiles,
      setViewAs,
    } = this.filesStore;

    const eventData = typeof e.data === "string" ? JSON.parse(e.data) : e.data;

    if (eventData.data) {
      const { data, methodName } = eventData.data;

      let res;

      switch (methodName) {
        case "setConfig":
          res = await setFrameConfig(data);
          break;
        case "getFolderInfo":
          res = selectedFolderStore;
          break;
        case "getFolders":
          res = folders;
          break;
        case "getFiles":
          res = files;
          break;
        case "getItems":
          res = filesList;
          break;
        case "getSelection":
          res = selection;
          break;
        case "getUserInfo":
          res = user;
          break;
        case "openCrateItemModal":
          {
            const item = new Event(Events.CREATE);

            const payload = {
              extension: data,
              id: -1,
            };

            item.payload = payload;

            window.dispatchEvent(item);
          }
          break;
        case "openCrateRoomModal":
          {
            const room = new Event(Events.ROOM_CREATE);

            window.dispatchEvent(room);
          }
          break;
        case "createFile":
          {
            const { folderId, title, templateId, formId } = data;
            res = await createFile(folderId, title, templateId, formId);

            refreshFiles();
          }
          break;
        case "createFolder":
          {
            const { parentFolderId, title } = data;
            res = await createFolder(parentFolderId, title);

            refreshFiles();
          }
          break;
        case "createRoom":
          {
            const { title, type } = data;
            res = await createRoom(title, type);

            refreshFiles();
          }
          break;
        case "setItemsView":
          {
            setViewAs(data);
          }
          break;
        default:
          res = "Wrong method";
      }

      frameCallback(res);
    }
  };
}

export default IntegrationStore;
