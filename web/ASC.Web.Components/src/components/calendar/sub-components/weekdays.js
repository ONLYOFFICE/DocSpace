import React from "react";
import styled from "styled-components";
import Weekday from "./weekday";
import Text from "../../text";
import PropTypes from "prop-types";
import { isArrayEqual } from "../../../utils/array";

const StyledWeekdays = styled.div`
  width: ${(props) => (props.size === "base" ? "272px" : "295px")};
  display: flex;
  margin-bottom: -5px;

  .dayText {
    width: 32px;
    height: 32px;
    text-align: center;
  }
`;

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
            <Weekday key={index}>
              <Text color={weekday.color} isBold={true} className={"dayText"}>
                {" "}
                {weekday.value}{" "}
              </Text>
            </Weekday>
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
