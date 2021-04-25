import React from "react";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import commonSettingsStyles from "../../utils/commonSettingsStyles";
import styled from "styled-components";
import Button from "@appserver/components/button";
import Checkbox from "@appserver/components/checkbox";
import FieldContainer from "@appserver/components/field-container";
import RadioButtonGroup from "@appserver/components/radio-button-group";
import RadioButton from "@appserver/components/radio-button";
import DocumentsModule from "./sub-components/documentsModule";
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
    font-size: 19px;
    font-weight: 600;
  }
  .automatic-backup_current_storage {
    margin-bottom: 8px;
  }
`;
class AutomaticBackup extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      backupMailTemporaryStorage: false,
      backupMailDocuments: false,
      backupMailThirdParty: false,
      backupMailThirdPartyStorage: false,
      isShowedStorageType: false, //if current automatic storage not choose

      isShowDocuments: false,
      isShowThirdParty: false,
      isShowThirdPartyStorage: false,

      isCheckedDocuments: false,
      isCheckedThirdParty: false,
      isCheckedThirdPartyStorage: false,

      monthlySchedule: "",
      dailySchedule: "",
      weeklySchedule: "",
    };
  }

  onClickCheckbox = (e) => {
    const name = e.target.name;
    let change = !this.state[name];
    this.setState({ [name]: change });
  };
  onClickPermissions = (e) => {
    console.log("res", e);
    const name = e.target.defaultValue;
    if (name === "enable") {
      this.setState({
        isShowedStorageTypes: true,
      });
    } else {
      this.setState({
        isShowedStorageTypes: false,
      });
    }
  };

  onClickShowStorage = (e) => {
    console.log("e0", e);
    const name = e.target.name;

    name === "DocumentsModule"
      ? this.setState({
          isShowDocuments: true,
          isCheckedDocuments: true,
          isShowThirdParty: false,
          isCheckedThirdParty: false,
        })
      : name === "ThirdPartyResource"
      ? this.setState({
          isShowDocuments: false,
          isCheckedDocuments: false,
          isShowThirdParty: true,
          isCheckedThirdParty: true,
        })
      : "";
  };
  onClickCheckbox = (e) => {
    const name = e.target.name;
    let change = !this.state[name];
    this.setState({ [name]: change });
  };

  onSelect = (options) => {
    console.log("options", options);
    const key = options.key;
    const label = options.label;
    key === 1
      ? this.setState({ dailySchedule: label })
      : key === 2
      ? this.setState({ weeklySchedule: label })
      : this.setState({ monthlySchedule: label });
  };
  render() {
    const { t } = this.props;
    const {
      backupMailTemporaryStorage,
      backupMailDocuments,
      backupMailThirdParty,
      backupMailThirdPartyStorage,
      isShowedStorageTypes,
      isCheckedDocuments,
      isCheckedThirdParty,
      isShowDocuments,
    } = this.state;
    console.log(" this.periodOptions", this.rt);
    return (
      <StyledComponent>
        <RadioButtonGroup
          className="automatic-backup_main "
          name={"DocumentsModule"}
          options={[
            {
              label: t("DisableAutomaticBackup"),
              value: "disable",
            },
            {
              label: t("EnableAutomaticBackup"),
              value: "enable",
            },
          ]}
          isDisabled={false}
          onClick={this.onClickPermissions}
          orientation="vertical"
          selected="disable"
        />

        {isShowedStorageTypes && (
          <>
            <RadioButton
              // name={"DocumentsModule"}
              // options={[
              //   {
              //     label: t("DocumentsModule"),
              //     value: "documentsModule",
              //   },
              //   {
              //     label: "Third radiobtn",
              //     value: "third",
              //   },
              // ]}
              // isDisabled={false}
              // onClick={() => console.log("res")}
              // orientation="vertical"
              // selected="documentsModule"

              fontSize="13px"
              fontWeight="400"
              label={t("DocumentsModule")}
              name={"DocumentsModule"}
              //onChange={this.onClickShowStorage}
              onClick={this.onClickShowStorage}
              isChecked={isCheckedDocuments}
              value="value"
              className="automatic-backup_current_storage"
            />
            {isShowDocuments && (
              <DocumentsModule
                isManualBackup={false}
                onClickCheckbox={this.onClickCheckbox}
                backupMailDocuments={backupMailDocuments}
                onSelect={this.onSelect}
              />
            )}
            <RadioButton
              fontSize="13px"
              fontWeight="400"
              label={t("ThirdPartyResource")}
              name={"ThirdPartyResource"}
              //onChange={this.onClickShowStorage}
              onClick={this.onClickShowStorage}
              isChecked={isCheckedThirdParty}
              value="value"
            />
          </>
        )}
      </StyledComponent>
    );
  }
}
export default withTranslation("Settings")(AutomaticBackup);
