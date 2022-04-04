import styled, { css } from "styled-components";
import Scrollbar from "@appserver/components/scrollbar";
import { desktop, tablet } from "@appserver/components/utils/device";
import { isMobile } from "react-device-detect";
import { Base } from "@appserver/components/themes";

const PanelStyles = css`
  .panel_combo-box {
    margin-left: 10px;

    .optionalBlock {
      margin-right: 4px;
      display: flex;
    }

    .combo-button {
      background: transparent;
      height: 36px;
    }

    .combo-button-label {
      margin: 0;
    }
  }

  .groupSelector,
  .peopleSelector {
    .combo-buttons_arrow-icon {
      flex: 0 0 6px;
      width: 6px;
      margin-top: auto;
      margin-bottom: auto;
      display: flex;
      justify-content: center;
      align-items: center;
    }
  }

  .footer {
    padding: 16px 0;
    width: calc(100% - 32px);
    margin: auto;
    left: 0;
    right: 0;
  }
`;

const StyledAsidePanel = styled.div`
  z-index: 310;

  .sharing_panel-header {
    font-weight: 700;
    margin: 14px 0;
    padding-right: 10px;
  }
  .upload_panel-header {
    font-weight: 700;
    padding: 19px auto 19px 17px;
  }
  .upload-panel_header-content {
    z-index: 320;
    position: fixed;
    left: 0;
    right: 0;
    background-color: ${(props) =>
      props.theme.filesPanels.aside.backgroundColor};
    height: ${isMobile ? "55px" : "48px"};
  }
  .upload-panel_header-content::after {
    position: absolute;
    width: 100%;
    max-width: 468px;
    height: 1px;
    background: #eceef1;
    content: "";
    top: 48px;
    width: calc(100% - 32px);
  }
  .upload-panel_body {
    padding-top: ${isMobile ? "55px" : "48px"};
    height: ${isMobile ? "calc(100vh - 55px)" : "calc(100vh - 48px)"};
  }
  .modal-dialog-aside {
    padding: 0;
    transform: translateX(${(props) => (props.visible ? "0" : "500px")});
    width: 500px;

    @media (max-width: 550px) {
      width: 320px;
      transform: translateX(${(props) => (props.visible ? "0" : "320px")});
    }
  }

  .header_aside-panel {
    transition: unset;
    transform: translateX(${(props) => (props.visible ? "0" : "500px")});
    width: 500px;
    overflow-y: hidden;

    @media (max-width: 550px) {
      width: 320px;
      transform: translateX(${(props) => (props.visible ? "0" : "320px")});
    }

    .sharing_panel-header-container {
      padding-right: 0;
    }
  }
  ${PanelStyles}
`;

StyledAsidePanel.defaultProps = { theme: Base };

const StyledVersionHistoryPanel = styled.div`
  ${PanelStyles}
  .version-history-modal-dialog {
    transform: translateX(${(props) => (props.visible ? "0" : "720px")});
    width: 500px;
  }
  .version-history-aside-panel {
    transform: translateX(${(props) => (props.visible ? "0" : "720px")});
    width: 500px;
  }
  .version-history-panel-header {
    height: 53px;
    margin-left: 0px;
    .version-history-panel-heading {
      font-weight: 700;
      margin-bottom: 13px;
      margin-top: 12px;
    }
  }
  .version-history-panel-body {
    padding-top: ${(props) => (props.isLoading ? "16px" : null)};
    padding-bottom: ${(props) => (props.isLoading ? "0px" : null)};
    margin-left: 16px;
    border-top: ${(props) => props.theme.filesPanels.versionHistory.borderTop};

    height: calc(100% - 53px);
    box-sizing: border-box;

    .version-comment-wrapper {
      margin-left: 79px;
    }
    .version_edit-comment {
      padding-left: 2px;
    }
  }
`;

StyledVersionHistoryPanel.defaultProps = { theme: Base };

const StyledAddUsersPanelPanel = styled.div`
  .header_aside-panel {
    transform: translateX(${(props) => (props.visible ? "0" : "500px")});
    width: 500px;

    @media (max-width: 550px) {
      width: 320px;
      transform: translateX(${(props) => (props.visible ? "0" : "320px")});
    }
  }
  ${PanelStyles}
  .combo-button-label {
    font-size: 14px;
  }
`;

