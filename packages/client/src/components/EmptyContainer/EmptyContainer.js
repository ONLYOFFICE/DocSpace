import React from "react";
import styled, { css } from "styled-components";
import EmptyScreenContainer from "@docspace/components/empty-screen-container";
import NoUserSelect from "@docspace/components/utils/commonStyles";
import {
  tablet,
  smallTablet,
  desktop,
  size,
} from "@docspace/components/utils/device";
import { isMobile, isMobileOnly } from "react-device-detect";
import { classNames } from "@docspace/components/utils/classNames";

const EmptyPageStyles = css`
  padding: 44px 0px 64px 0px;

  grid-column-gap: 40px;
  grid-template-columns: 100px 1fr;

  .empty-folder_link:not(:last-child) {
    margin-bottom: 10px;
  }

  .empty-folder_link {
    margin-right: 9px;
  }

  @media ${desktop} {
    .empty-folder_link:not(:last-child) {
      margin-bottom: 2px;
    }
  }

  @media ${tablet} {
    padding: 44px 0px 64px 0px;
    grid-column-gap: 33px;
  }

  @media ${smallTablet} {
    padding-right: 44px;
  }
`;

const EmptyFolderWrapper = styled.div`
  .empty-folder_container {
    .empty-folder_link {
      margin-right: 7px;
    }

    .empty-folder_container-links {
      display: grid;
      margin: 13px 0;
      grid-template-columns: 12px 1fr;
      grid-column-gap: 8px;
    }

    .second-description {
      display: grid;
      grid-template-columns: 1fr;

      margin: 32px 0 26px !important;
    }

    .flex-wrapper_container {
      display: flex;
      flex-wrap: wrap;

      row-gap: 16px;
    }

    .empty-folder_container-image {
      margin-top: 3px;
      cursor: pointer;
    }

    .empty-folder_container_up-image,
    .empty-folder_container_plus-image {
      margin: 4px 8px 0 0;
      cursor: pointer;
      width: 10px;
      height: 10px;
    }

    .empty-folder_container_plus-image {
      display: flex;
      line-height: unset;
      ${NoUserSelect}
    }
    .empty-folder_container_up-image {
      ${NoUserSelect}
    }

    .empty-folder_container-icon {
      height: 20px;
      width: 12px;
      margin: 4px 4px 0 0;
      cursor: pointer;
    }

    .empty-connect_container-links {
      position: relative;
      bottom: 16px;
    }

    ${(props) => props.isEmptyPage && `${EmptyPageStyles}`}

    ${(props) =>
      props.isEmptyPage &&
      isMobileOnly &&
      css`
        padding: 20px 42px 64px 11px !important;
      `}

    ${(props) =>
      (props.isEmptyPage || props.isEmptyFolderContainer) &&
      props.sectionWidth <= size.smallTablet &&
      !isMobileOnly &&
      css`
        padding-left: 12px !important;

        .empty-folder_link {
          margin-bottom: 0 !important;
        }
      `}
  }

  .empty-folder_room-not-found {
    margin-top: 70px;
  }
`;

const EmptyFoldersContainer = (props) => {
  const imageAlt = "Empty folder image";
  const {
    imageSrc,
    headerText,
    subheadingText,
    descriptionText,
    buttons,
    style,
    imageStyle,
    buttonStyle,
    isEmptyPage,
    sectionWidth,
    isEmptyFolderContainer,
    className,
  } = props;

  return (
    <EmptyFolderWrapper
      sectionWidth={sectionWidth}
      isEmptyPage={isEmptyPage}
      isEmptyFolderContainer={isEmptyFolderContainer}
    >
      <EmptyScreenContainer
        sectionWidth={sectionWidth}
        className={classNames("empty-folder_container", className)}
        style={style}
        imageStyle={imageStyle}
        imageSrc={imageSrc}
        imageAlt={imageAlt}
        buttonStyle={buttonStyle}
        headerText={headerText}
        subheadingText={subheadingText}
        descriptionText={descriptionText}
        buttons={buttons}
        isEmptyPage={isEmptyPage}
        isEmptyFolderContainer={isEmptyFolderContainer}
      />
    </EmptyFolderWrapper>
  );
};

export default EmptyFoldersContainer;
