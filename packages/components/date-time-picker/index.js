import React, { useRef, useState, useEffect } from "react";
import moment from "moment";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";

import Text from "@docspace/components/text";
import SelectorAddButton from "@docspace/components/selector-add-button";
import SelectedItem from "@docspace/components/selected-item";
import Calendar from "@docspace/components/calendar";
import TimePicker from "@docspace/components/time-picker";

import { isMobileOnly } from "react-device-detect";

import { useTranslation } from "react-i18next";

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

const DateTimePicker = (props) => {
  const {
    initialDate,
    initialTimeFrom,
    initialTimeTo,
    onChange,
    isApplied,
    setIsApplied,
    selectDateText,
    fromText,
    beforeText,
    selectTimeText,
    isLimit,
    className,
    id,
  } = props;
  const { t } = useTranslation(["Webhooks"]);

  const calendarRef = useRef();
  const selectorRef = useRef();
  const [isCalendarOpen, setIsCalendarOpen] = useState(false);
  const [isTimeOpen, setIsTimeOpen] = useState(false);

  const [dateObj, setDateObj] = useState({
    date: initialDate || null,
    timeFrom: initialTimeFrom || moment().startOf("day"),
    timeTo: initialTimeTo || moment().endOf("day"),
  });

  const deleteSelectedDate = (propKey, label, group, e) => {
    e.stopPropagation();
    setDateObj({
      date: null,
      timeFrom: moment().startOf("day"),
      timeTo: moment().endOf("day"),
    });
    setIsTimeOpen(false);
    setIsCalendarOpen(false);
    setIsApplied && setIsApplied(false);
  };

  const setDate = (date) => {
    setDateObj((prevDateObj) => ({ ...prevDateObj, date }));
  };
  const setTimeFrom = (date) => {
    setDateObj((prevDateObj) => ({ ...prevDateObj, timeFrom: date }));
  };
  const setTimeTo = (date) => {
    setDateObj((prevDateObj) => ({ ...prevDateObj, timeTo: date }));
  };

  const toggleCalendar = () =>
    setIsCalendarOpen((prevIsCalendarOpen) => !prevIsCalendarOpen);

  const closeCalendar = () => {
    setIsApplied && setIsApplied(false);
    setIsTimeOpen(false);
    setIsCalendarOpen(false);
  };

  const showTimePicker = () => setIsTimeOpen(true);

  const CalendarElement = () => (
    <StyledCalendar
      selectedDate={dateObj.date}
      setSelectedDate={setDate}
      onChange={closeCalendar}
      isMobile={isMobileOnly}
      forwardedRef={calendarRef}
    />
  );

  const DateSelector = () => (
    <div>
      <SelectorAddButton
        title={t("Add")}
        onClick={toggleCalendar}
        className="mr-8"
      />
      <Text isInline fontWeight={600} color="#A3A9AE">
        {selectDateText}
      </Text>
      {isCalendarOpen && <CalendarElement />}
    </div>
  );

  const SelectedDateTime = () => {
    const formattedTime = isTimeEqual
      ? ""
      : isLimit
      ? ` ${moment(dateObj.timeTo).format("HH:mm")}`
      : ` ${dateObj.timeFrom.format("HH:mm")} - ${moment(dateObj.timeTo).format(
          "HH:mm"
        )}`;

    return (
      <div>
        <SelectedItem
          className="selectedItem"
          onClose={deleteSelectedDate}
          label={dateObj.date.format("DD MMM YYYY") + formattedTime}
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
    return firstDate.format() === secondDate.format();
  };

  const isTimeEqual =
    isEqualDates(dateObj.timeFrom, dateObj.timeFrom.clone().startOf("day")) &&
    isEqualDates(dateObj.timeTo, dateObj.timeTo.clone().endOf("day"));

  const isTimeValid = dateObj.timeTo > dateObj.timeFrom;

  useEffect(() => {
    document.addEventListener("click", handleClick, { capture: true });
    return () =>
      document.removeEventListener("click", handleClick, { capture: true });
  }, []);

  useEffect(() => {
    onChange(dateObj);
  }, [dateObj]);

  useEffect(() => {
    initialDate !== undefined &&
      initialTimeTo &&
      setDateObj((prevDateObj) => ({
        date: initialDate,
        timeFrom: initialTimeFrom || prevDateObj.timeFrom,
        timeTo: initialTimeTo,
      }));
  }, [initialDate, initialTimeFrom, initialTimeTo]);

  return (
    <Selectors ref={selectorRef} className={className} id={id}>
      {dateObj.date === null ? (
        <DateSelector />
      ) : isApplied ? (
        <SelectedDateTime />
      ) : (
        <SelectedItem
          className="selectedItem"
          onClose={deleteSelectedDate}
          label={dateObj.date.format("DD MMM YYYY")}
        />
      )}
      {dateObj.date !== null &&
        !isApplied &&
        (isTimeOpen ? (
          <TimePickerCell>
            {!isLimit && (
              <span className="timePickerItem">
                <Text
                  isInline
                  fontWeight={600}
                  color="#A3A9AE"
                  className="mr-8"
                >
                  {fromText}
                </Text>
                <TimePicker
                  date={dateObj.timeFrom}
                  setDate={setTimeFrom}
                  hasError={!isTimeValid}
                  tabIndex={1}
                />
              </span>
            )}

            <Text isInline fontWeight={600} color="#A3A9AE" className="mr-8">
              {beforeText}
            </Text>
            <TimePicker
              date={dateObj.timeTo}
              setDate={setTimeTo}
              hasError={!isTimeValid}
              tabIndex={2}
            />
          </TimePickerCell>
        ) : (
          <TimePickerCell>
            <SelectorAddButton
              title={t("Add")}
              onClick={showTimePicker}
              className="mr-8"
            />
            <Text isInline fontWeight={600} color="#A3A9AE">
              {selectTimeText}
            </Text>
          </TimePickerCell>
        ))}
    </Selectors>
  );
};

DateTimePicker.propTypes = {
  /** Inital date */
  initialDate: PropTypes.object,
  /** Inital time from */
  initialTimeFrom: PropTypes.object,
  /** Inital time to */
  initialTimeTo: PropTypes.object,
  /** From text */
  fromText: PropTypes.string,
  /** Before text */
  beforeText: PropTypes.string,
  /** Select date text */
  selectDateText: PropTypes.string,
  /** Select time text */
  selectTimeText: PropTypes.string,
  /** Allows to set classname */
  className: PropTypes.string,
  /** Allows to set id */
  id: PropTypes.string,
  /** Allow you to handle changing events of component */
  onChange: PropTypes.func,
  /** Combines the limit into one date block */
  isApplied: PropTypes.bool,
  /** Allows to set isApplied status */
  setIsApplied: PropTypes.func,
  /** Hides before selector */
  isLimit: PropTypes.bool,
};

DateTimePicker.defaultProps = {
  fromText: "From",
  beforeText: "Before",
  selectDateText: "Select date",
  selectTimeText: "Select time",
  className: "",
  isLimit: false,
  onChange: () => {},
};

export default DateTimePicker;
