import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import ComboBox from "@appserver/components/combobox";
import Text from "@appserver/components/text";
import { StyledScheduleComponent } from "../../StyledBackup";
import { BackupTypes } from "@appserver/common/constants";

const { EveryWeekType, EveryMonthType } = BackupTypes;
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
  periodsObject,
  weekdaysLabelArray,
  weeklySchedule,
  monthNumbersArray,
  hoursArray,
  maxNumberCopiesArray,
  monthlySchedule,
}) => {
  const { t } = useTranslation("Settings");

  return (
    <StyledScheduleComponent
      weeklySchedule={weeklySchedule}
      monthlySchedule={monthlySchedule}
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
          isDisabled={isLoadingData}
          noBorder={false}
          scaled={false}
          scaledOptions={true}
          dropDownMaxHeight={500}
          className="schedule-backup_combobox days_option"
        />
        {weeklySchedule && (
          <ComboBox
            options={weekdaysLabelArray}
            selectedOption={{
              key: 0,
              label: selectedWeekdayLabel,
            }}
            onSelect={setWeekday}
            isDisabled={isLoadingData}
            noBorder={false}
            scaled={false}
            scaledOptions={true}
            dropDownMaxHeight={400}
            className="schedule-backup_combobox weekly_option"
          />
        )}
        {monthlySchedule && (
          <ComboBox
            options={monthNumbersArray}
            selectedOption={{
              key: 0,
              label: selectedMonthDay,
            }}
            onSelect={setMonthNumber}
            isDisabled={isLoadingData}
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
          isDisabled={isLoadingData}
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
            label: `${t("MaxCopies", {
              copiesCount: selectedMaxCopiesNumber,
            })}`,
          }}
          onSelect={setMaxCopies}
          isDisabled={isLoadingData}
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

  const weeklySchedule = +selectedPeriodNumber === EveryWeekType;
  const monthlySchedule = +selectedPeriodNumber === EveryMonthType;

  return {
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

    weeklySchedule,
    monthlySchedule,
  };
})(observer(ScheduleComponent));
