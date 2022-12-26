import React from "react";
import PropTypes from "prop-types";

import {
  tablet,
  mobile,
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "@docspace/components/utils/device";
import NoUserSelect from "@docspace/components/utils/commonStyles";
import { Provider } from "@docspace/components/utils/context";
import { isMobile, isMobileOnly } from "react-device-detect";

import SectionContainer from "./sub-components/section-container";
import SubSectionHeader from "./sub-components/section-header";
import SubSectionFilter from "./sub-components/section-filter";
import SubSectionBody from "./sub-components/section-body";
import SubSectionBodyContent from "./sub-components/section-body-content";

import SubSectionPaging from "./sub-components/section-paging";
//import SectionToggler from "./sub-components/section-toggler";
import InfoPanel from "./sub-components/info-panel";
import SubInfoPanelBody from "./sub-components/info-panel-body";
import SubInfoPanelHeader from "./sub-components/info-panel-header";

import SubSectionFooter from "./sub-components/section-footer";

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

function SectionHeader() {
  return null;
}
SectionHeader.displayName = "SectionHeader";

function SectionFilter() {
  return null;
}
SectionFilter.displayName = "SectionFilter";

function SectionBody() {
  return null;
}
SectionBody.displayName = "SectionBody";

function SectionFooter() {
  return null;
}
SectionFooter.displayName = "SectionFooter";

function SectionPaging() {
  return null;
}
SectionPaging.displayName = "SectionPaging";

function InfoPanelBody() {
  return null;
}
InfoPanelBody.displayName = "InfoPanelBody";

function InfoPanelHeader() {
  return null;
}
InfoPanelHeader.displayName = "InfoPanelHeader";

class Section extends React.Component {
  static SectionHeader = SectionHeader;
  static SectionFilter = SectionFilter;
  static SectionBody = SectionBody;
  static SectionFooter = SectionFooter;

  static SectionPaging = SectionPaging;
  static InfoPanelBody = InfoPanelBody;
  static InfoPanelHeader = InfoPanelHeader;

  constructor(props) {
    super(props);

    this.timeoutHandler = null;
    this.intervalHandler = null;

    this.scroll = null;
    this.selectoRef = React.createRef(null);
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
      withBodyScroll,
      children,
      isHeaderVisible,
      onOpenUploadPanel,
      isTabletView,
      dragging,
      maintenanceExist,
      setMaintenanceExist,
      snackbarExist,
      showText,
      isInfoPanelAvailable,
      settingsStudio,
      clearUploadedFilesHistory,
      withPaging,
      isInfoPanelVisible,
    } = this.props;

    let sectionHeaderContent = null;

    let sectionFilterContent = null;
    let sectionPagingContent = null;
    let sectionBodyContent = null;
    let sectionFooterContent = null;
    let infoPanelBodyContent = null;
    let infoPanelHeaderContent = null;

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
        case SectionPaging.displayName:
          sectionPagingContent = child;
          break;
        case SectionBody.displayName:
          sectionBodyContent = child;
          break;
        case SectionFooter.displayName:
          sectionFooterContent = child;
          break;
        case InfoPanelBody.displayName:
          infoPanelBodyContent = child;
          break;
        case InfoPanelHeader.displayName:
          infoPanelHeaderContent = child;
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
                    isSectionHeaderAvailable={isSectionHeaderAvailable}
                    settingsStudio={settingsStudio}
                    isInfoPanelVisible={isInfoPanelVisible}
                  >
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
                          settingsStudio={settingsStudio}
                          selectoRef={this.selectoRef}
                        >
                          {isSectionHeaderAvailable && isMobile && (
                            <SubSectionHeader
                              className="section-body_header"
                              isHeaderVisible={isHeaderVisible}
                              viewAs={viewAs}
                              showText={showText}
                              settingsStudio={settingsStudio}
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

                          <SubSectionFooter>
                            {sectionFooterContent
                              ? sectionFooterContent.props.children
                              : null}
                          </SubSectionFooter>

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
                            clearUploadedFilesHistory={
                              clearUploadedFilesHistory
                            }
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
                          clearUploadedFilesHistory={clearUploadedFilesHistory}
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

                  {isInfoPanelAvailable && (
                    <InfoPanel viewAs={viewAs}>
                      <SubInfoPanelHeader>
                        {infoPanelHeaderContent}
                      </SubInfoPanelHeader>
                      <SubInfoPanelBody>
                        {infoPanelBodyContent}
                      </SubInfoPanelBody>
                    </InfoPanel>
                  )}
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
        {!isMobile && uploadFiles && !dragging && withPaging && (
          <StyledSelectoWrapper>
            <Selecto
              ref={this.selectoRef}
              boundContainer=".section-wrapper-content"
              dragContainer=".section-wrapper"
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
  viewAs: PropTypes.string,
  onOpenUploadPanel: PropTypes.func,
  isTabletView: PropTypes.bool,
  isHeaderVisible: PropTypes.bool,
  isInfoPanelAvailable: PropTypes.bool,
  settingsStudio: PropTypes.bool,
  withPaging: PropTypes.bool,
};

Section.defaultProps = {
  withBodyScroll: true,
  isInfoPanelAvailable: true,
  settingsStudio: false,
  withPaging: true,
};

Section.InfoPanelHeader = InfoPanelHeader;
Section.InfoPanelBody = InfoPanelBody;
Section.SectionHeader = SectionHeader;
Section.SectionFilter = SectionFilter;
Section.SectionBody = SectionBody;
Section.SectionPaging = SectionPaging;
Section.SectionFooter = SectionFooter;

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const {
    isHeaderVisible,
    isTabletView,
    maintenanceExist,
    snackbarExist,
    setMaintenanceExist,
    showText,
  } = settingsStore;

  const { isVisible: isInfoPanelVisible } = auth.infoPanelStore;

  return {
    isTabletView,
    isHeaderVisible,

    maintenanceExist,
    snackbarExist,
    setMaintenanceExist,

    showText,

    isInfoPanelVisible,
  };
})(observer(Section));
