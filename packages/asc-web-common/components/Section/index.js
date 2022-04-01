import React from "react";
import PropTypes from "prop-types";

import {
  desktop,
  size,
  tablet,
  mobile,
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "@appserver/components/utils/device";
import { Provider } from "@appserver/components/utils/context";
import { isMobile, isFirefox, isMobileOnly } from "react-device-detect";

import SectionContainer from "./sub-components/section-container";
import SubSectionHeader from "./sub-components/section-header";
import SubSectionFilter from "./sub-components/section-filter";
import SubSectionBody from "./sub-components/section-body";
import SubSectionBodyContent from "./sub-components/section-body-content";
import SubSectionBar from "./sub-components/section-bar";
import SubSectionPaging from "./sub-components/section-paging";

import ReactResizeDetector from "react-resize-detector";
import FloatingButton from "../FloatingButton";
import { inject, observer } from "mobx-react";
import Selecto from "react-selecto";
import styled, { css } from "styled-components";

const StyledSelectoWrapper = styled.div`
  .selecto-selection {
    z-index: 200;
  }
`;

const StyledMainBar = styled.div`
  box-sizing: border-box;

  margin-left: -20px;
  width: calc(100vw - 256px);
  max-width: calc(100vw - 256px);

  #bar-banner {
    margin-bottom: -3px;
  }

  #bar-frame {
    min-width: 100%;
    max-width: 100%;
  }

  @media ${tablet} {
    width: ${(props) =>
      props.showText ? "calc(100vw - 240px)" : "calc(100vw - 52px)"};
    max-width: ${(props) =>
      props.showText ? "calc(100vw - 240px)" : "calc(100vw - 52px)"};
    margin-left: -16px;
  }

  ${isMobile &&
  css`
    width: ${(props) =>
      props.showText ? "calc(100vw - 240px)" : "calc(100vw - 52px)"} !important;
    max-width: ${(props) =>
      props.showText ? "calc(100vw - 240px)" : "calc(100vw - 52px)"} !important;
    margin-left: -16px;
  `}

  @media ${mobile} {
    width: 100vw !important;
    max-width: 100vw !important;
  }

  ${isMobileOnly &&
  css`
    width: 100vw !important;
    max-width: 100vw !important;

    #bar-frame {
      min-width: 100vw;
    }
  `}

  ${(props) =>
    !props.isSectionHeaderAvailable &&
    css`
      width: 100vw !important;
      max-width: 100vw !important;

      ${isMobile &&
      css`
        position: fixed;
        top: 48px;
        left: 0;
        margin-left: 0 !important;
        box-sizing: border-box;
      `}
    `}
`;

function SectionHeader() {
  return null;
}
SectionHeader.displayName = "SectionHeader";

function SectionBar() {
  return null;
}

SectionBar.displayName = "SectionBar";

function SectionFilter() {
  return null;
}
SectionFilter.displayName = "SectionFilter";

function SectionBody() {
  return null;
}
SectionBody.displayName = "SectionBody";

function SectionPaging() {
  return null;
}
SectionPaging.displayName = "SectionPaging";

class Section extends React.Component {
  static SectionHeader = SectionHeader;
  static SectionFilter = SectionFilter;
  static SectionBody = SectionBody;
  static SectionBar = SectionBar;
  static SectionPaging = SectionPaging;

  constructor(props) {
    super(props);

    this.timeoutHandler = null;
    this.intervalHandler = null;

    this.scroll = null;
  }

  componentDidUpdate(prevProps) {
    if (!this.scroll) {
      this.scroll = document.getElementsByClassName("section-scroll")[0];
    }
  }

  componentDidMount() {}

  componentWillUnmount() {
    if (this.intervalHandler) clearInterval(this.intervalHandler);
    if (this.timeoutHandler) clearTimeout(this.timeoutHandler);
  }

  onSelect = (e) => {
    if (this.props.dragging) return;
    const items = e.selected;
    this.props.setSelections(items);
  };

  dragCondition = (e) => {
    const path = e.inputEvent.composedPath();
    const isBackdrop = path.some(
      (x) => x.classList && x.classList.contains("backdrop-active")
    );
    const notSelectablePath = path.some(
      (x) => x.classList && x.classList.contains("not-selectable")
    );

    const isDraggable = path.some(
      (x) => x.classList && x.classList.contains("draggable")
    );

    if (notSelectablePath || isBackdrop || isDraggable) {
      return false;
    } else return true;
  };

