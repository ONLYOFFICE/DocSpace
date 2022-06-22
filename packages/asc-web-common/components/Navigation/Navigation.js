import React from "react";
import PropTypes from "prop-types";

import StyledContainer from "./StyledNavigation";
import ArrowButton from "./sub-components/arrow-btn";
import Text from "./sub-components/text";
import ControlButtons from "./sub-components/control-btn";
import DropBox from "./sub-components/drop-box";

import { Consumer } from "@appserver/components/utils/context";

import DomHelpers from "@appserver/components/utils/domHelpers";
import Backdrop from "@appserver/components/backdrop";

import { isMobile, isMobileOnly } from "react-device-detect";
import {
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
  isDesktop as isDesktopUtils,
} from "@appserver/components/utils/device";
import ToggleInfoPanelButton from "./sub-components/toggle-infopanel-btn";

const Navigation = ({
  tReady,
  showText,
  isRootFolder,
  title,
  canCreate,
  isTabletView,
  personal,
  onClickFolder,
  navigationItems,
  getContextOptionsPlus,
  getContextOptionsFolder,
  onBackToParentFolder,
  isRecycleBinFolder,
  isEmptyFilesList,
  clearTrash,
  showFolderInfo,
  isCurrentFolderInfo,
  toggleInfoPanel,
  isInfoPanelVisible,
  titles,
  ...rest
}) => {
  const [isOpen, setIsOpen] = React.useState(false);
  const [firstClick, setFirstClick] = React.useState(true);
  const [dropBoxWidth, setDropBoxWidth] = React.useState(0);
  const [maxHeight, setMaxHeight] = React.useState(false);

  const dropBoxRef = React.useRef(null);
  const containerRef = React.useRef(null);

  const isDesktop =
    (!isMobile && !isTabletUtils() && !isMobileUtils()) ||
    (isDesktopUtils() && !isMobile);

  const onMissClick = React.useCallback(
    (e) => {
      e.preventDefault;
      const path = e.path || (e.composedPath && e.composedPath());

      if (!firstClick) {
        !path.includes(dropBoxRef.current) ? toggleDropBox() : null;
      } else {
        setFirstClick((prev) => !prev);
      }
    },
    [firstClick, toggleDropBox, setFirstClick]
  );

  const onClickAvailable = React.useCallback(
    (id) => {
      onClickFolder && onClickFolder(id);
      toggleDropBox();
    },
    [onClickFolder, toggleDropBox]
  );

  const toggleDropBox = () => {
    if (isRootFolder) return setIsOpen(false);
    setIsOpen((prev) => !prev);

    setDropBoxWidth(DomHelpers.getOuterWidth(containerRef.current));

    const { top } = DomHelpers.getOffset(containerRef.current);

    setMaxHeight(`calc(100vh - ${top}px)`);

    setFirstClick(true);
  };

  const onResize = React.useCallback(() => {
    setDropBoxWidth(DomHelpers.getOuterWidth(containerRef.current));
  }, [containerRef.current]);

  React.useEffect(() => {
    if (isOpen) {
      window.addEventListener("click", onMissClick);
      window.addEventListener("resize", onResize);
    } else {
      window.removeEventListener("click", onMissClick);
      window.removeEventListener("resize", onResize);
      setFirstClick(true);
    }

    return () => {
      window.removeEventListener("click", onMissClick);
      window.removeEventListener("resize", onResize);
    };
  }, [isOpen, onResize, onMissClick]);

  const onBackToParentFolderAction = React.useCallback(() => {
    setIsOpen((val) => !val);
    onBackToParentFolder && onBackToParentFolder();
  }, [onBackToParentFolder]);

  return (
    <Consumer>
      {(context) => (
        <>
          {isOpen && (
            <>
              <Backdrop
                visible={isOpen}
                withBackground={false}
                withoutBlur={true}
                zIndex={400}
              />

              <DropBox
                {...rest}
                isDesktop={isDesktop}
                ref={dropBoxRef}
                maxHeight={maxHeight}
                dropBoxWidth={dropBoxWidth}
                sectionHeight={context.sectionHeight}
                showText={showText}
                isRootFolder={isRootFolder}
                onBackToParentFolder={onBackToParentFolderAction}
                title={title}
                personal={personal}
                canCreate={canCreate}
                navigationItems={navigationItems}
                getContextOptionsFolder={getContextOptionsFolder}
                getContextOptionsPlus={getContextOptionsPlus}
                toggleDropBox={toggleDropBox}
                toggleInfoPanel={toggleInfoPanel}
                isInfoPanelVisible={isInfoPanelVisible}
                onClickAvailable={onClickAvailable}
              />
            </>
          )}
          <StyledContainer
            ref={containerRef}
            width={context.sectionWidth}
            isRootFolder={isRootFolder}
            canCreate={canCreate}
            isTabletView={isTabletView}
            isRecycleBinFolder={isRecycleBinFolder}
            isDesktop={isDesktop}
            isInfoPanelVisible={isInfoPanelVisible}
          >
            <ArrowButton
              isRootFolder={isRootFolder}
              onBackToParentFolder={onBackToParentFolder}
            />
            <Text
              title={title}
              isOpen={false}
              isRootFolder={isRootFolder}
              onClick={toggleDropBox}
            />
            <ControlButtons
              personal={personal}
              isRootFolder={isRootFolder}
              canCreate={canCreate}
              getContextOptionsFolder={getContextOptionsFolder}
              getContextOptionsPlus={getContextOptionsPlus}
              isRecycleBinFolder={isRecycleBinFolder}
              isEmptyFilesList={isEmptyFilesList}
              clearTrash={clearTrash}
              toggleInfoPanel={toggleInfoPanel}
              isInfoPanelVisible={isInfoPanelVisible}
              isDesktop={isDesktop}
              titles={titles}
            />
          </StyledContainer>
          {isDesktop && (
            <ToggleInfoPanelButton
              isRootFolder={isRootFolder}
              toggleInfoPanel={toggleInfoPanel}
              isInfoPanelVisible={isInfoPanelVisible}
            />
          )}
        </>
      )}
    </Consumer>
  );
};

Navigation.propTypes = {
  tReady: PropTypes.bool,
  isRootFolder: PropTypes.bool,
  title: PropTypes.string,
  canCreate: PropTypes.bool,
  isDesktop: PropTypes.bool,
  isTabletView: PropTypes.bool,
  personal: PropTypes.bool,
  onClickFolder: PropTypes.func,
  navigationItems: PropTypes.arrayOf(PropTypes.object),
  getContextOptionsPlus: PropTypes.func,
  getContextOptionsFolder: PropTypes.func,
  onBackToParentFolder: PropTypes.func,
  titles: PropTypes.object,
};

export default React.memo(Navigation);
