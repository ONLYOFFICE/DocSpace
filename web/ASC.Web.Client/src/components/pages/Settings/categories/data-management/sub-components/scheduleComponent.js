import React from "react";
import ComboBox from "@appserver/components/combobox";

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
  onSelectPeriodAndWeekday,
  onSelectMonthNumberAndTimeOptions,
}) => {
  return (
    <div className="category-item-wrapper">
      <ComboBox
        options={periodOptions}
        selectedOption={{
          key: 0,
          label: selectedOption,
        }}
        onSelect={onSelectPeriodAndWeekday}
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
          onSelect={onSelectPeriodAndWeekday}
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
          options={monthNumberOptionsArray}
          selectedOption={{
            key: 0,
            label: selectedMonthOption,
          }}
          onSelect={onSelectMonthNumberAndTimeOptions}
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
        options={timeOptionsArray}
        selectedOption={{
          key: 0,
          label: selectedTimeOption,
        }}
        onSelect={onSelectMonthNumberAndTimeOptions}
        isDisabled={false}
        noBorder={false}
        scaled={false}
        scaledOptions={false}
        dropDownMaxHeight={300}
        size="base"
        className="backup_combobox"
      />
    </div>
  );
};

export default ScheduleComponent;
