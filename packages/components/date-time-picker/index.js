import React, { useState } from "react";
import styled from "styled-components";
import PropTypes from "prop-types";

import TimePicker from "@docspace/components/time-picker";
import DatePicker from "@docspace/components/date-picker";

import ClockIcon from "PUBLIC_DIR/images/clock.react.svg";

const Selectors = styled.div`
  position: relative;
  margin-top: 8px;
  margin-bottom: 16px;
  height: 32px;
  display: flex;
  align-items: center;

  .selectedItem {
    margin-bottom: 0;
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
`;

const TimeSelector = styled.span`
  margin-left: 4px;
  display: inline-flex;
  align-items: center;
`;

const DateTimePicker = (props) => {
  const { date, selectDateText, onChange, className, id } = props;

  const [isTimeFocused, setIsTimeFocused] = useState(false);

  const showTimePicker = () => setIsTimeFocused(true);
  const hideTimePicker = () => setIsTimeFocused(false);

  return (
    <Selectors className={className} id={id}>
      <DatePicker
        date={date}
        onChange={onChange}
        selectDateText={selectDateText}
      />
      <TimeSelector>
        {date !== null &&
          (isTimeFocused ? (
            <TimePicker
              date={date}
              setDate={onChange}
              tabIndex={1}
              onBlur={hideTimePicker}
              focusOnRender
            />
          ) : (
            <TimeCell onClick={showTimePicker}>
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
  date: PropTypes.object,
  /** Select date text */
  selectDateText: PropTypes.string,
  /** Allows to set classname */
  className: PropTypes.string,
  /** Allows to set id */
  id: PropTypes.string,
  /** Allow you to handle changing events of component */
  onChange: PropTypes.func,
};

export default DateTimePicker;
