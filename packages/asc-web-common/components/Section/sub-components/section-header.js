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
  height: 53px;
  min-height: 53px;
  margin-right: 20px;
  ${NoUserSelect}

  width: calc(100vw - 296px);
  max-width: calc(100vw - 296px);

  @media ${tablet} {
    width: ${(props) =>
      props.showText ? "calc(100vw - 272px)" : "calc(100vw - 84px)"};
    max-width: ${(props) =>
      props.showText ? "calc(100vw - 272px)" : "calc(100vw - 84px)"};
    height: 61px;
    min-height: 61px;
    margin-right: 0px !important;
  }

  ${isMobile &&
  css`
    width: ${(props) =>
      props.showText ? "calc(100vw - 272px)" : "calc(100vw - 84px)"} !important;
    max-width: ${(props) =>
      props.showText ? "calc(100vw - 272px)" : "calc(100vw - 84px)"} !important;
    height: 61px !important;
    min-height: 61px !important;
    margin-right: 0px !important;
  `}

  @media ${mobile} {
    width: calc(100vw - 32px) !important;
    max-width: calc(100vw - 32px) !important;
    height: 53px;
    min-height: 53px;
    margin-right: 0px !important;
  }

  ${isMobileOnly &&
  css`
    width: calc(100vw - 32px) !important;
    max-width: calc(100vw - 32px) !important;
    height: 53px;
    min-height: 53px;
    margin-top: -2px;
    margin-right: 0px !important;
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
