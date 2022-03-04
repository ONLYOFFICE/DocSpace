import React from "react";
import ComboBox from "@appserver/components/combobox";
import { useTranslation } from "react-i18next";
import { StyledScheduleComponent } from "../../StyledBackup";
import Text from "@appserver/components/text";
import { inject, observer } from "mobx-react";

const ScheduleComponent = ({
  selectedPeriodLabel,
  selectedWeekdayLabel,
  selectedHour,
  selectedMonthDay,
  selectedMaxCopiesNumber,

  setMaxCopies,
  setPeriod,
  setWeekday,
  setMonthNumber,
  setTime,

  isLoadingData,
  isDisableOptions,

  periodsObject,
  weekdaysLabelArray,
  monthNumbersArray,
  hoursArray,
  maxNumberCopiesArray,

  selectedPeriodNumber,
}) => {
  const { t } = useTranslation("Settings");

  return (
    <StyledScheduleComponent
      weeklySchedule={selectedPeriodNumber === "1"}
      monthlySchedule={selectedPeriodNumber === "2"}
      className="backup_schedule-component"
    >
      <Text className="schedule_description"> {t("AutoSavePeriod")}</Text>
      <div className="main_options">
        <ComboBox
          options={periodsObject}
          selectedOption={{
            key: 0,
            label: selectedPeriodLabel,
          }}
          onSelect={setPeriod}
          isDisabled={isLoadingData || isDisableOptions}
          noBorder={false}
          scaled={false}
          scaledOptions={true}
          dropDownMaxHeight={500}
          className="schedule-backup_combobox days_option"
        />
        {selectedPeriodNumber === "1" && (
          <ComboBox
            options={weekdaysLabelArray}
            selectedOption={{
              key: 0,
              label: selectedWeekdayLabel,
            }}
            onSelect={setWeekday}
            isDisabled={isLoadingData || isDisableOptions}
            noBorder={false}
            scaled={false}
            scaledOptions={true}
            dropDownMaxHeight={400}
            className="schedule-backup_combobox weekly_option"
          />
        )}
        {selectedPeriodNumber === "2" && (
          <ComboBox
            options={monthNumbersArray}
            selectedOption={{
              key: 0,
              label: selectedMonthDay,
            }}
            onSelect={setMonthNumber}
            isDisabled={isLoadingData || isDisableOptions}
            noBorder={false}
            scaled={false}
            scaledOptions={true}
            dropDownMaxHeight={400}
            className="schedule-backup_combobox month_options"
          />
        )}
        <ComboBox
          options={hoursArray}
          selectedOption={{
            key: 0,
            label: selectedHour,
          }}
          onSelect={setTime}
          isDisabled={isLoadingData || isDisableOptions}
          noBorder={false}
          scaled={false}
          scaledOptions={true}
          dropDownMaxHeight={400}
          className="schedule-backup_combobox time_options"
        />
      </div>
      <div className="maxCopiesOption">
        <ComboBox
          options={maxNumberCopiesArray}
          selectedOption={{
            key: 0,
            label: `${selectedMaxCopiesNumber} ${t("MaxCopies")}`,
          }}
          onSelect={setMaxCopies}
          isDisabled={isLoadingData || isDisableOptions}
          noBorder={false}
          scaled={false}
          scaledOptions={true}
          dropDownMaxHeight={400}
          className="schedule-backup_combobox max_copies"
        />
      </div>
    </StyledScheduleComponent>
  );
};
ScheduleComponent.defaultProps = {
  isDisableOptions: false,
};
//export default ScheduleComponent;

export default inject(({ backup }) => {
  const {
    selectedPeriodLabel,
    selectedWeekdayLabel,
    selectedHour,
    selectedMonthDay,
    selectedMaxCopiesNumber,
    setPeriod,
    setMonthNumber,
    setTime,
    setMaxCopies,
    setWeekday,
    selectedPeriodNumber,
  } = backup;

  // console.log(
  //   selectedPeriodLabel,
  //   selectedWeekdayLabel,
  //   selectedHour,
  //   selectedMonthDay,
  //   selectedMaxCopiesNumber,
  //   selectedPeriodNumber
  // );
  return {
    selectedPeriodLabel,
    selectedWeekdayLabel,
    selectedHour,
    selectedMonthDay,
    selectedMaxCopiesNumber,
    selectedPeriodNumber,

    setPeriod,
    setMonthNumber,
    setTime,
    setMaxCopies,
    setWeekday,
  };
})(observer(ScheduleComponent));
