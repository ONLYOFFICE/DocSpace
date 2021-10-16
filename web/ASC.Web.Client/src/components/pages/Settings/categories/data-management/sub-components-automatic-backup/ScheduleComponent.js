import React from "react";
import ComboBox from "@appserver/components/combobox";
import { useTranslation } from "react-i18next";
import { StyledScheduleComponent } from "../StyledBackup";
import Text from "@appserver/components/text";
const ScheduleComponent = ({
  weeklySchedule,
  monthlySchedule,

  selectedPeriodLabel,
  selectedWeekdayLabel,
  selectedHour,
  selectedMonthDay,
  selectedMaxCopies,

  onSelectMaxCopies,
  onSelectPeriod,
  onSelectWeekDay,
  onSelectMonthNumber,
  onSelectTime,

  isLoadingData,
  isDisableOptions,

  periodsObject,
  weekdaysLabelArray,
  monthNumbersArray,
  hoursArray,
  maxNumberCopiesArray,
}) => {
  const { t } = useTranslation("Settings");

  return (
    <StyledScheduleComponent
      weeklySchedule={weeklySchedule}
      monthlySchedule={monthlySchedule}
    >
      <Text className="schedule_description"> {t("ScheduleDescription")}</Text>
      <div className="main_options">
        <ComboBox
          options={periodsObject}
          selectedOption={{
            key: 0,
            label: selectedPeriodLabel,
          }}
          onSelect={onSelectPeriod}
          isDisabled={isLoadingData || isDisableOptions}
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
            onSelect={onSelectWeekDay}
            isDisabled={isLoadingData || isDisableOptions}
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
            onSelect={onSelectMonthNumber}
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
          onSelect={onSelectTime}
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
            label: `${selectedMaxCopies} ${t("MaxCopies")}`,
          }}
          onSelect={onSelectMaxCopies}
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
export default ScheduleComponent;
