import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';
import TextInput from '../text-input'
import DatePicker from "react-datepicker";
import { Icons } from '../icons'
import "react-datepicker/dist/react-datepicker.css";

const StyledOuter = styled.div`
  position: relative;
  display: inline-block;
`;

const StyledTextInput = styled(TextInput)`
  padding-right: 32px;
  width: 106px;
`;

const StyledIcon = styled(Icons.CalendarEmptyIcon)`
  position: absolute;
  top: 6px;
  right: 6px;
`;

class CustomInput extends React.Component {
  render () {
    return (
      <StyledOuter onClick={this.props.onClick}>
        <StyledTextInput type="text" autoComplete="off" maxLength={10} {...this.props}/>
        <StyledIcon size="medium"/>
      </StyledOuter>
    )
  }
}

const DateInput = props => {
  return (
    <DatePicker
      customInput={
        <CustomInput
          id={props.id}
          name={props.name}
          isDisabled={props.disabled}
          isReadOnly={props.readOnly}
          hasError={props.hasError}
          hasWarning={props.hasWarning}
        />
      }
      {...props}
    />
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