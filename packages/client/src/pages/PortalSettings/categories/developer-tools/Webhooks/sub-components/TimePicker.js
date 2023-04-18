import React, { useRef, useState } from "react";
import PropTypes from "prop-types";
import moment from "moment";
import styled from "styled-components";

import { TextInput } from "@docspace/components";

const TimeInput = styled.div`
  width: 57px;
  height: 32px;
  box-sizing: border-box;
  padding: 6px 8px;

  border: 1px solid #d0d5da;
  border-radius: 3px;

  transition: "all 0.2s ease 0s";

  display: flex;

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

export const TimePicker = ({ date, setDate, onChange }) => {
  const hoursInputRef = useRef(null);
  const minutesInputRef = useRef(null);
  const timeInputRef = useRef(null);

  const [hours, setHours] = useState(moment(date, "HH:mm").format("HH"));

  const [minutes, setMinutes] = useState(moment(date, "HH:mm").format("mm"));

  const handleHoursChange = (time) => {
    setHours(time);
    setDate(moment(date.format("YYYY-MM-DD") + " " + time + ":" + minutes));
    onChange(time);
  };
  const handleMinutesChange = (time) => {
    setMinutes(time);
    setDate(moment(date.format("YYYY-MM-DD") + " " + hours + ":" + time));
    onChange(time);
  };

  const handleChangeHours = (e) => {
    const hours = e.target.value;

    if (hours === "") {
      handleHoursChange("00");
      return;
    }
    if (!/^\d+$/.test(hours)) return;

    if (hours > 23) {
      focusMinutesInput();
      hours.length === 2 && handleHoursChange("0" + hours[0]);
      return;
    }

    if (hours.length === 1 && hours > 2) {
      handleHoursChange("0" + hours);
      focusMinutesInput();
      return;
    }

    hours.length === 2 && focusMinutesInput();

    handleHoursChange(hours);
  };

  const handleChangeMinutes = (e) => {
    const minutes = e.target.value;

    if (minutes === "") {
      handleMinutesChange("00");
      return;
    }
    if (!/^\d+$/.test(minutes)) return;

    if (minutes > 59) return;

    if (minutes.length === 1 && minutes > 5) {
      handleMinutesChange("0" + minutes);
      blurMinutesInput();
      return;
    }
    minutes.length === 2 && blurMinutesInput();

    handleMinutesChange(minutes);
  };

  const focusHoursInput = (e) => {
    const target = e.target;
    if (!minutesInputRef.current.contains(target)) hoursInputRef.current.select();
  };
  const focusMinutesInput = () => {
    minutesInputRef.current.select();
  };
  const blurMinutesInput = () => {
    minutesInputRef.current.blur();
  };

  const onHoursBlur = (e) => {
    e.target.value.length === 1 && handleHoursChange("0" + e.target.value);
  };
  const onMinutesBlur = (e) => {
    e.target.value.length === 1 && handleMinutesChange("0" + e.target.value);
  };

  return (
    <TimeInput ref={timeInputRef} onClick={focusHoursInput}>
      <TextInput
        withBorder={false}
        forwardedRef={hoursInputRef}
        value={hours}
        onChange={handleChangeHours}
        onBlur={onHoursBlur}
      />
      :
      <TextInput
        withBorder={false}
        forwardedRef={minutesInputRef}
        value={minutes}
        onChange={handleChangeMinutes}
        onClick={focusMinutesInput}
        onBlur={onMinutesBlur}
      />
    </TimeInput>
  );
};

TimePicker.propTypes = {
  date: PropTypes.object,
  setDate: PropTypes.func,

  /** Allow you to handle changing events of component */
  onChange: PropTypes.func,
};

TimePicker.defaultProps = {
  date: moment(),
  setDate: () => {},
  onChange: () => {},
};
