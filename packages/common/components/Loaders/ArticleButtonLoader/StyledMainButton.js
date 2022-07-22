import { tablet } from "@docspace/components/utils/device";
import { isTablet } from "react-device-detect";
import styled from "styled-components";

const StyledContainer = styled.div`
  width: 211px;
  margin: -9px 0 0;

  @media ${tablet} {
    display: none;
  }

  ${isTablet && "display: none"}
`;

export default StyledContainer;
