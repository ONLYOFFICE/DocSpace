import React from "react";

import { LANGUAGE } from "@docspace/common/constants";

import getCorrectDate from "@docspace/components/utils/getCorrectDate";

import Link from "@docspace/components/link";
import Text from "@docspace/components/text";
import Tag from "@docspace/components/tag";
import { decode } from "he";

import {
  connectedCloudsTypeTitleTranslation as getProviderTranslation,
  getDefaultRoomName,
  getFileTypeName,
} from "@docspace/client/src/helpers/filesUtils";
import CommentEditor from "../sub-components/CommentEditor";

// Property Content Components

const text = (text) => (
  <Text truncate className="property-content">
    {text}
  </Text>
);

const link = (text, onClick) => (
  <Link
    isTextOverflow
    className="property-content"
    isHovered={true}
    onClick={onClick}
  >
    {text}
  </Link>
);

const tagList = (tags, selectTag) => (
  <div className="property-tag_list">
    {tags.map((tag, i) => (
      <Tag
        key={i}
        className="property-tag"
        label={tag}
        onClick={() => selectTag(tag)}
      />
    ))}
  </div>
);

// Functional Helpers

export const decodeString = (str) => {
  const regex = /&#([0-9]{1,4});/gi;
  return str
    ? str.replace(regex, (match, numStr) => String.fromCharCode(+numStr))
    : "...";
};

export const parseAndFormatDate = (date, personal, culture) => {
  const locale = personal ? localStorage.getItem(LANGUAGE) : culture;
  const correctDate = getCorrectDate(locale, date);
  return correctDate;
};

// InfoHelper Class

class DetailsHelper {
  constructor(props) {
    this.t = props.t;
    this.item = props.item;
    this.history = props.history;
    this.openUser = props.openUser;
    this.personal = props.personal;
    this.culture = props.culture;
    this.isVisitor = props.isVisitor;
    this.isCollaborator = props.isCollaborator;
    this.selectTag = props.selectTag;
  }

  getPropertyList = () => {
    return this.getNeededProperties().map((propertyId) => ({
      id: propertyId,
      className: this.getPropertyClassName(propertyId),
      title: this.getPropertyTitle(propertyId),
      content: this.getPropertyContent(propertyId),
    }));
  };

  getPropertyClassName = (propertyId) => {
    switch (propertyId) {
      case "Owner":
        return "info_details_owner";
      case "Location":
        return "info_details_location";
      case "Type":
        return "info_details_type";
      case "Storage Type":
        return "info_details_storage-type";
      case "File extension":
        return "info_details_file-extension";
      case "Content":
        return "info_details_content";
      case "Size":
        return "info_details_size";
      case "Date modified":
        return "info_details_date_modified";
      case "Last modified by":
        return "info_details_last-modified-by";
      case "Creation date":
        return "info_details_creation-date";
      case "Versions":
        return "info_details_versions";
      case "Comments":
        return "info_details_comments";
      case "Tags":
        return "info_details_tags";
    }
  };

  getNeededProperties = () => {
    return (this.item.isRoom
      ? [
          "Owner",
          this.item.providerKey && "Storage Type",
          "Type",
          "Content",
          "Date modified",
          "Last modified by",
          "Creation date",
          this.item.tags.length && "Tags",
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
        ]
    ).filter((nP) => !!nP);
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
        return this.t("InfoPanel:StorageType");

      case "File extension":
        return this.t("FileExtension");

      case "Content":
        return this.t("Common:Content");
      case "Size":
        return this.t("Common:Size");

      case "Date modified":
        return this.t("DateModified");
      case "Last modified by":
        return this.t("LastModifiedBy");
      case "Creation date":
        return this.t("CreationDate");

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
    const onOpenUser = () => this.openUser(this.item.createdBy, this.history);

    return this.personal || this.isVisitor || this.isCollaborator
      ? text(decode(this.item.createdBy?.displayName))
      : link(decode(this.item.createdBy?.displayName), onOpenUser);
  };

  getItemLocation = () => {
    return text("...");
  };

  getItemType = () => {
    return text(
      this.item.isRoom
        ? getDefaultRoomName(this.item.roomType, this.t)
        : getFileTypeName(this.item.fileType, this.t)
    );
  };

  getItemFileExtention = () => {
    return text(
      this.item.fileExst ? this.item.fileExst.split(".")[1].toUpperCase() : "-"
    );
  };

  getItemStorageType = () => {
    return text(getProviderTranslation(this.item.providerKey, this.t));
  };

  getItemStorageAccount = () => {
    return text("...");
  };

  getItemContent = () => {
    return text(
      `${this.t("Translations:Folders")}: ${this.item.foldersCount} | ${this.t(
        "Translations:Files"
      )}: ${this.item.filesCount}`
    );
  };

  getItemSize = () => {
    return text(this.item.contentLength);
  };

  getItemDateModified = () => {
    return text(
      parseAndFormatDate(this.item.updated, this.personal, this.culture)
    );
  };

  getItemLastModifiedBy = () => {
    const onOpenUser = () => this.openUser(this.item.updatedBy, this.history);

    return this.personal || this.isVisitor || this.isCollaborator
      ? text(decode(this.item.updatedBy?.displayName))
      : link(decode(this.item.updatedBy?.displayName), onOpenUser);
  };

  getItemCreationDate = () => {
    return text(
      parseAndFormatDate(this.item.created, this.personal, this.culture)
    );
  };

  getItemVersions = () => {
    return text(this.item.version);
  };

  getItemComments = () => {
    return <CommentEditor t={this.t} item={this.item} />;
  };

  getItemTags = () => {
    return tagList(this.item.tags, this.selectTag);
  };
}

export default DetailsHelper;
