import styled from "styled-components";

const StyledComponent = styled.div`
  padding-top: 3px;
  width: 100%;

  .header {
    font-weight: 700;
    font-size: 16px;
    line-height: 22px;
  }

  .preview-header {
    padding-bottom: 20px;
  }

  .theme-standard {
    padding-top: 21px;
  }

  .theme-name {
    font-size: 15px;
    line-height: 16px;
    font-weight: 600;
  }

  .theme-container {
    padding: 12px 0 24px 0;
    display: flex;
  }

  .box {
    width: 46px;
    height: 46px;
    margin-right: 12px;
    border-radius: 8px;
    cursor: pointer;
  }

  .check-img {
    padding: 18px 0 0 15px;
  }

  .add-theme {
    background: #d0d5da;
    padding-top: 16px;
    padding-left: 16px;
    box-sizing: border-box;
  }

  .buttons-container {
    padding-top: 24px;
  }
`;

export { StyledComponent };
