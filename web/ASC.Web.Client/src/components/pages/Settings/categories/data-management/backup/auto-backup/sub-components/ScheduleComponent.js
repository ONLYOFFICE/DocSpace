import React from "react";
import ComboBox from "@appserver/components/combobox";
import { useTranslation } from "react-i18next";
import { StyledScheduleComponent } from "../../StyledBackup";
import Text from "@appserver/components/text";
const ScheduleComponent = ({
  selectedMonthlySchedule,
  selectedWeeklySchedule,

  selectedPeriodLabel,
  selectedWeekdayLabel,
  selectedHour,
  selectedMonthDay,
  selectedMaxCopiesNumber,

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
      weeklySchedule={selectedMonthlySchedule}
      monthlySchedule={selectedMonthlySchedule}
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
          onSelect={onSelectPeriod}
          isDisabled={isLoadingData || isDisableOptions}
          noBorder={false}
          scaled={false}
          scaledOptions={true}
          dropDownMaxHeight={500}
          className="schedule-backup_combobox days_option"
        />
        <div className="additional_options">
          {selectedWeeklySchedule && (
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
          {selectedMonthlySchedule && (
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
      </div>
      <div className="maxCopiesOption">
        <ComboBox
          options={maxNumberCopiesArray}
          selectedOption={{
            key: 0,
            label: `${selectedMaxCopiesNumber} ${t("MaxCopies")}`,
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
