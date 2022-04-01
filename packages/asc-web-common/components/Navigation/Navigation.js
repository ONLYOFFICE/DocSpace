import React from "react";
import PropTypes from "prop-types";

import Loaders from "@appserver/common/components/Loaders";

import StyledContainer from "./StyledNavigation";
import ArrowButton from "./sub-components/arrow-btn";
import Text from "./sub-components/text";
import ControlButtons from "./sub-components/control-btn";
import DropBox from "./sub-components/drop-box";

import { Consumer } from "@appserver/components/utils/context";

import DomHelpers from "@appserver/components/utils/domHelpers";

const Navigation = ({
  tReady,
  showText,
  isRootFolder,
  title,
  canCreate,
  isDesktop,
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
  ...rest
}) => {
  const [isOpen, setIsOpen] = React.useState(false);
  const [firstClick, setFirstClick] = React.useState(true);
  const [dropBoxWidth, setDropBoxWidth] = React.useState(0);

  const dropBoxRef = React.useRef(null);
  const containerRef = React.useRef(null);

  const onMissClick = (e) => {
    e.preventDefault;
    const path = e.path || (e.composedPath && e.composedPath());

    if (!firstClick) {
      !path.includes(dropBoxRef.current) ? toggleDropBox() : null;
    } else {
      setFirstClick((prev) => !prev);
    }
  };

  const onClickAvailable = React.useCallback(
    (id) => {
      onClickFolder && onClickFolder(id);
      toggleDropBox();
    },
    [onClickFolder, toggleDropBox]
  );

  const toggleDropBox = () => {
    if (isRootFolder) return setIsOpen(false);
    setDropBoxWidth(DomHelpers.getOuterWidth(containerRef.current));
    setIsOpen((prev) => !prev);
    setFirstClick(true);
  };

  React.useEffect(() => {
    if (isOpen) {
      window.addEventListener("click", onMissClick);
    } else {
      window.removeEventListener("click", onMissClick);
      setFirstClick(true);
    }

    return () => window.removeEventListener("click", onMissClick);
  }, [isOpen, onMissClick]);

  const onBackToParentFolderAction = React.useCallback(() => {
    setIsOpen((val) => !val);
    onBackToParentFolder && onBackToParentFolder();
  }, [onBackToParentFolder]);

  return (
    <Consumer>
      {(context) => (
        <>
          {isOpen && (
            <DropBox
              {...rest}
              ref={dropBoxRef}
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
              onClickAvailable={onClickAvailable}
            />
          )}
          <StyledContainer
            ref={containerRef}
            width={context.sectionWidth}
            isRootFolder={isRootFolder}
            canCreate={canCreate}
            title={title}
            isDesktop={isDesktop}
            isTabletView={isTabletView}
            isRecycleBinFolder={isRecycleBinFolder}
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
            />
          </StyledContainer>
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
};

export default React.memo(Navigation);
