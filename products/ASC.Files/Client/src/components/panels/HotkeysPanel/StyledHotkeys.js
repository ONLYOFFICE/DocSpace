import styled from "styled-components";
import Scrollbar from "@appserver/components/scrollbar";

const StyledHotkeysPanel = styled.div`
  .hotkeys-panel {
    width: 500px;

    @media (max-width: 550px) {
      width: 320px;
    }

    .scroll-body {
      padding-right: 0 !important;
    }
  }

  .hotkeys_header {
    margin: 0 16px;
    border-bottom: 1px solid #eceef1;

    .hotkeys_heading {
      font-weight: 700;
      font-size: 18px;
    }
  }

  .hotkeys_sub-header {
    font-weight: 700;
    font-size: 16px;
    padding-left: 16px;
    margin: 20px 0 6px 0;
  }

  .hotkeys_row {
    width: calc(100% - 32px);
    min-height: 41px;
    margin: 0 16px;
    box-sizing: border-box;
    border-bottom: none;

    .row_content {
      margin: 12px 0 12px 0px;
    }
  }

  .hotkey-key-description {
    max-width: 320px;
  }

  .hotkeys-key {
    margin-left: auto;
    width: 112px;
  }
`;

const StyledScrollbar = styled(Scrollbar)`
  position: relative;
  padding: 16px 0;
  height: calc(100vh - 87px) !important;
`;

export { StyledHotkeysPanel, StyledScrollbar };
