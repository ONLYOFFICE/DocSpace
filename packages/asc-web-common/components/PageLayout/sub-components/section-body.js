import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
//import equal from "fast-deep-equal/react";
//import { LayoutContextConsumer } from "studio/Layout/context";
import { isMobile } from "react-device-detect";
import { inject, observer } from "mobx-react";

import Scrollbar from "@appserver/components/scrollbar";
import DragAndDrop from "@appserver/components/drag-and-drop";
import { tablet } from "@appserver/components/utils/device";

const commonStyles = css`
  flex-grow: 1;
  height: 100%;

  -webkit-user-select: none;

  .section-wrapper-content {
    flex: 1 0 auto;
    padding: 17px 7px 16px 24px;
    outline: none;
    ${(props) => props.viewAs == "tile" && "padding-right:0;"}

    @media ${tablet} {
      padding: 16px 0 16px 24px;
    }

    .section-wrapper {
      display: flex;
      flex-direction: column;
      min-height: 100%;
    }

    .people-row-container,
    .files-row-container {
      margin-top: -22px;
    }
  }
`;

const StyledSectionBody = styled.div`
  ${commonStyles}

  ${(props) =>
    props.withScroll &&
    `

    margin-left: -24px;
  `}
`;

const StyledDropZoneBody = styled(DragAndDrop)`
  ${commonStyles}

  .drag-and-drop {
    user-select: none;
    height: 100%;
  }

  ${(props) =>
    props.withScroll &&
    `
    margin-left: -24px;
  `}
`;

const StyledSpacer = styled.div`
  display: none;
  min-height: 64px;

  @media ${tablet} {
    display: ${(props) => (props.pinned ? "none" : "block")};
  }
`;

class SectionBody extends React.Component {
  constructor(props) {
    super(props);

    this.focusRef = React.createRef();
  }

  // shouldComponentUpdate(nextProps) {
  //   return !equal(this.props, nextProps);
  // }

  componentDidMount() {
    if (!this.props.autoFocus) return;
    this.focusRef.current.focus();
  }

  componentWillUnmount() {
    this.focusRef = null;
  }

  render() {
    //console.log("PageLayout SectionBody render" );
    const {
      autoFocus,
      children,
      onDrop,
      pinned,
      uploadFiles,
      viewAs,
      withScroll,
      isLoaded,
    } = this.props;

    const focusProps = autoFocus
      ? {
          ref: this.focusRef,
          tabIndex: -1,
        }
      : {};

    return uploadFiles ? (
      <StyledDropZoneBody
        isDropZone
        onDrop={onDrop}
        withScroll={withScroll}
        viewAs={viewAs}
        pinned={pinned}
        isLoaded={isLoaded}
        className="section-body"
      >
        {withScroll ? (
          !isMobile ? (
            <Scrollbar scrollclass="section-scroll" stype="mediumBlack">
              <div className="section-wrapper">
                <div className="section-wrapper-content" {...focusProps}>
                  {children}
                  <StyledSpacer pinned={pinned} />
                </div>
              </div>
            </Scrollbar>
          ) : (
            <div className="section-wrapper">
              <div className="section-wrapper-content" {...focusProps}>
                {children}
                <StyledSpacer pinned={pinned} />
              </div>
            </div>
          )
        ) : (
          <div className="section-wrapper">
            {children}
            <StyledSpacer pinned={pinned} />
          </div>
        )}
      </StyledDropZoneBody>
    ) : (
      <StyledSectionBody
        viewAs={viewAs}
        withScroll={withScroll}
        pinned={pinned}
        isLoaded={isLoaded}
      >
        {withScroll ? (
          !isMobile ? (
            <Scrollbar stype="mediumBlack">
              <div className="section-wrapper">
                <div className="section-wrapper-content" {...focusProps}>
                  {children}
                  <StyledSpacer pinned={pinned} />
                </div>
              </div>
            </Scrollbar>
          ) : (
            <div className="section-wrapper">
              <div className="section-wrapper-content" {...focusProps}>
                {children}
                <StyledSpacer pinned={pinned} />
              </div>
            </div>
          )
        ) : (
          <div className="section-wrapper">
            {children}
            <StyledSpacer pinned={pinned} />
          </div>
        )}
      </StyledSectionBody>
    );
  }
}

SectionBody.displayName = "SectionBody";

SectionBody.propTypes = {
  withScroll: PropTypes.bool,
  autoFocus: PropTypes.bool,
  pinned: PropTypes.bool,
  onDrop: PropTypes.func,
  uploadFiles: PropTypes.bool,
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
    PropTypes.any,
  ]),
  viewAs: PropTypes.string,
  isLoaded: PropTypes.bool,
};

SectionBody.defaultProps = {
  autoFocus: false,
  pinned: false,
  uploadFiles: false,
  withScroll: true,
};

export default inject(({ auth }) => {
  return {
    isLoaded: auth.isLoaded,
  };
})(observer(SectionBody));
