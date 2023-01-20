import React, { useRef } from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";

import { Text } from "@docspace/components";

import ItemContextOptions from "./ItemContextOptions";

import { StyledTitle } from "../../styles/common";

const FilesItemTitle = ({
  t,
  selection,
  isSeveralItems,
  getIcon,
  selectionLength,
}) => {
  const itemTitleRef = useRef();

  if (isSeveralItems)
    return (
      <StyledTitle>
        <div className="item-icon">
          <ReactSVG className="icon" src={getIcon(32, ".file")} />
        </div>
        <Text className="text">
          {`${t("InfoPanel:ItemsSelected")}: ${selectionLength}`}
        </Text>
      </StyledTitle>
    );

  const icon = selection.icon;

  return (
    <StyledTitle ref={itemTitleRef}>
      <div className="item-icon">
        <img
          className={`icon ${selection.isRoom && "is-room"}`}
          src={icon}
          alt="thumbnail-icon"
        />
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
