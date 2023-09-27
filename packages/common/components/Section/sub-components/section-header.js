import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import { isMobile, isMobileOnly } from "react-device-detect";
import { tablet, mobile } from "@docspace/components/utils/device";
import NoUserSelect from "@docspace/components/utils/commonStyles";

import Base from "@docspace/components/themes/base";

const StyledSectionHeader = styled.div`
  position: relative;
  display: flex;

  height: 69px;
  min-height: 69px;

  @media ${tablet} {
    height: 61px;
    min-height: 61px;

    ${({ isTrashFolder, isEmptyPage }) =>
      isTrashFolder &&
      !isEmptyPage &&
      css`
        height: 109px;
        min-height: 109px;

        .header-container {
          flex-direction: column;
          height: 109px !important;
          min-height: 109px !important;

          .navigation-container {
            height: calc(100% - 32px);
          }
          .trash-warning {
            min-height: 32px;
            height: 32px;

            margin-bottom: 15px;
          }
        }
      `}

    .header-container {
      margin-bottom: 1px;
      -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
    }
  }

  ${isMobile &&
  css`
    height: 61px;
    min-height: 61px;

    ${({ isTrashFolder, isEmptyPage }) =>
      isTrashFolder &&
      !isEmptyPage &&
      css`
        height: 109px;
        min-height: 109px;

        .header-container {
          flex-direction: column;
          height: 109px !important;
          min-height: 109px !important;

          .navigation-container {
            height: calc(100% - 32px);
          }
          .trash-warning {
            min-height: 32px;
            height: 32px;

            margin-bottom: 15px;
          }
        }
      `}
  `}

  @media ${mobile} {
    height: 53px;
    min-height: 53px;

    ${({ isTrashFolder, isEmptyPage }) =>
      isTrashFolder &&
      !isEmptyPage &&
      css`
        height: 101px;
        min-height: 101px;

        .header-container {
          flex-direction: column;
          height: 101px !important;
          min-height: 101px !important;

          .navigation-container {
            height: calc(100% - 32px);
          }
          .trash-warning {
            min-height: 32px;
            height: 32px;

            margin-bottom: 15px;
          }
        }
      `}
  }

  ${isMobileOnly &&
  css`
    height: 53px;
    min-height: 53px;

    ${({ isTrashFolder, isEmptyPage }) =>
      isTrashFolder &&
      !isEmptyPage &&
      css`
        height: 101px;
        min-height: 101px;

        .header-container {
          flex-direction: column;
          height: 101px !important;
          min-height: 101px !important;

          .navigation-container {
            height: calc(100% - 32px);
          }
          .trash-warning {
            min-height: 32px;
            height: 32px;

            margin-bottom: 15px;
          }
        }
      `}
  `}
  ${(props) =>
    props.theme.interfaceDirection === "rtl"
      ? css`
          padding-left: 20px;
        `
      : css`
          padding-right: 20px;
        `}

  box-sizing: border-box;

  ${NoUserSelect}

  display: grid;
  align-items: center;

  width: 100%;
  max-width: 100%;

  .header-container {
    display: flex;
  }

  @media ${tablet} {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding-left: 16px;
            margin-left: 0px;
          `
        : css`
            padding-right: 16px;
            margin-right: 0px;
          `}
  }

  ${isMobile &&
  css`
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding-left: 0 !important;
            margin-left: -16px !important;
          `
        : css`
            padding-right: 0 !important;
            margin-right: -16px !important;
          `}
  `}

  @media ${mobile} {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-left: 0px;
          `
        : css`
            margin-right: 0px;
          `}
  }

  ${isMobileOnly &&
  css`
    width: 100vw !important;
    max-width: 100vw !important;
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding-left: 16px !important;
          `
        : css`
            padding-right: 16px !important;
          `}

    margin-bottom: ${(props) =>
      props.settingsStudio ? "8px !important" : "0"};
  `}
`;

StyledSectionHeader.defaultProps = { theme: Base };

const SectionHeader = (props) => {
  const {
    viewAs,
    settingsStudio = false,
    className,
    isEmptyPage,
    isTrashFolder,
    ...rest
  } = props;

  return (
    <StyledSectionHeader
      className={`section-header ${className}`}
      isEmptyPage={isEmptyPage}
      viewAs={viewAs}
      settingsStudio={settingsStudio}
      isTrashFolder={isTrashFolder}
      {...rest}
    />
  );
};

SectionHeader.displayName = "SectionHeader";

SectionHeader.propTypes = {
  isArticlePinned: PropTypes.bool,
  isHeaderVisible: PropTypes.bool,
  settingsStudio: PropTypes.bool,
};
export default SectionHeader;
