import styled, { css } from "styled-components";
import {
  commonSettingsStyles,
  UnavailableStyles,
} from "../../../utils/commonSettingsStyles";
import globalColors from "@docspace/components/utils/globalColors";
import { isMobileOnly } from "react-device-detect";
import {
  hugeMobile,
  tablet,
  mobile,
  smallTablet,
} from "@docspace/components/utils/device";

const linkColor = globalColors.black;

const INPUT_LENGTH = "350px";
const TEXT_LENGTH = "700px";
const commonStyles = css`
  ${(props) =>
    props.theme.interfaceDirection === "rtl"
      ? css`
          margin-left: 16px;
        `
      : css`
          margin-right: 16px;
        `}
  .backup_modules-description {
    margin-bottom: 24px;
    margin-top: 8px;
    max-width: ${TEXT_LENGTH};
    @media ${mobile} {
      margin-bottom: 8px;
    }
  }
  .backup_modules-header_wrapper {
    display: flex;
    svg {
      ${(props) =>
        props.theme.interfaceDirection === "rtl"
          ? css`
              margin: 5px 4px 0px 0px;
            `
          : css`
              margin: 5px 0px 0px 4px;
            `}
    }
  }
  .radio-button_text {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-left: 7px;
          `
        : css`
            margin-right: 7px;
          `}
    font-size: 13px;
    font-weight: 600;
  }
  .backup_radio-button {
    margin-bottom: 4px;
  }
  .backup_combo {
    margin-top: 16px;
    margin-bottom: 16px;
    width: 100%;
    max-width: ${INPUT_LENGTH};
    .combo-button-label {
      width: 100%;
      max-width: ${INPUT_LENGTH};
    }
  }
  .backup_text-input {
    margin: 4px 0 10px 0;
    width: 100%;
    max-width: ${INPUT_LENGTH};
    font-size: 13px;
  }
  .backup_checkbox {
    margin-top: 8px;
    margin-bottom: 16px;
  }
  .backup_radio-button-settings {
    margin-top: 8px;
    margin-bottom: 16px;
  }
  .backup_radio-button {
    max-width: ${TEXT_LENGTH};
    font-size: 12px;
    line-height: 15px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  }
`;

const StyledManualBackup = styled.div`
  ${commonStyles}
  .manual-backup_buttons {
    margin-top: 16px;
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 24px;
          `
        : css`
            margin-left: 24px;
          `}
    display: flex;
    align-items: center;
    justify-content: flex-start;

    button:first-child {
      max-width: 124px;
      ${(props) =>
        props.theme.interfaceDirection === "rtl"
          ? css`
              margin-left: 8px;
            `
          : css`
              margin-right: 8px;
            `}
    }
    button:last-child {
      max-width: 153px;
    }

    @media ${tablet} {
      button:first-child {
        max-width: 129px;
      }
      button:last-child {
        max-width: 160px;
      }
    }

    @media ${hugeMobile} {
      button:first-child {
        max-width: 155px;
      }
      button:last-child {
        max-width: 155px;
      }
    }
  }
  .manual-backup_storages-module {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 24px;
          `
        : css`
            margin-left: 24px;
          `}
    .manual-backup_buttons {
      ${(props) =>
        props.theme.interfaceDirection === "rtl"
          ? css`
              margin-right: 0;
            `
          : css`
              margin-left: 0;
            `}
    }
  }
  .manual-backup_third-party-module {
    margin-top: 16px;
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 24px;
          `
        : css`
            margin-left: 24px;
          `}
  }
  .manual-backup_folder-input {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 24px;
          `
        : css`
            margin-left: 24px;
          `}
    margin-top: 16px;
  }
`;

const StyledAutoBackup = styled.div`
  ${commonStyles}
  .auto-backup_third-party-module {
    margin-top: 16px;
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 24px;
          `
        : css`
            margin-left: 24px;
          `}
    button {
      margin-bottom: 16px;
    }
  }
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
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 37px;
          `
        : css`
            margin-left: 37px;
          `}

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
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-left: 8px;
          `
        : css`
            margin-right: 8px;
          `}
  }
  .backup_modules {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 37px;
          `
        : css`
            margin-left: 37px;
          `}
  }
  .auto-backup_storages-module {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 24px;
          `
        : css`
            margin-left: 24px;
          `}
    .backup_schedule-component {
      ${(props) =>
        props.theme.interfaceDirection === "rtl"
          ? css`
              margin-right: 0;
            `
          : css`
              margin-left: 0;
            `}
    }
  }
  .auto-backup_folder-input {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 24px;
          `
        : css`
            margin-left: 24px;
          `}
  }
  .backup_toggle-wrapper {
    margin-bottom: 16px;
    background-color: ${(props) =>
      props.theme.client.settings.backup.rectangleBackgroundColor};
    padding: 12px;
    max-width: 724px;
    box-sizing: border-box;
    display: grid;
    grid-template-columns: minmax(100px, 724px);
  }
  .auto-backup_buttons {
    ${!isMobileOnly && "margin-bottom: 24px"}
  }

  .auto-backup_badge {
    height: 16px;
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 8px;
          `
        : css`
            margin-left: 8px;
          `}
    cursor: auto;
  }
  ${(props) => !props.isEnableAuto && UnavailableStyles}
`;
const StyledStoragesModule = styled.div`
  .backup_storages-buttons {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: -63px;
          `
        : css`
            margin-left: -63px;
          `}
    margin-top: 40px;
  }
