import styled, { css } from "styled-components";

const StyledDialogAsideLoader = styled.div`
  ${(props) =>
    props.isPanel &&
    css`
      .dialog-aside-loader {
        padding: 0;
        transform: translateX(${(props) => (props.visible ? "0" : "500px")});
        width: 500px;

        @media (max-width: 550px) {
          width: 320px;
          transform: translateX(${(props) => (props.visible ? "0" : "320px")});
        }
      }
    `}

  ${(props) =>
    props.isPanel
      ? css`
          .dialog-loader-header {
            padding: 12px 16px;
          }

          .dialog-loader-body {
            padding: 16px;
          }

          .dialog-loader-footer {
            padding: 12px 16px;
            position: fixed;
            bottom: 0;
            width: 468px;

            @media (max-width: 550px) {
              width: 288px;
            }
          }
        `
      : css`
          .dialog-loader-header {
            border-bottom: ${(props) =>
              `1px solid ${props.theme.modalDialog.headerBorderColor}`};
            padding: 12px 16px;
          }

          .dialog-loader-body {
            padding: 16px;
          }

          .dialog-loader-footer {
            ${(props) =>
              props.withFooterBorder &&
              `border-top: 1px solid ${props.theme.modalDialog.headerBorderColor}`};
            padding: 16px;
            position: fixed;
            bottom: 0;
            width: calc(100% - 32px);
          }
        `}
`;

export default StyledDialogAsideLoader;
