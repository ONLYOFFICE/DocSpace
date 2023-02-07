import styled, { css } from "styled-components";

import { tablet } from "../utils/device";
import NoUserSelect from "../utils/commonStyles";
import Base from "../themes/base";

const activatedCss = css`
  cursor: pointer;
`;

const hoveredCss = css`
  cursor: pointer;
`;

const StyledGroupButton = styled.div`
  position: relative;
  display: inline-flex;
  vertical-align: middle;
`;

const StyledDropdownToggle = styled.div`
  font-family: Open Sans;
  font-style: normal;
  font-weight: ${(props) => props.fontWeight};
  font-size: ${(props) => props.theme.groupButton.fontSize};
  line-height: ${(props) => props.theme.groupButton.lineHeight};

  cursor: default;
  outline: 0;

  color: ${(props) =>
    props.disabled
      ? props.theme.groupButton.disableColor
      : props.theme.groupButton.color};

  float: ${(props) => props.theme.groupButton.float};
  height: ${(props) => props.theme.groupButton.height};
  margin: 14px 12px 19px ${(props) => (props.isSelect ? "0px" : "13px")};
  overflow: ${(props) => props.theme.groupButton.overflow};
  padding: ${(props) => props.theme.groupButton.padding};

  @media ${tablet} {
    margin: 18px 12px 19px ${(props) => (props.isSelect ? "0px" : "13px")};
  }

  text-align: center;
  text-decoration: none;
  white-space: nowrap;

  ${NoUserSelect}

  ${(props) =>
    !props.disabled &&
    (props.activated
      ? `${activatedCss}`
      : css`
          &:active {
            ${activatedCss}
          }
        `)}

  ${(props) =>
    !props.disabled &&
    (props.hovered
      ? `${hoveredCss}`
      : css`
          &:hover {
            ${hoveredCss}
          }
        `)}
`;
StyledDropdownToggle.defaultProps = { theme: Base };

const Caret = styled.div`
  display: inline-block;
  width: 8px;
  margin-left: 6px;

  ${(props) =>
    props.isOpen &&
    `
    padding-bottom: 2px;
    transform: scale(1, -1);
  `}
`;

const Separator = styled.div`
  vertical-align: middle;
  border: ${(props) => props.theme.groupButton.separator.border};
  width: ${(props) => props.theme.groupButton.separator.width};
  height: ${(props) => props.theme.groupButton.separator.height};
  margin: ${(props) => props.theme.groupButton.separator.margin};
`;
Separator.defaultProps = { theme: Base };

const StyledCheckbox = styled.div`
  display: inline-block;
  margin: ${(props) => props.theme.groupButton.checkbox.margin};

  @media ${tablet} {
    margin: ${(props) => props.theme.groupButton.checkbox.tabletMargin};
  }

  & > * {
    margin: 0px;
  }
`;
StyledCheckbox.defaultProps = { theme: Base };

export {
  StyledCheckbox,
  Separator,
  Caret,
  StyledDropdownToggle,
  StyledGroupButton,
};
