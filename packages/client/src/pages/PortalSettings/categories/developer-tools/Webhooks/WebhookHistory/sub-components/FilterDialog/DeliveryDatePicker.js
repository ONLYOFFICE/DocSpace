import React, { useState, useEffect, useRef } from "react";
import moment from "moment";
import { inject, observer } from "mobx-react";

import styled, { css } from "styled-components";

import Text from "@docspace/components/text";
import SelectorAddButton from "@docspace/components/selector-add-button";
import SelectedItem from "@docspace/components/selected-item";

import Calendar from "@docspace/components/calendar";
import TimePicker from "@docspace/components/time-picker";
import { isMobileOnly } from "react-device-detect";

import { useTranslation } from "react-i18next";

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

const StyledCalendar = styled(Calendar)`
  position: absolute;
  ${(props) =>
    props.isMobile &&
    css`
      position: fixed;
      bottom: 0;
      left: 0;
    `}
`;

const DeliveryDatePicker = ({
  Selectors,
  filters,
  setFilters,
  isApplied,
  setIsApplied,
  isTimeOpen,
  setIsTimeOpen,
}) => {
  const [isCalendarOpen, setIsCalendarOpen] = useState(false);

  const { t } = useTranslation(["Webhooks"]);

  const calendarRef = useRef();
  const selectorRef = useRef();

  const setDeliveryDate = (date) => {
    setFilters((prevFilters) => ({ ...prevFilters, deliveryDate: date }));
  };
  const setDeliveryFrom = (date) => {
    setFilters((prevFilters) => ({ ...prevFilters, deliveryFrom: date }));
  };
  const setDeliveryTo = (date) => {
    setFilters((prevFilters) => ({ ...prevFilters, deliveryTo: date }));
  };

  const toggleCalendar = () => setIsCalendarOpen((prevIsCalendarOpen) => !prevIsCalendarOpen);
  const closeCalendar = () => {
    setIsApplied(false);
    setIsCalendarOpen(false);
  };

  const showTimePicker = () => setIsTimeOpen(true);

  const deleteSelectedDate = (e) => {
    e.stopPropagation();
    setFilters((prevFilters) => ({
      deliveryDate: null,
      deliveryFrom: moment().startOf("day"),
      deliveryTo: moment().endOf("day"),
      status: prevFilters.status,
    }));
    setIsTimeOpen(false);
    setIsApplied(false);
  };

  const handleClick = (e) => {
    !selectorRef?.current?.contains(e.target) &&
      !calendarRef?.current?.contains(e.target) &&
      setIsCalendarOpen(false);
  };

  useEffect(() => {
    document.addEventListener("click", handleClick, { capture: true });
    return () => document.removeEventListener("click", handleClick, { capture: true });
  }, []);

  const CalendarElement = () => (
    <StyledCalendar
      selectedDate={filters.deliveryDate}
      setSelectedDate={setDeliveryDate}
      onChange={closeCalendar}
      isMobile={isMobileOnly}
      forwardedRef={calendarRef}
    />
  );

  const DateSelector = () => (
    <div>
      <SelectorAddButton title={t("Add")} onClick={toggleCalendar} style={{ marginRight: "8px" }} />
      <Text isInline fontWeight={600} color="#A3A9AE">
        {t("SelectDate")}
      </Text>
      {isCalendarOpen && <CalendarElement />}
    </div>
  );

  const SelectedDate = () => (
    <SelectedItem
      onClose={deleteSelectedDate}
      text={moment(filters.deliveryDate).format("DD MMM YYYY")}
    />
  );

  const SelectedDateWithCalendar = () => (
    <div>
      <SelectedItem
        onClose={deleteSelectedDate}
        text={moment(filters.deliveryDate).format("DD MMM YYYY")}
        onClick={toggleCalendar}
      />
      {isCalendarOpen && <CalendarElement />}
    </div>
  );

  const SelectedDateTime = () => (
    <div>
      <SelectedItem
        onClose={deleteSelectedDate}
        text={
          moment(filters.deliveryDate).format("DD MMM YYYY") +
          " " +
          moment(filters.deliveryFrom).format("HH:mm") +
          " - " +
          moment(filters.deliveryTo).format("HH:mm")
        }
        onClick={toggleCalendar}
      />
      {isCalendarOpen && <CalendarElement />}
    </div>
  );

  const TimeSelectorAdder = () => (
    <TimePickerCell>
      <SelectorAddButton title={t("Add")} onClick={showTimePicker} style={{ marginRight: "8px" }} />
      <Text isInline fontWeight={600} color="#A3A9AE">
        {t("SelectDeliveryTime")}
      </Text>
    </TimePickerCell>
  );

  const isEqualDates = (firstDate, secondDate) => {
    return firstDate.format() === secondDate.format();
  };

  const isTimeEqual =
    isEqualDates(filters.deliveryFrom, filters.deliveryFrom.clone().startOf("day")) &&
    isEqualDates(filters.deliveryTo, filters.deliveryTo.clone().endOf("day"));

  const isTimeValid = filters.deliveryTo > filters.deliveryFrom;

  return (
    <>
      <Text fontWeight={600} fontSize="15px">
        {t("DeliveryDate")}
      </Text>
      <Selectors ref={selectorRef}>
        {filters.deliveryDate === null ? (
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
        {filters.deliveryDate !== null &&
          !isApplied &&
          (isTimeOpen ? (
            <TimePickerCell>
              <span className="timePickerItem">
                <Text isInline fontWeight={600} color="#A3A9AE" style={{ marginRight: "8px" }}>
                  {t("From")}
                </Text>
                <TimePicker
                  date={filters.deliveryFrom}
                  setDate={setDeliveryFrom}
                  hasError={!isTimeValid}
                  tabIndex={1}
                />
              </span>
              <Text isInline fontWeight={600} color="#A3A9AE" style={{ marginRight: "8px" }}>
                {t("Before")}
              </Text>
              <TimePicker
                date={filters.deliveryTo}
                setDate={setDeliveryTo}
                hasError={!isTimeValid}
                tabIndex={2}
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
  const {} = webhooksStore;

  return {};
})(observer(DeliveryDatePicker));
