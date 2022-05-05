import styled from "styled-components";
import Scrollbar from "@appserver/components/scrollbar";
import { tablet } from "@appserver/components/utils/device";

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

      @media ${tablet} {
        height: unset;
      }
    }
  }

  .hotkey-key-description {
    max-width: 320px;
    width: 100%;

    @media (max-width: 550px) {
      max-width: 140px;
      word-wrap: break-word;
      white-space: normal;
    }
  }

  .hotkeys-key {
    margin: 0 auto 0 0;

    @media (max-width: 550px) {
      margin: 0 0 0 auto;
      width: fit-content;
    }
  }
`;

const StyledScrollbar = styled(Scrollbar)`
  position: relative;
  padding: 16px 0;
  height: calc(100vh - 87px) !important;
`;

export { StyledHotkeysPanel, StyledScrollbar };
