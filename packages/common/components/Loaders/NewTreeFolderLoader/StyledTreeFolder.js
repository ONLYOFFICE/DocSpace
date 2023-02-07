import styled from "styled-components";
import { desktop } from "@docspace/components/utils/device";

const StyledTreeFolder = styled.div`
  padding-right: 16px;
`;

const StyledLoader = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 8px 16px 1fr;
  grid-template-rows: 1fr;
  grid-column-gap: 6px;
  margin-bottom: 8px;
  box-sizing: border-box;

  .tree-node-loader_additional-rectangle {
    padding-top: 4px;
  }

  ${(props) => props.paddingLeft && `padding-left: ${props.paddingLeft}`};
`;

export { StyledLoader, StyledTreeFolder };
