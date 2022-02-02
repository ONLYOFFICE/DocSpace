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
  ...rest
}) => {
  const [isOpen, setIsOpen] = React.useState(false);
  const [firstClick, setFirstClick] = React.useState(true);
  const [changeWidth, setChangeWidth] = React.useState(false);
  const dropBoxRef = React.useRef(null);

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

  const toggleDropBox = React.useCallback(() => {
    if (isRootFolder) return;
    setIsOpen((prev) => !prev);
    setFirstClick(true);

    setTimeout(() => {
      setChangeWidth(
        DomHelpers.getOuterWidth(dropBoxRef.current) + 24 ===
          DomHelpers.getOuterWidth(document.getElementById("section"))
      );
    }, 0);
  }, [setIsOpen, setFirstClick, setChangeWidth, isRootFolder]);

  React.useEffect(() => {
    if (isOpen) {
      window.addEventListener("click", onMissClick);
    } else {
      window.removeEventListener("click", onMissClick);
      setFirstClick(true);
    }

    return () => window.removeEventListener("click", onMissClick);
  }, [isOpen, onMissClick]);
  return (
    <Consumer>
      {(context) => (
        <>
          {isOpen && (
            <DropBox
              {...rest}
              ref={dropBoxRef}
              changeWidth={changeWidth}
              width={context.sectionWidth}
              height={context.sectionHeight}
              showText={showText}
              isRootFolder={isRootFolder}
              onBackToParentFolder={onBackToParentFolder}
              title={title}
              personal={personal}
              isRootFolder={isRootFolder}
              canCreate={canCreate}
              navigationItems={navigationItems}
              getContextOptionsFolder={getContextOptionsFolder}
              getContextOptionsPlus={getContextOptionsPlus}
              toggleDropBox={toggleDropBox}
              onClickAvailable={onClickAvailable}
            />
          )}
          <StyledContainer
            width={context.sectionWidth}
            isRootFolder={isRootFolder}
            canCreate={canCreate}
            title={title}
            isDesktop={isDesktop}
            isTabletView={isTabletView}
          >
            <div className="header-container">
              <>
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
                />
              </>
            </div>
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

export default Navigation;
