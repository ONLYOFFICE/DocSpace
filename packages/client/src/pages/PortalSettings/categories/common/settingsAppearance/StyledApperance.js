import styled, { css } from "styled-components";
import { Base } from "@docspace/components/themes";

const StyledComponent = styled.div`
  padding-top: 5px;

  .header {
    font-weight: 700;
    font-size: 18px;
    line-height: 24px;
  }

  .preview-header {
    padding-bottom: 8px;
  }

  .theme-standard {
    padding-top: 16px;
  }

  .theme-name {
    font-size: 15px;
    line-height: 16px;
    font-weight: 600;
  }

  .theme-container {
    padding: 16px 0 16px 0;
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
    padding-top: 32px;
  }
`;

export { StyledComponent };
