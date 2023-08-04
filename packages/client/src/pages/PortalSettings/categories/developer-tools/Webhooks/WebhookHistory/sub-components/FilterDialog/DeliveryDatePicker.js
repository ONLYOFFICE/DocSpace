import React, { useState, useEffect, useRef } from "react";
import styled from "styled-components";
import moment from "moment";

import Text from "@docspace/components/text";
import { useTranslation } from "react-i18next";
import DatePicker from "@docspace/components/date-picker";
import Calendar from "@docspace/components/calendar";
import TimePicker from "@docspace/components/time-picker";
import SelectorAddButton from "@docspace/components/selector-add-button";
import SelectedItem from "@docspace/components/selected-item";

import { isMobileOnly } from "react-device-detect";

const Selectors = styled.div`
  position: relative;
  margin-top: 8px;
  margin-bottom: 16px;
  height: 32px;
  display: flex;
  align-items: center;

  .mr-8 {
    margin-right: 8px;
  }

  .selectedItem {
    margin-bottom: 0;
  }
`;

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

const DeliveryDatePicker = ({ filters, setFilters, isApplied, setIsApplied }) => {
  const { t } = useTranslation(["Webhooks"]);

  const calendarRef = useRef();
  const selectorRef = useRef();
  const [isCalendarOpen, setIsCalendarOpen] = useState(false);
  const [isTimeOpen, setIsTimeOpen] = useState(false);

  const deleteSelectedDate = (propKey, label, group, e) => {
    e.stopPropagation();
    setIsApplied(false);
    setFilters((prevFilters) => ({
      ...prevFilters,
      deliveryDate: null,
      deliveryFrom: moment().startOf("day"),
      deliveryTo: moment().endOf("day"),
    }));
    setIsTimeOpen(false);
    setIsCalendarOpen(false);
  };

  const setDeliveryFrom = (date) => {
    setFilters((prevfilters) => ({ ...prevfilters, deliveryFrom: date }));
  };
  const setDeliveryTo = (date) => {
    setFilters((prevfilters) => ({ ...prevfilters, deliveryTo: date }));
  };
  const onDateSet = (date) => {
    setIsApplied(false);
    setIsTimeOpen(false);
    setIsCalendarOpen(false);
    setFilters((prevFilters) => ({
      ...prevFilters,
      deliveryDate: date,
      deliveryFrom: moment().startOf("day"),
      deliveryTo: moment().endOf("day"),
    }));
  };

  const toggleCalendar = () => setIsCalendarOpen((prevIsCalendarOpen) => !prevIsCalendarOpen);

  const closeCalendar = () => {
    setIsApplied(false);
    setIsTimeOpen(false);
    setIsCalendarOpen(false);
  };

  const showTimePicker = () => setIsTimeOpen(true);

  const CalendarElement = () => (
    <StyledCalendar
      selectedDate={filters.deliveryDate}
      setSelectedDate={onDateSet}
      onChange={closeCalendar}
      isMobile={isMobileOnly}
      forwardedRef={calendarRef}
    />
  );

  const SelectedDateTime = () => {
    const formattedTime = isTimeEqual
      ? ""
      : ` ${filters.deliveryFrom.format("HH:mm")} - ${moment(filters.deliveryTo).format("HH:mm")}`;

    return (
      <div>
        <SelectedItem
          className="selectedItem delete-delivery-date-button"
          onClose={deleteSelectedDate}
          label={filters.deliveryDate.format("DD MMM YYYY") + formattedTime}
          onClick={toggleCalendar}
        />
        {isCalendarOpen && <CalendarElement />}
      </div>
    );
  };

  const handleClick = (e) => {
    !selectorRef?.current?.contains(e.target) &&
      !calendarRef?.current?.contains(e.target) &&
      setIsCalendarOpen(false);
  };
  const isEqualDates = (firstDate, secondDate) => {
    return firstDate.format("YYYY-MM-D HH:mm") === secondDate.format("YYYY-MM-D HH:mm");
  };

  const isTimeEqual =
    isEqualDates(filters.deliveryFrom, filters.deliveryFrom.clone().startOf("day")) &&
    isEqualDates(filters.deliveryTo, filters.deliveryTo.clone().endOf("day"));

  const isTimeValid = filters.deliveryTo > filters.deliveryFrom;

  useEffect(() => {
    document.addEventListener("click", handleClick, { capture: true });
    return () => document.removeEventListener("click", handleClick, { capture: true });
  }, []);

  console.log(filters.deliveryDate, "delivery date picker");

  return (
    <>
      <Text fontWeight={600} fontSize="15px">
        {t("DeliveryDate")}
      </Text>
      <Selectors ref={selectorRef}>
        {isApplied && filters.deliveryDate !== null ? (
          <SelectedDateTime />
        ) : (
          <DatePicker
            outerDate={filters.deliveryDate}
            onChange={onDateSet}
            selectedDateText={t("SelectDate")}
            showCalendarIcon={false}
          />
        )}
        {filters.deliveryDate !== null &&
          !isApplied &&
          (isTimeOpen ? (
            <TimePickerCell>
              <span className="timePickerItem">
                <Text isInline fontWeight={600} color="#A3A9AE" className="mr-8">
                  {t("From")}
                </Text>
                <TimePicker
                  classNameInput="from-time"
                  onChange={setDeliveryFrom}
                  hasError={!isTimeValid}
                  tabIndex={1}
                />
              </span>

              <Text isInline fontWeight={600} color="#A3A9AE" className="mr-8">
                {t("Before")}
              </Text>
              <TimePicker
                classNameInput="before-time"
                date={filters.deliveryTo}
                setDate={setDeliveryTo}
                hasError={!isTimeValid}
                tabIndex={2}
              />
            </TimePickerCell>
          ) : (
            <TimePickerCell>
              <SelectorAddButton
                title={t("Add")}
                onClick={showTimePicker}
                className="mr-8 add-delivery-time-button"
              />
              <Text isInline fontWeight={600} color="#A3A9AE">
                {t("SelectDeliveryTime")}
              </Text>
            </TimePickerCell>
          ))}
      </Selectors>
    </>
  );
};

export default DeliveryDatePicker;
