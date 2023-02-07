import React from "react";
import PropTypes from "prop-types";

import Text from "../../text";
import { isArrayEqual } from "../../utils/array";
import { StyledWeekdays, StyledWeekday } from "../styled-calendar";

class Weekdays extends React.Component {
  shouldComponentUpdate(nextProps) {
    const { optionsWeekdays, size } = this.props;
    if (
      isArrayEqual(optionsWeekdays, nextProps.optionsWeekdays) &&
      size === nextProps.size
    ) {
      return false;
    }
    return true;
  }
  render() {
    //console.log("Weekdays render");
    const { optionsWeekdays, size } = this.props;

    return (
      <StyledWeekdays size={size}>
        {optionsWeekdays.map((weekday, index) => {
          return (
            <StyledWeekday key={index} disable={weekday.disabled}>
              <Text isBold={true} className={"calendar-weekday_text"}>
                {" "}
                {weekday.value}{" "}
              </Text>
            </StyledWeekday>
          );
        })}
      </StyledWeekdays>
    );
  }
}

Weekdays.propTypes = {
  optionsWeekdays: PropTypes.array,
  size: PropTypes.string,
};

export default Weekdays;