  onScroll = (e) => {
    this.scroll.scrollBy(e.direction[0] * 10, e.direction[1] * 10);
  };
  render() {
    const {
      onDrop,
      showPrimaryProgressBar,
      primaryProgressBarIcon,
      primaryProgressBarValue,
      showPrimaryButtonAlert,
      showSecondaryProgressBar,
      secondaryProgressBarValue,
      secondaryProgressBarIcon,
      showSecondaryButtonAlert,
      uploadFiles,
      viewAs,
      //withBodyAutoFocus,
      withBodyScroll,
      children,
      isHeaderVisible,
      //headerBorderBottom,
      onOpenUploadPanel,
      isTabletView,
      firstLoad,
      dragging,
      isBackdropVisible,
      isDesktop,
      isHomepage,
      maintenanceExist,
      setMaintenanceExist,
      snackbarExist,
      showText,
    } = this.props;

    let sectionHeaderContent = null;
    let sectionBarContent = null;
    let sectionFilterContent = null;
    let sectionPagingContent = null;
    let sectionBodyContent = null;
    React.Children.forEach(children, (child) => {
      const childType =
        child && child.type && (child.type.displayName || child.type.name);

      switch (childType) {
        case SectionHeader.displayName:
          sectionHeaderContent = child;
          break;
        case SectionFilter.displayName:
          sectionFilterContent = child;
          break;
        case SectionBar.displayName:
          sectionBarContent = child;
          break;
        case SectionPaging.displayName:
          sectionPagingContent = child;
          break;
        case SectionBody.displayName:
          sectionBodyContent = child;
          break;
        default:
          break;
      }
    });

    const isSectionHeaderAvailable = !!sectionHeaderContent,
      isSectionFilterAvailable = !!sectionFilterContent,
      isSectionPagingAvailable = !!sectionPagingContent,
      isSectionBodyAvailable =
        !!sectionBodyContent ||
        isSectionFilterAvailable ||
        isSectionPagingAvailable,
      isSectionBarAvailable = !!sectionBarContent,
      isSectionAvailable =
        isSectionHeaderAvailable ||
        isSectionFilterAvailable ||
        isSectionBodyAvailable ||
        isSectionPagingAvailable;

    const renderSection = () => {
      return (
        <>
          {isSectionAvailable && (
            <ReactResizeDetector
              refreshRate={100}
              refreshMode="debounce"
              refreshOptions={{ trailing: true }}
            >
              {({ width, height }) => (
                <Provider
                  value={{
                    sectionWidth: width,
                    sectionHeight: height,
                  }}
                >
                  <SectionContainer
                    widthProp={width}
                    showText={showText}
                    viewAs={viewAs}
                    maintenanceExist={maintenanceExist}
                    isSectionBarAvailable={isSectionBarAvailable}
                    isSectionHeaderAvailable={isSectionHeaderAvailable}
                  >
                    {!isMobile && (
                      <StyledMainBar
                        width={width}
                        id="main-bar"
                        className={"main-bar"}
                        showText={showText}
                        isSectionHeaderAvailable={isSectionHeaderAvailable}
                      >
                        <SubSectionBar
                          setMaintenanceExist={setMaintenanceExist}
                        >
                          {sectionBarContent
                            ? sectionBarContent.props.children
                            : null}
                        </SubSectionBar>
                      </StyledMainBar>
                    )}

                    {isSectionHeaderAvailable && !isMobile && (
                      <SubSectionHeader
                        maintenanceExist={maintenanceExist}
                        snackbarExist={snackbarExist}
                        className="section-header_header"
                        isHeaderVisible={isHeaderVisible}
                        viewAs={viewAs}
                        showText={showText}
                      >
                        {sectionHeaderContent
                          ? sectionHeaderContent.props.children
                          : null}
                      </SubSectionHeader>
                    )}
                    {isSectionFilterAvailable && !isMobile && (
                      <>
                        <SubSectionFilter
                          className="section-header_filter"
                          viewAs={viewAs}
                        >
                          {sectionFilterContent
                            ? sectionFilterContent.props.children
                            : null}
                        </SubSectionFilter>
                      </>
                    )}
                    {isSectionBodyAvailable && (
                      <>
                        <SubSectionBody
                          onDrop={onDrop}
                          uploadFiles={uploadFiles}
                          withScroll={withBodyScroll}
                          autoFocus={isMobile || isTabletView ? false : true}
                          viewAs={viewAs}
                          isHomepage={isHomepage}
                        >
                          {isMobile && (
                            <StyledMainBar
                              width={width}
                              id="main-bar"
                              className={"main-bar"}
                              showText={showText}
                              isSectionHeaderAvailable={
                                isSectionHeaderAvailable
                              }
                            >
                              <SubSectionBar
                                setMaintenanceExist={setMaintenanceExist}
                              >
                                {sectionBarContent
                                  ? sectionBarContent.props.children
                                  : null}
                              </SubSectionBar>
                            </StyledMainBar>
                          )}

                          {isSectionHeaderAvailable && isMobile && (
                            <SubSectionHeader
                              className="section-body_header"
                              isHeaderVisible={isHeaderVisible}
                              viewAs={viewAs}
                              showText={showText}
                            >
                              {sectionHeaderContent
                                ? sectionHeaderContent.props.children
                                : null}
                            </SubSectionHeader>
                          )}

                          {isSectionFilterAvailable && isMobile && (
                            <SubSectionFilter className="section-body_filter">
                              {sectionFilterContent
                                ? sectionFilterContent.props.children
                                : null}
                            </SubSectionFilter>
                          )}
                          <SubSectionBodyContent>
                            {sectionBodyContent
                              ? sectionBodyContent.props.children
                              : null}
                          </SubSectionBodyContent>
                          {isSectionPagingAvailable && (
                            <SubSectionPaging>
                              {sectionPagingContent
                                ? sectionPagingContent.props.children
                                : null}
                            </SubSectionPaging>
                          )}
                        </SubSectionBody>
                      </>
                    )}
                    {!(isMobile || isMobileUtils() || isTabletUtils()) ? (
                      showPrimaryProgressBar && showSecondaryProgressBar ? (
                        <>
                          <FloatingButton
                            className="layout-progress-bar"
                            icon={primaryProgressBarIcon}
                            percent={primaryProgressBarValue}
                            alert={showPrimaryButtonAlert}
                            onClick={onOpenUploadPanel}
                          />
                          <FloatingButton
                            className="layout-progress-second-bar"
                            icon={secondaryProgressBarIcon}
                            percent={secondaryProgressBarValue}
                            alert={showSecondaryButtonAlert}
                          />
                        </>
                      ) : showPrimaryProgressBar &&
                        !showSecondaryProgressBar ? (
                        <FloatingButton
                          className="layout-progress-bar"
                          icon={primaryProgressBarIcon}
                          percent={primaryProgressBarValue}
                          alert={showPrimaryButtonAlert}
                          onClick={onOpenUploadPanel}
                        />
                      ) : !showPrimaryProgressBar &&
                        showSecondaryProgressBar ? (
                        <FloatingButton
                          className="layout-progress-bar"
                          icon={secondaryProgressBarIcon}
                          percent={secondaryProgressBarValue}
                          alert={showSecondaryButtonAlert}
                        />
                      ) : (
                        <></>
                      )
                    ) : (
                      <></>
                    )}
                  </SectionContainer>
                </Provider>
              )}
            </ReactResizeDetector>
          )}
        </>
      );
    };

    const scrollOptions = this.scroll
      ? {
          container: this.scroll,
          throttleTime: 0,
          threshold: 100,
        }
      : {};

    return (
      <>
        {renderSection()}
        {!isMobile && uploadFiles && !dragging && (
          <StyledSelectoWrapper>
            <Selecto
              boundContainer={".section-wrapper"}
              dragContainer={".section-body"}
              selectableTargets={[".files-item"]}
              hitRate={0}
              selectByClick={false}
              selectFromInside={true}
              ratio={0}
              continueSelect={false}
              onSelect={this.onSelect}
              dragCondition={this.dragCondition}
              scrollOptions={scrollOptions}
              onScroll={this.onScroll}
            />
          </StyledSelectoWrapper>
        )}
      </>
    );
  }
}

