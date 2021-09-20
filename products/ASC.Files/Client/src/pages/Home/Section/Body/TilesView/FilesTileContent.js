import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";

import Link from "@appserver/components/link";
import Text from "@appserver/components/text";

import TileContent from "./sub-components/TileContent";
import withContent from "../../../../../HOCs/withContent";
import withBadges from "../../../../../HOCs/withBadges";

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

  .badge-ext {
    margin-left: -8px;
    margin-right: 8px;
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
  .can-convert {
    height: 14px;
    width: 14px;
  }

  @media (max-width: 1024px) {
    display: inline-flex;
    height: auto;

    & > div {
      margin-top: 0;
    }
  }
`;

const FilesTileContent = ({
  item,
  titleWithoutExt,
  linkStyles,
  onFilesClick,
  badgesComponent,
}) => {
  const { fileExst } = item;

  return (
    <>
      <SimpleFilesTileContent
        sideColor="#333"
        isFile={fileExst}
        //disableSideInfo
      >
        <Link
          className="title-link item-file-name"
          containerWidth="100%"
          type="page"
          title={titleWithoutExt}
          fontWeight="600"
          fontSize="14px"
          target="_blank"
          href={item.href}
          {...linkStyles}
          color="#333"
          isTextOverflow
        >
          {titleWithoutExt}
          {fileExst ? (
            <Text
              className="badge-ext"
              as="span"
              color="#A3A9AE"
              fontSize="14px"
              fontWeight={600}
              title={fileExst}
              truncate={true}
            >
              {fileExst}
            </Text>
          ) : null}
        </Link>

        <div className="badges">{badgesComponent}</div>
      </SimpleFilesTileContent>
    </>
  );
};

export default withRouter(
  withTranslation(["Home", "Translations"])(
    withContent(withBadges(FilesTileContent))
  )
);
