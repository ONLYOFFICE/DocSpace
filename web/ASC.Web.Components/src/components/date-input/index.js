import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';
import DatePicker from "react-datepicker";
import InputBlock from '../input-block';
import "react-datepicker/dist/react-datepicker.css";

const StyledDatePicker = styled.div`
  width: 108px;
`;

const DateInput = props => {
  //console.log("DateInput render");
  const iconClick = function(){
    this.onClick();
  };

  return (
    <StyledDatePicker>
      <DatePicker
        customInput={
          <InputBlock 
            id={props.id}
            name={props.name}
            isDisabled={props.disabled}
            isReadOnly={props.readOnly}
            hasError={props.hasError}
            hasWarning={props.hasWarning}
            iconName="CalendarEmptyIcon"
            isIconFill={true}
            iconColor="#A3A9AE"
            onIconClick={iconClick}
            scale={true}
          />
        }
        {...props}
      />
    </StyledDatePicker>

  );
}

DateInput.propTypes = {
  id: PropTypes.string,
  name: PropTypes.string,
  disabled: PropTypes.bool,
  readOnly: PropTypes.bool,
  selected: PropTypes.instanceOf(Date),
  onChange: PropTypes.func,
  dateFormat: PropTypes.string,
  hasError: PropTypes.bool,
  hasWarning: PropTypes.bool
}

DateInput.defaultProps = {
  selected: null,
  dateFormat: "dd.MM.yyyy"
}

export default DateInput