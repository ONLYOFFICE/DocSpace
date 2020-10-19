import React from "react";
import styled from "styled-components";
import Day from "./day";
import PropTypes from "prop-types";
import { isArrayEqual } from "../../../utils/array";

const StyledDays = styled.div`
  display: flex;
  flex-wrap: wrap;
  ${(props) => (props.size === "base" ? "width: 270px;" : "width: 294px;")}
`;

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
