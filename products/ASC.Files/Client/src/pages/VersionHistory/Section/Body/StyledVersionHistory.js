import styled from "styled-components";
import Row from "@appserver/components/row";
import { tablet } from "@appserver/components/utils/device";

const StyledBody = styled.div`
  height: 100%;
  width: 100%;
  .version-list {
    height: 100%;
    width: 100%;
  }

  .loader-history-rows {
    padding-right: 16px;
  }
`;

const StyledVersionList = styled.div`

.row_context-menu-wrapper {
    .expandButton {
      ${(props) =>
        props.isRestoreProcess &&
        `
        touch-action: none;
        pointer-events: none;
        `}
      svg {
        path {
          ${(props) =>
            props.isRestoreProcess &&
            `
              fill: #d0d5da;
            `};
        }
      }
    }
  
  }

  .row_content {

    .version_link,
    .version-link-file,
    .version_content-length,
    .version_link-action,
    .row_context-menu-wrapper,
    .version_text {
      ${(props) =>
        props.isRestoreProcess &&
        `
          color: #d0d5da;
          touch-action: none;
          pointer-events: none;
        `}
    }

    .versioned, .not-versioned {
      ${(props) =>
        props.isRestoreProcess &&
        `
        touch-action: none;
        pointer-events: none;
        `}
    }

    .versioned { 
        svg {
            path {
          ${(props) =>
            props.isRestoreProcess &&
            `
              fill: #d0d5da;
            `};
        }
      }
    }

    .not-versioned{
        svg {
            path {

          ${(props) =>
            props.isRestoreProcess &&
            `
              stroke: #d0d5da;
            `};
        }
      }
    }

}
    .icon-link {
      ${(props) =>
        props.isRestoreProcess &&
        `
        touch-action: none;
        pointer-events: none;
        `}
      svg {
        path {
          ${(props) => props.isRestoreProcess && " fill: #d0d5da"}
        }
      }
    }
  }
`;

const StyledVersionRow = styled(Row)`
  @media ${tablet} {
    box-sizing: border-box;
    position: relative;
  }

  .row_content {
    position: relative;
    padding-top: 12px;
    padding-bottom: 12px;

    ${(props) => props.isTabletView && "height: 42px"};
    ${(props) => !props.isTabletView && "padding-right:16px"};
  }

  .version_badge {
    cursor: ${(props) => (props.canEdit ? "pointer" : "default")};
    margin-right: 16px;
    margin-left: 0px;

    .version_badge-text {
      position: absolute;
      left: 6px;
    }

    @media ${tablet} {
      margin-top: 0px;
    }
  }

  .version-link-file {
    @media ${tablet} {
      margin-top: -1px;
    }
  }

  .icon-link {
    width: 10px;
    height: 10px;
    margin-left: 9px;
    margin-right: 32px;
    @media ${tablet} {
      margin-top: -1px;
    }
  }

  .version_modal-dialog {
    display: none;

    @media ${tablet} {
      display: block;
    }
  }

  .version_edit-comment {
    display: block;

    @media ${tablet} {
      display: none;
      margin-left: 63px;
    }
  }

  .textarea-desktop {
    margin: 9px 23px 1px -7px;
  }

  .version_content-length {
    display: block;
    margin-left: auto;

    @media ${tablet} {
      display: none;
    }
  }

  .version_link {
    display: ${(props) =>
      props.showEditPanel ? "none" : props.canEdit ? "block" : "none"};
    /* text-decoration: underline dashed; */
    white-space: break-spaces;
    margin-left: -7px;
    margin-top: 4px;

    cursor: ${(props) => (props.isEditing ? "default" : "pointer")};

    @media ${tablet} {
      display: none;
      text-decoration: none;
    }
  }

  .version_text {
    display: ${(props) => (props.canEdit ? "none" : "block")};
    margin-left: -7px;
    margin-top: 5px;

    @media ${tablet} {
      display: inline-block;
      margin-left: 1px;
      margin-top: 5px;
    }
  }

  .version_links-container {
    display: flex;
    margin-left: auto;

    .version_link-action {
      display: block;
      margin-left: auto;
      margin-top: 5px;
      ${(props) =>
        props.isRestoring &&
        css`
          cursor: default;
        `}
      :last-child {
        margin-left: 8px;
      }

      @media ${tablet} {
        display: none;
      }
    }
  }

  .version-comment-wrapper {
    white-space: normal !important;
  }

  .row_context-menu-wrapper {
    display: none;

    right: 16px !important;

    .expandButton {
      ${(props) =>
        props.isSavingComment &&
        `
        touch-action: none;
        pointer-events: none;
        `}
      svg {
        path {
          ${(props) =>
            props.isSavingComment &&
            `
              fill: #d0d5da;
            `};
        }
      }
    }

    @media ${tablet} {
      display: block;
      position: absolute;
      right: 0px;
      top: 6px;
    }
  }

  .row_content {
    display: block;

    .version_link-action {
      ${(props) =>
        props.isSavingComment &&
        `
          color: #d0d5da;
          touch-action: none;
          pointer-events: none;
        `}
    }
  }

  .modal-dialog-aside-footer {
    width: 90%;

    .version_save-button {
      width: 100%;
    }
  }

  .version_edit-comment-button-primary {
    margin-right: 8px;
    width: 87px;
  }
  .version_edit-comment-button-second {
    width: 87px;
  }
  .version_modal-dialog .modal-dialog-aside-header {
    border-bottom: unset;
  }
  .version_modal-dialog .modal-dialog-aside-body {
    margin-top: -24px;
  }
`;

export { StyledBody, StyledVersionRow, StyledVersionList };
