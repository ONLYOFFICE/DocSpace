import Scrollbar from "../scrollbar";
import styled, { css } from "styled-components";
import NoUserSelect from "../utils/commonStyles";
import Base from "../themes/base";

const StyledScrollbar = styled(Scrollbar)`
  width: ${(props) => props.theme.tabsContainer.scrollbar.width} !important;
  height: ${(props) => props.theme.tabsContainer.scrollbar.height} !important;
`;

StyledScrollbar.defaultProps = { theme: Base };

const NavItem = styled.div`
  position: relative;
  white-space: nowrap;
  display: flex;
`;
NavItem.defaultProps = { theme: Base };

const Label = styled.div`
  height: ${(props) => props.theme.tabsContainer.label.height};
  border-radius: ${(props) => props.theme.tabsContainer.label.borderRadius};
  min-width: ${(props) => props.theme.tabsContainer.label.minWidth};
  margin-right: ${(props) => props.theme.tabsContainer.label.marginRight};
  width: ${(props) => props.theme.tabsContainer.label.width};

  .title_style {
    text-align: center;
    margin: ${(props) => props.theme.tabsContainer.label.title.margin};
    overflow: ${(props) => props.theme.tabsContainer.label.title.overflow};

    ${NoUserSelect}
  }

  ${(props) =>
    props.isDisabled &&
    css`
      pointer-events: none;
    `}

  ${(props) =>
    props.selected
      ? css`
          cursor: default;
          .title_style {
            color: ${(props) => props.theme.tabsContainer.label.title.color};
          }
        `
      : css`
          &:hover {
            cursor: pointer;
            background-color: ${(props) =>
              props.theme.tabsContainer.label.hoverBackgroundColor};
            .title_style {
              color: ${(props) =>
                props.theme.tabsContainer.label.title.hoverColor};
            }
          }
        `}

${(props) =>
    props.isDisabled &&
    props.selected &&
    css`
      background-color: ${(props) =>
        props.theme.tabsContainer.label.disableBackgroundColor};
      .title_style {
        color: ${(props) => props.theme.tabsContainer.label.title.disableColor};
      }
    `}
`;

Label.defaultProps = { theme: Base };
export { NavItem, Label, StyledScrollbar };
