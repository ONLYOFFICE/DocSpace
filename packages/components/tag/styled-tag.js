import styled, { css } from "styled-components";
import { ReactSVG } from "react-svg";

import Text from "../text";

const StyledTag = styled.div`
  width: fit-content;

  max-width: ${(props) => props.tagMaxWidth};

  display: flex;
  align-items: center;

  box-sizing: border-box;

  padding: 2px 10px;
  margin-right: 4px;

  background: ${(props) =>
    props.isDisabled ? "#F8F9F9" : props.isNewTag ? "#ECEEF1" : "#F3F4F4"};

  border-radius: 6px;

  .tag-text {
    line-height: 20px;

    pointer-events: none;
  }

  .tag-icon {
    margin-left: 12px;
    cursor: pointer;
  }

  ${(props) =>
    !props.isDisabled &&
    css`
      cursor: pointer;

      &:hover {
        background: #eceef1;
      }
    `}
`;

const StyledDropdownIcon = styled(ReactSVG)`
  display: flex;
  align-items: center;

  pointer-events: none;
`;

const StyledDropdownText = styled(Text)`
  display: flex;
  align-items: center;

  line-height: 30px;

  margin-left: 8px !important;

  pointer-events: none;
`;

export { StyledTag, StyledDropdownText, StyledDropdownIcon };
