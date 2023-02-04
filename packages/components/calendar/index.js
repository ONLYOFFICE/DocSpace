import React, { useEffect, useState } from "react";
import PropTypes from "prop-types";
import moment from "moment";

import { Container } from "./styled-components";
import { Days, Months, Years } from "./sub-components";

const Calendar = () => {
  moment.locale("en");
  const [selectedDate, setSelectedDate] = useState(moment());
  const [observedDate, setObservedDate] = useState(moment());
  const [selectedScene, setSelectedScene] = useState(0);

  return (
    <Container>
      {selectedScene === 0 ? (
        <Days
          observedDate={observedDate}
          setObservedDate={setObservedDate}
          setSelectedScene={setSelectedScene}
          selectedDate={selectedDate}
          setSelectedDate={setSelectedDate}
        />
      ) : selectedScene === 1 ? (
        <Months
          observedDate={observedDate}
          setObservedDate={setObservedDate}
          setSelectedScene={setSelectedScene}
          selectedDate={selectedDate}
        />
      ) : (
        <Years
          observedDate={observedDate}
          setObservedDate={setObservedDate}
          setSelectedScene={setSelectedScene}
          selectedDate={selectedDate}
        />
      )}
    </Container>
  );
};

export default Calendar;
