import React from "react";
import ComboBox from "@appserver/components/combobox";
import { useTranslation } from "react-i18next";
import Checkbox from "@appserver/components/checkbox";
const ScheduleComponent = ({
  weeklySchedule,
  monthlySchedule,
  weekOptions,
  selectedOption,
  selectedWeekdayOption,
  selectedTimeOption,
  selectedMonthOption,
  timeOptionsArray,
  periodOptions,
  monthNumberOptionsArray,
  onSelectMonthNumberAndTimeOptions,
  selectedMaxCopies,
  onSelectMaxCopies,
  maxNumberCopiesArray,
  onSelectPeriod,
  onSelectWeekDay,
  isLoadingData,
  isDisableOptions,
}) => {
  const { t } = useTranslation("Settings");
  //console.log("selectedWeekdayOption", selectedWeekdayOption);
  return (
    <div>
      <ComboBox
        options={periodOptions}
        selectedOption={{
          key: 0,
          label: selectedOption,
        }}
        onSelect={onSelectPeriod}
        isDisabled={isLoadingData || isDisableOptions}
        noBorder={false}
        scaled={false}
        scaledOptions={false}
        dropDownMaxHeight={300}
        size="content"
        className="backup_combobox "
      />
      {weeklySchedule && (
        <ComboBox
          options={weekOptions}
          selectedOption={{
            key: 0,
            label: selectedWeekdayOption,
          }}
          onSelect={onSelectWeekDay}
          isDisabled={isLoadingData || isDisableOptions}
          noBorder={false}
          scaled={false}
          scaledOptions={false}
          dropDownMaxHeight={300}
          size="content"
          className="backup_combobox"
        />
      )}
      {monthlySchedule && (
        <ComboBox
          options={monthNumberOptionsArray}
          selectedOption={{
            key: 0,
            label: selectedMonthOption,
          }}
          onSelect={onSelectMonthNumberAndTimeOptions}
          isDisabled={isLoadingData || isDisableOptions}
          noBorder={false}
          scaled={false}
          scaledOptions={false}
          dropDownMaxHeight={300}
          size="content"
          className="backup_combobox"
        />
      )}
      <ComboBox
        options={timeOptionsArray}
        selectedOption={{
          key: 0,
          label: selectedTimeOption,
        }}
        onSelect={onSelectMonthNumberAndTimeOptions}
        isDisabled={isLoadingData || isDisableOptions}
        noBorder={false}
        scaled={false}
        scaledOptions={false}
        dropDownMaxHeight={300}
        size="content"
        className="backup_combobox"
      />
      <ComboBox
        options={maxNumberCopiesArray}
        selectedOption={{
          key: 0,
          label: `${selectedMaxCopies} ${t("MaxCopies")}`,
        }}
        onSelect={onSelectMaxCopies}
        isDisabled={isLoadingData || isDisableOptions}
        noBorder={false}
        scaled={false}
        scaledOptions={false}
        dropDownMaxHeight={300}
        size="content"
        className="backup_combobox"
      />
      {/* <div className="backup-include_mail">
        <Checkbox
          name={"backupMail"}
          isChecked={backupMail}
          label={t("IncludeMail")}
          onChange={onClickCheckbox}
        />
      </div> */}
    </div>
  );
};
ScheduleComponent.defaultProps = {
  isDisableOptions: false,
};
export default ScheduleComponent;
