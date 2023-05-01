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
  const closeCalendar = () => setIsCalendarOpen(false);

  const showTimePicker = () => setIsTimeOpen(true);

  const deleteSelectedDate = () => {
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
  return (
    <>
      <Text fontWeight={600} fontSize="15px">
        Delivery date
      </Text>
      <Selectors>
        {filterSettings.deliveryDate === null ? (
          <DateSelector />
        ) : isApplied ? (
          <SelectedDateTime />
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
                <TimePicker date={filterSettings.deliveryFrom} setDate={setDeliveryFrom} />
              </span>
              <Text isInline fontWeight={600} color="#A3A9AE" style={{ marginRight: "8px" }}>
                Before
              </Text>
              <TimePicker date={filterSettings.deliveryTo} setDate={setDeliveryTo} />
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
