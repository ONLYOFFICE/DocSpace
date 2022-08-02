import styled from "styled-components";

import { desktop } from "@docspace/components/utils/device";

const StyledList = styled.div`
  padding: 0 16px;
`;
const StyledRow = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 16px 32px 1fr 16px;
  ${(props) =>
    props.withoutFirstRectangle &&
    props.withoutLastRectangle &&
    "grid-template-columns: 32px 1fr"};
  ${(props) =>
    props.withoutFirstRectangle &&
    !props.withoutLastRectangle &&
    "grid-template-columns: 32px 1fr 16px"};
  grid-template-rows: 1fr;
  grid-column-gap: 8px;
  margin-bottom: 16px;
  justify-items: center;
  align-items: center;
  .list-loader_rectangle {
    padding-right: 4px;
  }
  .list-loader_rectangle-content {
    width: 32px;
    height: 32px;
  }
  .list-loader_rectangle-row {
    margin-right: auto;
    max-width: 167px;
  }
`;

export { StyledRow, StyledList };
