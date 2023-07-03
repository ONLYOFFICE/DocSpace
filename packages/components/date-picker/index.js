import React, { useRef, useState, useEffect } from "react";
import moment from "moment";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";

import Text from "@docspace/components/text";
import SelectorAddButton from "@docspace/components/selector-add-button";
import SelectedItem from "@docspace/components/selected-item";
import Calendar from "@docspace/components/calendar";

import { isMobileOnly } from "react-device-detect";

import { useTranslation } from "react-i18next";

const DateSelector = styled.div`
  width: fit-content;
  cursor: pointer;

  .mr-8 {
    margin-right: 8px;
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

const DatePicker = (props) => {
  const {
    date,
    onChange,
    selectDateText,
    className,
    id,
    minDate,
    maxDate,
    locale,
  } = props;
  const { t } = useTranslation(["Webhooks"]);

  const calendarRef = useRef();
  const selectorRef = useRef();

  const [isCalendarOpen, setIsCalendarOpen] = useState(false);

  const toggleCalendar = () =>
    setIsCalendarOpen((prevIsCalendarOpen) => !prevIsCalendarOpen);

  const closeCalendar = () => {
    setIsCalendarOpen(false);
  };

  const deleteSelectedDate = (propKey, label, group, e) => {
    e.stopPropagation();
    onChange(null);
    setIsCalendarOpen(false);
  };

  const CalendarElement = () => (
    <StyledCalendar
      selectedDate={date}
      setSelectedDate={onChange}
      onChange={closeCalendar}
      isMobile={isMobileOnly}
      forwardedRef={calendarRef}
      minDate={minDate}
      maxDate={maxDate}
      locale={locale}
    />
  );

  const handleClick = (e) => {
    !selectorRef?.current?.contains(e.target) &&
      !calendarRef?.current?.contains(e.target) &&
      setIsCalendarOpen(false);
  };

  useEffect(() => {
    document.addEventListener("click", handleClick, { capture: true });
    return () =>
      document.removeEventListener("click", handleClick, { capture: true });
  }, []);

  return (
    <div className={className} id={id}>
      {date === null ? (
        <>
          <DateSelector onClick={toggleCalendar} ref={selectorRef}>
            <SelectorAddButton
              title={t("Select")}
              className="mr-8"
              iconName={"images/calendar.react.svg"}
            />
            <Text isInline fontWeight={600} color="#A3A9AE">
              {selectDateText}
            </Text>
          </DateSelector>
          {isCalendarOpen && <CalendarElement />}
        </>
      ) : (
        <SelectedItem
          className="selectedItem"
          onClose={deleteSelectedDate}
          label={date.format("DD MMM YYYY")}
        />
      )}
    </div>
  );
};

DatePicker.propTypes = {
  /** Allows to change select date text */
  selectDateText: PropTypes.string,
  /** Date object */
  date: PropTypes.object,
  /** Allow you to handle changing events of component */
  onChange: PropTypes.func,
  /** Allows to set classname */
  className: PropTypes.string,
  /** Allows to set id */
  id: PropTypes.string,
  /** Specifies min choosable calendar date */
  minDate: PropTypes.object,
  /** Specifies max choosable calendar date */
  maxDate: PropTypes.object,
  /** Specifies calendar locale */
  locale: PropTypes.string,
};

DatePicker.defaultProps = {
  selectDateText: "Select date",
};

export default DatePicker;
