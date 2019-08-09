import React, { Component } from 'react';
import PropTypes from 'prop-types'
import styled, { css } from 'styled-components';
import DatePicker, { registerLocale } from "react-datepicker";
import ComboBox from '../combobox';

import "react-datepicker/dist/react-datepicker.css";
import enGB from 'date-fns/locale/en-GB';

registerLocale('en-GB', enGB);


const CalendarContainer = styled.div`
    width: 293px;
    margin-top 0;
    padding: 16px 16px 0px 17px;    

    border-radius: 6px;
    -moz-border-radius: 6px;
    -webkit-border-radius: 6px;
    box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -moz-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -webkit-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
`;

const DaysStyle = css`
    width: 32px;
    height: 32px;
    border-radius: 16px;
    text-align: center;
`;

const CalendarStyle = styled.div`

.react-datepicker {
    border:none !important;
}

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

.react-datepicker__day--weekend {
    color: #A3A9AE !important;
}

.react-datepicker__day--outside-month {
    color: #ECEEF1 !important;
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
    ${DaysStyle}    
    color: #333;
    &:hover {
        border-radius: 16px;
        background-color: ${props => props.color ? `${props.color} !important` : `none !important`}
        color: #fff !important;
    }
}

.react-datepicker__day--selected {
    ${DaysStyle}
    background-color: #F8F9F9;
}
`;

const DataSelector = styled.div`
    position: relative;
    display: flex;
    margin-bottom: 16px;
`;

const ComboboxStyled = styled.div`
    margin-left: 8px;
    width: 80px;
`;

class Calendar extends Component {

    constructor(props) {
        super(props);
        this.state = {
            startDate: props.startDate
        };
        const minDate = new Date(props.minDate.getFullYear(), props.minDate.getMonth(), 1);
        const maxDate = new Date(props.maxDate.getFullYear(), props.maxDate.getMonth() + 1, 0);
    }

    // need rework!!!
    month = [
        { key: "Jan", label: "January" },
        { key: "Feb", label: "February" },
        { key: "Mar", label: "March" },
        { key: "Apr", label: "April" },
        { key: "May", label: "May" },
        { key: "Jun", label: "June" },
        { key: "Jul", label: "July" },
        { key: "Aug", label: "August" },
        { key: "Sep", label: "September" },
        { key: "Oct", label: "October" },
        { key: "Nov", label: "November" },
        { key: "Dec", label: "December" }];
    /*
        options = {
            era: 'long',
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            weekday: 'long',
            timezone: 'UTC',
            hour: 'numeric',
            minute: 'numeric',
            second: 'numeric'
        };
    */

    options = {
        month: 'long'
    };

    arrayDates = function () {
        let date1 = this.props.minDate.getFullYear();
        let date2 = this.props.maxDate.getFullYear();
        const yearList = [];
        for (let i = date1; i <= date2; i++) {
            yearList.push({ key: `${i}`, label: `${i}` });
        }
        return yearList;
    }

    getCurrentDate = function () {
        let year = this.arrayDates();
        year = year.find(x => x.label == this.state.startDate.getFullYear());
        return (year.key);
    }
    //need rework too!!!
    getCurrentMonth = function () {
        let month = this.month.find(x => x.label === this.state.startDate.toLocaleString('default', { month: 'long' }));
        return (month.key);
    }

    handleChange(date) {
        this.setState({ startDate: date });
        this.props.onChange && this.props.onChange(date);
    }

    render() {
        console.log(this.props.minDate.toLocaleString("en-GB", this.options));
        console.log(this.props.maxDate.toLocaleString("en-GB", this.options));

        return (
            <CalendarContainer>
                <DataSelector>
                    <div>
                        <ComboBox isDisabled={this.props.disabled} options={this.month} selectedOption={this.getCurrentMonth()} />
                    </div>
                    <ComboboxStyled>
                        <ComboBox isDisabled={this.props.disabled} options={this.arrayDates()} selectedOption={this.getCurrentDate()} />
                    </ComboboxStyled>
                </DataSelector>

                <CalendarStyle color={this.props.themeColor}>
                    <DatePicker
                        inline
                        selected={this.state.startDate}
                        onChange={this.handleChange.bind(this)}
                        dateFormat={this.props.dateFormat}
                        disabled={this.props.disabled}
                        locale="en-GB"
                        minDate={this.minDate}
                        maxDate={this.maxDate}
                        openToDate={this.props.openToDate}
                        showDisabledMonthNavigation
                    //renderCustomHeader={({ }) => { }}
                    />
                </CalendarStyle>
            </CalendarContainer>
        );
    }
}

Calendar.propTypes = {
    onChange: PropTypes.func,
    disabled: PropTypes.bool,
    dateFormat: PropTypes.string,
    themeColor: PropTypes.string,
    startDate: PropTypes.instanceOf(Date),
    openToDate: PropTypes.instanceOf(Date),
    minDate: PropTypes.instanceOf(Date),
    maxDate: PropTypes.instanceOf(Date)
}

Calendar.defaultProps = {
    startDate: new Date(),
    openToDate: new Date(),
    minDate: new Date("1970/01/01"),
    maxDate: new Date("3000/01/01"),
    dateFormat: "dd.MM.yyyy",
    themeColor: '#ED7309',
}

export default Calendar;