import React from "react";

import { Events } from "@docspace/common/constants";
import { frameCallbackData, frameCallCommand } from "@docspace/common/utils";

const useSDK = ({
  frameConfig,
  setFrameConfig,
  selectedFolderStore,
  folders,
  files,
  filesList,
  selection,
  user,
  createFile,
  createFolder,
  createRoom,
  refreshFiles,
  setViewAs,
}) => {
  const handleMessage = async (e) => {
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
        case "getList":
          res = filesList;
          break;
        case "getSelection":
          res = selection;
          break;
        case "getUserInfo":
          res = user;
          break;
        case "openModal": {
          const { type, options } = data;

          if (type === "CreateFile" || type === "CreateFolder") {
            const item = new Event(Events.CREATE);

            const payload = {
              extension: options,
              id: -1,
            };

            item.payload = payload;

            window.dispatchEvent(item);
          }

          if (type === "CreateRoom") {
            const room = new Event(Events.ROOM_CREATE);

            window.dispatchEvent(room);
          }
          break;
        }
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
        case "setListView":
          {
            setViewAs(data);
          }
          break;
        default:
          res = "Wrong method";
      }

      frameCallbackData(res);
    }
  };

  if (window.parent && !frameConfig) {
    frameCallCommand("setConfig");
  }

  React.useEffect(() => {
    window.addEventListener("message", handleMessage, false);

    return () => {
      window.removeEventListener("message", handleMessage, false);
    };
  }, []);
};

export default useSDK;
