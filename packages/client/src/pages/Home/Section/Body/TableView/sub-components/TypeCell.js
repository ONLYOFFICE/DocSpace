import React from "react";
import { StyledText } from "./CellStyles";
import { FileType, RoomsType } from "@docspace/common/constants";

const TypeCell = ({ t, item, sideColor }) => {
  const { fileExst, fileType, roomType } = item;
  const getItemType = () => {
    switch (fileType) {
      case FileType.Unknown:
        return t("Common:Unknown");
      case FileType.Archive:
        return t("Common:Archive");
      case FileType.Video:
        return t("Common:Video");
      case FileType.Audio:
        return t("Common:Audio");
      case FileType.Image:
        return t("Common:Image");
      case FileType.Spreadsheet:
        return t("Spreadsheet");
      case FileType.Presentation:
        return t("Presentation");
      case FileType.Document:
      case FileType.OForm:
      case FileType.OFormTemplate:
        return t("Document");

      default:
        return t("Folder");
    }
  };

  const getRoomType = () => {
    switch (roomType) {
      case RoomsType.CustomRoom:
        return `Custom`;
      case RoomsType.FillingFormsRoom:
        return `Filling form`;
      case RoomsType.EditingRoom:
        return `Collaboration`;
      case RoomsType.ReadOnlyRoom:
        return `View-only`;
      case RoomsType.ReviewRoom:
        return `Review`;
      default:
        return ``;
    }
  };

  const type = item.isRoom ? getRoomType() : getItemType();
  const Exst = fileExst ? fileExst.slice(1).toUpperCase() : "";

  return (
    <StyledText fontSize="12px" fontWeight="400" color={sideColor} truncate>
      {type} {Exst}
    </StyledText>
  );
};
export default TypeCell;
