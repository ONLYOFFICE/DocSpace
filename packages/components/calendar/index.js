import React from "react";
import PropTypes from "prop-types";
import {
  Container,
} from "./styled-components";

import { Header, DaysBody } from "./sub-components";

const Calendar = () => {

  return (
    <Container>
      <Header/>
      <DaysBody/>
    </Container>
  );
};

export default Calendar;
