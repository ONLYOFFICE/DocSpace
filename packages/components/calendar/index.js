import React, { useEffect, useState } from "react";
import propTypes from "prop-types";
import moment from "moment";

import { Days, Months, Years } from "./sub-components";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

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
  onChange,
  isMobile,
  forwardedRef,
}) => {
  moment.locale(locale);

  function handleDateChange(date) {
    const formattedDate = moment(
      date.format("YYYY-MM-DD") +
        (selectedDate ? ` ${selectedDate.format("HH:mm")}` : "")
    );
    setSelectedDate(formattedDate);
    onChange(formattedDate);
  }

  const [observedDate, setObservedDate] = useState(moment());
  const [selectedScene, setSelectedScene] = useState(0);
  [minDate, maxDate] = getValidDates(minDate, maxDate);
  initialDate = moment(initialDate);

  useEffect(() => {
    const today = moment();
    if (!initialDate) {
      initialDate =
        today <= maxDate && initialDate >= minDate
          ? today
          : today.diff(minDate, "day") > today.diff(maxDate, "day")
          ? minDate.clone()
          : maxDate.clone();
      initialDate.startOf("day");
    } else if (initialDate > maxDate || initialDate < minDate) {
      initialDate =
        today <= maxDate && today >= minDate
          ? today
          : today.diff(minDate, "day") > today.diff(maxDate, "day")
          ? minDate.clone()
          : maxDate.clone();
      initialDate.startOf("day");
      console.error(
        "Initial date is out of min/max dates boundaries. Initial date will be set as closest boundary value"
      );
    }
    setObservedDate(initialDate);
  }, []);

  return (
    <ColorTheme
      themeId={ThemeType.Calendar}
      id={id}
      className={className}
      style={style}
      isMobile={isMobile}
      ref={forwardedRef}
    >
      {selectedScene === 0 ? (
        <Days
          observedDate={observedDate}
          setObservedDate={setObservedDate}
          setSelectedScene={setSelectedScene}
          selectedDate={selectedDate}
          handleDateChange={handleDateChange}
          minDate={minDate}
          maxDate={maxDate}
          isMobile={isMobile}
        />
      ) : selectedScene === 1 ? (
        <Months
          observedDate={observedDate}
          setObservedDate={setObservedDate}
          setSelectedScene={setSelectedScene}
          selectedDate={selectedDate}
          minDate={minDate}
          maxDate={maxDate}
          isMobile={isMobile}
        />
      ) : (
        <Years
          observedDate={observedDate}
          setObservedDate={setObservedDate}
          setSelectedScene={setSelectedScene}
          selectedDate={selectedDate}
          minDate={minDate}
          maxDate={maxDate}
          isMobile={isMobile}
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
  /** Specifies the calendar locale */
  locale: propTypes.string,
  /** Value of selected date (moment object)*/
  selectedDate: propTypes.object,
  /** Allows handling the changing events of the component */
  onChange: propTypes.func,
  /** Changes the selected date state */
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
  onChange: (date) => {
    console.log(date);
  },
};

export default Calendar;
