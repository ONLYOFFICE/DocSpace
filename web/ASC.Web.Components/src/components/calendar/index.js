import React, { Component } from 'react';
import PropTypes from 'prop-types'
import styled, { css } from 'styled-components';
import DatePicker, { registerLocale } from "react-datepicker";
import ComboBox from '../combobox';
import "react-datepicker/dist/react-datepicker.css";
import enGB from 'date-fns/locale/en-GB';
import calendar, { isDate, isSameDay, isSameMonth, getDateISO, getNextMonth, getPreviousMonth, WEEK_DAYS, CALENDAR_MONTHS } from "./helper-calendar";



const DataSelector = styled.div`
    position: relative;
    display: inline-block;

`;

const CalendarContainer = styled.div`
    width:500px;
    height:500px;
`;

const DaysStyle = css`
    width: 32px;
    height: 32px;
    border-radius: 16px;
    text-align: center;
`;


const CalendarStyle = styled.div`

.react-datepicker__month-container {
    font-size: 13px;
    font-family: Open Sans;
    font-style: normal;
    font-weight: bold;
    text-align: center;
}

.react-datepicker__header {
    background-color: #fff;
    border: none;
}

.react-datepicker__month {
    margin-top: 7px;
}

.react-datepicker__week {

}

.react-datepicker__day-name {
    width: 32px;
}

.react-datepicker__day-names {
    display: block;
    font-size: 13px;
}

.react-datepicker__day {
    line-height: 2.5em;
    color: #333;
    ${DaysStyle}

    
    &:hover {
        border-radius: 16px;

        background-color:  #ED7309;
        color: #fff;
    }
}

.react-datepicker__day--selected {
    ${DaysStyle}
    background-color: #F8F9F9;
}

react-datepicker__time-name {
    text-align: center;
}
`;

registerLocale('en-GB', enGB);

class Calendar extends Component {

    constructor(props) {
        super(props);
        this.state = {
            startDate: props.startDate
        };
    }

    options = [
        { key: 0, label: '25 per page' },
        { key: 1, label: '50 per page', },
        { key: 2, label: '100 per page' }];

    handleChange(date) {
        this.setState({ startDate: date });
        this.props.onChange && this.props.onChange(date);
    }



    render() {
        //console.log('RENDER');
        
        return (
            <CalendarContainer>
                <DataSelector>
                    <ComboBox isDisabled={this.props.disabled} options={this.options} />
                    <ComboBox isDisabled={this.props.disabled} options={this.options} />
                </DataSelector>

                <CalendarStyle>

                    <DatePicker
                        inline
                        selected={this.state.startDate}
                        onChange={this.handleChange.bind(this)}
                        dateFormat={this.props.dateFormat}
                        disabled={this.props.disabled}
                        locale="en-GB"
                        renderCustomHeader={() => { }}
                    />

                </CalendarStyle>

            </CalendarContainer>
        );
    }
}



Calendar.propTypes = {
    disabled: PropTypes.bool,
    startDate: PropTypes.instanceOf(Date),
    dateFormat: PropTypes.string,
    themeColor: PropTypes.color
}

Calendar.defaultProps = {
    startDate: new Date(),
    dateFormat: "dd.MM.yyyy",
    themeColor: '#ED7309'
}

export default Calendar;

/*
const DataSelector2 = styled.div`
    background: #eee;
    border: 1px solid #ccc;
    padding: 10px;
    float: left;
    width: 23.5%;
    margin-right: 2%;
    -webkit-box-sizing: border-box;
    -moz-box-sizing: border-box;
    box-sizing: border-box;
`;
*/