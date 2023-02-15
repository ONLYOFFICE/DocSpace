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

import { isMobile, isMobileOnly } from "react-device-detect";
import {
  tablet,
  mobile,
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "@docspace/components/utils/device";

import { Base } from "@docspace/components/themes";

const StyledBox = styled.div`
  position: absolute;
  top: 0px;
  left: ${isMobile ? "-16px" : "-20px"};

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
          ? sectionHeight - navHeight
          : currentHeight
      );
    }, [sectionHeight]);

    return (
      <>
        <StyledBox
          ref={ref}
          maxHeight={maxHeight}
          height={sectionHeight < dropBoxHeight ? sectionHeight : null}
          showText={showText}
          dropBoxWidth={dropBoxWidth}
          isDesktop={isDesktop}
        >
          <StyledContainer
            canCreate={canCreate}
            isDropBoxComponent={true}
            isInfoPanelVisible={isInfoPanelVisible}
            isDesktopClient={isDesktopClient}
          >
            <ArrowButton
              isRootFolder={isRootFolder}
              onBackToParentFolder={onBackToParentFolder}
            />
            <Text title={title} isOpen={true} onClick={toggleDropBox} />
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
            itemData={[navigationItems, onClickAvailable]}
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
