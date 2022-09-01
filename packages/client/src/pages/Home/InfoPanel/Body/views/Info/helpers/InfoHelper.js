import React from "react";

import { LANGUAGE } from "@docspace/common/constants";

import getCorrectDate from "@docspace/components/utils/getCorrectDate";

import Link from "@docspace/components/link";
import Text from "@docspace/components/text";
import Tag from "@docspace/components/tag";

import {
  connectedCloudsTypeTitleTranslation,
  getDefaultRoomName,
  getFileTypeName,
} from "@docspace/client/src/helpers/filesUtils";

// Property Content Components

const styledText = (text) => (
  <Text truncate className="property-content">
    {text}
  </Text>
);

const styledLink = (text, href) => (
  <Link
    isTextOverflow
    className="property-content"
    href={href}
    isHovered={true}
  >
    {text}
  </Link>
);

const styledTagList = (tags) => (
  <div className="property-tag_list">
    {tags.map((tag) => (
      <Tag className="property-tag" label={tag} />
    ))}
  </div>
);

// Functional Helpers

const decodeString = (str) => {
  const regex = /&#([0-9]{1,4});/gi;
  return str
    ? str.replace(regex, (match, numStr) => String.fromCharCode(+numStr))
    : "...";
};

const parseAndFormatDate = (date, personal, culture) => {
  const locale = personal ? localStorage.getItem(LANGUAGE) : culture;
  const correctDate = getCorrectDate(locale, date);
  return correctDate;
};

// InfoHelper Class

class InfoHelper {
  constructor(t, item, personal, culture) {
    this.t = t;
    this.item = item;
    this.personal = personal;
    this.culture = culture;
  }

  getPropertyList = () => {
    return this.getNeededProperties().map((propertyId) => ({
      id: propertyId,
      title: this.getPropertyTitle(propertyId),
      content: this.getPropertyContent(propertyId),
    }));
  };

  getNeededProperties = () => {
    return this.item.isRoom
      ? [
          "Owner",
          "Storage Type",
          "Storage account",
          "Type",
          "Content",
          "Date modified",
          "Last modified by",
          "Creation date",
          "Tags",
        ]
      : this.item.isFolder
      ? [
          "Owner",
          //"Location",
          "Type",
          "Content",
          "Date modified",
          "Last modified by",
          "Creation date",
        ]
      : [
          "Owner",
          //"Location",
          "Type",
          "File extension",
          "Size",
          "Date modified",
          "Last modified by",
          "Creation date",
          "Versions",
          "Comments",
        ];
  };

  getPropertyTitle = (propertyId) => {
    switch (propertyId) {
      case "Owner":
        return this.t("Common:Owner");
      case "Location":
        return this.t("Common:Location");

      case "Type":
        return this.t("Common:Type");
      case "Storage Type":
        return this.t("InfoPanel:Storage type");
      case "Storage account":
        return this.t("InfoPanel:Storage account");

      case "File extension":
        return this.t("FileExtension");

      case "Content":
        return this.t("Common:Content");
      case "Size":
        return this.t("Common:Size");

      case "Date modified":
        return this.t("Files:ByLastModifiedDate");
      case "Last modified by":
        return this.t("LastModifiedBy");
      case "Creation date":
        return this.t("Files:ByCreationDate");

      case "Versions":
        return this.t("InfoPanel:Versions");
      case "Comments":
        return this.t("Common:Comments");
      case "Tags":
        return this.t("Common:Tags");
    }
  };

  getPropertyContent = (propertyId) => {
    switch (propertyId) {
      case "Owner":
        return this.getItemOwner();
      case "Location":
        return this.getItemLocation();

      case "Type":
        return this.getItemType();
      case "Storage Type":
        return this.getItemStorageType();
      case "Storage account":
        return this.getItemStorageAccount();

      case "File extension":
        return this.getItemFileExtention();

      case "Content":
        return this.getItemContent();
      case "Size":
        return this.getItemSize();

      case "Date modified":
        return this.getItemDateModified();
      case "Last modified by":
        return this.getItemLastModifiedBy();
      case "Creation date":
        return this.getItemCreationDate();

      case "Versions":
        return this.getItemVersions();
      case "Comments":
        return this.getItemComments();
      case "Tags":
        return this.getItemTags();
    }
  };

  /// Property  //

  getItemOwner = () => {
    return this.personal
      ? styledText(decodeString(this.item.createdBy?.displayName))
      : styledLink(
          decodeString(this.item.createdBy?.displayName),
          this.item.createdBy?.profileUrl
        );
  };

  getItemLocation = () => {
    return styledText("...");
  };

  getItemType = () => {
    return styledText(
      this.item.isRoom
        ? getDefaultRoomName(this.item.roomType, this.t)
        : getFileTypeName(this.item.fileType, this.t)
    );
  };

  getItemFileExtention = () => {
    return styledText(
      this.item.fileExst ? this.item.fileExst.split(".")[1].toUpperCase() : "-"
    );
  };

  getItemStorageType = () => {
    return styledText(
      this.item.providerKey
        ? connectedCloudsTypeTitleTranslation(this.item.providerKey, this.t)
        : "ONLYOFFICE DocSpace"
    );
  };

  getItemStorageAccount = () => {
    return styledText("...");
  };

  getItemContent = () => {
    return styledText(
      `${this.t("Translations:Folders")}: ${this.item.foldersCount} | ${this.t(
        "Translations:Files"
      )}: ${this.item.filesCount}`
    );
  };

  getItemSize = () => {
    return styledText(this.item.contentLength);
  };

  getItemDateModified = () => {
    return styledText(
      parseAndFormatDate(this.item.updated, this.personal, this.culture)
    );
  };

  getItemLastModifiedBy = () => {
    return this.personal
      ? styledText(decodeString(this.item.updatedBy?.displayName))
      : styledLink(
          decodeString(this.item.updatedBy?.displayName),
          this.item.updatedBy?.profileUrl
        );
  };

  getItemCreationDate = () => {
    return styledText(
      parseAndFormatDate(this.item.created, this.personal, this.culture)
    );
  };

  getItemVersions = () => {
    return styledText(this.item.version);
  };

  getItemComments = () => {
    return styledText(this.item.comment);
  };

  getItemTags = () => {
    return styledTagList(this.item.tags);
  };
}

export default InfoHelper;