const StyledAddGroupsPanel = styled.div`
  .header_aside-panel {
    transform: translateX(${(props) => (props.visible ? "0" : "500px")});
    width: 500px;

    @media (max-width: 550px) {
      width: 320px;
      transform: translateX(${(props) => (props.visible ? "0" : "320px")});
    }
  }
  ${PanelStyles}
  .combo-button-label {
    font-size: 14px;
  }
`;

const StyledEmbeddingPanel = styled.div`
  .header_aside-panel {
    transform: translateX(${(props) => (props.visible ? "0" : "500px")});
    width: 500px;

    @media (max-width: 550px) {
      width: 320px;
      transform: translateX(${(props) => (props.visible ? "0" : "320px")});
    }
  }
  ${PanelStyles}
`;

const StyledContent = styled.div`
  box-sizing: border-box;
  position: relative;
  width: 100%;
  height: 100%;
  background-color: ${(props) =>
    props.theme.filesPanels.content.backgroundColor};

  .header_aside-panel-header {
    max-width: 500px;
    margin: 0 0 0 16px;
    line-height: 57px;
    font-weight: 700;
  }

  .header_aside-panel-plus-icon {
    margin-left: auto;
  }

  .sharing-access-combo-box-icon {
    height: 16px;
    path {
      fill: ${(props) =>
        props.isDisabled
          ? props.theme.filesPanels.content.disabledFill
          : props.theme.filesPanels.content.fill};
    }

    svg {
      width: 16px;
      min-width: 16px;
      height: 16px;
      min-height: 16px;
    }
  }

  .panel-loader-wrapper {
    margin-top: 8px;
    padding-left: 32px;
  }
  .panel-loader {
    display: inline;
    margin-right: 10px;
  }

  .layout-progress-bar {
    position: fixed;
    right: 15px;
    bottom: 21px;

    @media ${tablet} {
      bottom: 83px;
    }
  }
`;

StyledContent.defaultProps = { theme: Base };

const StyledHeaderContent = styled.div`
  display: flex;
  align-items: center;
  padding: 0 16px;

  .sharing_panel-icons-container {
    display: flex;
    margin-left: auto;
    .sharing_panel-drop-down-wrapper {
      position: relative;

      .sharing_panel-drop-down {
        padding: 4px 0;
      }
      .sharing_panel-plus-icon {
        //margin-right: 12px;
      }
    }
  }

  .upload_panel-icons-container {
    display: flex;
    margin-left: auto;
    .upload_panel-vertical-dots-icon {
    }
  }

  .files-operations-header,
  .sharing_panel-header {
    font-weight: 700;
    margin: 14px 0;
  }

  @media ${desktop} {
    .files-operations-header,
    .sharing_panel-header {
      margin: 12px 0;
      font-size: 18px;
    }
  }
`;

const StyledBody = styled.div`
  &.files-operations-body {
    padding: 0 0 0 16px;
    box-sizing: border-box;
    width: 100%;
    height: calc(100vh - 125px);

    .styled-element {
      margin-left: -2px;
    }
  }

  .embedding-panel_body {
    padding: 0 16px;
  }

  .change-owner_body {
    padding: 0 16px;
    display: flex;
    flex-direction: column;
  }

  .change-owner_owner-label {
    margin: 16px 0;
  }

  .selector-wrapper {
    position: fixed;
    height: calc(100%);
    width: 100%;

    .column-options {
      width: 100%;

      .header-options {
        .combo-button-label {
          max-width: 435px;

          @media (max-width: 550px) {
            width: 255px;
          }
        }
      }

      .row-option {
        .option_checkbox {
          width: 440px;

          @media (max-width: 550px) {
            width: 265px;
          }
        }
      }

      .body-options {
        width: 100%;
      }
    }
  }

  .embedding-panel_links-container {
    display: flex;
    .embedding-panel_link {
      margin-right: 8px;
      height: 32px;
      background-color: ${(props) =>
        props.theme.filesPanels.body.backgroundColor};
      line-height: 30px;
      padding: 0px 8px;
    }
  }

  .embedding-panel_inputs-container {
    display: flex;

    .embedding-panel_input {
      margin-right: 8px;
      width: 94px;
    }
  }

  .embedding-panel_code-container {
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  }

  .embedding-panel_text {
    padding: 8px 0 4px 0;
  }

  .embedding-panel_copy-icon {
    position: absolute;
    z-index: 1;
    margin: 8px;
    right: 16px;
  }

  .sharing-access-combo-box-icon {
    path {
      fill: ${(props) => props.theme.filesPanels.body.fill};
    }
  }
`;

