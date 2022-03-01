import styled, { css } from "styled-components";

const StyledComponent = styled.div`
  .convert-password-dialog_caption {
    padding-bottom: 12px;
  }

  .convert-password-dialog_button-accept {
    margin-right: 8px;
  }

  .conversation-password-wrapper {
    display: block;
  }

  ${(props) =>
    props.isTabletView
      ? css`
          .convert-password_footer {
            position: fixed;
            bottom: 0;
            width: 100%;
            display: grid;
            max-width: 292px;
            grid-template-columns: 1fr 1fr;
            grid-gap: 8px;
          }
          .convert-password-dialog_button {
            width: 100%;
          }
        `
      : css`
          #convert-password-dialog_button-accept {
            margin-right: 8px;
          }
        `}
`;

export default StyledComponent;
