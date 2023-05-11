import React, { useEffect, useCallback } from "react";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";
import RoomSelector from "../../components/RoomSelector";
import SelectFolderDialog from "../../components/panels/SelectFolderDialog";
import {
  frameCallEvent,
  frameCallbackData,
  createPasswordHash,
} from "@docspace/common/utils";

const Sdk = (props) => {
  useEffect(() => {
    window.addEventListener("message", handleMessage, false);
    return () => {
      window.removeEventListener("message", handleMessage, false);
    };
  }, [handleMessage]);

  const { mode } = props.match.params;

  const handleMessage = (e) => {
    const { setFrameConfig, login, logout, hashSettings, user } = props;

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
            res = createPasswordHash(password, hashSettings);
          }
          break;
        case "getUserInfo":
          res = user;
          break;
        case "getHashSettings": {
          res = hashSettings;
          break;
        }
        case "login":
          {
            const { email, passwordHash } = data;
            res = login(email, passwordHash);
          }
          break;
        case "logout":
          res = logout();
          break;
        default:
          res = "Wrong method";
      }

      frameCallbackData(res);
    }
  };

  const onSelect = useCallback(
    (item) => {
      frameCallEvent({ event: "onSelectCallback", data: item });
    },
    [frameCallEvent]
  );

  const onClose = useCallback(() => {
    frameCallEvent({ event: "onCloseCallback" });
  }, [frameCallEvent]);

  let component;

  switch (mode) {
    case "room-selector":
      component = (
        <RoomSelector
          withCancelButton
          withHeader={false}
          onAccept={onSelect}
          onCancel={onClose}
        />
      );
      break;
    default:
      component = null;
  }

  return component;
};

export default inject(({ auth }) => {
  const { login, logout, settingsStore } = auth;
  const { theme, setFrameConfig, frameConfig, hashSettings } = settingsStore;
  return {
    theme,
    setFrameConfig,
    frameConfig,
    login,
    logout,
    hashSettings,
    user: auth.userStore.user,
  };
})(observer(Sdk));
