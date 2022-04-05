import styled, { css } from "styled-components";

const StyledContent = styled.div`
  width: 100%;
  height: 100%;

  display: grid;

  grid-template-columns: 1fr;
  grid-template-rows: auto 1fr auto;
`;

const StyledHeaderContent = styled.div`
  width: calc(100% - 16px);
  max-width: calc(100% - 16px);
  height: 52px;

  border-bottom: 1px solid #eceef1;

  padding: 0 16px;

  margin-bottom: 24px;
  margin-right: -16px;

  display: flex;
  align-items: center;
  justify-content: space-between;

  .sharing_panel-header-info {
    max-width: calc(100% - 33px);

    display: flex;
    align-items: center;
    justify-content: start;

    .sharing_panel-arrow {
      margin-right: 16px;
    }

    .sharing_panel-header {
    }
  }

  .sharing_panel-icons-container {
    display: flex;
    margin-left: 16px;
  }
`;

const StyledBodyContent = styled.div`
  width: calc(100%);
  height: 100%;

  padding: 0 0 0 16px;
  margin-right: -16px;

  display: flex;
  flex-direction: column;
  align-items: start;
`;

const StyledExternalLink = styled.div`
  width: 100%;
  height: 40px;

  margin-bottom: 32px;

  background-color: #f8f9f9;
`;

const StyledFooterContent = styled.div`
  width: calc(100% - 16px);
  border-top: 1px solid #eceef1;

  padding: 16px;

  display: flex;
  flex-direction: column;
  align-items: start;

  .sharing_panel-notification {
    margin-bottom: 16px;
  }

  .sharing_panel-checkbox {
    margin-bottom: 16px;
  }

  .sharing_panel-button {
    max-height: 40px;
  }
`;

export {
  StyledContent,
  StyledHeaderContent,
  StyledBodyContent,
  StyledExternalLink,
  StyledFooterContent,
};
