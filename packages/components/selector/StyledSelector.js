import styled, { css } from "styled-components";

import Base from "../themes/base";

const StyledSelector = styled.div`
  width: 100%;
  height: 100%;

  display: flex;
  flex-direction: column;

  overflow: hidden;
`;

StyledSelector.defaultProps = { theme: Base };

const StyledSelectorHeader = styled.div`
  width: calc(100% - 32px);
  min-height: 53px;
  height: 53px;
  max-height: 53px;

  padding: 0 16px;

  border-bottom: ${(props) => props.theme.selector.border};

  display: flex;
  align-items: center;

  .arrow-button {
    cursor: pointer;
    margin-right: 12px;
  }

  .heading-text {
    font-weight: 700;
    font-size: 21px;
    line-height: 28px;
  }
`;

StyledSelectorHeader.defaultProps = { theme: Base };

const StyledSelectorBody = styled.div`
  width: 100%;

  height: ${(props) =>
    props.footerVisible
      ? `calc(100% - 16px - ${props.footerHeight}px - ${props.headerHeight}px)`
      : `calc(100% - 16px - ${props.headerHeight}px)`};

  padding: 16px 0 0 0;

  .search-input,
  .search-loader {
    padding: 0 16px;

    margin-bottom: 12px;
  }
`;

StyledSelectorBody.defaultProps = { theme: Base };

const StyledSelectorFooter = styled.div`
  width: calc(100% - 32px);
  max-height: 73px;
  height: 73px;
  min-height: 73px;

  padding: 0 16px;

  display: flex;
  align-items: center;
  justify-content: space-between;

  gap: 8px;

  border-top: ${(props) => props.theme.selector.border};

  .button {
    min-height: 40px;

    margin-bottom: 2px;
  }
`;

StyledSelectorFooter.defaultProps = { theme: Base };

export {
  StyledSelector,
  StyledSelectorHeader,
  StyledSelectorBody,
  StyledSelectorFooter,
};
