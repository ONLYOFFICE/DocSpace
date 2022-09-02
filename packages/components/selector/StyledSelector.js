import styled, { css } from "styled-components";

const StyledSelector = styled.div`
  width: 100%;
  height: 100%;

  display: flex;
  flex-direction: column;

  overflow: hidden;
`;

const StyledSelectorHeader = styled.div`
  width: calc(100% - 32px);
  min-height: 53px;
  height: 53px;
  max-height: 53px;

  padding: 0 16px;

  border-bottom: 1px solid #eceef1;

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

const StyledSelectorBody = styled.div`
  width: 100%;
  height: 100%;

  padding: 16px 0;

  box-sizing: border-box;

  .search-input {
    padding: 0 16px;

    margin-bottom: 12px;
  }
`;

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

  border-top: 1px solid #eceef1;

  .button {
    min-height: 40px;

    margin-bottom: 2px;
  }
`;

export {
  StyledSelector,
  StyledSelectorHeader,
  StyledSelectorBody,
  StyledSelectorFooter,
};
