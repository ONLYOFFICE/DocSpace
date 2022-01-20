import Text from '@appserver/components/text';
import styled, { css } from 'styled-components';

import ToggleButton from '@appserver/components/toggle-button';

const StyledFilterBlock = styled.div`
  position: fixed;
  top: 0;
  right: 0;

  width: 425px;
  height: 100vh;

  z-index: 400;

  display: flex;
  flex-direction: column;

  background: #fff;
`;

const StyledFilterBlockHeader = styled.div`
  height: 52px;
  min-height: 52px;

  padding: 0 16px;
  margin: 0;

  border-bottom: 1px solid #eceef1;

  display: flex;
  align-items: center;
  justify-content: space-between;

  svg {
    cursor: pointer;
  }
`;

const StyledFilterBlockItem = styled.div`
  padding: ${(props) => (!props.withoutHeader ? '12px 16px 0px 16px' : '6px 16px 0px 16px')};

  display: flex;
  flex-direction: column;
  justify-content: start;
`;

const StyledFilterBlockItemHeader = styled.div`
  height: 16px;
  line-height: 16px;

  display: flex;
  align-items: center;
`;

const StyledFilterBlockItemContent = styled.div`
  margin-top: ${(props) => !props.withoutHeader && '12px'};

  height: fit-content;

  display: flex;
  flex-direction: row;
  align-items: center;
  flex-wrap: wrap;
`;

const StyledFilterBlockItemSelector = styled.div`
  height: 32px;
  width: 100%;

  display: flex;
  flex-direction: row;
  align-items: center;

  margin: 0 0 11px;
`;

const StyledFilterBlockItemSelectorText = styled(Text)`
  font-weight: 600;
  font-size: 13px;
  line-height: 15px;
  color: #a3a9ae;

  margin-left: 8px;
`;

const selectedItemTag = css`
  background: #265a8f;
  border-color: #265a8f;
`;

const StyledFilterBlockItemTag = styled.div`
  height: 30px;
  max-height: 30px;

  display: flex;
  flex-direction: row;
  align-items: center;

  border: 1px solid #eceef1;
  border-radius: 16px;

  box-sizing: border-box;

  padding: 4px 15px;

  margin: 0 6px 12px 0;

  cursor: pointer;

  ${(props) => props.isSelected && selectedItemTag}
`;

const selectedItemTagText = css`
  color: #ffffff;
`;

const StyledFilterBlockItemTagText = styled(Text)`
  height: 20px;

  font-weight: normal;
  font-size: 13px;
  line-height: 20px;

  ${(props) => props.isSelected && selectedItemTagText}
`;

const StyledFilterBlockItemTagIcon = styled.div`
  margin-left: 8px;

  display: flex;
  align-items: center;
  justify-content: space-between;
`;

const StyledFilterBlockItemToggle = styled.div`
  width: 100%;
  height: 36px;

  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: space-between;
`;

const StyledFilterBlockItemToggleText = styled(Text)`
  font-weight: 600;
  font-size: 13px;
  line-height: 36px;
`;

const StyledFilterBlockItemToggleButton = styled(ToggleButton)`
  position: static;
`;

const StyledFilterBlockItemSeparator = styled.div`
  height: 1px;
  width: 100%;

  background: #eceef1;

  margin: 2px 0 0 0;
`;

const StyledFilterBlockFooter = styled.div`
  position: fixed;
  bottom: 0;
  right: 0;

  z-index: 401;

  width: 425px;
  height: 72px;
  min-height: 72px;

  border-top: 1px solid #eceef1;

  box-sizing: border-box;

  padding: 0 16px;
  margin: 0;

  display: flex;
  align-items: center;
  justify-content: center;
`;

export {
  StyledFilterBlock,
  StyledFilterBlockHeader,
  StyledFilterBlockItem,
  StyledFilterBlockItemHeader,
  StyledFilterBlockItemContent,
  StyledFilterBlockItemSelector,
  StyledFilterBlockItemSelectorText,
  StyledFilterBlockItemTag,
  StyledFilterBlockItemTagText,
  StyledFilterBlockItemTagIcon,
  StyledFilterBlockItemToggle,
  StyledFilterBlockItemToggleText,
  StyledFilterBlockItemToggleButton,
  StyledFilterBlockItemSeparator,
  StyledFilterBlockFooter,
};
