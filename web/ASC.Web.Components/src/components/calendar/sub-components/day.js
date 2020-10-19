import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import Text from "../../text";
import isEqual from "lodash/isEqual";

const StyledDay = styled.div`
  display: flex;
  flex-basis: 14.2857%; /*(1/7*100%)*/
  text-align: center;
  line-height: 33px;
  user-select: none;
  ${(props) =>
    props.size === "base" ? "margin-top: 3px;" : "margin-top: 7.5px;"}
`;

const DayContent = styled.div`
  width: 32px;
  height: 32px;
  .textStyle {
    text-align: center;
  }
`;

class Day extends React.Component {
  shouldComponentUpdate(nextProps) {
    const { day, size, onDayClick } = this.props;
    if (
      isEqual(day, nextProps.day) &&
      size === nextProps.size &&
      onDayClick === nextProps.onDayClick
    ) {
      return false;
    }
    return true;
  }

  render() {
    //console.log("Day render");
    const { day, size, onDayClick } = this.props;

    return (
      <StyledDay size={size} className={`calendar_day ${day.disableClass}`}>
        <DayContent
          onClick={onDayClick.bind(this, day)}
          className={day.className}
        >
          <Text isBold={true} color="inherit;" className="textStyle">
            {day.value}
          </Text>
        </DayContent>
      </StyledDay>
    );
  }
}

Day.propTypes = {
  day: PropTypes.object,
  size: PropTypes.string,
  onDayClick: PropTypes.func,
};

export default Day;
