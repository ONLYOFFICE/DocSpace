import styled, { css } from "styled-components";
import Scrollbar from "@docspace/components/scrollbar";
import { desktop, mobile, tablet } from "@docspace/components/utils/device";
import { isMobile, isMobileOnly } from "react-device-detect";
import { Base } from "@docspace/components/themes";

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

  .footer {
    padding: 16px;
    width: 100%;
    margin: auto;
    left: 0;
    right: 0;
  }
`;

const StyledAsidePanel = styled.div`
  z-index: 310;

  .sharing_panel-header {
    width: 100%;
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
    background: ${(props) => props.theme.filesPanels.sharing.borderBottom};
    content: "";
    top: 48px;
    width: calc(100% - 32px);
  }
  .upload-panel_body {
    padding-top: ${isMobile ? "67px" : "60px"};
    height: ${isMobile ? "calc(100vh - 67px)" : "calc(100vh - 60px)"};
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

  ${PanelStyles}
`;

StyledAsidePanel.defaultProps = { theme: Base };

const StyledVersionHistoryPanel = styled.div`
  ${PanelStyles}

  .version-history-modal-dialog {
    transition: unset;
    transform: translateX(${(props) => (props.visible ? "0" : "480px")});
    width: 480px;
    max-width: 480px;
  }

  .version-history-panel-header {
    margin-bottom: 12px;
    height: 53px;
    margin-left: 0px;
    .version-history-panel-heading {
      font-weight: 700;
      margin-bottom: 13px;
      margin-top: 12px;
    }
  }

  .version-history-panel-body {
    padding-bottom: ${(props) => (props.isLoading ? "0px" : null)};
    margin-left: 16px;

    height: calc(100% - 53px);
    box-sizing: border-box;

    .version-comment-wrapper {
      margin-left: 85px;
    }

    .version_edit-comment {
      padding-left: 7px;
    }
  }
`;

StyledVersionHistoryPanel.defaultProps = { theme: Base };

const StyledEmbeddingPanel = styled.div`
  ${PanelStyles}
`;

const StyledContent = styled.div`
  box-sizing: border-box;
  position: relative;
  width: 100%;
  height: 100%;
  background-color: ${(props) =>
    props.theme.filesPanels.content.backgroundColor};

  .upload-panel_header-content {
    margin-right: 0 !important;
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

  margin-right: -16px;

  border-bottom: ${(props) => props.theme.filesPanels.sharing.borderBottom};

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

StyledHeaderContent.defaultProps = { theme: Base };

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
  padding: 16px;
  width: 100%;
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

  .new_file_panel-first-button {
    margin-right: 8px;
  }
  .new_files_panel-button {
    width: 100%;
  }

  @media ${desktop} {
    padding: 16px;
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

      border: 1px solid #eceef1;
      border-radius: 16px;
      line-height: 30px;
      padding: 4px 15px;
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
  StyledEmbeddingPanel,
  StyledVersionHistoryPanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
  StyledSharingBody,
  StyledFooter,
  StyledLinkRow,
  StyledModalRowContainer,
};
