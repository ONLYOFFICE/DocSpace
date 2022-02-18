import React, { useCallback, useEffect } from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";

import { VariableSizeList } from "react-window";
import CustomScrollbarsVirtualList from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";

import ArrowButton from "./arrow-btn";
import Text from "./text";
import ControlButtons from "./control-btn";
import Item from "./item";

import { isMobile, isMobileOnly } from "react-device-detect";
import {
  tablet,
  mobile,
  isMobile as IsMobileUtils,
  isTablet as isTabletUtils,
} from "@appserver/components/utils/device";
import { Base } from "@appserver/components/themes";

const StyledBox = styled.div`
  position: absolute;
  top: 0px;
  left: ${isMobile ? "-16px" : "-20px"};

  height: ${(props) => (props.height ? `${props.height}px` : "fit-content")};
  ${(props) =>
    props.changeWidth &&
    !isMobile &&
    css`
      width: ${(props) => `calc(${props.width}px + 24px)`};
    `}
  z-index: 399;
  display: flex;
  flex-direction: column;

  background: ${(props) => props.theme.navigation.background};

  filter: drop-shadow(0px 12px 40px rgba(4, 15, 27, 0.12));
  border-radius: 0px 0px 6px 6px;

  @media ${tablet} {
    top: 0px;
    left: -16px;
    width: ${(props) =>
      props.showText ? css`calc(100vw - 240px)` : css`calc(100vw - 52px)`};
    max-width: 100vw !important;
  }

  @media ${mobile} {
    width: 100vw !important;
    max-width: 100vw !important;
  }

  ${isMobile &&
  css`
    top: 0px;
    left: -16px;
    width: ${(props) =>
      props.showText ? css`calc(100vw - 240px)` : css`calc(100vw - 52px)`};
    max-width: 100vw !important;
  `}

  ${isMobileOnly &&
  css`
    width: 100vw !important;
    max-width: 100vw !important;
  `}
`;

StyledBox.defaultProps = { theme: Base };

const StyledContainer = styled.div`
  margin-top: 14px;
  margin-bottom: 6px;

  position: relative;
  top: 0px;

  align-items: center;

  min-width: 100px;
  max-width: calc(100vw - 32px);

  padding: ${isMobile ? "0px 16px" : "0px 20px"};

  display: grid;
  grid-template-columns: ${(props) =>
    props.canCreate ? "auto auto auto auto 1fr" : "auto auto auto 1fr"};

  @media ${tablet} {
    padding: 0 16px;
    margin-top: 17px;
    grid-template-columns: ${(props) =>
      props.canCreate ? "auto 1fr auto auto" : "auto 1fr auto"};
  }

  ${
    isMobile &&
    css`
      margin-top: 17px;
      grid-template-columns: ${(props) =>
        props.canCreate ? "auto 1fr auto auto" : "auto 1fr auto"};
    `
  }

  @media ${mobile} {
    padding: 0 16px;
    margin-top: 12px;
    // padding-bottom: 7px;
  }

  ${
    isMobileOnly &&
    css`
      margin-top: 12px;
      padding-bottom: 7px;
    `
  }

  .arrow-button {
    margin-right: 12px;
    min-width: 17px;

    align-items: center;
  }

  .add-button {
    margin-right: 10px;
    min-width: 17px;

    @media ${tablet} {
      display: none;
    }

    ${
      isMobile &&
      css`
        display: none;
      `
    }
  }

  .trash-button {
    min-width: 17px;
    margin-left: 6px;
  }

  .option-button {
    min-width: 17px;
  }
  }
`;

const Row = React.memo(({ data, index, style }) => {
  const isRoot = index === data[0].length - 1;
  return (
    <Item
      key={data[0][index].id}
      id={data[0][index].id}
      title={data[0][index].title}
      isRoot={isRoot}
      onClick={data[1]}
      style={{ ...style }}
    />
  );
});

const DropBox = React.forwardRef(
  (
    {
      width,
      height,
      showText,
      changeWidth,
      isRootFolder,
      onBackToParentFolder,
      title,
      personal,
      canCreate,
      navigationItems,
      getContextOptionsFolder,
      getContextOptionsPlus,
      toggleDropBox,
      onClickAvailable,
    },
    ref
  ) => {
    const [dropBoxHeight, setDropBoxHeight] = React.useState(0);
    const countItems = navigationItems.length;

    const getItemSize = (index) => {
      if (index === countItems - 1) return 51;
      return isMobile || IsMobileUtils() || isTabletUtils() ? 36 : 30;
    };

    React.useEffect(() => {
      const itemsHeight = navigationItems.map((item, index) =>
        getItemSize(index)
      );

      const currentHeight = itemsHeight.reduce((a, b) => a + b);

      setDropBoxHeight(
        currentHeight > window.innerHeight - 99
          ? window.innerHeight - 99
          : currentHeight
      );
    });

    return (
      <StyledBox
        ref={ref}
        width={width}
        height={height < dropBoxHeight ? height : null}
        showText={showText}
        changeWidth={changeWidth}
      >
        <StyledContainer canCreate={canCreate}>
          <ArrowButton
            isRootFolder={isRootFolder}
            onBackToParentFolder={onBackToParentFolder}
          />
          <Text title={title} isOpen={true} onClick={toggleDropBox} />
          <ControlButtons
            personal={personal}
            isRootFolder={isRootFolder}
            canCreate={canCreate}
            getContextOptionsFolder={getContextOptionsFolder}
            getContextOptionsPlus={getContextOptionsPlus}
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
