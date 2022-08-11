import styled, { css } from "styled-components";
import Row from "@appserver/components/row";
import VersionSvg from "../../../../../public/images/versionrevision_active.react.svg";
import { tablet } from "@appserver/components/utils/device";
import { Base } from "@appserver/components/themes";

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
              fill: ${(props) =>
                props.theme.filesVersionHistory.versionList.fill};
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
          color:${(props) => props.theme.filesVersionHistory.versionList.color};
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
              fill: ${(props) =>
                props.theme.filesVersionHistory.versionList.fill};
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
              stroke: ${(props) =>
                props.theme.filesVersionHistory.versionList.stroke};
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
          ${(props) =>
            props.isRestoreProcess &&
            `fill: ${(props) =>
              props.theme.filesVersionHistory.versionList.fill}`}
        }
      }
    }
  }
`;

StyledVersionList.defaultProps = { theme: Base };

const StyledVersionRow = styled(Row)`
  .row_content {
    position: relative;
    padding-top: 12px;
    padding-bottom: 12px;
    height: auto;
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
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;

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

  .version_edit-comment {
    display: block;
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
    display: ${(props) => (props.showEditPanel ? "none" : "block")};
    margin-left: -7px;
    margin-top: 5px;

    @media ${tablet} {
      display: ${(props) => (props.showEditPanel ? "none" : "inline-block")};
      margin-left: -7px;
      margin-top: 5px;
    }
  }

  .version-comment-wrapper {
    white-space: normal !important;
  }

  .row_context-menu-wrapper {
    display: block;
    position: absolute;
    right: 16px !important;
    top: 6px;

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
              fill: ${(props) =>
                props.theme.filesVersionHistory.versionList.fill};
            `};
        }
      }
    }
  }

  .row_content {
    display: block;

    .version_link-action {
      ${(props) =>
        props.isSavingComment &&
        `
          color: ${(props) =>
            props.theme.filesVersionHistory.versionList.color};
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

  .row-header {
    max-width: 350px;
  }
`;

StyledVersionRow.defaultProps = { theme: Base };

const StyledVersionSvg = styled(VersionSvg)`
  path {
    stroke-dasharray: ${(props) => (props.isVersion ? "2 0" : "3 1")};
    stroke-linejoin: ${(props) => (props.isVersion ? "unset" : "round")};

    ${(props) =>
      props.isVersion &&
      css`
        stroke-width: 2;
      `}
  }
`;

StyledVersionSvg.defaultProps = { theme: Base };

export { StyledBody, StyledVersionRow, StyledVersionList, StyledVersionSvg };
