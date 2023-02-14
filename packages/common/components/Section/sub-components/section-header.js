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

    .header-container {
      margin-bottom: 1px;
      -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
    }
  }

  ${isMobile &&
  css`
    height: 61px;
    min-height: 61px;
  `}

  @media ${mobile} {
    height: 53px;
    min-height: 53px;
  }

  ${isMobileOnly &&
  css`
    height: 53px;
    min-height: 53px;
  `}

  ${({ isTrashFolder, isEmptyPage }) =>
    isTrashFolder &&
    !isEmptyPage &&
    css`
      @media ${tablet} {
        height: 109px;
        min-height: 109px;
      }
    `}

  padding-right: 20px;

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
    padding-right: 16px;
    margin-right: 0px;
  }

  ${isMobile &&
  css`
    padding-right: 0 !important;
    margin-right: -16px !important;
  `}

  @media ${mobile} {
    margin-right: 0px;
  }

  ${isMobileOnly &&
  css`
    width: 100vw !important;
    max-width: 100vw !important;

    padding-right: 16px !important;

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
    ...rest
  } = props;

  const pathname = window.location.pathname.toLowerCase();
  const isTrashFolder = pathname.indexOf("trash") !== -1;

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
