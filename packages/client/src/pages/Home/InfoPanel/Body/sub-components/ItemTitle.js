import React from "react";
import { ReactSVG } from "react-svg";

import { StyledTitle } from "../styles/common";
import ItemContextOptions from "./ItemContextOptions";

import { Text } from "@docspace/components";

const ItemTitle = ({ t, selection, isGallery, isFileCategory, getIcon }) => {
  if ((isFileCategory && selection.isSelectedFolder) || isGallery) return null;

  return !Array.isArray(selection) ? (
    <StyledTitle>
      <img
        className={`icon ${selection.isRoom && "is-room"}`}
        src={selection.icon}
        alt="thumbnail-icon"
      />
      <Text className="text">{selection.title}</Text>
      <ItemContextOptions selectedItem={selection} />
    </StyledTitle>
  ) : (
    <StyledTitle>
      <ReactSVG className="icon" src={getIcon(32, ".file")} />
      <Text className="text">
        {`${t("ItemsSelected")}: ${selection.length}`}
      </Text>
    </StyledTitle>
  );
};

export default ItemTitle;
