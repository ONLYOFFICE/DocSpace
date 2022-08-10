import styled, { css } from "styled-components";
import { isMobile } from "react-device-detect";
import { mobile, tablet } from "@docspace/components/utils/device";

const StyledFilter = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 1fr 95px;
  grid-template-rows: 1fr;
  grid-column-gap: 8px;

  /* ${isMobile &&
  css`
    margin-top: -22px;
  `} */

  @media ${mobile} {
    grid-template-columns: 1fr 50px;
  }
`;

export default StyledFilter;
