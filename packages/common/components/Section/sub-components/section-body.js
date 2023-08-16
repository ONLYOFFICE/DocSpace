import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
//import equal from "fast-deep-equal/react";
//import { LayoutContextConsumer } from "client/Layout/context";
import { isMobile, isMobileOnly } from "react-device-detect";
import { inject, observer } from "mobx-react";

import Scrollbar from "@docspace/components/scrollbar";
import DragAndDrop from "@docspace/components/drag-and-drop";
import {
  tablet,
  desktop,
  smallTablet,
  mobile,
} from "@docspace/components/utils/device";

const settingsStudioStyles = css`
  ${({ settingsStudio }) =>
    settingsStudio
      ? css`
          padding: 0 7px 16px 20px;

          @media ${tablet} {
            padding: 0 0 16px 24px;
          }

          @media ${smallTablet} {
            padding: 8px 0 16px 24px;
          }

          @media ${mobile} {
            padding: 0 0 16px 24px;
          }
        `
      : css`
          @media ${tablet} {
            padding: 19px 0 16px 24px;
          }
        `}
`;

const paddingStyles = css`
  padding: 19px 3px 16px 20px;
  outline: none;

  ${settingsStudioStyles};

  ${isMobile &&
  css`
    padding: 0 0 16px 23px !important;
  `};

  ${isMobileOnly &&
  css`
    padding: 0px 0 16px 24px !important;
  `};
`;

const commonStyles = css`
  flex-grow: 1;

  ${(props) => (props.isDesktop ? "height: auto" : "height: 100%")};

  ${(props) => !props.withScroll && `height: 100%;`}
  border-left: none;
  border-right: none;
  border-top: none;

  .section-wrapper {
    height: 100%;
    ${(props) =>
      !props.withScroll &&
      css`
        display: flex;
        flex-direction: column;
        height: 100%;
        box-sizing: border-box;
      `};
    ${(props) => !props.withScroll && paddingStyles}
  }

  .section-wrapper-content {
    ${paddingStyles}
    flex: 1 0 auto;
    outline: none;
    ${(props) =>
      props.viewAs == "tile" &&
      css`
        padding-left: 20px;
      `}

    ${(props) =>
      (props.viewAs == "settings" || props.viewAs == "profile") &&
      css`
        padding-top: 0;

        @media ${tablet} {
          padding-top: 0;
        }
      `}

    .section-wrapper {
      display: flex;
      flex-direction: column;
      min-height: 100%;
    }

    .files-tile-container {
      margin-top: ${isMobile ? "-12px" : "0px"};
    }

    .people-row-container,
    .files-row-container {
      margin-top: -22px;

      ${!isMobile &&
      css`
        margin-top: -17px;
      `}

      @media ${desktop} {
        ${(props) =>
          props.viewAs === "row" &&
          css`
            margin-top: -15px;
          `}
      }
    }
  }
`;

const StyledSectionBody = styled.div`
  max-width: 100vw !important;
  user-select: none;

  ${commonStyles};

  ${(props) =>
    props.withScroll &&
    css`
      margin-left: -20px;

      @media ${tablet} {
        margin-left: -24px;
      }
    `}

  ${isMobile &&
  css`
    margin-left: -24px;
  `}

    .additional-scroll-height {
    ${({ withScroll }) =>
      !withScroll &&
      css`
        height: 64px;
      `}
  }
`;

const StyledDropZoneBody = styled(DragAndDrop)`
  max-width: 100vw !important;

  ${commonStyles};

  .drag-and-drop {
    user-select: none;
    height: 100%;
  }

  ${(props) =>
    props.withScroll &&
    css`
      margin-left: -20px;

      @media ${tablet} {
        margin-left: -24px;
      }

      ${isMobile &&
      css`
        margin-left: -24px;
      `}
    `}
`;

const StyledSpacer = styled.div`
  display: ${isMobile ? "block" : "none"};
  min-height: 64px;

  @media ${tablet} {
    display: block;
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
    const { withScroll } = this.props;
    if (!this.props.autoFocus) return;
    if (withScroll) this.focusRef.current.focus();
  }

  componentWillUnmount() {
    this.focusRef = null;
  }

  render() {
    //console.log(" SectionBody render" );
    const {
      autoFocus,
      children,
      onDrop,
      uploadFiles,
      viewAs,
      withScroll,
      isLoaded,
      isDesktop,
      settingsStudio,
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
        isLoaded={isLoaded}
        isDesktop={isDesktop}
        settingsStudio={settingsStudio}
        className="section-body"
      >
        {withScroll ? (
          !isMobileOnly ? (
            <Scrollbar
              id="sectionScroll"
              scrollclass="section-scroll"
              stype="mediumBlack"
            >
              <div className="section-wrapper">
                <div className="section-wrapper-content" {...focusProps}>
                  {children}
                  <StyledSpacer />
                </div>
              </div>
            </Scrollbar>
          ) : (
            <div className="section-wrapper">
              <div className="section-wrapper-content" {...focusProps}>
                {children}
                <StyledSpacer />
              </div>
            </div>
          )
        ) : (
          <div className="section-wrapper">
            {children}
            <StyledSpacer />
          </div>
        )}
      </StyledDropZoneBody>
    ) : (
      <StyledSectionBody
        viewAs={viewAs}
        withScroll={withScroll}
        isLoaded={isLoaded}
        isDesktop={isDesktop}
        settingsStudio={settingsStudio}
      >
        {withScroll ? (
          !isMobileOnly ? (
            <Scrollbar
              id="sectionScroll"
              scrollclass="section-scroll"
              stype="mediumBlack"
            >
              <div className="section-wrapper">
                <div className="section-wrapper-content" {...focusProps}>
                  {children}
                  <StyledSpacer className="settings-mobile" />
                </div>
              </div>
            </Scrollbar>
          ) : (
            <div className="section-wrapper">
              <div className="section-wrapper-content" {...focusProps}>
                {children}
                <StyledSpacer className="settings-mobile" />
              </div>
            </div>
          )
        ) : (
          <div className="section-wrapper">{children}</div>
        )}
      </StyledSectionBody>
    );
  }
}

SectionBody.displayName = "SectionBody";

SectionBody.propTypes = {
  withScroll: PropTypes.bool,
  autoFocus: PropTypes.bool,
  onDrop: PropTypes.func,
  uploadFiles: PropTypes.bool,
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
    PropTypes.any,
  ]),
  viewAs: PropTypes.string,
  isLoaded: PropTypes.bool,
  settingsStudio: PropTypes.bool,
};

SectionBody.defaultProps = {
  autoFocus: false,
  uploadFiles: false,
  withScroll: true,
  settingsStudio: false,
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const { isDesktopClient: isDesktop } = settingsStore;
  return {
    isLoaded: auth.isLoaded,
    isDesktop,
  };
})(observer(SectionBody));
