import React, { useRef, useState, useEffect } from "react";
import PropTypes from "prop-types";
import moment from "moment";
import styled, { css } from "styled-components";

import TextInput from "@docspace/components/text-input";
import Base from "../themes/base";

const TimeInput = styled.div`
  width: 57px;
  height: 32px;
  box-sizing: border-box;
  padding: 6px 8px;

  border: 1px solid #d0d5da;
  border-radius: 3px;

  transition: "all 0.2s ease 0s";

  display: flex;

  border-color: ${(props) => (props.hasError ? "#f21c0e" : "#d0d5da")};

  background-color: ${(props) => props.theme.input.backgroundColor};

  ${(props) =>
    props.isFocused &&
    css`
      border-color: #4781d1;
    `}

  :focus {
    border-color: #4781d1;
  }

  input {
    padding: 0;
  }

  input:last-of-type {
    text-align: end;
  }
`;

TimeInput.defaultProps = { theme: Base };

const TimePicker = ({
  initialTime,
  onChange,
  className,
  hasError,
  tabIndex,
  classNameInput,
  onBlur,
  focusOnRender,
  forwardedRef,
}) => {
  const hoursInputRef = useRef(null);
  const minutesInputRef = useRef(null);

  const [date, setDate] = useState(
    initialTime ? moment(initialTime) : moment().startOf("day")
  );

  const [isInputFocused, setIsInputFocused] = useState(false);

  const [hours, setHours] = useState(moment(date, "HH:mm").format("HH"));

  const [minutes, setMinutes] = useState(moment(date, "HH:mm").format("mm"));

  const mountRef = useRef(false);

  useEffect(() => {
    focusOnRender && hoursInputRef.current.select();
    mountRef.current = true;
  }, []);

  const changeHours = (time) => {
    setHours(time);
    setDate(
      moment(
        date.format("YYYY-MM-DD") + " " + time + ":" + minutes,
        "YYYY-MM-DD HH:mm"
      )
    );
    onChange(
      moment(
        date.format("YYYY-MM-DD") + " " + time + ":" + minutes,
        "YYYY-MM-DD HH:mm"
      )
    );
  };
  const changeMinutes = (time) => {
    setMinutes(time);
    setDate(
      moment(
        date.format("YYYY-MM-DD") + " " + hours + ":" + time,
        "YYYY-MM-DD HH:mm"
      )
    );
    onChange(
      moment(
        date.format("YYYY-MM-DD") + " " + hours + ":" + time,
        "YYYY-MM-DD HH:mm"
      )
    );
  };

  const handleChangeHours = (e) => {
    const hours = e.target.value;

    if (hours.length > 2) {
      focusMinutesInput();
      return;
    }

    if (hours === "") {
      changeHours("00");
      return;
    }
    if (!/^\d+$/.test(hours)) return;

    if (hours > 23) {
      focusMinutesInput();
      hours.length === 2 && changeHours("0" + hours[0]);
      return;
    }

    if (hours.length === 1 && hours > 2) {
      changeHours("0" + hours);
      focusMinutesInput();
      return;
    }

    hours.length === 2 && focusMinutesInput();

    changeHours(hours);
  };

  const handleChangeMinutes = (e) => {
    const minutes = e.target.value;

    if (minutes.length > 2) {
      blurMinutesInput();
      return;
    }

    if (minutes === "") {
      changeMinutes("00");
      return;
    }
    if (!/^\d+$/.test(minutes)) return;

    if (minutes > 59) {
      onBlur && onBlur();
      return;
    }

    if (minutes.length === 1 && minutes > 5) {
      changeMinutes("0" + minutes);
      blurMinutesInput();
      return;
    }
    if (minutes.length === 2) {
      blurMinutesInput();
    }

    changeMinutes(minutes);
  };

  const focusHoursInput = (e) => {
    const target = e.target;
    if (!minutesInputRef.current.contains(target))
      hoursInputRef.current.select();
  };
  const focusMinutesInput = () => {
    minutesInputRef.current.select();
  };
  const blurMinutesInput = () => {
    onBlur && onBlur();
    minutesInputRef.current.blur();
  };

  const onHoursBlur = (e) => {
    e.target.value.length === 1 && changeHours("0" + e.target.value);
    setIsInputFocused(false);
  };
  const onMinutesBlur = (e) => {
    e.target.value.length === 1 && changeMinutes("0" + e.target.value);
    setIsInputFocused(false);
  };

  const focusInput = () => setIsInputFocused(true);

  return (
    <TimeInput
      onClick={focusHoursInput}
      className={className}
      hasError={hasError}
      isFocused={isInputFocused}
      ref={forwardedRef}
    >
      <TextInput
        className={`${classNameInput}` + `-hours-input`}
        withBorder={false}
        forwardedRef={hoursInputRef}
        value={hours}
        onChange={handleChangeHours}
        onBlur={onHoursBlur}
        tabIndex={tabIndex}
        onFocus={focusInput}
      />
      :
      <TextInput
        className={`${classNameInput}` + `-minutes-input`}
        withBorder={false}
        forwardedRef={minutesInputRef}
        value={minutes}
        onChange={handleChangeMinutes}
        onClick={focusMinutesInput}
        onBlur={onMinutesBlur}
        onFocus={focusInput}
      />
    </TimeInput>
  );
};
TimePicker.propTypes = {
  /** Default time */
  initialTime: PropTypes.object,
  /** Allows to set classname */
  className: PropTypes.string,
  /** Allow you to handle changing events of component */
  onChange: PropTypes.func,
  /** Indicates error */
  hasError: PropTypes.bool,
  /** Tab index allows to make element focusable */
  hasError: PropTypes.bool,
  /** Triggers function on blur */
  onBlur: PropTypes.func,
  /** Focus input on render */
  focusOnRender: PropTypes.bool,
  /** Passes ref to child component */
  forwardedRef: PropTypes.object,
};

TimePicker.defaultProps = {
  onChange: () => {},
  className: "",
  hasError: false,
  focusOnRender: false,
};

export default TimePicker;
