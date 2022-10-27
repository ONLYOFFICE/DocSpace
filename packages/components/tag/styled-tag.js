import styled, { css } from "styled-components";
import { ReactSVG } from "react-svg";

import Text from "../text";
import Base from "../themes/base";

const StyledTag = styled.div`
  width: fit-content;

  max-width: ${(props) => (props.tagMaxWidth ? props.tagMaxWidth : "100%")};

  display: flex;
  align-items: center;

  box-sizing: border-box;

  padding: 2px 10px;
  margin-right: 4px;

  background: ${(props) =>
    props.isDisabled
      ? props.theme.tag.disabledBackground
      : props.isNewTag
      ? props.theme.tag.newTagBackground
      : props.theme.tag.background};

  border-radius: 6px;

  .tag-text {
    color: ${(props) =>
      props.isDefault
        ? props.theme.tag.defaultTagColor
        : props.theme.tag.color};
    line-height: 20px;
    pointer-events: none;
  }

  .tag-icon {
    margin-left: 12px;
    cursor: pointer;
  }

  .third-party-tag {
    width: 16px;
    height: 16px;
  }

  ${(props) =>
    !props.isDisabled &&
    css`
      cursor: pointer;

      &:hover {
        background: ${(props) => props.theme.tag.hoverBackground};
      }
    `}
`;

StyledTag.defaultProps = { theme: Base };

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
