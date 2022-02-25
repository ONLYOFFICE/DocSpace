import styled, { css } from "styled-components";
import commonSettingsStyles from "../../../utils/commonSettingsStyles";
import globalColors from "@appserver/components/utils/globalColors";

const linkColor = globalColors.black;

const INPUT_LENGTH = "350px";
const TEXT_LENGTH = "700px";

const commonStyles = css`
  margin-right: 16px;

  .radio-button_text {
    margin-right: 7px;
    margin-left: 4px;
    font-size: 13px;
    font-weight: 600;
  }
  .backup_radio-button {
    margin-bottom: 4px;
  }

  .backup_combo {
    margin-top: 16px;
    width: 100%;
    max-width: ${INPUT_LENGTH};
    .combo-button-label {
      width: 100%;
      max-width: ${INPUT_LENGTH};
    }
  }
  .backup_text-input {
    margin: 10px 0;
    width: 100%;
    max-width: ${INPUT_LENGTH};
    font-size: 13px;
  }

  .backup-description {
    max-width: ${TEXT_LENGTH};
    font-size: 12px;
    line-height: 15px;
    margin-left: 24px;
  }
  .select-folder_file-input {
    margin-left: 24px;
    margin-top: 16px;
  }
`;

const StyledManualBackup = styled.div`
  ${commonStyles}
  .manual-backup_buttons {
    margin-top: 16px;
    margin-left: 24px;
  }

  .manual-backup_storages-module {
    margin-left: 24px;
    .manual-backup_buttons {
      margin-left: 0px;
    }
  }
`;

const StyledAutoBackup = styled.div`
  ${commonStyles}

  .automatic-backup_main {
    margin-bottom: 30px;
    .radio-button_text {
      font-size: 13px;
    }
  }
  .backup_toggle-btn {
    position: static;
  }
  .backup_toggle-btn-description {
    margin-left: 37px;
    max-width: 1024px;
  }
  .toggle-button-text {
    font-weight: 600;
    margin-bottom: 4px;
  }

  .input-with-folder-path {
    margin-top: 16px;
    margin-bottom: 8px;
    width: 100%;
    max-width: ${INPUT_LENGTH};
  }
  .save-button {
    margin-right: 8px;
  }
  .backup_modules {
    margin-left: 37px;
  }
  .auto-backup_storages-module {
    margin-left: 24px;
    .backup_schedule-component {
      margin-left: 0;
    }
  }

  .backup_toggle-wrapper {
    margin-bottom: 16px;
    background-color: #f8f9f9;
    padding: 12px;
    max-width: 1144px;
  }
`;

const StyledStoragesModule = styled.div`
  .backup_storages-buttons {
    margin-left: -63px;
    margin-top: 40px;
  }
`;
const StyledRestoreBackup = styled.div`
  ${commonStyles}

  .restore-description {
    max-width: ${TEXT_LENGTH};
    font-size: 12px;
    line-height: 15px;
  }

  .restore-source {
    font-weight: 600;
    margin-top: 30px;
    margin-bottom: 15px;
    font-size: 15px;
  }
  .restore-warning {
    font-weight: 600;
    margin-top: 18px;
    margin-bottom: 15px;
    font-size: 16px;
    color: #f21c0e;
  }
  .restore-warning_link {
    margin: 15px 0;
  }
  .restore-backup-checkbox {
    margin-bottom: 24px;
  }
  .restore-backup-checkbox_notification {
    margin-top: 15px;
    max-width: ${TEXT_LENGTH};
    .checkbox-text {
      white-space: normal;
    }
  }
  .restore-backup_list {
    color: ${linkColor};
    text-decoration: underline dotted;
    cursor: pointer;
    font-weight: 600;
  }

  .restore-backup_input {
    margin: 16px 0;
    max-width: ${INPUT_LENGTH};
  }
  .restore-description {
    margin-bottom: 24px;
  }

  .restore-backup_modules {
    margin-top: 24px;
  }

  .backup_radio-button {
    margin-bottom: 16px;
  }
`;

const StyledModules = styled.div`
  margin-bottom: 24px;

  .backup-description {
    ${(props) => props.isDisabled && `color: #A3A9AE`}
  }
