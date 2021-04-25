import React from "react";
import Text from "@appserver/components/text";
import { useTranslation, withTranslation } from "react-i18next";
import ComboBox from "@appserver/components/combobox";
import styled from "styled-components";
import Button from "@appserver/components/button";
import Checkbox from "@appserver/components/checkbox";

class DocumentsModule extends React.Component {
  constructor(props) {
    super(props);
    const { t } = props;

    this.periodOptions = [
      {
        key: "1",
        label: t("DailyPeriodSchedule"),
      },
      {
        key: "2",
        label: t("WeeklyPeriodSchedule"),
      },
      {
        key: "3",
        label: t("MonthlyPeriodSchedule"),
      },
    ];

    this.timeOptionsArray = [];
    for (let item = 0; item < 24; item++) {
      let obj = {
        key: item,
        label: `${item}:00`,
      };
      this.timeOptionsArray.push(obj);
    }
  }
  render() {
    const {
      t,
      isManualBackup,
      backupMailDocuments,
      onClickCheckbox,
      onSelect,
    } = this.props;
    return (
      <>
        <div className="category-item-wrapper">
          {isManualBackup && (
            <div className="category-item-heading">
              <Text className="inherit-title-link header">
                {t("DocumentsModule")}
              </Text>
            </div>
          )}
          <Text className="category-item-description">
            {t("DocumentsModuleDescription")}
          </Text>

          {!isManualBackup && (
            <>
              <ComboBox
                options={this.periodOptions}
                selectedOption={{
                  key: 0,
                  label: `${t("DailyPeriodSchedule")}`,
                }}
                onSelect={onSelect}
                isDisabled={false}
                noBorder={false}
                scaled={false}
                scaledOptions={false}
                dropDownMaxHeight={300}
                size="base"
                className="backup_combobox"
              />
              <ComboBox
                options={this.timeOptionsArray}
                selectedOption={{
                  key: 0,
                  label: `12:00`,
                }}
                onSelect={onSelect}
                isDisabled={false}
                noBorder={false}
                scaled={false}
                scaledOptions={false}
                dropDownMaxHeight={300}
                size="base"
                className="backup_combobox"
              />
            </>
          )}

          <div className="backup-include_mail">
            <Checkbox
              name={"backupMailDocuments"}
              isChecked={backupMailDocuments}
              label={t("IncludeMail")}
              onChange={onClickCheckbox}
            />
          </div>

          {isManualBackup && (
            <div className="manual-backup_buttons">
              <Button
                label={t("MakeCopy")}
                onClick={() => console.log("click")}
                primary
                isDisabled={false}
                size="medium"
                tabIndex={10}
              />
            </div>
          )}
        </div>
      </>
    );
  }
}

export default withTranslation("Settings")(DocumentsModule);