StyledBody.defaultProps = { theme: Base };

const StyledSharingBody = styled(Scrollbar)`
  position: relative;
  padding: 16px 0;

  width: calc(100% + 16px) !important;

  .link-row__container {
    height: 47px;
  }

  .link-row__container,
  .sharing-row {
    .styled-element {
      margin-right: 0;
      margin-left: 0;
    }
  }

  .row_content {
    overflow: visible;
    height: auto;
  }

  .sharing-row {
    padding-left: 16px;
    //width: calc(100% - 16px);
    box-sizing: border-box;
    border-bottom: none;
  }

  .nav-thumb-vertical {
    opacity: 0;
    transition: opacity 200ms ease;
  }

  :hover {
    .nav-thumb-vertical {
      opacity: 1;
    }
  }

  .sharing_panel-text {
    line-height: 24px;
    font-weight: 600;
    font-size: 14px;
  }

  .sharing_panel-link {
    a {
      text-decoration: none !important;

      span {
        font-weight: 600;
      }
    }
  }

  .sharing_panel-link-combo-box {
    margin-left: auto;
    .combo-button {
      height: 24px;
      width: 94px;

      svg {
        bottom: 6px;
        position: absolute;
        height: 8px;
        width: 8px;
      }
    }
  }

  .sharing_panel-owner-icon {
    padding-right: 19px;
  }

  .sharing_panel-remove-icon {
    margin-left: auto;
    line-height: 24px;
    display: flex;
    align-items: center;
    flex-direction: row-reverse;

    svg {
      width: 16px;
      height: 16px;
    }
  }

  .panel_combo-box {
    margin-left: 0px;

    .combo-button {
      height: 30px;
      margin: 0;
      padding: 0;
      border: none;
    }

    .combo-button-label {
      margin: 0;
    }
  }

  .sharing_panel-text-area {
    position: fixed;
    bottom: 70px;
    width: 94%;
    left: 0;
    right: 0;
    margin: auto;
  }

  @media ${desktop} {
    .link-row__container {
      height: 41px;

      .link-row {
        min-height: 41px;
      }
    }

    .sharing-row {
      min-height: 41px;
      //padding-right: 15px;

      .sharing_panel-remove-icon {
        font-size: 12px;
      }
    }

    .sharing_panel-text,
    .sharing_panel-link span {
      font-size: 13px;
    }
  }
`;

const StyledFooter = styled.div`
  display: flex;
  position: fixed;
  bottom: 0;
  padding: 16px 0;
  width: calc(100% - 32px);
  margin: auto;
  left: 0;
  right: 0;
  background-color: ${(props) =>
    props.theme.filesPanels.footer.backgroundColor};
  border-top: ${(props) => props.theme.filesPanels.footer.borderTop};
  box-sizing: border-box;

  .sharing_panel-checkbox {
    span {
      font-weight: 600;
      font-size: 14px;
    }

    .checkbox {
      margin-right: 6px;
    }
  }

  .sharing_panel-button {
    margin-left: auto;
  }
  .new_files_panel-button {
    margin-right: 8px;
  }

  @media ${desktop} {
    padding: 10px 0;
    min-height: 57px;

    .sharing_panel-checkbox {
      span {
        font-size: 13px;
      }
    }

    .sharing_panel-button {
      margin-top: 2px;
    }
  }
`;

StyledFooter.defaultProps = { theme: Base };

