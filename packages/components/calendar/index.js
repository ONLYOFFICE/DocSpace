import React, { useState } from "react";
import propTypes from "prop-types";
import moment from "moment";

import { Days, Months, Years } from "./sub-components";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

const Calendar = ({
  locale,
  selectedDate,
  onChange,
  minDate,
  maxDate,
  id,
  className,
  style,
}) => {
  moment.locale(locale);
  const setSelectedDate = onChange;
  const [observedDate, setObservedDate] = useState(moment());
  const [selectedScene, setSelectedScene] = useState(0);
  minDate = moment(minDate);
  maxDate = moment(maxDate);

  return (
    <ColorTheme
      themeId={ThemeType.Calendar}
      id={id}
      className={className}
      style={style}
    >
      {selectedScene === 0 ? (
        <Days
          observedDate={observedDate}
          setObservedDate={setObservedDate}
          setSelectedScene={setSelectedScene}
          selectedDate={selectedDate}
          setSelectedDate={setSelectedDate}
          minDate={minDate}
          maxDate={maxDate}
        />
      ) : selectedScene === 1 ? (
        <Months
          observedDate={observedDate}
          setObservedDate={setObservedDate}
          setSelectedScene={setSelectedScene}
          selectedDate={selectedDate}
          minDate={minDate}
          maxDate={maxDate}
        />
      ) : (
        <Years
          observedDate={observedDate}
          setObservedDate={setObservedDate}
          setSelectedScene={setSelectedScene}
          selectedDate={selectedDate}
          minDate={minDate}
          maxDate={maxDate}
        />
      )}
    </ColorTheme>
  );
};

Calendar.propTypes = {
  /** Class name */
  className: propTypes.string,
  /** Used as HTML `id` property  */
  id: propTypes.string,
  /** Specifies the locale of calendar */
  locale: propTypes.string,
  /** Value of selected date */
  selectedDate: propTypes.object,
  /** Allow you to handle changing events of component */
  onChange: propTypes.func,
  /** Specifies the minimum selectable date */
  minDate: propTypes.object,
  /** Specifies the maximum selectable date */
  maxDate: propTypes.object,
  /** Accepts css style */
  style: propTypes.oneOfType([propTypes.object, propTypes.array]),
};

Calendar.defaultProps = {
  locale: "en",
  minDate: new Date("1970/01/01"),
  maxDate: new Date("2040/01/01"),
  id: "",
  className: "",
};

export default Calendar;
