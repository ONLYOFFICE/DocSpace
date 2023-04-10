import React, { useRef, useState } from "react";
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

export const TimePicker = () => {
  const hoursInputRef = useRef(null);
  const minutesInputRef = useRef(null);
  const timeInputRef = useRef(null);

  const [hours, setHours] = useState("00");

  const [minutes, setMinutes] = useState("00");

  const handleChangeHours = (e) => {
    setHours(e.target.value);
  };
  const handleChangeMinutes = (e) => {
    setMinutes(e.target.value);
    console.log(e);
  };

  const focusHoursInput = (e) => {
    const target = e.target;
    console.log(minutesInputRef);
    if (!minutesInputRef.current.contains(target)) hoursInputRef.current.focus();
  };

  const HoursInput = () => (
    <TextInput
      withBorder={false}
      forwardedRef={hoursInputRef}
      value={hours}
      onChange={handleChangeHours}
      // mask={[/[0-2]/, /\d/]}
    />
  );
  const MinutesInput = () => (
    <TextInput
      withBorder={false}
      forwardedRef={minutesInputRef}
      value={minutes}
      onChange={handleChangeMinutes}
      // mask={[/[0-6]/, /\d/]}
    />
  );

  return (
    <TimeInput ref={timeInputRef} onClick={focusHoursInput}>
      <HoursInput />:
      <MinutesInput />
    </TimeInput>
  );
};
