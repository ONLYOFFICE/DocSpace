import styled, { css } from "styled-components";
import { isMobileOnly } from "react-device-detect";

const StyledModuleTile = styled.div`
  width: auto;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  user-select: none;
  margin-top: 9px;

  &:hover {
    .selectable {
      text-decoration: underline;
    }
  }

  .sub-title-content {
    text-align: center;
    cursor: pointer;

    .sub-title-image {
      border: none;
      height: 64px;
      width: 64px;
      background: #ffffff;
      box-shadow: 0px 4px 20px rgba(85, 85, 85, 0.1);
      border-radius: 6px;
      padding: 10px;
    }
    .sub-title-text {
      text-align: center;
      font-weight: 600;
      margin-top: 5px;
    }

    ${isMobileOnly &&
    css`
      .sub-title-image {
        height: 54px;
        width: 54px;
      }

      .sub-title-text {
        font-size: 14px;
      }
    `}

    a {
      text-decoration: none;
    }
  }
`;

export default StyledModuleTile;
