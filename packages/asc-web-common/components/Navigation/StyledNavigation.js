import styled, { css } from "styled-components";
import { isMobile, isMobileOnly } from "react-device-detect";
import { tablet, desktop, mobile } from "@appserver/components/utils/device";

const StyledContainer = styled.div`
  padding: ${(props) => (props.isDropBox ? "14px 0 3px" : "14px 0 0px")};

  width: fit-content;

  display: grid;

  grid-template-columns: ${(props) =>
    props.isRootFolder ? "1fr auto" : "29px 1fr auto"};

  align-items: center;

  .arrow-button {
    width: 17px;
    min-width: 17px;
  }

  @media ${tablet} {
    width: 100%;
    padding: ${(props) => (props.isDropBox ? "16px 0 5px" : "16px 0 0px")};
  }
  ${isMobile &&
  css`
    width: 100%;
    padding: ${(props) =>
      props.isDropBox ? "16px 0 5px" : " 16px 0 0px"} !important;
  `}

  @media ${mobile} {
    width: 100%;
    padding: ${(props) => (props.isDropBox ? "12px 0 5px" : "12px 0 0")};
  }

  ${isMobileOnly &&
  css`
    width: 100% !important;
    padding: ${(props) =>
      props.isDropBox ? "18px 0 5px" : "18px 0 0"} !important;
  `}
`;

export default StyledContainer;
