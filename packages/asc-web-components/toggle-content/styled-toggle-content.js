import styled, { css } from "styled-components";

import Base from "../themes/base";

const StyledContainer = styled.div`
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  .span-toggle-content {
    cursor: pointer;
    user-select: none;

    svg {
      ${(props) =>
        !props.enableToggle &&
        css`
          display: none;
        `}

      path {
        fill: ${(props) => props.theme.toggleContent.iconColor};
      }
    }
  }

  .arrow-toggle-content {
    margin-right: ${(props) => props.theme.toggleContent.arrowMarginRight};
    margin-bottom: ${(props) => props.theme.toggleContent.arrowMarginBottom};

    transform: ${(props) => props.open && props.theme.toggleContent.transform};
  }

  .heading-toggle-content {
    height: ${(props) => props.theme.toggleContent.headingHeight};
    line-height: ${(props) => props.theme.toggleContent.headingHeight};
    box-sizing: border-box;
    font-style: normal;

    ${(props) =>
      props.enableToggle
        ? css`
            &:hover {
              border-bottom: ${(props) =>
                props.theme.toggleContent.hoverBorderBottom};
            }
          `
        : css`
            cursor: default;
          `}
  }
`;
StyledContainer.defaultProps = { theme: Base };

const StyledContent = styled.div`
  color: ${(props) => props.theme.toggleContent.childrenContent.color};
  padding-top: ${(props) =>
    props.theme.toggleContent.childrenContent.paddingTop};
  display: ${(props) => (props.isOpen ? "block" : "none")};
`;

StyledContent.defaultProps = { theme: Base };

export { StyledContent, StyledContainer };
