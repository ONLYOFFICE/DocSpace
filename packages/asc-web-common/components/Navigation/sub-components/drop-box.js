import React, { useCallback, useEffect } from 'react';
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';

import { VariableSizeList } from 'react-window';
import CustomScrollbarsVirtualList from '@appserver/components/scrollbar/custom-scrollbars-virtual-list';

import ArrowButton from './arrow-btn';
import Text from './text';
import ControlButtons from './control-btn';
import Item from './item';

import { isMobile, isMobileOnly } from 'react-device-detect';
import {
  tablet,
  mobile,
  isMobile as IsMobileUtils,
  isTablet as isTabletUtils,
} from '@appserver/components/utils/device';

const StyledBox = styled.div`
  position: absolute;
  top: 1px;
  left: ${isMobile ? '-16px' : '-24px'};
  ${isMobile &&
  css`
    width: 100vw !important;
  `}
  height: fit-content;
  min-width: 185px;
  ${(props) =>
    props.changeWidth &&
    !isMobile &&
    css`
      width: ${(props) => `calc(${props.width}px + 24px)`};
    `}
  z-index: 399;
  display: flex;
  flex-direction: column;

  background: #ffffff;

  filter: drop-shadow(0px 12px 40px rgba(4, 15, 27, 0.12));
  border-radius: 0px 0px 6px 6px;

  @media ${tablet} {
    left: -16px;
    width: 100vw;
  }
`;

const StyledContainer = styled.div`
  position: relative;
  top: -1px;
  align-items: center;
  min-width: 100px;
  max-width: calc(100vw - 32px);
  height: ${isMobile ? (isMobileOnly ? '53px !important' : '61px !important') : '53px'};
  padding: ${isMobile ? '0px 16px' : '0px 24px'};
  @media ${tablet} {
    padding: 0px 16px;
    height: 61px;
  }

  @media ${mobile} {
    height: 53px;
  }
  display: grid;
  grid-template-columns: ${(props) =>
    props.canCreate ? 'auto auto auto auto 1fr' : 'auto auto auto 1fr'};

  @media ${tablet} {
    grid-template-columns: ${(props) => (props.canCreate ? 'auto 1fr auto auto' : 'auto 1fr auto')};
  }

  .arrow-button {
    margin-right: 15px;
    min-width: 17px;
    padding: 16px 0 12px;
    align-items: center;

    @media ${tablet} {
      padding: 20px 0 16px 8px;
      margin-left: -8px;
      margin-right: 16px;
    }
  }

  .add-button {
    margin-bottom: -1px;
    margin-left: 16px;

    @media ${tablet} {
      margin-left: auto;

      & > div:first-child {
        padding: 8px 8px 8px 8px;
        margin-right: -8px;
      }
    }
  }

  .option-button {
    margin-bottom: -1px;

    @media (min-width: 1024px) {
      margin-left: 8px;
    }

    @media ${tablet} {
      & > div:first-child {
        padding: 8px 8px 8px 8px;
        margin-right: -8px;
      }
    }
  }
`;

const Row = React.memo(({ data, index, style }) => {
  const isRoot = index === data[0].length - 1;
  const paddingBottom = isRoot ? '20px' : 0;
  return (
    <Item
      key={data[0][index].id}
      id={data[0][index].id}
      title={data[0][index].title}
      isRoot={isRoot}
      onClick={data[1]}
      style={{ ...style, paddingBottom: paddingBottom }}
    />
  );
});

const DropBox = React.forwardRef(
  (
    {
      width,
      height,
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
    ref,
  ) => {
    const countItems = navigationItems.length;

    const getItemSize = (index) => {
      if (index === countItems - 1) return 29;
      return isMobile || IsMobileUtils() || isTabletUtils() ? 36 : 30;
    };

    const getListHeight = useCallback(() => {
      const itemsHeight = navigationItems.map((item, index) => getItemSize(index));

      const currentHeight = itemsHeight.reduce((a, b) => a + b);

      if (isMobileOnly)
        return currentHeight + 30 > height - 109 ? height - 109 : currentHeight + 30;
      if (isMobile) return currentHeight + 30 > height - 120 ? height - 120 : currentHeight + 30;
      return currentHeight + 20 > height - 53 ? height - 53 : currentHeight + 20;
    }, [height]);

    // useEffect(() => {
    //   const items = [];
    //   const itemsText = [];
    //   const sectionWidth = document.getElementById('section').offsetWidth;
    //   navigationItems.forEach((item) => {
    //     items.push(document.getElementById(item.id));
    //     itemsText.push(document.getElementById(`item-text-${item.id}`));
    //   });
    //   let maxTextWidth = 0;

    //   itemsText.forEach((item) => {
    //     item.offsetWidth > maxTextWidth ? (maxTextWidth = item.offsetWidth) : null;
    //   });

    //   if (sectionWidth < maxTextWidth + 57) {
    //     setCurrentWidth(sectionWidth);
    //   } else {
    //     setCurrentWidth(maxTextWidth + 57);
    //   }

    //   items.forEach((item, index) => {});
    // }, []);
    return (
      <StyledBox ref={ref} width={width} height={height + 20} changeWidth={changeWidth}>
        <StyledContainer canCreate={canCreate}>
          <ArrowButton isRootFolder={isRootFolder} onBackToParentFolder={onBackToParentFolder} />
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
          height={getListHeight()}
          width={'auto'}
          itemCount={countItems}
          itemSize={getItemSize}
          itemData={[navigationItems, onClickAvailable]}
          outerElementType={CustomScrollbarsVirtualList}>
          {Row}
        </VariableSizeList>
      </StyledBox>
    );
  },
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
