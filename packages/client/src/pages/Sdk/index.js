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
  loadCurrentUser,
  getIcon,
  isLoaded,
  getSettings,
  user,
  updateProfileCulture,
}) => {
  useEffect(() => {
    window.addEventListener("message", handleMessage, false);
    return () => {
      window.removeEventListener("message", handleMessage, false);
      setFrameConfig(null);
    };
  }, [handleMessage]);

  const callCommand = useCallback(() => frameCallCommand("setConfig"), [
    frameCallCommand,
  ]);

  useEffect(() => {
    if (window.parent && !frameConfig && isLoaded) {
      callCommand("setConfig");
    }
  }, [callCommand, isLoaded]);

  const { mode } = match.params;

  const selectorType = new URLSearchParams(window.location.search).get(
    "selectorType"
  );

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

      try {
        switch (methodName) {
          case "setConfig":
            {
              const requests = await Promise.all([
                setFrameConfig(data),
                user && updateProfileCulture(user.id, data.locale),
              ]);
              res = requests[0];
            }
            break;
          case "createHash":
            {
              const { password, hashSettings } = data;
              res = createPasswordHash(password, hashSettings);
            }
            break;
          case "getUserInfo":
            res = await loadCurrentUser();
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
            res = await logout();
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
  }, [frameCallEvent]);

  const onCloseCallback = !!frameConfig?.events.onCloseCallback
    ? {
        onClose,
      }
    : {};

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
          filteredType={selectorType}
          withSubfolders={false}
          displayType="aside"
          embedded={true}
          searchParam={frameConfig?.filter.search}
          ByExtension
          {...onCloseCallback}
        />
      );
      break;
    default:
      component = <AppLoader />;
  }

  return component;
};

export default inject(({ auth, settingsStore, peopleStore }) => {
  const { login, logout, userStore } = auth;
  const {
    theme,
    setFrameConfig,
    frameConfig,
    getSettings,
    isLoaded,
  } = auth.settingsStore;
  const { loadCurrentUser, user } = userStore;
  const { updateProfileCulture } = peopleStore.targetUserStore;
  const { getIcon } = settingsStore;

  return {
    theme,
    setFrameConfig,
    frameConfig,
    login,
    logout,
    getSettings,
    loadCurrentUser,
    getIcon,
    isLoaded,
    updateProfileCulture,
    user,
  };
})(observer(Sdk));
