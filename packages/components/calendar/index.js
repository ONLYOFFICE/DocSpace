import React, { useState } from "react";
import PropTypes from "prop-types";
import moment from "moment";

import { Container } from "./styled-components";
import { Days, Months, Years } from "./sub-components";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

const Calendar = () => {
  moment.locale("en");
  const [selectedDate, setSelectedDate] = useState(moment());
  const [observedDate, setObservedDate] = useState(moment());
  const [selectedScene, setSelectedScene] = useState(0);
  const minDate = moment("2020-12-31");
  const maxDate = moment("2030-12-1");

  return (
    <ColorTheme themeId={ThemeType.Calendar}>
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

export default Calendar;