`;
const StyledRestoreBackup = styled.div`
  ${commonStyles}
  .restore-backup_third-party-module {
    margin-top: 16px;

    button {
      margin-bottom: 16px;
    }
  }
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
    color: ${(props) => props.theme.client.settings.backup.warningColor};
  }
  .restore-backup_warning-link {
    margin-top: 16px;
    max-width: ${TEXT_LENGTH};
  }
  .restore-backup_warning-description {
    max-width: ${TEXT_LENGTH};
  }
  .restore-backup-checkbox {
    margin: 24px 0;
  }
  .restore-backup-checkbox_notification {
    margin-top: 11px;
    max-width: ${TEXT_LENGTH};
    .checkbox-text {
      white-space: normal;
    }
  }

  .restore-backup_list {
    text-decoration: underline dotted;
    cursor: ${(props) => (props.isEnableRestore ? "pointer" : "cursor")};
    font-weight: 600;
  }
  .restore-backup_input {
    margin: 16px 0;
    max-width: ${INPUT_LENGTH};

    @media ${smallTablet} {
      max-width: none;
    }
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
    @media ${smallTablet} {
      width: 100%;
    }
  }
  ${(props) => !props.isEnableRestore && UnavailableStyles}
`;
const StyledModules = styled.div`
  margin-bottom: 24px;
  .backup-description {
    ${(props) => props.isDisabled && `color: #A3A9AE`};
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 25px;
          `
        : css`
            margin-left: 25px;
          `}
    max-width: 700px;
  }
`;

const StyledScheduleComponent = styled.div`
  ${(props) =>
    props.theme.interfaceDirection === "rtl"
      ? css`
          margin-right: 24px;
        `
      : css`
          margin-left: 24px;
        `}
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
      ${(props) =>
        props.theme.interfaceDirection === "rtl"
          ? css`
              margin: 3px 4px 0 0;
            `
          : css`
              margin: 3px 0 0 4px;
            `}
    }
  }
`;
const StyledBackup = styled.div`
  ${commonSettingsStyles}
  .backup_connection {
    display: flex;
    margin-bottom: 12px;
    display: grid;

    ${(props) =>
      props.isConnectedAccount
        ? "grid-template-columns:minmax(100px,  310px) 32px"
        : "grid-template-columns:minmax(100px,  350px) 32px"};
    grid-gap: 8px;
  }

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
        ${(props) =>
          props.theme.interfaceDirection === "rtl"
            ? css`
                margin: auto 7.29px auto 0;
              `
            : css`
                margin: auto 0 auto 7.29px;
              `}
      }
    }
  }
  .backup_third-party-context {
    margin-top: 4px;
  }
`;
const StyledBackupList = styled.div`
  height: 100%;
  width: 100%;
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
    margin: 0 !important;
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding-right: 10px;
          `
        : css`
            padding-left: 10px;
          `}
  }
  .backup-list_item {
    border-radius: 3px;
    cursor: default;
    align-items: center;
    display: grid;
    height: 48px;
    grid-template-areas: "trash icon-name full-name  radiobutton";
    grid-template-columns: 25px 32px auto 32px;
    ${(props) =>
      props.isChecked &&
      css`
        background: ${(props) =>
          props.theme.client.settings.backup.backupCheckedListItemBackground};
      `}
    padding-left: 16px;
    padding-right: 16px;
  }
  .backup-restore_dialog-scroll-body {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: -16px;
            margin-left: -17px;
          `
        : css`
            margin-left: -16px;
            margin-right: -17px;
          `}

    .nav-thumb-vertical {
      ${(props) =>
        props.theme.interfaceDirection === "rtl"
          ? css`
              margin-right: -8px !important;
            `
          : css`
              margin-left: -8px !important;
            `}
    }
  }
  .backup-restore_empty-list {
    margin-top: 96px;
    margin-left: 16px;
    margin-right: 16px;
    color: ${(props) => props.theme.client.settings.backup.textColor};
  }

  .backup-list_content {
    display: grid;
    height: 100%;
    grid-template-rows: max-content auto max-content;
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
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 14.5px;
          `
        : css`
            margin-left: 14.5px;
          `}
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
