import styled from "styled-components";
import { desktop } from "@appserver/components/utils/device";

const StyledContainer = styled.div`
  width: 209px;

  @media ${desktop} {
    width: 225px;
  }
`;

export default StyledContainer;
