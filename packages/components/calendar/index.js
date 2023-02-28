import React, { useEffect, useState } from "react";
import propTypes from "prop-types";
import moment from "moment";

import { Days, Months, Years } from "./sub-components";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

import { getValidDates } from "./utils";

const Calendar = ({
  locale,
  selectedDate,
  setSelectedDate,
  minDate,
  maxDate,
  id,
  className,
  style,
  initialDate,
}) => {
  moment.locale(locale);

  const [observedDate, setObservedDate] = useState(moment());
  const [selectedScene, setSelectedScene] = useState(0);
  [minDate, maxDate] = getValidDates(minDate, maxDate);
  initialDate = moment(initialDate);

  useEffect(() => {
    if (!initialDate || initialDate > maxDate || initialDate < minDate) {
      console.log(
        "initial date is not good: ",
        moment()
          .seconds((minDate.seconds() + maxDate.seconds()) / 2)
          .format("YYYY-MM-DD")
      );
    } else {
      setSelectedDate(initialDate);
      setObservedDate(initialDate);
      console.log("initial date is ok", selectedDate.format("YYYY-MM-DD"));
    }
  }, []);

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
  /** Value of selected date (moment object)*/
  selectedDate: propTypes.object,
  /** Allow you to handle changing events of component */
  setSelectedDate: propTypes.func,
  /** Specifies the minimum selectable date */
  minDate: propTypes.object,
  /** Specifies the maximum selectable date */
  maxDate: propTypes.object,
  /** Accepts css style */
  style: propTypes.oneOfType([propTypes.object, propTypes.array]),
  /** First shown date */
  initialDate: propTypes.object,
};

Calendar.defaultProps = {
  locale: "en",
  minDate: new Date("1970/01/01"),
  maxDate: new Date("2040/01/01"),
  id: "",
  className: "",
  initialDate: new Date(),
};

export default Calendar;
