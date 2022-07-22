import styled, { css } from "styled-components";
import RectangleLoader from "../RectangleLoader";

import { isMobile, isMobileOnly } from "react-device-detect";
import { tablet, mobile } from "@docspace/components/utils/device";

const StyledContainer = styled.div`
  margin: 0;

  max-width: 211px;
  padding: 0 20px 0 0;

  @media ${tablet} {
    width: ${(props) => (props.showText ? "240px" : "52px")};
    padding: ${(props) =>
      props.showText ? "0 16px 0 16px" : "10px 16px 10px 12px"};
    box-sizing: border-box;
  }

  ${isMobile &&
  css`
    max-width: ${(props) => (props.showText ? "240px" : "52px")};
    width: ${(props) => (props.showText ? "240px" : "52px")};
    padding: ${(props) =>
      props.showText ? "0 16px 0 16px" : "10px 16px 10px 12px"};
    box-sizing: border-box;
  `}

  @media ${mobile} {
    width: 100%;
    padding: 0 16px;
    box-sizing: border-box;
  }

  ${isMobileOnly &&
  css`
    width: 100%;
    padding: 0 16px;
    box-sizing: border-box;
  `}
`;

const StyledBlock = styled.div`
  margin: 0;
  width: 100%;
  height: auto;
  display: flex;
  flex-direction: column;
  margin-bottom: 20px;

  @media ${tablet} {
    margin-bottom: 24px;
  }
`;

const StyledRectangleLoader = styled(RectangleLoader)`
  height: 20px;
  width: 216px;
  padding: 0 0 16px;

  @media ${tablet} {
    height: 20px;
    width: 20px;
    padding: 0 0 24px;
  }

  ${isMobile &&
  css`
    height: 20px;
    width: 20px;
    padding: 0 0 24px;
  `}

  @media ${mobile} {
    width: 100%;
    padding: 0 0 24px;
  }

  ${isMobileOnly &&
  css`
    width: 100% !important;
    padding: 0 0 24px !important;
  `}
`;

export { StyledBlock, StyledContainer, StyledRectangleLoader };
