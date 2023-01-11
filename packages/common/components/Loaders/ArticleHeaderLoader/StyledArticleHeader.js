import { isMobile } from "react-device-detect";
import styled from "styled-components";
import { tablet } from "@docspace/components/utils/device";
const StyledContainer = styled.div`
  max-width: 211px;
  margin-left: 1px;
  padding: 18px 21px 21px 20px;

  @media ${tablet} {
    margin-left: 0;
  }

  ${isMobile} {
    margin-left: 0;
  }
`;

export default StyledContainer;