const StyledLinkRow = styled.div`
  margin-right: -16px;
  padding: 0 16px;
  box-sizing: border-box;
  background-color: ${(props) =>
    props.theme.filesPanels.linkRow.backgroundColor};

  .sharing-access-combo-box-icon {
    path {
      fill: ${(props) =>
        props.isDisabled
          ? props.theme.filesPanels.linkRow.disabledFill
          : props.theme.filesPanels.linkRow.fill};
    }
  }

  .sharing_panel-link-container {
    display: flex;

    .sharing_panel-link {
      a {
        text-decoration: none;

        ${(props) =>
          props.isDisabled &&
          css`
            :hover {
              text-decoration: none;
            }
          `};
      }
    }
  }

  .link-row {
    ${(props) => !props.withToggle && "border-bottom:none;"}
  }

  .sharing-row__toggle-button {
    margin-top: 1px;
  }

  .row_content {
    display: grid;
    grid-template-columns: 1fr 28px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  }

  .combo-button {
    background: transparent;
  }

  @media ${desktop} {
    .sharing-row__toggle-button {
      margin-top: 0;
    }
  }
`;

StyledLinkRow.defaultProps = { theme: Base };

const StyledSelectFolderPanel = styled.div`
  ${(props) =>
    props.displayType === "aside" &&
    css`
      height: 100%;
    `}

  ${(props) =>
    props.noTreeSwitcher &&
    css`
      span.rc-tree-switcher.rc-tree-switcher-noop {
        display: none;
      }
    `}

    

  .modal-dialog_header {
    display: flex;
    align-items: center;
  }
  .select-folder-modal-dialog-header {
    margin-bottom: 16px;
  }
  .modal-dialog_header-title {
    ${(props) => props.isNeedArrowIcon && `margin-left:16px;`}
  }
  .select-folder-dialog_tree-folder {
    margin-top: 12px;
    height: ${(props) => (props.displayType === "aside" ? "100%" : "291px")};
  }

  .select-folder-dialog-buttons-save {
    margin-right: 8px;
  }
  .select-folder-dialog-modal_buttons {
    margin-top: 16px;
  }

  .select-folder-dialog_header {
    display: flex;
    align-items: center;
  }
  .select-folder-dialog_header-icon {
    margin-right: 16px;
  }
  .select-folder-dialog_aside_body {
    height: ${(props) =>
      props.isFooter ? "calc(100% - 110px)" : "calc(100% - 64px)"};
    width: 296px;
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
      overflow-x: hidden !important;
    }
  }
  .tree-folder-Loader {
    ${(props) =>
      props.displayType === "aside"
        ? css`
            margin-top: 16px;
          `
        : css`
            height: ${props.heightContent};
          `}
  }

  .files-tree-menu {
    margin-top: 0 !important;
  }
`;

