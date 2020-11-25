import React from "react";
import styled, { css } from "styled-components";
import { utils } from "asc-web-components";
import isEqual from "lodash/isEqual";
import classnames from "classnames";
import { isSafari } from "react-device-detect";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { LayoutContextConsumer } from "../../Layout/context";

import { getIsLoaded } from "../../../store/auth/selectors";

const { tablet } = utils.device;

const StyledSectionHeader = styled.div`
  border-bottom: 1px solid #eceef1;
  height: 55px;
  margin-right: 24px;
  margin-top: -1px;

  @media ${tablet} {
    margin-right: 16px;
    border-bottom: none;
    ${(props) =>
      props.borderBottom &&
      `
      border-bottom: 1px solid #eceef1;
      padding-bottom: 16px
    `};
    height: 49px;
  }

  .section-header {
    width: calc(100% - 76px);

    @media ${tablet} {
      max-width: calc(100vw - 32px);
      width: ${(props) =>
        props.isArticlePinned ? `calc(100% - 272px)` : "100%"};

      background-color: #fff;

      ${(props) =>
        isSafari
          ? props.isLoaded
            ? css`
                position: fixed;
                top: ${(props) => (!props.isHeaderVisible ? "56px" : "0")};
              `
            : css`
                margin-top: 2px;
              `
          : css`
              position: fixed;
              top: ${(props) => (!props.isHeaderVisible ? "56px" : "0")};
            `};

      z-index: 155;

      padding-right: 16px;

      padding-top: 4px;
    }

    h1,
    h2,
    h3,
    h4,
    h5,
    h6 {
      max-width: calc(100vw - 435px);

      @media ${tablet} {
        max-width: ${(props) =>
          props.isArticlePinned ? `calc(100vw - 320px)` : `calc(100vw - 96px)`};
      }
    }
  }

  .section-header,
  .section-header--hidden {
    @media ${tablet} {
      transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
      -moz-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
      -ms-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
      -webkit-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
      -o-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
    }
  }

  .section-header--hidden {
    @media ${tablet} {
      top: -61px;
    }
  }
`;

class SectionHeader extends React.Component {
  constructor(props) {
    super(props);

    this.focusRef = React.createRef();
  }

  shouldComponentUpdate(nextProps) {
    return !isEqual(this.props, nextProps);
  }

  render() {
    //console.log("PageLayout SectionHeader render");
    // eslint-disable-next-line react/prop-types

    const {
      isArticlePinned,
      borderBottom,
      isHeaderVisible,
      dispatch,
      isLoaded,
      ...rest
    } = this.props;

    return (
      <StyledSectionHeader
        isHeaderVisible={isHeaderVisible}
        isArticlePinned={isArticlePinned}
        borderBottom={borderBottom}
        isLoaded={isLoaded}
      >
        {console.log("isLoaded", isLoaded)}
        <LayoutContextConsumer>
          {(value) => (
            <div
              className={classnames("section-header", {
                "section-header--hidden": !value.isVisible,
              })}
              {...rest}
            />
          )}
        </LayoutContextConsumer>
      </StyledSectionHeader>
    );
  }
}

SectionHeader.displayName = "SectionHeader";
SectionHeader.propTypes = {
  isLoaded: PropTypes.bool,
};

const mapStateToProps = (state) => {
  return {
    isLoaded: getIsLoaded(state),
  };
};
export default connect(mapStateToProps)(SectionHeader);
