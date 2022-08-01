import React from "react";

import Text from "@appserver/components/text";

import Checkbox from "@appserver/components/checkbox";
import { StyledParam } from "../StyledParam";
import HelpButton from "@appserver/components/help-button";
import ThirpartyComboBox from "./ThirpartyComboBox";

const StorageLocation = ({
  t,
  roomParams,
  setRoomParams,
  setIsScrollLocked,
}) => {
  return (
    <StyledParam storageLocation>
      <div className="set_room_params-info">
        <div className="set_room_params-info-title">
          <Text className="set_room_params-info-title-text">
            {t("StorageLocationTitle")}
          </Text>
          <HelpButton
            displayType="auto"
            className="set_room_params-info-title-help"
            iconName="/static/images/info.react.svg"
            offsetRight={0}
            tooltipContent={t("StorageLocationDescription")}
            size={12}
          />
        </div>
        <div className="set_room_params-info-description">
          {t("StorageLocationDescription")}
        </div>
      </div>

      <ThirpartyComboBox
        t={t}
        roomParams={roomParams}
        setRoomParams={setRoomParams}
        setIsScrollLocked={setIsScrollLocked}
      />
    </StyledParam>
  );
};

export default StorageLocation;
