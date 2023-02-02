import React, { useEffect, useState } from "react";
import PropTypes from "prop-types";
import moment from "moment";

import { Container } from "./styled-components";
import { Days, Months, Years } from "./sub-components";

const Calendar = () => {
  const [selectedDate, setSelectedDate] = useState(moment());

  return (
    <Container>
      {/* <Days selectedDate={selectedDate} setSelectedDate={setSelectedDate} /> */}
      {/* <Months selectedDate={selectedDate} setSelectedDate={setSelectedDate} /> */}
      <Years selectedDate={selectedDate} setSelectedDate={setSelectedDate} />
    </Container>
  );
};

export default Calendar;
