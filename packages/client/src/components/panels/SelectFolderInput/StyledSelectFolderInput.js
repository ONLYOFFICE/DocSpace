import styled, { css } from "styled-components";

const StyledComponent = styled.div`
  .select-folder_file-input {
    margin-bottom: 16px;
    margin-top: 3px;
    width: 100%;
    max-width: ${props => (props.maxWidth ? props.maxWidth : "350px")};
    :hover {
      .icon {
        svg {
          path {
            fill: ${props => props.theme.iconButton.hoverColor};
          }
        }
      }
    }
  }

  .panel-loader-wrapper {
    margin-top: 8px;
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding-right: 32px;
          `
        : css`
            padding-left: 32px;
          `}
  }
  .panel-loader {
    display: inline;
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-left: 10px;
          `
        : css`
            margin-right: 10px;
          `}
  }
`;

export default StyledComponent;
