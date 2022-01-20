import Text from '@appserver/components/text';
import styled, { css } from 'styled-components';

const StyledSelector = styled.div`
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

const StyledSelectorHeader = styled.div`
  height: 52px;
  min-height: 52px;

  padding: 0 16px;
  margin: 0;

  display: flex;
  align-items: center;
  justify-content: start;

  svg {
    cursor: pointer;
  }

  .arrow-button {
    margin-right: 12px;
  }
`;

const StyledSelectorSearchInput = styled.div`
  height: 32px;

  margin: 0 0 8px;
  padding: 0 16px;
`;

const StyledSelectorContent = styled.div`
  margin: 0;
  padding: 0;

  display: flex;
  flex-direction: column;
`;

const hoverSelectorItem = css`
  background: #f3f4f4;
`;

const StyledSelectorItem = styled.div`
  box-sizing: border-box !important;

  margin: 0;
  padding: 8px 16px;

  display: flex;
  align-items: center;
  justify-content: space-between;

  ${(props) => !props.isHeader && `cursor: pointer;`}

  ${(props) => props.isSelected && !props.isHeader && hoverSelectorItem}

  &:hover {
    ${(props) => !props.isHeader && hoverSelectorItem}
  }

  .selector-item__avatar {
    min-width: 32px !important;
    div {
      display: flex;
      align-items: center;
      justify-content: center;
    }
  }

  .selector-item__checkbox {
    min-width: 16px !important;
    margin-left: 12px;

    pointer-events: none;

    svg {
      margin: 0 !important;
    }
  }
`;

const StyledSelectorItemUser = styled.div`
  width: calc(100% - 16px - 12px);

  display: flex;
  align-items: center;
  justify-content: start;
`;

const StyledSelectorItemUserText = styled(Text)`
  font-size: 14px;
  line-height: 15px;

  ${(props) => props.isGroup && !props.isHeader && `border-bottom: 1px dashed #333333;`};

  margin-left: 12px;
`;

const StyledSelectorItemSeparator = styled.div`
  height: 1px;

  margin: 8px 16px;

  background: #dfe2e3;
`;

const StyledSelectorFooter = styled.div`
  position: fixed;
  bottom: 0;
  right: 0;

  z-index: 401;

  width: 425px;
  height: 72px;
  min-height: 72px;

  background: #ffffff;

  border-top: 1px solid #eceef1;

  box-sizing: border-box;

  padding: 0 16px;
  margin: 0;

  display: flex;
  align-items: center;
  justify-content: center;
`;

export {
  StyledSelector,
  StyledSelectorHeader,
  StyledSelectorSearchInput,
  StyledSelectorContent,
  StyledSelectorItem,
  StyledSelectorItemUser,
  StyledSelectorItemUserText,
  StyledSelectorItemSeparator,
  StyledSelectorFooter,
};