Section.propTypes = {
  children: PropTypes.any,
  withBodyScroll: PropTypes.bool,
  withBodyAutoFocus: PropTypes.bool,
  showPrimaryProgressBar: PropTypes.bool,
  primaryProgressBarValue: PropTypes.number,
  showPrimaryButtonAlert: PropTypes.bool,
  progressBarDropDownContent: PropTypes.any,
  primaryProgressBarIcon: PropTypes.string,
  showSecondaryProgressBar: PropTypes.bool,
  secondaryProgressBarValue: PropTypes.number,
  secondaryProgressBarIcon: PropTypes.string,
  showSecondaryButtonAlert: PropTypes.bool,
  onDrop: PropTypes.func,
  setSelections: PropTypes.func,
  uploadFiles: PropTypes.bool,
  hideAside: PropTypes.bool,
  viewAs: PropTypes.string,
  uploadPanelVisible: PropTypes.bool,
  onOpenUploadPanel: PropTypes.func,
  isTabletView: PropTypes.bool,
  isHeaderVisible: PropTypes.bool,
  firstLoad: PropTypes.bool,
  isHomepage: PropTypes.bool,
};

Section.defaultProps = {
  withBodyScroll: true,
  withBodyAutoFocus: false,
};

Section.SectionHeader = SectionHeader;
Section.SectionFilter = SectionFilter;
Section.SectionBody = SectionBody;
Section.SectionPaging = SectionPaging;

export default inject(({ auth }) => {
  const { isLoaded, settingsStore } = auth;
  const {
    isHeaderVisible,
    isTabletView,

    isBackdropVisible,

    setIsBackdropVisible,
    isDesktopClient,
    maintenanceExist,
    snackbarExist,
    setMaintenanceExist,

    showText,
  } = settingsStore;

  return {
    isLoaded,
    isTabletView,
    isHeaderVisible,

    isBackdropVisible,
    setIsBackdropVisible,
    maintenanceExist,
    snackbarExist,
    setMaintenanceExist,
    isDesktop: isDesktopClient,

    showText,
  };
})(observer(Section));
