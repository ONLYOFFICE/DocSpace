import styled, { css } from "styled-components";

const StyledDialogAsideLoader = styled.div`
  ${(props) =>
    props.isPanel
      ? css`
          .dialog-loader-header {
            padding: 12px 16px;

            height: 53px;

            border-bottom: ${(props) =>
              `1px solid ${props.theme.modalDialog.headerBorderColor}`};

            box-sizing: border-box;
          }

          .dialog-loader-body {
            padding: 16px;
          }

          .dialog-loader-footer {
            padding: 12px 16px;
            position: fixed;
            bottom: 0;

            height: 71px;

            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 8px;

            box-sizing: border-box;

            border-top: ${(props) =>
              `1px solid ${props.theme.modalDialog.headerBorderColor}`};
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
