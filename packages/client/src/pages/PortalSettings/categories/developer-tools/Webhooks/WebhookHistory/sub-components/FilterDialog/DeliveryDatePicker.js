import React, { useState } from "react";
import moment from "moment";
import { inject, observer } from "mobx-react";

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

const DeliveryDatePicker = ({
  Selectors,
  filterSettings,
  setDeliveryDate,
  setDeliveryFrom,
  setDeliveryTo,
  clearFilterDate,
  isApplied,
  setIsApplied,
}) => {
  const [isCalendarOpen, setIsCalendarOpen] = useState(false);
  const [isTimeOpen, setIsTimeOpen] = useState(false);

  const toggleCalendar = () => setIsCalendarOpen((prevIsCalendarOpen) => !prevIsCalendarOpen);
  const closeCalendar = () => {
    setIsApplied(false);
    setIsCalendarOpen(false);
  };

  const showTimePicker = () => setIsTimeOpen(true);

  const deleteSelectedDate = (e) => {
    e.stopPropagation();
    clearFilterDate();
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

  const SelectedDateWithCalendar = () => (
    <span>
      <SelectedItem
        onClose={deleteSelectedDate}
        text={moment(filterSettings.deliveryDate).format("DD MMM YYYY")}
        onClick={toggleCalendar}
      />
      {isCalendarOpen && (
        <Calendar
          selectedDate={filterSettings.deliveryDate}
          setSelectedDate={setDeliveryDate}
          onChange={closeCalendar}
        />
      )}
    </span>
  );

  const SelectedDateTime = () => (
    <span>
      <SelectedItem
        onClose={deleteSelectedDate}
        text={
          moment(filterSettings.deliveryDate).format("DD MMM YYYY") +
          " " +
          moment(filterSettings.deliveryFrom).format("HH:mm") +
          " - " +
          moment(filterSettings.deliveryTo).format("HH:mm")
        }
        onClick={toggleCalendar}
      />
      {isCalendarOpen && (
        <Calendar
          selectedDate={filterSettings.deliveryDate}
          setSelectedDate={setDeliveryDate}
          onChange={closeCalendar}
        />
      )}
    </span>
  );

  const TimeSelectorAdder = () => (
    <TimePickerCell>
      <SelectorAddButton title="add" onClick={showTimePicker} style={{ marginRight: "8px" }} />
      <Text isInline fontWeight={600} color="#A3A9AE">
        Select Delivery time
      </Text>
    </TimePickerCell>
  );

  const isEqualDates = (firstDate, secondDate) => {
    return firstDate.format() === secondDate.format();
  };

  const isTimeEqual =
    isEqualDates(filterSettings.deliveryFrom, filterSettings.deliveryFrom.clone().startOf("day")) &&
    isEqualDates(filterSettings.deliveryTo, filterSettings.deliveryTo.clone().endOf("day"));

  const isTimeValid = filterSettings.deliveryTo > filterSettings.deliveryFrom;

  return (
    <>
      <Text fontWeight={600} fontSize="15px">
        Delivery date
      </Text>
      <Selectors>
        {filterSettings.deliveryDate === null ? (
          <DateSelector />
        ) : isApplied ? (
          isTimeEqual ? (
            <SelectedDateWithCalendar />
          ) : (
            <SelectedDateTime />
          )
        ) : (
          <SelectedDate />
        )}
        {filterSettings.deliveryDate !== null &&
          !isApplied &&
          (isTimeOpen ? (
            <TimePickerCell>
              <span className="timePickerItem">
                <Text isInline fontWeight={600} color="#A3A9AE" style={{ marginRight: "8px" }}>
                  From
                </Text>
                <TimePicker
                  date={filterSettings.deliveryFrom}
                  setDate={setDeliveryFrom}
                  hasError={!isTimeValid}
                />
              </span>
              <Text isInline fontWeight={600} color="#A3A9AE" style={{ marginRight: "8px" }}>
                Before
              </Text>
              <TimePicker
                date={filterSettings.deliveryTo}
                setDate={setDeliveryTo}
                hasError={!isTimeValid}
              />
            </TimePickerCell>
          ) : (
            <TimeSelectorAdder />
          ))}
      </Selectors>
    </>
  );
};

export default inject(({ webhooksStore }) => {
  const { filterSettings, setDeliveryDate, setDeliveryFrom, setDeliveryTo, clearFilterDate } =
    webhooksStore;

  return { filterSettings, setDeliveryDate, setDeliveryFrom, setDeliveryTo, clearFilterDate };
})(observer(DeliveryDatePicker));
