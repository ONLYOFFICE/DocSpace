import React from "react";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import commonSettingsStyles from "../../utils/commonSettingsStyles";
import { inject, observer } from "mobx-react";
import Button from "@appserver/components/button";
import Checkbox from "@appserver/components/checkbox";
import FieldContainer from "@appserver/components/field-container";
import RadioButtonGroup from "@appserver/components/radio-button-group";
import RadioButton from "@appserver/components/radio-button";
import DocumentsModule from "./sub-components/documentsModule";
import ComboBox from "@appserver/components/combobox";
import styled from "styled-components";
import moment from "moment";

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
    const { t, language } = props;

    this.lng = language.substring(0, language.indexOf("-"));
    moment.locale(this.lng);

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

      monthlySchedule: false,
      dailySchedule: false,
      weeklySchedule: false,

      selectedOption: t("DailyPeriodSchedule"),
      selectedWeekdayOption: "",
      selectedTimeOption: "12:00",
      selectedMonthOption: "1",
      weekOptions: [],
    };

    this.periodOptions = [
      {
        key: 1,
        label: t("DailyPeriodSchedule"),
      },
      {
        key: 2,
        label: t("WeeklyPeriodSchedule"),
      },
      {
        key: 3,
        label: t("MonthlyPeriodSchedule"),
      },
    ];

    this.timeOptionsArray = [];
    this.getTimeOptions();
    this.monthNumberOptionsArray = [];
    this.getMonthNumbersOption();
    this.weekdaysOptions = [];
    this.arrayWeekdays = moment.weekdays();
  }

  componentDidMount() {
    this.getWeekdaysOptions();
  }

  getTimeOptions = () => {
    for (let item = 0; item < 24; item++) {
      let obj = {
        key: item,
        label: `${item}:00`,
      };
      this.timeOptionsArray.push(obj);
    }
  };

  getMonthNumbersOption = () => {
    for (let item = 1; item <= 31; item++) {
      let obj = {
        key: item + 24,
        label: `${item}`,
      };
      this.monthNumberOptionsArray.push(obj);
    }
  };
  getWeekdaysOptions = () => {
    for (let item = 0; item < this.arrayWeekdays.length; item++) {
      let obj = {
        key: item + 4,
        label: `${this.arrayWeekdays[item]}`,
      };
      this.weekdaysOptions.push(obj);
    }
    const isEnglishLanguage = this.lng === "en";

    if (!isEnglishLanguage) {
      const startWeek = this.weekdaysOptions[0];
      this.weekdaysOptions.shift();
      this.weekdaysOptions.push(startWeek);
    }

    this.setState({
      weekOptions: this.weekdaysOptions,
      selectedWeekdayOption: this.weekdaysOptions[0].label,
    });
  };

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
        isCheckedDocuments: true,
        isShowDocuments: true,
      });
    } else {
      this.setState({
        isShowedStorageTypes: false,
        isCheckedDocuments: false,
        isShowDocuments: false,
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

  onSelectPeriodAndWeekday = (options) => {
    console.log("options", options);

    const key = options.key;
    const label = options.label;

    if (key <= 3) {
      this.setState({ selectedOption: label });
      key === 1
        ? this.setState({ weeklySchedule: false, monthlySchedule: false })
        : key === 2
        ? this.setState({ weeklySchedule: true, monthlySchedule: false })
        : this.setState({ monthlySchedule: true, weeklySchedule: false });
    } else {
      this.setState({
        selectedWeekdayOption: label,
      });
    }
  };

  onSelectMonthNumberAndTimeOptions = (options) => {
    const key = options.key;
    const label = options.label;
    if (key <= 24) {
      this.setState({ selectedTimeOption: label });
    } else {
      this.setState({
        selectedMonthOption: label,
      });
    }
  };
  render() {
    const { t, language } = this.props;
    const {
      backupMailTemporaryStorage,
      backupMailDocuments,
      backupMailThirdParty,
      backupMailThirdPartyStorage,
      isShowedStorageTypes,
      isCheckedDocuments,
      isCheckedThirdParty,
      isShowDocuments,
      weeklySchedule,
      monthlySchedule,
      weekOptions,
      selectedOption,
      selectedWeekdayOption,
      selectedTimeOption,
      selectedMonthOption,
    } = this.state;
    console.log(" this.arrayWeekdays", this.arrayWeekdays);
    console.log("isCheckedDocuments", isCheckedDocuments);
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
              fontSize="13px"
              fontWeight="400"
              label={t("DocumentsModule")}
              name={"DocumentsModule"}
              onClick={this.onClickShowStorage}
              isChecked={isCheckedDocuments}
              value="value"
              className="automatic-backup_current_storage"
            />
            {isShowDocuments && (
              <div className="category-item-wrapper">
                <Text className="category-item-description">
                  {t("DocumentsModuleDescription")}
                </Text>

                <ComboBox
                  options={this.periodOptions}
                  selectedOption={{
                    key: 0,
                    label: selectedOption,
                  }}
                  onSelect={this.onSelectPeriodAndWeekday}
                  isDisabled={false}
                  noBorder={false}
                  scaled={false}
                  scaledOptions={false}
                  dropDownMaxHeight={300}
                  size="base"
                  className="backup_combobox"
                />
                {weeklySchedule && (
                  <ComboBox
                    options={weekOptions}
                    selectedOption={{
                      key: 0,
                      label: selectedWeekdayOption,
                    }}
                    onSelect={this.onSelectPeriodAndWeekday}
                    isDisabled={false}
                    noBorder={false}
                    scaled={false}
                    scaledOptions={false}
                    dropDownMaxHeight={300}
                    size="base"
                    className="backup_combobox"
                  />
                )}
                {monthlySchedule && (
                  <ComboBox
                    options={this.monthNumberOptionsArray}
                    selectedOption={{
                      key: 0,
                      label: selectedMonthOption,
                    }}
                    onSelect={this.onSelectMonthNumberAndTimeOptions}
                    isDisabled={false}
                    noBorder={false}
                    scaled={false}
                    scaledOptions={false}
                    dropDownMaxHeight={300}
                    size="base"
                    className="backup_combobox"
                  />
                )}
                <ComboBox
                  options={this.timeOptionsArray}
                  selectedOption={{
                    key: 0,
                    label: selectedTimeOption,
                  }}
                  onSelect={this.onSelectMonthNumberAndTimeOptions}
                  isDisabled={false}
                  noBorder={false}
                  scaled={false}
                  scaledOptions={false}
                  dropDownMaxHeight={300}
                  size="base"
                  className="backup_combobox"
                />

                <div className="backup-include_mail">
                  <Checkbox
                    name={"backupMailDocuments"}
                    isChecked={backupMailDocuments}
                    label={t("IncludeMail")}
                    onChange={this.onClickCheckbox}
                  />
                </div>
              </div>
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
export default inject(({ auth }) => {
  const { language } = auth;

  return {
    language,
  };
})(withTranslation("Settings")(observer(AutomaticBackup)));
