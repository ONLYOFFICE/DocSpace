import styled from "styled-components";

import Base from "@appserver/components/themes/base";

const StyledContainer = styled.div`
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  .span-toggle-content {
    cursor: pointer;
    user-select: none;

    svg {
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

    &:hover {
      border-bottom: ${(props) => props.theme.toggleContent.hoverBorderBottom};
    }
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
