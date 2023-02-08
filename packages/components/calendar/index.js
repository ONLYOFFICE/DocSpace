import React, { useState } from "react";
import PropTypes from "prop-types";
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
}) => {
  moment.locale(locale);
  const setSelectedDate = onChange;
  const [observedDate, setObservedDate] = useState(moment());
  const [selectedScene, setSelectedScene] = useState(0);
  minDate = moment(minDate);
  maxDate = moment(maxDate);

  return (
    <ColorTheme themeId={ThemeType.Calendar} id={id} className={className}>
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

Calendar.PropTypes = {
  /** Class name */
  className: PropTypes.string,
  /** Used as HTML `id` property  */
  id: PropTypes.string,
  /** Specifies the locale of calendar */
  locale: PropTypes.string,
  /** Value of selected date */
  selectedDate: PropTypes.object,
  /** Allow you to handle changing events of component */
  onChange: PropTypes.func,
  /** Specifies the minimum selectable date */
  minDate: PropTypes.object,
  /** Specifies the maximum selectable date */
  maxDate: PropTypes.object,
};

Calendar.defaultProps = {
  locale: "en",
  minDate: new Date("1970/01/01"),
  maxDate: new Date("2040/01/01"),
  id: "",
  className: "",
};

export default Calendar;
