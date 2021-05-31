import styled from "styled-components";
import commonSettingsStyles from "../../utils/commonSettingsStyles";

const StyledComponent = styled.div`
  ${commonSettingsStyles}

  .manual-backup_buttons {
    margin-top: 16px;
  }
  .backup-include_mail,
  .backup_combobox {
    margin-top: 16px;
    margin-bottom: 16px;
  }
  .inherit-title-link {
    margin-bottom: 8px;
  }
  .note_description {
    margin-top: 8px;
  }
  .radio-button_text {
    font-size: 19px;
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
    font-size: 19px;
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
  }

  .input-with-folder-path,
  .text-input-with-folder-path {
    width: 100%;
    max-width: 820px;
  }
`;
const StyledModules = styled.div`
  margin-bottom: 40px;
  .category-item-description {
    ${(props) => props.isDisabled && `color: #A3A9AE`}
  }
`;

const StyledScheduleComponent = styled.div`
  .time_options {
    .drop-down_variable-size {
      width: 60px !important;
    }
  }
`;

const StyledBackup = styled.div`
  ${commonSettingsStyles}
`;
export {
  StyledModules,
  StyledComponent,
  StyledScheduleComponent,
  StyledBackup,
};
