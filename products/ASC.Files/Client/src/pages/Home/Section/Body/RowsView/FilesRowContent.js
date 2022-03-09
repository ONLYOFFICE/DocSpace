import React, { useCallback } from "react";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import { isMobile, isTablet } from "react-device-detect";

import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import RowContent from "@appserver/components/row-content";

import withContent from "../../../../../HOCs/withContent";
import withBadges from "../../../../../HOCs/withBadges";
import { Base } from "@appserver/components/themes";

const SimpleFilesRowContent = styled(RowContent)`
  .row-main-container-wrapper {
    width: 100%;
    max-width: min-content;
    min-width: inherit;
    margin-right: 0px;
  }

  .row_update-text {
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .new-items {
    min-width: 12px;
    width: max-content;
    margin: 0 -2px -2px -2px;
  }

  .badge-version {
    width: max-content;
    margin: -2px 6px -2px -2px;
  }

  .badge-new-version {
    width: max-content;
  }

  .is-editing {
    path {
      fill: ${(props) => props.theme.filesSection.rowView.editingIconColor};
    }
  }
  ${(props) =>
    ((props.sectionWidth <= 1024 && props.sectionWidth > 500) || isTablet) &&
    `
    .row-main-container-wrapper {
      display: flex;
      justify-content: space-between;
      max-width: inherit;
    }

    .badges {
      flex-direction: row-reverse;
    }

    .tablet-badge {
      margin-top: 5px;
    }

    .tablet-edit,
    .can-convert {
     margin-top: 6px;
     margin-right: 24px !important;
    }

    .badge-version {
      margin-right: 22px;
    }

    .new-items {
      min-width: 16px;
      margin: 7px 22px 0 0;
    }
  `}
`;

SimpleFilesRowContent.defaultProps = { theme: Base };

const FilesRowContent = ({
  t,
  item,
  sectionWidth,
  titleWithoutExt,
  updatedDate,
  linkStyles,
  badgesComponent,
  quickButtons,
  theme,
}) => {
  const {
    contentLength,
    fileExst,
    filesCount,
    foldersCount,
    providerKey,
    title,
  } = item;

  return (
    <>
      <SimpleFilesRowContent
        sectionWidth={sectionWidth}
        isMobile={isMobile}
        isFile={fileExst || contentLength}
      >
        <Link
          className="row-content-link"
          containerWidth="55%"
          type="page"
          title={title}
          fontWeight="600"
          fontSize="15px"
          target="_blank"
          {...linkStyles}
          isTextOverflow={true}
        >
          {titleWithoutExt}
        </Link>
        <div className="badges">
          {badgesComponent}
          {quickButtons}
        </div>
        {!!fileExst && (
          <Text
            containerMinWidth="200px"
            containerWidth="15%"
            fontSize="12px"
            fontWeight={400}
            // color={sideColor}
            className="row_update-text"
          >
            {updatedDate && updatedDate}
          </Text>
        )}
        <Text
          containerMinWidth="90px"
          containerWidth="10%"
          as="div"
          className="row-content-text"
          fontSize="12px"
          fontWeight={400}
          title=""
          truncate={true}
        >
          {!fileExst && !contentLength && !providerKey
            ? `${foldersCount} ${t("Folders")} | ${filesCount} ${t("Files")}`
            : ""}
        </Text>
      </SimpleFilesRowContent>
    </>
  );
};

export default inject(({ auth }) => {
  return { theme: auth.settingsStore.theme };
})(
  observer(
    withRouter(
      withTranslation(["Home", "Translations"])(
        withContent(withBadges(FilesRowContent))
      )
    )
  )
);
