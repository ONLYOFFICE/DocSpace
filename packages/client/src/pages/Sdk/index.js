import React, { useEffect, useCallback } from "react";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";
import AppLoader from "@docspace/common/components/AppLoader";
import RoomSelector from "../../components/RoomSelector";
import SelectFileDialog from "../../components/panels/SelectFileDialog";
import {
  frameCallEvent,
  frameCallbackData,
  createPasswordHash,
  frameCallCommand,
} from "@docspace/common/utils";

const Sdk = ({
  frameConfig,
  match,
  setFrameConfig,
  login,
  logout,
  hashSettings,
  user,
  getIcon,
}) => {
  useEffect(() => {
    window.addEventListener("message", handleMessage, false);
    return () => {
      window.removeEventListener("message", handleMessage, false);
    };
  }, [handleMessage]);

  useEffect(() => {
    if (window.parent && !frameConfig) frameCallCommand("setConfig");
  }, [frameConfig?.frameId, frameCallCommand]);

  const { mode } = match.params;

  const toRelativeUrl = (data) => {
    try {
      const url = new URL(data);
      const rel = url.toString().substring(url.origin.length);
      return rel;
    } catch {
      return data;
    }
  };

  const handleMessage = async (e) => {
    const eventData = typeof e.data === "string" ? JSON.parse(e.data) : e.data;

    if (eventData.data) {
      const { data, methodName } = eventData.data;

      let res;

      switch (methodName) {
        case "setConfig":
          res = setFrameConfig(data);
          break;
        case "createHash":
          {
            const { password, hashSettings } = data;
            try {
              res = createPasswordHash(password, hashSettings);
            } catch (e) {
              res = e;
            }
          }
          break;
        case "getUserInfo": {
          res = user;
          break;
        }
        case "getHashSettings": {
          res = hashSettings;
          break;
        }
        case "login":
          {
            const { email, passwordHash } = data;
            try {
              res = await login(email, passwordHash);
            } catch (e) {
              res = e;
            }
          }
          break;
        case "logout":
          res = await logout();
          break;
        default:
          res = "Wrong method";
      }
      frameCallbackData(res);
    }
  };

  const onSelectRoom = useCallback(
    (data) => {
      data[0].icon = toRelativeUrl(data[0].icon);
      frameCallEvent({ event: "onSelectCallback", data });
    },
    [frameCallEvent]
  );

  const onSelectFile = useCallback(
    (data) => {
      data.icon = getIcon(64, data.fileExst);

      frameCallEvent({ event: "onSelectCallback", data });
    },
    [frameCallEvent]
  );

  const onClose = useCallback(() => {
    frameCallEvent({ event: "onCloseCallback" });
    setFrameConfig(null);
  }, [frameCallEvent]);

  let component;

  switch (mode) {
    case "room-selector":
      component = (
        <RoomSelector
          withCancelButton
          withHeader={false}
          onAccept={onSelectRoom}
          onCancel={onClose}
        />
      );
      break;
    case "file-selector":
      component = (
        <SelectFileDialog
          isPanelVisible={true}
          onSelectFile={onSelectFile}
          onClose={onClose}
          filteredType="exceptPrivacyTrashArchiveFolders"
          withSubfolders={false}
          displayType="aside"
          embedded={true}
        />
      );
      break;
    default:
      component = <AppLoader />;
  }

  return component;
};

export default inject(({ auth, settingsStore }) => {
  const { login, logout, userStore } = auth;
  const {
    theme,
    setFrameConfig,
    frameConfig,
    hashSettings,
  } = auth.settingsStore;
  const { user } = userStore;
  const { getIcon } = settingsStore;
  return {
    theme,
    setFrameConfig,
    frameConfig,
    login,
    logout,
    hashSettings,
    user,
    getIcon,
  };
})(observer(Sdk));
