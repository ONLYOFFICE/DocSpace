import React from "react";
import PropTypes from "prop-types";

import Day from "./day";
import { isArrayEqual } from "../../utils/array";
import { StyledDays } from "../styled-calendar";

class Days extends React.Component {
  shouldComponentUpdate(nextProps) {
    const { optionsDays, size, onDayClick } = this.props;
    if (
      isArrayEqual(optionsDays, nextProps.optionsDays) &&
      size === nextProps.size &&
      onDayClick === nextProps.onDayClick
    ) {
      return false;
    }
    return true;
  }
  render() {
    //console.log("Days render");

    const { optionsDays, size, onDayClick } = this.props;

    return (
      <StyledDays size={size}>
        {optionsDays.map((day, index) => {
          return (
            <Day size={size} key={index} day={day} onDayClick={onDayClick} />
          );
        })}
      </StyledDays>
    );
  }
}

Days.propTypes = {
  optionsDays: PropTypes.array,
  size: PropTypes.string,
  onDayClick: PropTypes.func,
};

export default Days;
