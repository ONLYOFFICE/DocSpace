import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";

import { VariableSizeList } from "react-window";
import CustomScrollbarsVirtualList from "@docspace/components/scrollbar/custom-scrollbars-virtual-list";

import ArrowButton from "./arrow-btn";
import Text from "./text";
import ControlButtons from "./control-btn";
import Item from "./item";
import StyledContainer from "../StyledNavigation";
import NavigationLogo from "./logo-block";

import { isMobile, isMobileOnly, isTablet } from "react-device-detect";
import {
  tablet,
  mobile,
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "@docspace/components/utils/device";
import { ReactSVG } from "react-svg";

import { Base } from "@docspace/components/themes";

const StyledBox = styled.div`
  position: absolute;
  top: 0px;
  left: ${isMobile ? "-16px" : "-20px"};
  ${({ withLogo }) =>
    withLogo &&
    css`
      left: 207px;
    `};

  padding: ${isMobile ? "0 16px " : "0 20px"};
  padding-top: 18px;

  width: unset;

  height: ${(props) => (props.height ? `${props.height}px` : "fit-content")};
  max-height: calc(100vh - 48px);

  z-index: 401;
  display: table;
  margin: auto;
  flex-direction: column;

  background: ${(props) => props.theme.navigation.background};

  filter: drop-shadow(0px 12px 40px rgba(4, 15, 27, 0.12));
  border-radius: 0px 0px 6px 6px;

  .title-container {
    display: grid;
    grid-template-columns: minmax(1px, max-content) auto;
  }

  @media ${tablet} {
    width: ${({ dropBoxWidth }) => dropBoxWidth + "px"};
    left: -16px;
    padding: 0 16px;
    padding-top: 14px;
  }

  ${isMobile &&
  css`
    width: ${({ dropBoxWidth }) => dropBoxWidth + "px"};
    padding-top: 14px;
  `}

  @media ${mobile} {
    padding-top: 10px !important;
  }

  ${isMobileOnly &&
  css`
    margin-left: 16px;
    padding: 0 16px !important;
    padding-top: 14px !important;
    max-height: ${(props) => props.maxHeight};
  `}
`;

StyledBox.defaultProps = { theme: Base };

const Row = React.memo(({ data, index, style }) => {
  const isRoot = index === data[0].length - 1;
  return (
    <Item
      key={data[0][index].id}
      id={data[0][index].id}
      title={data[0][index].title}
      isRootRoom={data[0][index].isRootRoom}
      isRoot={isRoot}
      onClick={data[1]}
      withLogo={data[2].withLogo}
      style={{ ...style }}
    />
  );
});

const DropBox = React.forwardRef(
  (
    {
      sectionHeight,
      showText,
      dropBoxWidth,
      isRootFolder,
      onBackToParentFolder,
      title,
      personal,
      canCreate,
      navigationItems,
      getContextOptionsFolder,
      getContextOptionsPlus,
      toggleDropBox,
      toggleInfoPanel,
      onClickAvailable,
      isInfoPanelVisible,
      maxHeight,
      isOpen,
      isDesktop,
      isDesktopClient,
      showRootFolderNavigation,
      withLogo,
      burgerLogo,
      titleIcon,
    },
    ref
  ) => {
    const [dropBoxHeight, setDropBoxHeight] = React.useState(0);
    const countItems = navigationItems.length;

    const getItemSize = (index) => {
      if (index === countItems - 1) return 51;
      return isMobile || isMobileUtils() || isTabletUtils() ? 36 : 30;
    };

    React.useEffect(() => {
      const itemsHeight = navigationItems.map((item, index) =>
        getItemSize(index)
      );

      const currentHeight = itemsHeight.reduce((a, b) => a + b);

      let navHeight = 41;

      if (isMobile || isTabletUtils()) {
        navHeight = 49;
      }

      if (isMobileOnly || isMobileUtils()) {
        navHeight = 45;
      }

      setDropBoxHeight(
        currentHeight + navHeight > sectionHeight
          ? sectionHeight - navHeight - 20
          : currentHeight
      );
    }, [sectionHeight]);

    const navigationTitleNode = (
      <div className="title-block">
        {titleIcon && <ReactSVG className="title-icon" src={titleIcon} />}
        <Text title={title} isOpen={true} onClick={toggleDropBox} />
      </div>
    );

    const navigationTitleContainerNode = showRootFolderNavigation ? (
      <div className="title-container">
        <Text
          title={navigationItems[navigationItems.length - 2].title}
          isOpen={true}
          isRootFolderTitle
        />
        {navigationTitleNode}
      </div>
    ) : (
      navigationTitleNode
    );

    const isTabletView = (isTabletUtils() || isTablet) && !isMobileOnly;

    return (
      <>
        <StyledBox
          ref={ref}
          maxHeight={maxHeight}
          height={sectionHeight < dropBoxHeight ? sectionHeight : null}
          showText={showText}
          dropBoxWidth={dropBoxWidth}
          isDesktop={isDesktop}
          withLogo={withLogo}
        >
          <StyledContainer
            canCreate={canCreate}
            isDropBoxComponent={true}
            isInfoPanelVisible={isInfoPanelVisible}
            isDesktopClient={isDesktopClient}
            withLogo={!!withLogo && isTabletView}
          >
            {withLogo && (
              <NavigationLogo
                logo={withLogo}
                burgerLogo={burgerLogo}
                className="navigation-logo drop-box-logo"
              />
            )}
            <ArrowButton
              isRootFolder={isRootFolder}
              onBackToParentFolder={onBackToParentFolder}
            />

            {navigationTitleContainerNode}

            <ControlButtons
              isDesktop={isDesktop}
              personal={personal}
              isRootFolder={isRootFolder}
              isDropBoxComponent={true}
              canCreate={canCreate}
              getContextOptionsFolder={getContextOptionsFolder}
              getContextOptionsPlus={getContextOptionsPlus}
              toggleInfoPanel={toggleInfoPanel}
              toggleDropBox={toggleDropBox}
              isInfoPanelVisible={isInfoPanelVisible}
            />
          </StyledContainer>

          <VariableSizeList
            height={dropBoxHeight}
            width={"auto"}
            itemCount={countItems}
            itemSize={getItemSize}
            itemData={[
              navigationItems,
              onClickAvailable,
              { withLogo: !!withLogo },
            ]}
            outerElementType={CustomScrollbarsVirtualList}
          >
            {Row}
          </VariableSizeList>
        </StyledBox>
      </>
    );
  }
);

DropBox.propTypes = {
  width: PropTypes.number,
  changeWidth: PropTypes.bool,
  isRootFolder: PropTypes.bool,
  onBackToParentFolder: PropTypes.func,
  title: PropTypes.string,
  personal: PropTypes.bool,
  canCreate: PropTypes.bool,
  navigationItems: PropTypes.arrayOf(PropTypes.object),
  getContextOptionsFolder: PropTypes.func,
  getContextOptionsPlus: PropTypes.func,
  toggleDropBox: PropTypes.func,
  onClickAvailable: PropTypes.func,
};

export default React.memo(DropBox);
