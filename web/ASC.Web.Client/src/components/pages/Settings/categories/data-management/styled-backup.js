import styled from "styled-components";
import commonSettingsStyles from "../../utils/commonSettingsStyles";

const StyledComponent = styled.div`
  ${commonSettingsStyles}

  .manual-backup_buttons {
    margin-top: 16px;
  }

  .backup_combobox {
    margin-top: 8px;
  }
  .inherit-title-link {
    margin-bottom: 8px;
  }
  .note_description {
    margin-top: 8px;
  }

  .automatic-backup_main {
    margin-bottom: 30px;
    .radio-button_text {
      font-size: 13px;
    }
  }
  .radio-button_text {
    margin-right: 7px;
    margin-left: 8px;
    //font-size: 19px;
    font-weight: 600;
  }
  .backup_radio-button {
    margin-bottom: 8px;
  }
  .backup_combobox {
    display: inline-block;
    margin-right: 8px;
  }

  .input-with-folder-path {
    margin-top: 16px;
    margin-bottom: 8px;
  }

  .input-with-folder-path,
  .text-input-with-folder-path {
    width: 100%;
    max-width: 820px;
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
    font-size: 19px;
    color: #c30;
  }
  .restore-warning_link {
    margin: 15px 0;
  }
  .restore-backup-checkbox {
    margin-bottom: 24px;
  }
  .restore-backup-checkbox_notification {
    margin-top: 15px;
  }
  .restore-backup_list {
    color: #316daa;
    text-decoration: underline;
    cursor: pointer;
  }
`;
const StyledModules = styled.div`
  margin-bottom: 40px;
  .category-item-description {
    ${(props) => props.isDisabled && `color: #A3A9AE`}
  }
`;
const StyledRestoreModules = styled.div`
  .category-item-description {
    ${(props) => props.isDisabled && `color: #A3A9AE`}
  }
  .radio-button_text {
    font-weight: normal;
  }
`;
const StyledScheduleComponent = styled.div`
  .time_options {
    .drop-down_variable-size {
      width: 74px !important;
    }
  }
  .month_options {
    .drop-down_variable-size {
      width: 46px !important;
    }
  }
  .main_options {
    max-width: 820px;
  }

  .max_copies {
    width: 100%;
    max-width: 820px;
  }
  .combo-button {
    width: 100% !important;
  }
  .combo-button-label {
    max-width: 100% !important;
  }
`;

const StyledBackup = styled.div`
  .schedule-information {
    font-size: 13px;
    font-weight: 600;
    margin-bottom: 8px;
  }
  ${commonSettingsStyles}
`;

const StyledBackupList = styled.div`
  height: 100%;
  .backup-list_modal-dialog_body {
    height: 300px;
  }
  .backup-list_options {
    display: flex;
  }

  .backup-list_aside-header_title {
    margin: 0px;
    line-height: 56px;
    max-width: 474px;
    width: 400px;
    white-space: nowrap;
    text-overflow: ellipsis;
    overflow: hidden;
    padding-left: 16px;
    border-bottom: 1px solid #eceef1;
  }
  .backup-list_aside-header {
    margin-bottom: 16px;
  }
  .backup-list_aside-header_description {
    margin-top: 16px;
  }
  .backup-list_modal-header_description {
    margin-bottom: 16px;
  }
  .backup-list_aside-body_wrapper {
    height: calc(100% - 200px);
    width: 320px;
    padding: 0 16px;
  }
  .backup-list_aside_body {
    margin-top: 16px;
    height: 100%;
    width: 290px;
  }
  .backup-list_restore-link {
    margin-right: 16px;
  }
  .backup-list_trash-icon,
  .backup-list_restore-link {
    cursor: pointer;
  }
  .backup-list_trash-icon {
    margin-top: 1px;
  }
  .backup-list_trash-icon:hover {
    text-decoration: underline;
  }
`;
export {
  StyledModules,
  StyledComponent,
  StyledScheduleComponent,
  StyledBackup,
  StyledRestoreModules,
  StyledBackupList,
};
