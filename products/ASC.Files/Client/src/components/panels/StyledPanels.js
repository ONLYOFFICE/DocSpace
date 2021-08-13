import styled, { css } from "styled-components";
import Scrollbar from "@appserver/components/scrollbar";
import { tablet } from "@appserver/components/utils/device";

const PanelStyles = css`
  .panel_combo-box {
    margin-left: 10px;

    .optionalBlock {
      margin-right: 4px;
      display: flex;
    }

    .combo-button {
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
    background-color: #fff;
  }
  .upload-panel_body {
    padding-top: 64px;
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
  }
  ${PanelStyles}
`;

const StyledVersionHistoryPanel = styled.div`
  ${PanelStyles}
  .version-history-modal-dialog {
    transform: translateX(${(props) => (props.visible ? "0" : "720px")});
    width: 720px;
  }
  .version-history-aside-panel {
    transform: translateX(${(props) => (props.visible ? "0" : "720px")});
    width: 720px;
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
    padding: ${(props) => (props.isLoading ? "16px 0" : null)};
    margin: 0 16px;
    border-top: 1px solid #eceef1;

    .version-comment-wrapper {
      margin-left: 79px;
    }
    .version_edit-comment {
      padding-left: 2px;
    }
  }
`;

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
  background-color: #fff;

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
      fill: ${(props) => (props.isDisabled ? "#D0D5DA" : "#A3A9AE")};
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
    .upload_panel-remove-icon {
      padding-right: 8px;
    }
  }

  .files-operations-header,
  .sharing_panel-header {
    font-weight: 700;
    margin: 14px 0;
    margin-left: 16px;
  }
`;

const StyledBody = styled.div`
  &.files-operations-body {
    padding: 0 16px;
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
    height: calc(100% - 57px);
    width: 100%;

    .column-options {
      padding: 0px 16px;
      padding-bottom: 16px;
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
      background-color: #eceef1;
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
      fill: #333;
    }
  }
`;

const StyledSharingBody = styled(Scrollbar)`
  position: relative;
  padding: 16px 0;
  .row_content {
    overflow: visible;
    height: auto;
  }

  .sharing-row {
    margin: 0 16px;
    width: calc(100% - 16px);
    box-sizing: border-box;
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
  background-color: #fff;
  border-top: 1px solid #eceef1;
  box-sizing: border-box;

  .sharing_panel-checkbox {
    span {
      font-weight: 600;
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
`;

const StyledLinkRow = styled.div`
  width: calc(100% + 16px);
  padding: 0 16px;
  box-sizing: border-box;
  background-color: #f8f9f9;

  .sharing-access-combo-box-icon {
    path {
      fill: ${(props) => (props.isDisabled ? "#D0D5DA" : "#a3a9ae")};
    }
  }

  .link-row {
    ${(props) => !props.withToggle && "border-bottom:none;"}
  }
  .row_content {
    display: grid;
    grid-template-columns: 1fr 28px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  }

  .combo-button {
    background: transparent;
  }
`;

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
};
