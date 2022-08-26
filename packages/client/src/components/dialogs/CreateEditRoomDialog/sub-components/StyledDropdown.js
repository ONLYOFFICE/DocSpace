import styled from "styled-components";

import { smallTablet } from "@docspace/components/utils/device";

import DropDown from "@docspace/components/drop-down";
import { Base } from "@docspace/components/themes";

const StyledDropDownWrapper = styled.div`
  width: 100%;
  position: relative;
`;

const StyledDropDown = styled(DropDown)`
  margin-top: ${(props) => (props.marginTop ? props.marginTop : "4px")};
  padding: 6px 0;
  background: ${(props) =>
    props.theme.createEditRoomDialog.dropdown.background};
  border: 1px solid
    ${(props) => props.theme.createEditRoomDialog.dropdown.borderColor};
  box-shadow: 0px 12px 40px rgba(4, 15, 27, 0.12);
  border-radius: 3px;
  overflow: hidden;

  width: 446px;
  max-width: 446px;
  div {
    max-width: 446px;
  }

  @media ${smallTablet} {
    width: calc(100vw - 34px);
    max-width: calc(100vw - 34px);
    div {
      max-width: calc(100vw - 34px);
    }
  }

  .dropdown-item {
    height: 32px !important;
    max-height: 32px !important;
    cursor: pointer;
    box-sizing: border-box;
    width: 100%;
    padding: 6px 8px;
    font-weight: 400;
    font-size: 13px;
    line-height: 20px;
    &:hover {
      background: ${(props) =>
        props.theme.createEditRoomDialog.dropdown.item.hoverBackground};
    }

    &-separator {
      height: 7px !important;
      max-height: 7px !important;
    }
  }
`;

StyledDropDown.defaultProps = { theme: Base };

export { StyledDropDownWrapper, StyledDropDown };
