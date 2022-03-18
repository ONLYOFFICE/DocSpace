import { isMobile } from "react-device-detect";
import styled from "styled-components";
import { tablet } from "@appserver/components/utils/device";
const StyledContainer = styled.div`
  max-width: 216px;
  margin-left: 1px;

  @media ${tablet} {
    margin-left: 0;
  }

  ${isMobile} {
    margin-left: 0;
  }
`;

export default StyledContainer;
