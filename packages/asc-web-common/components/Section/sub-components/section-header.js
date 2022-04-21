import React from "react";
import styled, { css } from "styled-components";
import classnames from "classnames";
import PropTypes from "prop-types";
import { LayoutContextConsumer } from "studio/Layout/context";
import { isMobile, isMobileOnly } from "react-device-detect";
import { tablet, desktop, mobile } from "@appserver/components/utils/device";
import NoUserSelect from "@appserver/components/utils/commonStyles";

import Base from "@appserver/components/themes/base";

const StyledSectionHeader = styled.div`
  position: relative;

  padding-right: 20px;

  box-sizing: border-box;

  ${NoUserSelect}

  display: grid;
  align-items: center;

  width: 100%;
  max-width: 100%;

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

    margin-top: -2px !important;
  `}
`;

StyledSectionHeader.defaultProps = { theme: Base };

const SectionHeader = (props) => {
  const { viewAs, className, ...rest } = props;

  return (
    <StyledSectionHeader
      className={`section-header ${className}`}
      viewAs={viewAs}
      {...rest}
    />
  );
};

SectionHeader.displayName = "SectionHeader";

SectionHeader.propTypes = {
  isArticlePinned: PropTypes.bool,
  isHeaderVisible: PropTypes.bool,
};
export default SectionHeader;
