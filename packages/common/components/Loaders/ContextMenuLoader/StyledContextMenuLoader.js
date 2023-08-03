import styled from "styled-components";
import { tablet } from "@docspace/components/utils/device";

const StyledContextMenuLoader = styled.div`
  width: 100%;
  height: 32px;
  display: grid;
  grid-template-columns: 16px 1fr;
  grid-column-gap: 8px;
  justify-items: center;
  align-items: center;

  @media ${tablet} {
    padding: 4px 12px 4px 16px;
    grid-column-gap: 0px;
  }

  .context-menu-rectangle {
    margin-right: auto;
    margin-left: 8px;
  }
`;

export { StyledContextMenuLoader };