`;

const StyledScheduleComponent = styled.div`
  margin-left: 24px;
  .additional_options {
    max-width: ${INPUT_LENGTH};
    display: grid;
    grid-template-columns: ${(props) =>
      props.weeklySchedule || props.monthlySchedule ? "1fr 1fr" : "1fr"};
    grid-gap: 8px;
  }

  .weekly_option,
  .month_options {
    max-width: 171px;
    width: 100%;
    .drop-down_variable-size {
      max-width: 171px !important;
      width: 100% !important;
    }
  }
  .schedule-backup_combobox {
    display: inline-block;
    margin-top: 8px;
  }

  .main_options {
    max-width: 363px;
  }
  .days_option,
  .time_options {
    max-width: ${INPUT_LENGTH};
    width: 100%;
    .drop-down_variable-size {
      max-width: ${INPUT_LENGTH} !important;
      width: 100% !important;
    }
  }

  .max_copies {
    width: 100%;
    max-width: ${INPUT_LENGTH};
    .drop-down_variable-size {
      max-width: ${INPUT_LENGTH};
      width: 100% !important;
    }
  }
  .combo-button {
    width: 100% !important;
  }
  .combo-button-label {
    max-width: 100% !important;
    font-weight: 400;
  }
  .schedule_description {
    font-weight: 600;
  }
`;

const StyledBackup = styled.div`
  .schedule-information {
    font-size: 13px;
    font-weight: 600;
    margin-bottom: 8px;
  }
  ${commonSettingsStyles}

  .backup_modules-separation {
    margin-bottom: 28px;
    border-bottom: 1px solid #eceef1;
  }
  .backup_modules-header {
    font-size: 16px;
    font-weight: bold;
    padding-bottom: 8px;
  }
  .layout-progress-bar {
    ${(props) => props.isDesktop && "cursor: default;"}
  }
`;

const StyledBackupList = styled.div`
  height: 100%;

  .loader {
    height: inherit;
  }

  .backup-list-row-list {
    height: 100%;
    width: 100%;
  }

  .restore_dialog-button {
    ${(props) =>
      props.displayType === "aside" &&
      css`
        width: 293px;
      `}
  }

  .backup-list_trash-icon {
    width: 16px;
    height: 16px;
  }
  .backup-restore_dialog-header {
    margin-bottom: 16px;
  }
  .backup-restore_dialog-clear {
    text-decoration: underline dotted;
    margin: 10px 0 16px 0;
  }

  .backup-list_trash-icon {
    cursor: pointer;
    margin-top: -3px;
    grid-area: trash;

    path {
      fill: #a3a9ae;
    }
  }

  .backup-list_icon {
    grid-area: icon-name;
  }

  .backup-list_full-name {
    grid-area: full-name;
    font-weight: 600;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .backup-list_file-exst {
    color: #a3a9ae;
    grid-area: ext;
  }

  .backup-list_radio-button {
    grid-area: radiobutton;
  }

  .radio-button {
    padding-left: 10px;
    margin: 0 !important;
  }

  .backup-list_item {
    border-radius: 3px;

    cursor: default;

    align-items: center;
    display: grid;
    height: 48px;
    grid-template-areas: "trash icon-name full-name  radiobutton";

    grid-template-columns: 25px 32px calc(100% - 97px) 1fr;

    ${(props) => props.isChecked && `background: #F3F4F4;`}

    padding-left: 16px;
    padding-right: 10px;
  }

  .restore_dialog-button:first-child {
    margin-right: 10px;
  }

  .backup-restore_dialog-scroll-body {
    height: calc(100% - 64px);
    margin-left: -17px;
    margin-right: -17px;

    .scroll-body {
      height: calc(100% - 48px);
    }
    .nav-thumb-vertical {
      margin-left: -8px !important;
    }
  }
  .backup-restore_empty-list {
    margin-top: 96px;
  }
`;
export {
  StyledModules,
  StyledRestoreBackup,
  StyledScheduleComponent,
  StyledBackup,
  StyledBackupList,
  StyledManualBackup,
  StyledAutoBackup,
  StyledStoragesModule,
};
