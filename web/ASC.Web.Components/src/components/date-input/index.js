import React from 'react'
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
        <StyledTextInput type="text" {...this.props}/>
        <StyledIcon size="medium"/>
      </StyledOuter>
    )
  }
}

const DateInput = props => {
  return (
    <DatePicker
      customInput={<CustomInput/>}
      selected={props.selected}
      onChange={props.onChange}
      {...props}
    />
  );
}

export default DateInput