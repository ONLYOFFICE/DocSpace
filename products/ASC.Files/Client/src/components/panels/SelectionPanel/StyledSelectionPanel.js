import styled from "styled-components";

const StyledBody = styled.div`
  .selection-panel_body {
    height: 507px;
    display: grid;
    grid-template-columns: 256px 1fr;
    grid-template-areas: "tree files" "footer footer";
    grid-template-rows: auto max-content;

    .selection-panel_files-body {
      grid-area: files;
      display: grid;
      grid-template-rows: max-content auto;
    }
    .selection-panel_files-list-body {
      height: 100%;
    }
    .selection-panel_tree-body {
      grid-area: tree;
      height: 100%;
      border-right: 1px solid #eceef1;

      display: grid;
      grid-template-rows: max-content auto;

      .selection-panel_folder-title {
        padding: 20px 12px 20px 0px;
      }
    }

    .selection-panel_files-header {
      padding: 16px;
    }

    .selection-panel_footer {
      grid-area: footer;
      border-top: 1px solid #eceef1;
      margin-left: -13px;
      margin-right: -13px;
      padding-left: 16px;

      .selection-panel_buttons {
        padding-top: 16px;

        button:first-child {
          margin-right: 8px;
        }
      }
    }
  }
`;

const StyledRow = styled.div`
  display: grid;
  grid-template-columns: 32px auto 32px;
  grid-gap: 8px;
  position: relative;
  height: 48px;
  width: calc(100% - 16px);
  padding-left: 16px;

  .selection-panel_clicked-area {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    cursor: pointer;
  }
  .selection-panel_text {
    margin-top: auto;
    margin-bottom: 16px;
    overflow: hidden;
    p {
      text-overflow: ellipsis;
      overflow: hidden;
      white-space: nowrap;
    }
  }
  .selection-panel_icon,
  .selection-panel_checkbox {
    margin: auto 0;
  }

  .selection-panel_icon {
    svg {
      path {
        fill: #a3a9ae;
      }
    }
  }
`;
export { StyledBody, StyledRow };
