import styled from "styled-components";
import Scrollbar from "@docspace/components/scrollbar";
import { Base } from "@docspace/components/themes";

const StyledEmbeddingPanel = styled.div`
  .embedding-panel {
    .scroll-body {
      padding-right: 0 !important;
    }
  }

  .embedding_header {
    padding: 0 16px;
    border-bottom: ${(props) => props.theme.filesPanels.sharing.borderBottom};

    .hotkeys_heading {
      font-weight: 700;
      font-size: 18px;
    }
  }
`;

StyledEmbeddingPanel.defaultProps = { theme: Base };

const StyledScrollbar = styled(Scrollbar)`
  position: relative;
  padding: 16px 0;
  height: calc(100vh - 87px) !important;
`;

const StyledBody = styled.div`
  .embedding-panel_body {
    padding: 20px 16px 0 16px;
  }

  .embedding-panel_links-container {
    display: flex;
    .embedding-panel_link {
      box-sizing: border-box;

      margin-right: 8px;

      border: 1px solid #eceef1;
      border-radius: 16px;
      line-height: 20px;
      padding: 3px 15px;

      text-decoration: none;
    }

    .embedding-panel_link_active {
      color: #ffffff;
      background: #265a8f;
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

  .embedding-panel_preview-button {
    margin-top: auto;
  }
`;

export { StyledEmbeddingPanel, StyledScrollbar, StyledBody };
