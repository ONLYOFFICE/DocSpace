import React from "react";
import { ReactSVG } from "react-svg";

import { Text } from "@docspace/components";

import ItemContextOptions from "./ItemContextOptions";

import { StyledTitle } from "../styles/common";

const ItemTitle = ({
  t,
  selection,
  isGallery,
  isSeveralItems,
  setBufferSelection,
  getIcon,
}) => {
  return isSeveralItems ? (
    <StyledTitle>
      <ReactSVG className="icon" src={getIcon(32, ".file")} />
      <Text className="text">
        {`${t("ItemsSelected")}: ${selection.length}`}
      </Text>
    </StyledTitle>
  ) : isGallery ? (
    <StyledTitle>
      <ReactSVG className="icon" src={getIcon(32, ".docxf")} />
      <Text className="text">{selection?.attributes?.name_form}</Text>
    </StyledTitle>
  ) : (
    <StyledTitle>
      <img
        className={`icon ${selection.isRoom && "is-room"}`}
        src={selection.icon}
        alt="thumbnail-icon"
      />
      <Text className="text">{selection.title}</Text>
      {selection && (
        <ItemContextOptions
          selection={selection}
          setBufferSelection={setBufferSelection}
        />
      )}
    </StyledTitle>
  );
};

export default ItemTitle;
