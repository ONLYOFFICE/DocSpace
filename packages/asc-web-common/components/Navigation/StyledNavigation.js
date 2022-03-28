import styled, { css } from "styled-components";
import { isMobile, isMobileOnly } from "react-device-detect";
import { tablet, desktop, mobile } from "@appserver/components/utils/device";

const StyledContainer = styled.div`
  width: 100% !important;
  display: grid;
  align-items: center;
  grid-template-columns: ${(props) =>
    props.isRootFolder ? "auto 1fr" : "29px auto 1fr"};

  .arrow-button {
    width: 17px;
    min-width: 17px;
  }

  height: 53px;

  @media ${tablet} {
    height: 61px;
  }

  @media ${mobile} {
    height: 53px;
  }
`;

export default StyledContainer;
