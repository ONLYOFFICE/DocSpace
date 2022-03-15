import React from "react";
import styled, { css } from "styled-components";
import equal from "fast-deep-equal/react";
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
  margin-right: 20px;

  ${NoUserSelect}

  @media ${tablet} {
    height: 61px;
    margin-right: 16px;
  }

  ${isMobile &&
  css`
    height: 61px !important;
    margin-top: 48px !important;
    margin-right: 0px !important;
  `}

  @media ${mobile} {
    max-width: calc(100vw - 32px);
    height: 53px;
    margin-top: 0px;
    margin-right: 0px;
  }

  ${isMobileOnly &&
  css`
    max-width: calc(100vw - 32px);
    height: 53px !important;
    margin-top: 48px !important;
    margin-right: 0px !important;
  `}

  ${isMobile &&
  css`
    .section-header,
    .section-header--hidden {
      &,
      .group-button-menu-container > div:first-child {
        transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
        -moz-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
        -ms-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
        -webkit-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
        -o-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
      }
      .group-button-menu-container {
        padding-bottom: 0;
        > div:first-child {
          top: ${(props) => (!props.isSectionHeaderVisible ? "56px" : "0px")};

          @media ${desktop} {
            ${isMobile &&
            css`
              position: absolute;
            `}
          }
        }
      }
    }
  `}
  .section-header--hidden {
    ${isMobile &&
    css`
      top: -61px;
    `}
  }
`;

StyledSectionHeader.defaultProps = { theme: Base };

class SectionHeader extends React.Component {
  constructor(props) {
    super(props);

    this.focusRef = React.createRef();
  }

  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    // console.log("PageLayout SectionHeader render");
    // eslint-disable-next-line react/prop-types

    const { isArticlePinned, isHeaderVisible, viewAs, ...rest } = this.props;

    return (
      <LayoutContextConsumer>
        {(value) => (
          <StyledSectionHeader
            isArticlePinned={isArticlePinned}
            isSectionHeaderVisible={value.isVisible}
            viewAs={viewAs}
          >
            <div
              className={classnames("section-header hidingHeader", {
                "section-header--hidden":
                  value.isVisible === undefined ? false : !value.isVisible,
              })}
              {...rest}
            />
          </StyledSectionHeader>
        )}
      </LayoutContextConsumer>
    );
  }
}

SectionHeader.displayName = "SectionHeader";

SectionHeader.propTypes = {
  isArticlePinned: PropTypes.bool,
  isHeaderVisible: PropTypes.bool,
};
export default SectionHeader;
