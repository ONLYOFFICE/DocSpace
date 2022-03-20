import RectangleLoader from "@appserver/common/components/Loaders/RectangleLoader";
import { FileType } from "@appserver/common/constants";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import Tooltip from "@appserver/components/tooltip";
import { inject, observer } from "mobx-react";
import React, { useEffect, useState } from "react";
import { withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";
import { combineUrl } from "@appserver/common/utils";
//import config from "@appserever/files/package.json";

import { AppServerConfig } from "@appserver/common/constants";

import {
  StyledAccess,
  StyledAccessUser,
  StyledOpenSharingPanel,
  StyledProperties,
  StyledSubtitle,
  StyledThumbnail,
  StyledTitle,
} from "./styles/styles.js";

const SingleItem = (props) => {
  const {
    t,
    currentFolderItem,
    isRecycleBinFolder,
    onSelectItem,
    setSharingPanelVisible,
    getFolderInfo,
    getIcon,
    getFolderIcon,
    getShareUsers,
  } = props;

  const [item, setItem] = useState({
    id: currentFolderItem.id,
    isFolder: true,
    title: currentFolderItem.title,
    iconUrl: currentFolderItem.iconUrl,
    thumbnailUrl: currentFolderItem.thumbnailUrl,
    properties: [],
    access: {
      owner: {
        img: "",
        link: "",
      },
      others: [],
    },
  });

  console.log(currentFolderItem);

  return (
    <>
      <StyledTitle>
        <ReactSVG className="icon" src={item.iconUrl} />
        <Text className="text" fontWeight={600} fontSize="16px">
          {item.title}
        </Text>
      </StyledTitle>

      <div className="no-thumbnail-img-wrapper">
        <ReactSVG className="no-thumbnail-img" src={item.thumbnailUrl} />
      </div>

      <StyledSubtitle>
        <Text fontWeight="600" fontSize="14px" color="#000000">
          {t("SystemProperties")}
        </Text>
      </StyledSubtitle>

      <StyledProperties>
        {item.properties.map((p) => (
          <div key={p.title} className="property">
            <Text className="property-title">{p.title}</Text>
            {p.content}
          </div>
        ))}
      </StyledProperties>
    </>
  );
};

export default inject(({}) => {
  return {};
})(
  withTranslation(["InfoPanel", "Home", "Common", "Translations"])(
    observer(SingleItem)
  )
);
