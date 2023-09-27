import React from "react";
import PropTypes from "prop-types";

import StyledContainer from "./StyledNavigation";
import ArrowButton from "./sub-components/arrow-btn";
import Text from "./sub-components/text";
import ControlButtons from "./sub-components/control-btn";
import DropBox from "./sub-components/drop-box";

import { Consumer } from "@docspace/components/utils/context";

import DomHelpers from "@docspace/components/utils/domHelpers";
import Backdrop from "@docspace/components/backdrop";
import { isMobileOnly } from "react-device-detect";

import { ReactSVG } from "react-svg";
import {
  isTablet as isTabletUtils,
  isDesktop as isDesktopUtils,
  isSmallTablet as isSmallTabletUtils,
} from "@docspace/components/utils/device";
import ToggleInfoPanelButton from "./sub-components/toggle-infopanel-btn";
import TrashWarning from "./sub-components/trash-warning";
import NavigationLogo from "./sub-components/logo-block";

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
  isTrashFolder,
  isRecycleBinFolder,
  isEmptyFilesList,
  clearTrash,
  showFolderInfo,
  isCurrentFolderInfo,
  toggleInfoPanel,
  isInfoPanelVisible,
  titles,
  withMenu,
  onPlusClick,
  isEmptyPage,
  isDesktop: isDesktopClient,
  isRoom,
  isFrame,
  hideInfoPanel,
  showRootFolderTitle,
  withLogo,
  burgerLogo,
  isPublicRoom,
  titleIcon,

  ...rest
}) => {
  const [isOpen, setIsOpen] = React.useState(false);
  const [firstClick, setFirstClick] = React.useState(true);
  const [dropBoxWidth, setDropBoxWidth] = React.useState(0);
  const [maxHeight, setMaxHeight] = React.useState(false);

  const dropBoxRef = React.useRef(null);
  const containerRef = React.useRef(null);

  const isDesktop =
    (!isTabletUtils() && !isSmallTabletUtils()) || isDesktopUtils();

  const infoPanelIsVisible = React.useMemo(
    () => isDesktop && (!isEmptyPage || (isEmptyPage && isRoom)),
    [isDesktop, isEmptyPage, isRoom]
  );

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
    (id, isRootRoom) => {
      onClickFolder && onClickFolder(id, isRootRoom);
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

  const showRootFolderNavigation =
    showRootFolderTitle &&
    navigationItems &&
    navigationItems.length > 1 &&
    !isSmallTabletUtils() &&
    !isMobileOnly;

  const navigationTitleNode = (
    <div className="title-block">
      {titleIcon && <ReactSVG className="title-icon" src={titleIcon} />}
      <Text
        title={title}
        isOpen={false}
        isRootFolder={isRootFolder}
        onClick={toggleDropBox}
      />
    </div>
  );

  const navigationTitleContainerNode = showRootFolderNavigation ? (
    <div className="title-container">
      <Text
        title={navigationItems[navigationItems.length - 2].title}
        isOpen={false}
        isRootFolder={isRootFolder}
        isRootFolderTitle
      />
      {navigationTitleNode}
    </div>
  ) : (
    navigationTitleNode
  );

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
                isDesktopClient={isDesktopClient}
                showRootFolderNavigation={showRootFolderNavigation}
                withLogo={withLogo}
                burgerLogo={burgerLogo}
                titleIcon={titleIcon}
              />
            </>
          )}
          <StyledContainer
            ref={containerRef}
            width={context.sectionWidth}
            isRootFolder={isRootFolder}
            canCreate={canCreate}
            isTabletView={isTabletView}
            isTrashFolder={isTrashFolder}
            isRecycleBinFolder={isRecycleBinFolder}
            isDesktop={isDesktop}
            isDesktopClient={isDesktopClient}
            isInfoPanelVisible={isInfoPanelVisible}
            withLogo={!!withLogo}
            className="navigation-container"
          >
            {withLogo && (
              <NavigationLogo
                className="navigation-logo"
                logo={withLogo}
                burgerLogo={burgerLogo}
              />
            )}
            <ArrowButton
              isRootFolder={isRootFolder}
              onBackToParentFolder={onBackToParentFolder}
            />

            {navigationTitleContainerNode}

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
              withMenu={withMenu}
              onPlusClick={onPlusClick}
              isFrame={isFrame}
              isPublicRoom={isPublicRoom}
              isTrashFolder={isTrashFolder}
            />
          </StyledContainer>
          {isTrashFolder && !isEmptyPage && (
            <TrashWarning
              title={titles.trashWarning}
              isTabletView={isTabletView}
            />
          )}
          {infoPanelIsVisible && !hideInfoPanel && (
            <ToggleInfoPanelButton
              id="info-panel-toggle--open"
              isRootFolder={isRootFolder}
              toggleInfoPanel={toggleInfoPanel}
              isInfoPanelVisible={isInfoPanelVisible}
              titles={titles}
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
  isEmptyPage: PropTypes.bool,
  isRoom: PropTypes.bool,
};

export default React.memo(Navigation);
