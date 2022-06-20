import styled, { css } from "styled-components";
import ModalDialog from "@appserver/components/modal-dialog";
const commonStyles = css`
  .empty-folder_container {
    grid-template-areas:
      "img img"
      "headerText headerText";
    grid-template-rows: 72px 1fr;

    padding-bottom: 0;

    .ec-image {
      margin: auto;
    }
    .ec-header {
      margin: auto;
    }
  }
`;

const StyledModalDialog = styled(ModalDialog)`
  .heading {
    line-height: 52px;
    font-size: 21px;
  }
  .modal-dialog-aside-header {
    height: 53px;
  }
`;

const StyledBody = styled.div`
  .selection-panel_body {
    height: 495px;
    display: grid;
    grid-template-columns: 245px 1fr;
    grid-template-areas: "tree files" "footer footer";
    grid-template-rows: auto max-content;
    margin-right: -4px;

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
      border-right: 1px solid ${(props) => props.theme.row.borderBottom};

      display: grid;
      grid-template-rows: max-content auto;

      .selection-panel_folder-title {
        padding: 12px 20px 14px 0px;
      }
      .selection-panel_tree-folder {
        margin-left: -12px;
      }

      .span.rc-tree-switcher {
        padding-left: 16px;
      }
    }

    .selection-panel_files-header {
      padding: 16px;
      word-break: break-word;
      .selection-panel_title {
        ${(props) => props.header && "padding-top: 16px"};
      }
    }
    .selection-panel_footer {
      grid-area: footer;
      border-top: 1px solid ${(props) => props.theme.row.borderBottom};
      margin-left: -13px;
      margin-right: -7px;
      padding-left: 16px;

      padding-top: 16px;

      .selection-panel_buttons {
        ${(props) => props.footer && "margin-top:16px"};
        button:first-child {
          margin-right: 8px;
        }
      }
    }
  }

  ${commonStyles}
`;

const StyledRow = styled.div`
  display: grid;
  grid-template-columns: 32px auto 32px;
  grid-gap: 8px;
  position: relative;
  height: 48px;
  width: calc(100% - 16px);

  padding-left: 16px;

  cursor: pointer;

  ${(props) =>
    props.isChecked && `background: ${props.theme.row.backgroundColor}`};

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

  ${(props) =>
    props.folderSelection &&
    css`
      .selection-panel_icon {
        ::after {
          position: absolute;
          display: block;
          background-color: ${(props) =>
            props.theme.modalDialog.colorDisabledFileIcons};

          border-top-right-radius: 45%;
          left: 18px;
          top: 6px;
          width: 27px;
          height: 32px;
          content: "";
          opacity: 0.7;
        }
      }
      .selection-panel_text p {
        color: ${(props) => props.theme.text.disableColor};
      }
      cursor: default;
    `}
`;

const StyledAsideBody = styled.div`
  height: 100%;

  .selection-panel_aside-body {
    height: calc(100% - 32px);
    display: grid;
    grid-template-rows: max-content auto max-content;
  }
  .selection-panel_files-list-body {
    height: 100%;
    margin-left: -16px;
    margin-right: -6px;
  }
  .selection-panel_files {
    height: 100%;
  }

  .selection-panel_aside-header {
    margin-bottom: 12px;
    div:first-child {
      ${(props) => props.header && " margin-bottom: 12px;"}
    }
  }
  .selection-panel_aside-folder-title {
    margin-top: 12px;
  }
  .selection-panel_folder-selection-title {
    margin-bottom: 4px;
  }
  .selection-panel_aside-tree {
    margin-left: -16px;
    margin-right: -16px;
    max-width: 477px;
    overflow: hidden;
    .selection-panel_aside-loader {
      overflow: auto;
      padding-left: 16px;
    }
  }

  .selection-panel_aside-title {
    padding-bottom: 16px;
  }

  .selection-panel_aside-footer {
    border-top: 1px solid ${(props) => props.theme.row.borderBottom};
    margin-left: -13px;
    margin-right: -13px;
    padding-left: 16px;
    padding-right: 16px;
    padding-top: 16px;
    padding-bottom: 12px;

    .selection-panel_aside-buttons {
      ${(props) => props.footer && "margin-top:16px"};
      display: grid;
      grid-template-columns: 1fr 1fr;

      button:first-child {
        margin-right: 8px;
      }
    }
  }

  ${commonStyles}
`;

const StyledAsideHeader = styled.div`
  display: flex;
  align-items: center;

  .selection-panel_aside-header-icon {
    margin-right: 16px;
  }
`;

const StyledTree = styled.div`
  height: 100%;

  .files-tree-menu {
    margin-bottom: 22px;
    margin-top: 0px !important;
  }
  .selection-panel_tree-folder {
    height: 100%;
  }
  #folder-tree-scroll-bar {
    .nav-thumb-horizontal {
      height: 0px !important;
    }

    ${(props) =>
      props.displayType === "modal" &&
      css`
        .nav-thumb-vertical {
          margin-left: 4px !important;
          width: 4px !important;
        }
      `}

    .scroll-body {
      ${(props) => props.isLoadingNodes && "overflow-y: hidden !important"};
      overflow-x: hidden !important;
      padding-right: 0px !important;
    }
  }

  .selection-panel_empty-folder {
    margin-top: 12px;
    margin-left: 12px;
  }
`;

const StyledItemsLoader = styled.div`
  display: flex;
  .panel-loader {
    margin-left: 16px;
    margin-right: 8px;
  }
`;

export {
  StyledBody,
  StyledRow,
  StyledAsideBody,
  StyledAsideHeader,
  StyledTree,
  StyledModalDialog,
  StyledItemsLoader,
};