const StyledSelectFilePanel = styled.div`
  height: 100%;
  ${(props) =>
    props.noTreeSwitcher &&
    css`
      span.rc-tree-switcher.rc-tree-switcher-noop {
        display: none;
      }
    `}

  .files-list-body {
    height: 100%;
    ${(props) =>
      props.displayType === "aside" &&
      css`
        margin-left: -16px;
        margin-right: -16px;
        .nav-thumb-vertical {
          margin-left: -7px !important;
        }
      `}

    ${(props) =>
      props.displayType === "modal" &&
      css`
        .nav-thumb-vertical {
          margin-left: 4px !important;
          width: 4px !important;
        }
      `}
  }
  .select-file-dialog_aside_body-files_list {
    height: 100%;
  }
  .select-file-dialog_empty-container {
    .ec-header {
      word-break: break-word;
    }
  }

  .modal-dialog-filter-title {
    margin-top: 12px;
    ${(props) => props.displayType === "modal" && `margin-left: 12px`};
    margin-bottom: 12px;
    font-size: 12px;
    line-height: 16px;
    color: ${(props) => props.theme.filesPanels.selectFile.borderRight};
  }
  .select-file-dialog-modal_buttons {
    ${(props) =>
      props.isHeaderChildren ? "margin-top: 20px" : "margin-top:20px"};
  }
  .select-file-dialog_aside-body_wrapper {
    height: calc(100% - 109px);
  }

  .select-folder-dialog_aside-body_wrapper {
    width: 320px;
    box-sizing: border-box;
    height: 100%;
  }

  .select-file-dialog_aside_body,
  .select-file-dialog_aside_body_files-list {
    height: 100%;
    width: 293px;
  }
  .select-file-dialog_aside_body_files-list {
    margin-left: -17px;
    padding-left: 16px;
    ${(props) =>
      props.isChecked &&
      `background: ${(props) => props.theme.filesPanels.selectFile.background}`}
  }

  .file-name {
    border-bottom: ${(props) =>
      props.theme.filesPanels.selectFile.borderBottom};
  }
  .file-name {
    display: flex;
    padding: 7px 0px;
  }
  .panel-loader-wrapper {
    .first-row-content__mobile {
      width: ${(props) => (props.displayType === "aside" ? "147px" : "234px")};
      height: ${(props) => (props.displayType === "aside" ? "16px" : "10px")};
    }

    @media ${desktop} {
      .second-row-content__mobile {
        max-width: 185px;
        height: 8px;
        display: block;
      }
      .row-content {
        grid-template-rows: 10px;
        grid-row-gap: 6px;
        margin-top: -3px;
      }
    }

    .second-row-content__mobile {
      width: 229px;
    }
  }
  .loader-wrapper_margin {
    margin-left: ${(props) =>
      props.displayType === "aside" ? "16px" : "12px"};
  }
  .select-file-dialog_modal-loader {
    height: 290px;
    padding-top: 16px;
    box-sizing: border-box;
  }
  .panel-loader {
    display: inline;
    margin-right: 10px;
  }

  .modal-dialog_tree-body {
    grid-area: tree;
  }
  .modal-dialog_files-body {
    grid-area: files-list;
  }

  .modal-dialog_body {
    display: grid;
    grid-template-columns: 228px 477px;
    height: 295px;
    grid-template-areas: "tree files-list";
    .modal-dialog_tree-body {
      padding-top: 0;
      border-right: ${(props) =>
        props.theme.filesPanels.selectFile.borderRight};

      span.rc-tree-title {
        max-width: ${(props) =>
          props.displayType === "aside" ? "243px" : "181px"};
      }
    }
  }
  .select-file-dialog-aside_buttons {
    position: fixed;
    bottom: 0;
    padding-top: 8px;
    background-color: ${(props) =>
      props.theme.filesPanels.selectFile.buttonsBackground};
    height: 40px;
    width: 100%;
  }
  .select-file-dialog-buttons-save {
    margin-right: 8px;
  }
  .select-file-modal-dialog-buttons-save {
    margin-right: 8px;
  }
  .select-folder-dialog-buttons-save {
    margin-right: 8px;
  }
  .select-folder-dialog-modal_buttons {
    margin-top: 8px;
  }
`;

StyledSelectFilePanel.defaultProps = { theme: Base };

