import styled from "styled-components";
import { utils } from "asc-web-components";
const { mobile, smallTablet } = utils.device;

const StyledGroup = styled.div`
  width: 400px;
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: 1fr 1fr 1fr 1fr;
  grid-row-gap: 14px;

  @media ${mobile}, ${smallTablet} {
    width: 100%;
  }
`;

const StyledLastRow = styled.div`
  display: grid;
  grid-template-columns: 100px 100px;
  grid-template-rows: 1fr;
  grid-column-gap: 8px;
`;

const StyledSpacer = styled.div`
  margin-top: 32px;
`;

export { StyledGroup, StyledLastRow, StyledSpacer };
