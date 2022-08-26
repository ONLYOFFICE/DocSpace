import React from "react";

import { LANGUAGE } from "@docspace/common/constants";
import { FileType } from "@docspace/common/constants";

import getCorrectDate from "@docspace/components/utils/getCorrectDate";

import Link from "@docspace/components/link";
import Text from "@docspace/components/text";

class InfoHelper {
  constructor(t, item, personal, culture, getFolderIcon, getIcon) {
    this.t = t;
    this.item = item;
    this.personal = personal;
    this.culture = culture;
    this.getFolderIcon = getFolderIcon;
    this.getIcon = getIcon;
  }

  styledLink = (text, href) => (
    <Link
      isTextOverflow
      className="property-content"
      href={href}
      isHovered={true}
    >
      {text}
    </Link>
  );

  styledText = (text) => (
    <Text truncate className="property-content">
      {text}
    </Text>
  );

  getNeededProperties = () => {
    return this.item.isRoom
      ? [
          "Owner",
          "Storage Type",
          "Storage account",
          "Type",
          "Size",
          "Date modified",
          "Last modified by",
          "Creation date",
          "Tags",
        ]
      : this.item.isFolder
      ? [
          "Owner",
          "Location",
          "Type",
          "Size",
          "Date modified",
          "Last modified by",
          "Creation date",
        ]
      : [
          "Owner",
          "Location",
          "Type",
          "File extension",
          "Size",
          "Date modified",
          "Last modified by",
          "Creation date",
        ];
  };

  getItemIcon = (size) => {
    console.log(this.item.providerKey, size);
    return this.item.isRoom
      ? this.item.logo && this.item.logo.big
        ? this.item.logo.big
        : this.item.icon
      : this.item.isFolder
      ? this.getFolderIcon(this.item.providerKey, size)
      : this.getIcon(size, this.item.fileExst || ".file");
  };

  getItemSize = () => {
    return this.item.isFolder
      ? `${this.t("Translations:Folders")}: ${
          this.item.foldersCount
        } | ${this.t("Translations:Files")}: ${this.item.filesCount}`
      : this.item.contentLength;
  };

  getItemType = () => {
    const type = this.item.fileType;
    switch (type) {
      case FileType.Unknown:
        return this.t("Common:Unknown");
      case FileType.Archive:
        return this.t("Common:Archive");
      case FileType.Video:
        return this.t("Common:Video");
      case FileType.Audio:
        return this.t("Common:Audio");
      case FileType.Image:
        return this.t("Common:Image");
      case FileType.Spreadsheet:
        return this.t("Home:Spreadsheet");
      case FileType.Presentation:
        return this.t("Home:Presentation");
      case FileType.Document:
        return this.t("Home:Document");
      default:
        return this.t("Home:Folder");
    }
  };

  decodeString = (str) => {
    console.log(str);
    const regex = /&#([0-9]{1,4});/gi;
    return str
      ? str.replace(regex, (match, numStr) => String.fromCharCode(+numStr))
      : "...";
  };

  parseAndFormatDate = (date) => {
    const locale = this.personal
      ? localStorage.getItem(LANGUAGE)
      : this.culture;
    const correctDate = getCorrectDate(locale, date);
    return correctDate;
  };
}

export default InfoHelper;
