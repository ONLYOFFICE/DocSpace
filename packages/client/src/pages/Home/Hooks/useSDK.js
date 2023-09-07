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

  getSettings,
  logout,
  login,
  addTagsToRoom,
  createTag,
  removeTagsFromRoom,
  loadCurrentUser,
  updateProfileCulture,
  getRooms,
}) => {
  const handleMessage = async (e) => {
    const eventData = typeof e.data === "string" ? JSON.parse(e.data) : e.data;

    if (eventData.data) {
      const { data, methodName } = eventData.data;

      let res;

      try {
        switch (methodName) {
          case "setConfig":
            {
              const requests = await Promise.all([
                setFrameConfig(data),
                user &&
                  data.locale &&
                  updateProfileCulture(user.id, data.locale),
              ]);
              res = requests[0];
            }
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
            res = await loadCurrentUser();
            break;
          case "getRooms":
            {
              res = await getRooms(data);
            }
            break;
          case "openModal":
            {
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
              res = await createRoom(data);

              refreshFiles();
            }
            break;
          case "createTag":
            res = await createTag(data);
            break;
          case "addTagsToRoom":
            {
              const { roomId, tags } = data;
              res = await addTagsToRoom(roomId, tags);
            }
            break;
          case "removeTagsFromRoom":
            {
              const { roomId, tags } = data;
              res = await removeTagsFromRoom(roomId, tags);
            }
            break;
          case "setListView":
            setViewAs(data);
            break;
          case "createHash":
            {
              const { password, hashSettings } = data;
              res = createPasswordHash(password, hashSettings);
            }
            break;
          case "getHashSettings":
            {
              const settings = await getSettings();
              res = settings.passwordHash;
            }
            break;
          case "login":
            {
              const { email, passwordHash } = data;
              res = await login(email, passwordHash);
            }
            break;
          case "logout":
            res = logout();
            break;
          default:
            res = "Wrong method";
        }
      } catch (e) {
        res = e;
      }

      frameCallbackData(res);
    }
  };

  // if (window.parent && !frameConfig) {
  //   frameCallCommand("setConfig");
  // }

  React.useEffect(() => {
    window.addEventListener("message", handleMessage, false);

    return () => {
      window.removeEventListener("message", handleMessage, false);
    };
  }, []);

  React.useEffect(() => {
    frameCallCommand("setConfig");
  }, [frameConfig?.frameId]);
};

export default useSDK;
