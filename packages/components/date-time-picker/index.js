import React, { useState, useRef, useEffect } from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";

import TimePicker from "@docspace/components/time-picker";
import DatePicker from "@docspace/components/date-picker";

import ClockIcon from "PUBLIC_DIR/images/clock.react.svg";
import moment from "moment";

const Selectors = styled.div`
  position: relative;
  margin-top: 8px;
  margin-bottom: 16px;
  height: 32px;
  display: flex;
  align-items: center;

  .selectedItem {
    margin-bottom: 0;
    cursor: pointer;
    ${(props) =>
      props.hasError &&
      css`
        color: red;
      `}
  }
`;

const TimeCell = styled.span`
  display: flex;
  align-items: center;

  box-sizing: border-box;

  width: 73px;
  height: 32px;

  background-color: #eceef1;
  border-radius: 3px;

  padding: 6px 8px;

  cursor: pointer;

  .clockIcon {
    width: 12px;
    height: 12px;
    padding: 0 10px 0 2px;
  }

  ${(props) =>
    props.hasError &&
    css`
      color: red;
    `}
`;

const TimeSelector = styled.span`
  margin-left: 4px;
  display: inline-flex;
  align-items: center;
`;

const DateTimePicker = (props) => {
  const {
    initialDate,
    selectDateText,
    onChange,
    className,
    id,
    hasError,
    minDate,
    maxDate,
    locale,
    openDate,
  } = props;

  const [isTimeFocused, setIsTimeFocused] = useState(false);

  const [date, setDate] = useState(initialDate ? moment(initialDate) : null);

  const showTimePicker = () => setIsTimeFocused(true);
  const hideTimePicker = () => setIsTimeFocused(false);

  const handleChange = (date) => {
    onChange && onChange(date);
    setDate(date);
  };

  const timePickerRef = useRef(null);

  const handleClick = (e) => {
    !timePickerRef?.current?.contains(e.target) && setIsTimeFocused(false);
  };
  const handleKeyDown = (event) => {
    if (event.key === "Enter" || event.key === "Tab") {
      setIsTimeFocused(false);
    }
  };

  useEffect(() => {
    document.addEventListener("click", handleClick, { capture: true });
    document.addEventListener("keydown", handleKeyDown, { capture: true });
    return () => {
      document.removeEventListener("click", handleClick, { capture: true });
      document.removeEventListener("keydown", handleKeyDown, { capture: true });
    };
  }, []);

  return (
    <Selectors className={className} id={id} hasError={hasError}>
      <DatePicker
        initialDate={initialDate}
        date={date}
        onChange={handleChange}
        selectDateText={selectDateText}
        minDate={minDate}
        maxDate={maxDate}
        locale={locale}
        openDate={openDate}
        outerDate={date}
      />
      <TimeSelector>
        {date !== null &&
          (isTimeFocused ? (
            <TimePicker
              initialTime={date}
              onChange={handleChange}
              tabIndex={1}
              onBlur={hideTimePicker}
              focusOnRender
              forwardedRef={timePickerRef}
            />
          ) : (
            <TimeCell onClick={showTimePicker} hasError={hasError}>
              <ClockIcon className="clockIcon" />
              {date.format("HH:mm")}
            </TimeCell>
          ))}
      </TimeSelector>
    </Selectors>
  );
};

DateTimePicker.propTypes = {
  /** Date object */
  initialDate: PropTypes.oneOfType([
    PropTypes.object,
    PropTypes.string,
    PropTypes.number,
  ]),
  /** Select date text */
  selectDateText: PropTypes.string,
  /** Allows to set classname */
  className: PropTypes.string,
  /** Allows to set id */
  id: PropTypes.string,
  /** Allow you to handle changing events of component */
  onChange: PropTypes.func,
  /** Specifies min choosable calendar date */
  minDate: PropTypes.oneOfType([
    PropTypes.object,
    PropTypes.string,
    PropTypes.number,
  ]),
  /** Specifies max choosable calendar date */
  maxDate: PropTypes.oneOfType([
    PropTypes.object,
    PropTypes.string,
    PropTypes.number,
  ]),
  /** Specifies calendar locale */
  locale: PropTypes.string,
  /** Indicates the input field has an error  */
  hasError: PropTypes.bool,
  /** Allows to set first shown date in calendar */
  openDate: PropTypes.oneOfType([
    PropTypes.object,
    PropTypes.string,
    PropTypes.number,
  ]),
};

export default DateTimePicker;
