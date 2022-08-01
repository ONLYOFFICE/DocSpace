import React from "react";
import styled, { css } from "styled-components";
//import classnames from "classnames";
import PropTypes from "prop-types";
//import { LayoutContextConsumer } from "client/Layout/context";
import { isMobile, isMobileOnly } from "react-device-detect";
import { tablet, mobile } from "@docspace/components/utils/device";
import NoUserSelect from "@docspace/components/utils/commonStyles";

import Base from "@docspace/components/themes/base";

const StyledSectionHeader = styled.div`
  position: relative;

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
  const { viewAs, settingsStudio = false, className, ...rest } = props;

  return (
    <StyledSectionHeader
      className={`section-header ${className}`}
      viewAs={viewAs}
      settingsStudio={settingsStudio}
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
