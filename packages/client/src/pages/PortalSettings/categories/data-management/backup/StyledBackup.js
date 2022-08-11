import styled, { css } from "styled-components";
import commonSettingsStyles from "../../../utils/commonSettingsStyles";
import globalColors from "@docspace/components/utils/globalColors";
import { isMobileOnly } from "react-device-detect";

const linkColor = globalColors.black;
const borderColor = globalColors.grayLightMid;

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
`;

const StyledManualBackup = styled.div`
  ${commonStyles}
  .manual-backup_buttons {
    margin-top: 16px;
    margin-left: 24px;
    button:first-child {
      width: 50%;
      max-width: 164px;
      margin-right: 8px;
    }
    button:last-child {
      max-width: 164px;
      width: calc(50% - 8px);
    }
  }

  .manual-backup_storages-module {
    margin-left: 24px;
    .manual-backup_buttons {
      margin-left: 0px;
    }
  }
  .manual-backup_folder-input {
    margin-left: 24px;
    margin-top: 16px;
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

  .auto-backup_folder-input {
    margin-left: 24px;
    margin-top: 16px;
  }

  .backup_toggle-wrapper {
    margin-bottom: 16px;
    background-color: ${(props) =>
      props.theme.client.settings.backup.rectangleBackgroundColor};
    padding: 12px;
    max-width: 1144px;
    box-sizing: border-box;
  }
  .auto-backup_buttons {
    ${!isMobileOnly && "margin-bottom: 24px"}
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

  .restore-backup_warning {
    font-weight: 600;
    margin-top: 24px;
    margin-bottom: 8px;
    font-size: 16px;
    color: #f21c0e;
  }
  .restore-backup_warning-link {
    margin: 16px 0 24px 0;
    max-width: ${TEXT_LENGTH};
  }
  .restore-backup_warning-description {
    max-width: ${TEXT_LENGTH};
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
    color: ${(props) => props.theme.text.color};
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
  .restore-backup_button {
    ${isMobileOnly && "width:100%"}
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

  .days_option {
    grid-area: days;
    width: 100%;

    ${(props) =>
      (props.weeklySchedule || props.monthlySchedule) &&
      css`
        max-width: 138px;
      `};
  }
  .additional_options {
    max-width: ${INPUT_LENGTH};
    display: grid;
    grid-template-columns: ${(props) =>
      props.weeklySchedule || props.monthlySchedule ? "1fr 1fr" : "1fr"};
    grid-gap: 8px;
  }

  .weekly_option,
  .month_options {
    grid-area: weekly-monthly;
    width: 100%;

    max-width: ${(props) => (!props.isMobileOnly ? "124px" : INPUT_LENGTH)};
  }
  .schedule-backup_combobox {
    display: inline-block;
    margin-top: 8px;
  }

  .main_options {
    max-width: 363px;
  }

  ${!isMobileOnly
    ? css`
        .main_options {
          max-width: ${INPUT_LENGTH};
          display: grid;

          ${(props) =>
            props.weeklySchedule || props.monthlySchedule
              ? css`
                  grid-template-areas: "days weekly-monthly time";
                  grid-template-columns: 1fr 1fr 1fr;
                `
              : css`
                  grid-template-areas: "days  time";
                  grid-template-columns: 1fr 1fr;
                `};
          grid-gap: 8px;
        }
      `
    : css`
        .days_option {
          grid-area: time;

          max-width: ${INPUT_LENGTH};

          width: 100%;
        }
      `}

  .time_options {
    grid-area: time;
    ${isMobileOnly &&
    css`
      max-width: ${INPUT_LENGTH};
    `};
    width: 100%;
  }

  .max_copies {
    width: 100%;
    max-width: ${INPUT_LENGTH};
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
  .schedule_help-section {
    display: flex;
    .schedule_help-button {
      margin: 3px 0 0 4px;
    }
  }
`;

const StyledBackup = styled.div`
  ${commonSettingsStyles}

  .backup_modules-separation {
    margin-bottom: 28px;
    border-bottom: ${(props) =>
      props.theme.client.settings.backup.separatorBorder};
  }
  .backup_modules-header {
    font-size: 16px;
    font-weight: bold;
    padding-bottom: 8px;
  }
  .layout-progress-bar {
    ${!isMobileOnly && "cursor: default;"}
  }

  .backup_modules-description {
    margin-bottom: 24px;
    max-width: ${TEXT_LENGTH};
  }
  .backup_modules-header_wrapper {
    display: flex;
    svg {
      margin: 5px 0 0 4px;
    }
  }

  .backup-section_wrapper {
    margin-bottom: 27px;
    .backup-section_heading {
      display: flex;
      margin-bottom: 8px;
      .backup-section_text {
        font-weight: 700;
        font-size: 16px;
        line-height: 22px;
      }
      .backup-section_arrow-button {
        margin: auto 0 auto 7.29px;
      }
    }
  }
`;

const StyledBackupList = styled.div`
  height: 100%;
  width: calc(100% - 16px);
  .loader {
    height: inherit;
  }

  .backup-list-row-list {
    height: 100%;
    width: 100%;
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

    grid-template-columns: 25px 32px auto 32px;

    ${(props) => props.isChecked && `background: #F3F4F4;`}

    padding-left: 16px;
  }

  .backup-restore_dialog-scroll-body {
    margin-left: -17px;
    margin-right: -17px;

    .nav-thumb-vertical {
      margin-left: -8px !important;
    }
  }
  .backup-restore_empty-list {
    margin-top: 96px;
    margin-left: 16px;
    margin-right: 16px;
  }

  #backup-list_help {
    display: flex;
    background-color: ${(props) => props.theme.backgroundColor};
    margin-bottom: 16px;
  }
  .backup-list_tooltip {
    margin-left: 8px;
  }

  .backup-list_content {
    display: grid;
    height: calc(100% - 32px);
    grid-template-rows: max-content auto max-content;

    .backup-list_agreement-text {
      user-select: none;
      div:first-child {
        display: inline-block;
      }
    }

    .backup-list_footer {
      padding: 16px 16px 0 16px;

      ${(props) => !props.isEmpty && `border-top: 1px solid ${borderColor}`};
      margin-left: -16px;
      margin-right: -16px;

      .restore_dialog-button {
        display: flex;
        button:first-child {
          margin-right: 10px;
          width: 50%;
        }
        button:last-child {
          width: 50%;
        }
      }
    }
  }
`;

const StyledSettingsHeader = styled.div`
  display: flex;
  position: fixed;
  top: ${(props) => (props.isVisible ? "48px" : "-48px")};

  transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
  -moz-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
  -ms-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
  -webkit-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
  -o-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);

  background-color: #fff;
  z-index: 149;
  width: 100%;
  height: 50px;

  .backup_header {
    margin-left: 14.5px;
  }
  .backup_arrow-button {
    margin: auto 0;
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
  StyledSettingsHeader,
};
