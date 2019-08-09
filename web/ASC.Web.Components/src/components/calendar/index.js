import React, { Component } from 'react';
import PropTypes from 'prop-types'
import styled, { css } from 'styled-components';
import DatePicker, { registerLocale } from "react-datepicker";
import ComboBox from '../combobox';
import "react-datepicker/dist/react-datepicker.css";
import enGB from 'date-fns/locale/en-GB';

registerLocale('en-GB', enGB);


const CalendarContainer = styled.div`
    width:325px
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

.react-datepicker__day--weekend {
    color: #A3A9AE !important;
}

.react-datepicker__day--outside-month {
    color: #ECEEF1 !important;
}

.react-datepicker__day {
    line-height: 2.5em;
    color: #333;
    ${DaysStyle}

    
    &:hover {
        border-radius: 16px;
        background-color:  #ED7309;
        color: #fff !important;
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


const DataSelector = styled.div`
    position: relative;
    display: flex;
    margin-bottom: 16px;

`;

class Calendar extends Component {

    constructor(props) {
        super(props);
        this.state = {
            startDate: props.startDate
        };

        const date = new Date();
        const firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
        const lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);
    }

    arrayDates = function () {
        var date1 = "JUN 2015";
        var date2 = "JUN 2025";
        var year1 = Number(date1.substr(4));
        var year2 = Number(date2.substr(4));
        var yearList = [];
        for (var i = year1; i <= year2; i++) {
            yearList.push({ key: `${i}`, label: `${i}` });
        }
        return yearList;
    }

    getCurrentDate = function () {
        var a = this.arrayDates();
        a = a.find(x => x.label == new Date().getFullYear());
        return (a.key);
    }

    handleChange(date) {
        this.setState({ startDate: date });
        this.props.onChange && this.props.onChange(date);
    }

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

    render() {
        //console.log('RENDER');

        return (
            <CalendarContainer>
                <DataSelector>
                    <div>
                        <ComboBox isDisabled={this.props.disabled} options={this.month} />
                    </div>
                    <div>
                        <ComboBox isDisabled={this.props.disabled} options={this.arrayDates()} selectedOption={this.getCurrentDate()} />
                    </div>

                </DataSelector>

                <CalendarStyle>
                    <DatePicker
                        inline
                        selected={this.state.startDate}
                        onChange={this.handleChange.bind(this)}
                        dateFormat={this.props.dateFormat}
                        disabled={this.props.disabled}
                        locale="en-GB"
                        minDate={this.firstDay}
                        maxDate={this.lastDay}
                        openToDate={this.props.openToDate}
                    //renderCustomHeader={({ }) => { }}
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
    themeColor: PropTypes.color,
    openToDate: PropTypes.instanceOf(Date)
}

Calendar.defaultProps = {
    startDate: new Date(),
    dateFormat: "dd.MM.yyyy",
    themeColor: '#ED7309'
}

export default Calendar;