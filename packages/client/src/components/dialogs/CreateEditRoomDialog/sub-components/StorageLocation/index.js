import React from "react";

import Text from "@docspace/components/text";

import { StyledParam } from "../StyledParam";
import ThirpartyComboBox from "./ThirpartyComboBox";
import SecondaryInfoButton from "../SecondaryInfoButton";

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
          <SecondaryInfoButton content={t("StorageLocationDescription")} />
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
