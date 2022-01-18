import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";

import Link from "@appserver/components/link";

import TileContent from "./sub-components/TileContent";
import withContent from "../../../../../HOCs/withContent";
import withBadges from "../../../../../HOCs/withBadges";
import { isMobile } from "react-device-detect";

const SimpleFilesTileContent = styled(TileContent)`
  .row-main-container {
    height: auto;
    max-width: 100%;
    align-self: flex-end;

    a {
      word-break: break-word;
    }
  }

  .main-icons {
    align-self: flex-end;
  }

  .badge {
    margin-right: 8px;
    cursor: pointer;
    height: 16px;
    width: 16px;
  }

  .new-items {
    position: absolute;
    right: 29px;
    top: 19px;
  }

  .badges {
    display: flex;
    align-items: center;
  }

  .share-icon {
    margin-top: -4px;
    padding-right: 8px;
  }

  .title-link {
    font-size: 14px;
  }

  .favorite,
  .can-convert,
  .edit {
    svg:not(:root) {
      width: 14px;
      height: 14px;
    }
  }

  @media (max-width: 1024px) {
    display: inline-flex;
    height: auto;

    & > div {
      margin-top: 0;
    }
  }
`;

const FilesTileContent = ({ item, titleWithoutExt, linkStyles }) => {
  const { fileExst, title } = item;

  return (
    <>
      <SimpleFilesTileContent sideColor="#333" isFile={fileExst}>
        <Link
          className="title-link item-file-name"
          containerWidth="100%"
          type="page"
          title={title}
          fontWeight="600"
          fontSize={isMobile ? "15px" : "13px"}
          target="_blank"
          {...linkStyles}
          color="#333"
          isTextOverflow
        >
          {titleWithoutExt}
        </Link>
      </SimpleFilesTileContent>
    </>
  );
};

export default withRouter(
  withTranslation(["Home", "Translations"])(
    withContent(withBadges(FilesTileContent))
  )
);
