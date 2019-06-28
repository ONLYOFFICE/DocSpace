import React from 'react'
import styled from 'styled-components';
import TextInput from '../text-input'
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

//TODO: replace svgUrl with real icon url

const svg = `<svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
<path d="M14.4444 1H12.6667V0H11.1111V1H4.88889V0H3.33333V1H1.55556C0.696134 1 0.00780207 1.71599 0.00780207 2.60001L0 14.4C0 15.284 0.696097 16 1.55556 16H14.4444C15.3039 16 16 15.284 16 14.4V2.59997C16 1.71599 15.3039 1 14.4444 1ZM14 14H2V5H14V14Z" fill="#A3A9AE"/>
</svg>`;

const svgUrl = encodeURIComponent(svg.replace(new RegExp('"', 'g'), '\''));

const StyledTextInput = styled(TextInput)`
  background-image: url("data:image/svg+xml;utf8,${svgUrl}");
  background-position: 82px center;
  background-repeat: no-repeat;  
  padding-right: 32px;
  width: 106px;
`;

class CustomInput extends React.Component {
  render () {
    return (
      <StyledTextInput type="text" {...this.props}/>
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