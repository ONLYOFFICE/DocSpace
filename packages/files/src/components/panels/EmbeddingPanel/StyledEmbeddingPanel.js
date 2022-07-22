import styled, { css } from "styled-components";

const StyledBody = styled.div`
  .embedding-panel_body {
    padding: ${(props) => (props.isPersonal ? "0 4px 4px" : "0 16px")};
  }

  .embedding-panel_links-container {
    display: flex;
    .embedding-panel_link {
      margin-right: 8px;

      border: 1px solid #eceef1;
      border-radius: 16px;
      line-height: 20px;
      padding: 3px 15px;

      box-sizing: border-box;

      text-decoration: none;
    }

    .embedding-panel_active-link {
      background: #265a8f;

      color: #ffffff;
    }
  }

  .embedding-panel_inputs-container {
    display: flex;
    margin-top: 16px;

    .embedding-panel_input {
      margin-right: 8px;
      width: 94px;
    }
  }

  .embedding-panel_code-container {
    margin-top: 16px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  }

  .embedding-panel_text {
    padding: 0px 0 4px 0;
  }

  .embedding-panel_copy-icon {
    position: absolute;
    z-index: 1;
    margin: 8px;
    right: 16px;
  }

  .embedding-panel_copy-textarea {
    textarea {
      width: 330px;
    }
  }
`;

export default StyledBody;
