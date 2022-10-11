import React, { useRef } from "react";
import { withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";

import { Text } from "@docspace/components";

import ItemContextOptions from "./ItemContextOptions";

import { StyledTitle } from "../../styles/common";

const FilesItemTitle = ({
  t,

  selection,
  isSeveralItems,

  setBufferSelection,
  getIcon,

  getContextOptions,
  getContextOptionActions,
}) => {
  if (isSeveralItems)
    return (
      <StyledTitle>
        <ReactSVG className="icon" src={getIcon(32, ".file")} />
        <Text className="text">
          {`${t("InfoPanel:ItemsSelected")}: ${selection.length}`}
        </Text>
      </StyledTitle>
    );

  const itemTitleRef = useRef();

  return (
    <StyledTitle ref={itemTitleRef}>
      <img
        className={`icon ${selection.isRoom && "is-room"}`}
        src={selection.icon}
        alt="thumbnail-icon"
      />
      <Text className="text">{selection.title}</Text>
      {selection && (
        <ItemContextOptions
          t={t}
          itemTitleRef={itemTitleRef}
          selection={selection}
          setBufferSelection={setBufferSelection}
          getContextOptions={getContextOptions}
          getContextOptionActions={getContextOptionActions}
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
])(FilesItemTitle);
