import React, { useEffect, useCallback } from "react";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";
import RoomSelector from "../../components/RoomSelector";
import SelectFolderDialog from "../../components/panels/SelectFolderDialog";
import { frameCallEvent } from "@docspace/common/utils";

const Selector = (props) => {
  useEffect(() => {
    window.addEventListener("message", handleMessage, false);
    return () => {
      window.removeEventListener("message", handleMessage, false);
    };
  }, [handleMessage]);

  const handleMessage = (e) => {
    const { setFrameConfig } = props;

    const eventData = typeof e.data === "string" ? JSON.parse(e.data) : e.data;

    if (eventData.data) {
      const { data, methodName } = eventData.data;

      let res;

      switch (methodName) {
        case "setConfig":
          res = setFrameConfig(data);
          break;
        default:
          res = "Wrong method";
      }

      frameCallbackData(res);
    }
  };

  const onAcceptItem = useCallback(
    (item) => {
      frameCallEvent({ event: "onSelectCallback", data: item });
    },
    [frameCallEvent]
  );

  return (
    <RoomSelector withCancelButton withHeader={false} onAccept={onAcceptItem} />
  );
};

export default inject(({ auth }) => {
  const { theme, setFrameConfig, frameConfig } = auth.settingsStore;
  return {
    theme,
    setFrameConfig,
    frameConfig,
  };
})(observer(Selector));
