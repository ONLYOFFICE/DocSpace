import React, { useState } from "react";
import moment from "moment";

import styled from "styled-components";

import { Text } from "@docspace/components";
import { SelectorAddButton } from "@docspace/components";
import { SelectedItem } from "@docspace/components";

import { Calendar } from "@docspace/components";
import { TimePicker } from "../../../sub-components/TimePicker";

const TimePickerCell = styled.span`
  margin-left: 8px;
  display: inline-flex;
  align-items: center;

  .timePickerItem {
    display: inline-flex;
    align-items: center;
    margin-right: 16px;
  }
`;

export const DeliveryDatePicker = ({
  Selectors,
  filterSettings,
  setFilterSettings,
  isApplied,
  setIsApplied,
}) => {
  const [isCalendarOpen, setIsCalendarOpen] = useState(false);
  const [isTimeOpen, setIsTimeOpen] = useState(false);

  const toggleCalendar = () => setIsCalendarOpen((prevIsCalendarOpen) => !prevIsCalendarOpen);
  const closeCalendar = () => setIsCalendarOpen(false);

  const showTimePicker = () => setIsTimeOpen(true);

  const setDeliveryFrom = (date) =>
    setFilterSettings((prevFilterSetting) => ({ ...prevFilterSetting, deliveryFrom: date }));

  const setDeliveryTo = (date) =>
    setFilterSettings((prevFilterSetting) => ({ ...prevFilterSetting, deliveryTo: date }));

  const setDeliveryDate = (date) =>
    setFilterSettings((prevFilterSetting) => ({ ...prevFilterSetting, deliveryDate: date }));

  const deleteSelectedDate = () => {
    setFilterSettings((prevFilterSetting) => ({ ...prevFilterSetting, deliveryDate: null }));
    setFilterSettings((prevFilterSetting) => ({
      ...prevFilterSetting,
      deliveryFrom: moment().startOf("day"),
      deliveryTo: moment().endOf("day"),
    }));
    setIsTimeOpen(false);
    setIsApplied(false);
  };

  const DateSelector = () => (
    <span>
      <SelectorAddButton title="add" onClick={toggleCalendar} style={{ marginRight: "8px" }} />
      <Text isInline fontWeight={600} color="#A3A9AE">
        Select date
      </Text>
      {isCalendarOpen && (
        <Calendar
          selectedDate={filterSettings.deliveryDate}
          setSelectedDate={setDeliveryDate}
          onChange={closeCalendar}
        />
      )}
    </span>
  );

  const SelectedDate = () => (
    <SelectedItem
      onClose={deleteSelectedDate}
      text={moment(filterSettings.deliveryDate).format("DD MMM YYYY")}
    />
  );

  const SelectedDateTime = () => (
    <SelectedItem
      onClose={deleteSelectedDate}
      text={
        moment(filterSettings.deliveryDate).format("DD MMM YYYY") +
        " " +
        moment(filterSettings.deliveryFrom).format("HH:mm") +
        " - " +
        moment(filterSettings.deliveryTo).format("HH:mm")
      }
    />
  );

  const TimeSelectorAdder = () => (
    <TimePickerCell>
      <SelectorAddButton title="add" onClick={showTimePicker} style={{ marginRight: "8px" }} />
      <Text isInline fontWeight={600} color="#A3A9AE">
        Select Delivery time
      </Text>
    </TimePickerCell>
  );

  const TimeSelector = () => (
    <TimePickerCell>
      <span className="timePickerItem">
        <Text isInline fontWeight={600} color="#A3A9AE" style={{ marginRight: "8px" }}>
          From
        </Text>
        <TimePicker date={filterSettings.deliveryFrom} setDate={setDeliveryFrom} />
      </span>
      <Text isInline fontWeight={600} color="#A3A9AE" style={{ marginRight: "8px" }}>
        Before
      </Text>
      <TimePicker date={filterSettings.deliveryTo} setDate={setDeliveryTo} />
    </TimePickerCell>
  );
  return (
    <>
      <Text fontWeight={600} fontSize="15px">
        Delivery date
      </Text>
      <Selectors>
        {isApplied ? (
          <SelectedDateTime />
        ) : filterSettings.deliveryDate === null ? (
          <DateSelector />
        ) : (
          <SelectedDate />
        )}
        {filterSettings.deliveryDate !== null &&
          !isApplied &&
          (isTimeOpen ? <TimeSelector /> : <TimeSelectorAdder />)}
      </Selectors>
    </>
  );
};
