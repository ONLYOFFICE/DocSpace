import { css } from "styled-components";
import { isMobile } from "react-device-detect";

const marginStyles = css`
  margin-left: -24px;
  margin-right: -24px;
  padding-left: 24px;
  padding-right: 24px;

  ${isMobile &&
  css`
    margin-left: -20px;
    margin-right: -20px;
    padding-left: 20px;
    padding-right: 20px;
  `}

  @media (max-width: 1024px) {
    margin-left: -16px;
    margin-right: -16px;
    padding-left: 16px;
    padding-right: 16px;
  }

  @media (max-width: 375px) {
    margin-left: -16px;
    margin-right: -8px;
    padding-left: 16px;
    padding-right: 8px;
  }
`;

export default marginStyles;