const StyledFilesList = styled.div`
  .select-file-dialog_icon {
    margin-right: 8px;
  }

  .radio-button_text{
    ${(props) => props.displayType === "aside" && "margin: 0 !important;"};
  }

  .entry-title {
    font-weight: 600;
    max-width: 100%;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .files-list_file-owner {
    max-width: ${(props) =>
      props.displayType === "aside" ? "213px" : "406px"};
    overflow: hidden;
    white-space: nowrap;
    text-overflow: ellipsis;
    color: ${(props) => props.theme.filesPanels.filesList.color};
    font-weight: 600;

    ${(props) => props.displayType === "modal" && ` font-size: 11px;`}

    height: ${(props) => (props.displayType === "aside" ? "16px" : "12px")};
    padding-bottom: ${(props) =>
      props.displayType === "aside" ? "10px" : "11px"};
  }
  .file-exst {
    color: ${(props) => props.theme.filesPanels.filesList.color};
    font-weight: 600;
  }
  .modal-dialog_file-name:hover {
    background-color: ${(props) =>
      props.theme.filesPanels.filesList.backgroundColor};
  }
  .files-list_full-name {
    white-space: nowrap;
    text-overflow: ellipsis;
    overflow: hidden;
    max-width: ${(props) => props.displayType === "aside" && "213px"};

    grid-area: full-name;
    display: flex;
    padding-top: 10px;
  }
  .select-file-dialog_icon {
    grid-area: icon-name;
    padding-top: 12px;
  }
  .select-file-dialog_checked {
    grid-area: checked-button;
  }
  .files-list_file-children_wrapper {
    grid-area: owner-name;
    margin-top: ${(props) => props.displayType === "modal" && "-8px"};
  }
  .modal-dialog_file-name {
    border-radius: 3px;
    padding-right: 12px;
    ${(props) =>
      props.isChecked &&
      `background: ${props.theme.filesPanels.filesList.backgroundColor};`}
    cursor: pointer;
    border-bottom: ${(props) => props.theme.filesPanels.filesList.borderBottom};
    display: grid;

    ${(props) =>
      props.displayType === "aside"
        ? css`
            height: 56px;
            grid-template-areas: "checked-button icon-name full-name" "checked-button icon-name owner-name";
          `
        : css`
            height: 49px;
            grid-template-areas: "checked-button icon-name full-name" "checked-button icon-name owner-name";
          `}
    grid-template-columns: 22px 33px 1fr;
    ${(props) => props.displayType === "modal" && ` grid-row-gap: 4px;`}

    padding-left: ${(props) =>
      props.displayType === "aside" ? "16px" : "12px"};
    box-sizing: border-box;
  }
`;

StyledFilesList.defaultProps = { theme: Base };

const StyledModalRowContainer = styled.div`
  display: flex;
  flex-direction: column;
  min-height: 47px;

  .link-row__container {
    display: flex;
    align-items: center;
    height: 41px;
    width: 100%;

    .link-row {
      border-bottom: none;
    }

    .link-row::after {
      height: 0;
    }
  }

  .panel_combo-box {
    margin-left: 0px;

    .combo-button {
      height: 30px;
      margin: 0;
      padding: 0;
      border: none;
    }

    .optionalBlock {
      margin-right: 4px;
      display: flex;
    }

    .combo-button-label {
      margin: 0;
    }

    .sharing-access-combo-box-icon {
      height: 16px;
      path {
        fill: ${(props) =>
          props.isDisabled
            ? props.theme.filesPanels.modalRow.disabledFill
            : props.theme.filesPanels.modalRow.fill};
      }

      svg {
        width: 16px;
        min-width: 16px;
        height: 16px;
        min-height: 16px;
      }
    }
  }

  .embedding-panel_code-container {
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  }

  .embedding-panel_text {
    padding: 8px 0 4px 0;
  }

  .embedding-panel_copy-icon {
    position: absolute;
    z-index: 1;
    margin: 8px;
    right: 0px;
  }

  .embedding-panel_links-container {
    display: flex;
    .embedding-panel_link {
      margin-right: 8px;
      height: 32px;
      background-color: ${(props) =>
        props.theme.filesPanels.modalRow.backgroundColor};
      line-height: 30px;
      padding: 0px 8px;
    }
  }

  .embedding-panel_inputs-container {
    display: flex;

    .embedding-panel_input {
      margin-right: 8px;
      width: 94px;
    }
  }

  .embedding-panel_code-container {
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  }

  .embedding-panel_text {
    padding: 8px 0 4px 0;
  }

  .embedding-panel_copy-icon {
    position: absolute;
    z-index: 1;
    margin: 8px;
    right: 0;
  }

  .panel-loader-wrapper {
    margin-top: 8px;
    padding-left: 32px;
  }
  .panel-loader {
    display: inline;
    margin-right: 10px;
  }

  @media (max-width: 1024px) {
    .row_content {
      height: 19px;
      overflow: initial;
    }
  }
`;

StyledModalRowContainer.defaultProps = { theme: Base };

export {
  StyledAsidePanel,
  StyledAddGroupsPanel,
  StyledAddUsersPanelPanel,
  StyledEmbeddingPanel,
  StyledVersionHistoryPanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
  StyledSharingBody,
  StyledFooter,
  StyledLinkRow,
  StyledSelectFolderPanel,
  StyledSelectFilePanel,
  StyledFilesList,
  StyledModalRowContainer,
};
