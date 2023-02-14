import styled from "styled-components";
import DropDownItem from "@docspace/components/drop-down-item";

export const StyledDropDownItem = styled(DropDownItem)`
  color: #fff;

  .drop-down-item_icon svg {
    path {
      fill: #fff !important;
    }
  }

  &:hover {
    background: #444;
  }
`;
