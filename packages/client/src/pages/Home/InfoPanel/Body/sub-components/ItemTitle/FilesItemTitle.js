import React, { useRef } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";

import { Text } from "@docspace/components";

import ItemContextOptions from "./ItemContextOptions";

import { StyledTitle } from "../../styles/common";
import RoomIcon from "@docspace/client/src/components/RoomIcon";

const FilesItemTitle = ({ t, selection, isSeveralItems }) => {
  const itemTitleRef = useRef();

  if (isSeveralItems) return <></>;

  const icon = selection.icon;
  const isLoadedRoomIcon = !!selection.logo?.large;
  const showDefaultRoomIcon = !isLoadedRoomIcon && selection.isRoom;

  return (
    <StyledTitle ref={itemTitleRef}>
      <div className="item-icon">
        {!showDefaultRoomIcon && (
          <img
            className={`icon ${selection.isRoom && "is-room"}`}
            src={icon}
            alt="thumbnail-icon"
          />
        )}
        {showDefaultRoomIcon && <RoomIcon title={selection.title} />}
      </div>
      <Text className="text">{selection.title}</Text>
      {selection && (
        <ItemContextOptions
          t={t}
          itemTitleRef={itemTitleRef}
          selection={selection}
        />
      )}
    </StyledTitle>
  );
};

export default withTranslation([
  "Files",
  "Common",
  "Translations",
  "InfoPanel",
  "SharingPanel",
])(observer(FilesItemTitle));
