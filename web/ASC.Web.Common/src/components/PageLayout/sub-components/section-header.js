import React from "react";
import styled, { css } from "styled-components";
import { utils } from "asc-web-components";
import equal from "fast-deep-equal/react";
import classnames from "classnames";
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
      props.isLoaded &&
      css`
        position: absolute;
        top: 56px;
      `}

    ${(props) =>
      props.borderBottom &&
      `
      border-bottom: 1px solid #eceef1;
      padding-bottom: 16px
    `};
    height: 49px;
    width: ${(props) => !props.isLoaded && "100%"};
  }

  .section-header {
    @media ${tablet} {
      max-width: calc(100vw - 32px);
      width: 100%;
      padding-top: 4px;
      ${(props) =>
        props.isLoaded &&
        css`
          position: fixed;
          top: ${(props) => (!props.isHeaderVisible ? "56px" : "0")};
          width: ${(props) =>
            props.isArticlePinned ? `calc(100% - 272px)` : "100%"};
          background-color: #fff;
          z-index: 155;
          padding-right: 16px;
        `}
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
    return !equal(this.props, nextProps);
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
