import React, { Component } from 'react';
import PropTypes from 'prop-types'
import styled, { css } from 'styled-components';
import ComboBox from '../combobox';
import moment from 'moment/min/moment-with-locales';

const HoverStyle = css`
    &:hover {
        background-color: #ECEEF1;    
        border-radius: 16px;
        cursor: pointer;
    }
`;

const ComboBoxStyle = styled.div`
    position: relative;
    display: flex;
    padding-bottom: 16px;
`;

const ComboBoxDateStyle = styled.div`
    min-width: 80px;
    height: 32px;
    margin-left: 8px;
`;

const CalendarStyle = styled.div`
    min-width: 280px;
    max-width: 293px;
    border-radius: 6px;
    -moz-border-radius: 6px;
    -webkit-border-radius: 6px;
    box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -moz-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -webkit-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    padding: 16px 16px 16px 17px;
    box-sizing: content-box;
    font-family: Open Sans;
    font-style: normal;
    font-weight: bold;
    font-size: 13px;
    text-align: center;
    /*cursor: default;*/

    .calendar-month {
        ${HoverStyle}
    }

    .calendar-month_neighboringMonth {
        color: #ECEEF1;
        pointer-events: none;
    }

    .calendar-month_weekend {
        color: #A3A9AE;
        ${HoverStyle}
    }

    .calendar-month_weekdays_weekend {
        color: #A3A9AE;
    }

    .calendar-month_selected-day {


        background-color: ${props => props.color};
        border-radius: 16px;
        cursor: pointer;
        color: #fff;

    }
`;

const Weekday = styled.div`
    overflow: hidden;
    /*flex-basis: 100%;*/
    flex-basis: 14.2857%;
    /*min-height: 32px;*/
    /*min-width: 32px;*/
`;

const Weekdays = styled.div`
    display: flex;
    margin-bottom: 15px;
`;

const Month = styled.div`
    width: 100%;
`;

const Days = styled.div`
    display: flex;
    flex-wrap: wrap;
`;

const Day = styled.div`
    display: flex;
    flex-basis: 14.2857%;
    padding: 4px;
    text-align: center;
    line-height: 2.5em;
    /*min-height: 32px;*/
    /*min-width: 32px;*/
`;

const AbbrDay = styled.abbr`
    width: 32px;
    height: 32px;
`;

class Calendar extends Component {
    constructor(props) {
        super(props);
    }

    state = {
        months: moment.months(),
        openToDate: this.props.openToDate,
        selectedDate: this.props.selectedDate,

    };

    selectedYear = (value) => {
        this.setState({ openToDate: new Date(value.key, this.state.openToDate.getMonth()) });
    }

    selectedMonth = (value) => {
        this.setState({ openToDate: new Date(this.state.openToDate.getFullYear(), value.key) });
    }

    getListMonth = (date1, date2) => {
        const monthList = new Array();
        for (let i = date1; i <= date2; i++) {
            monthList.push({ key: `${i}`, label: `${this.state.months[i]}` });
        }
        return monthList;
    }

    getArrayMonth = () => {
        let date1 = this.props.minDate.getMonth();
        let date2 = this.props.maxDate.getMonth();
        let monthList = new Array();
        if (this.props.minDate.getFullYear() !== this.props.maxDate.getFullYear()) {
            monthList = this.getListMonth(0, 11);
        } else { monthList = this.getListMonth(date1, date2); }
        return monthList;
    }

    getCurrentMonth = () => {
        let month = this.getArrayMonth();
        let selectedMonth = month.find(x => x.key == this.state.openToDate.getMonth());
        return (selectedMonth);
    }

    getArrayYears = () => {
        let date1 = this.props.minDate.getFullYear();
        let date2 = this.props.maxDate.getFullYear();
        const yearList = [];
        for (let i = date1; i <= date2; i++) {
            let newDate = new Date(i, 0, 1);
            yearList.push({ key: `${i}`, label: `${moment(newDate).format('YYYY')}` });
        }
        return yearList;
    }

    getCurrentYear = () => {
        let year = this.getArrayYears();
        year = year.find(x => x.key == this.state.openToDate.getFullYear());
        return (year);
    }

    selectedYear = (value) => {
        this.setState({ openToDate: new Date(value.key, this.state.openToDate.getMonth()) });
    }

    selectedMonth = (value) => {
        this.setState({ openToDate: new Date(this.state.openToDate.getFullYear(), value.key) });
    }

    formatSelectedDate = (date) => {
        return (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear();
    }

    onDayClick = (day) => {
        let year = this.state.openToDate.getFullYear();
        let month = this.state.openToDate.getMonth() + 1;
        let date = new Date(month + "/" + day + "/" + year);
        if (this.formatSelectedDate(date) != this.formatSelectedDate(this.state.selectedDate)) {
            this.setState({ selectedDate: date })
            this.props.onChange && this.props.onChange(date);
        }
    }




    formatDate = (date) => {
        return (date.getMonth() + 1) + "/" + 1 + "/" + date.getFullYear();
    }

    compareDays = () => {
        var date1 = this.formatDate(this.state.openToDate);
        var date2 = this.formatDate(this.state.selectedDate);
        return (date1 === date2) ? true : false;
    }

    firstDayOfMonth = () => {
        const selectedDate = this.state.openToDate;
        const firstDay = moment(selectedDate).locale("en").startOf("month").format("d");
        return firstDay;
    };

    getWeekDays = () => {
        let arrayWeekDays = [];
        const weekdays = moment.weekdaysMin(true);
        for (let i = 0; i < weekdays.length; i++) {
            let className = "";
            (i === 6 || i === 5) ? className = "calendar-month_weekdays_weekend" : className = "calendar-month_weekdays";
            arrayWeekDays.push(<Weekday className={className} key={weekdays[i]}>{weekdays[i]}</Weekday>)
        }
        return arrayWeekDays;
    }

    getDays = () => {
        let keys = 0;
        let prevMonthDays = this.firstDayOfMonth();
        const year = this.state.openToDate.getFullYear();
        const month = this.state.openToDate.getMonth() + 1;
        const days = new Date(year, month, 0).getDate();
        let prevDays = new Date(year, month - 1, 0).getDate();
        const arrayDays = [];

        // Days + Weekend days 
        let seven = 7;
        let className = "";
        const dateNow = this.state.selectedDate.getDate();
        const isEqual = this.compareDays();
        for (let i = 1; i <= days; i++) {
            if (i === (seven - prevMonthDays - 1)) { className = "calendar-month_weekend"; }
            else if (i === (seven - prevMonthDays)) { seven += 7; className = "calendar-month_weekend"; }
            else { className = "calendar-month"; }
            if (i === dateNow && isEqual) { className = "calendar-month_selected-day" }
            arrayDays.push(
                <Day key={keys++}>
                    <AbbrDay onClick={this.onDayClick.bind(this, i)} className={className}>{i}</AbbrDay>
                </Day>
            );
        }

        // Neighboring Month Days (Prev month)
        while (prevMonthDays != 0) {
            arrayDays.unshift(
                <Day key={keys++}>
                    <AbbrDay className="calendar-month_neighboringMonth" >
                        {prevDays--}
                    </AbbrDay>
                </Day>
            );
            prevMonthDays--;
            //console.log("loop");
        }

        //Calculating Neighboring Month Days
        let maxDays = 35; // max days in month table (no)
        const firstDay = this.firstDayOfMonth();
        if (firstDay > 5 && days >= 30) { maxDays += 7; }
        else if (firstDay >= 5 && days > 30) { maxDays += 7; }        

        //Neighboring Month Days (Next month)
        let nextDay = 1;
        for (let i = days; i < maxDays - firstDay; i++) {
            if (firstDay == 0 && days == 28) { break; }
            arrayDays.push(
                <Day key={keys++}>
                    <AbbrDay className="calendar-month_neighboringMonth" >
                        {nextDay++}
                    </AbbrDay>
                </Day>
            );
        }
        return arrayDays;
    }

    render() {
        //console.log("render");

        moment.locale(this.props.language);
        this.state.months = moment.months();
        const disabled = this.props.disabled;
        const dropDownSize = this.getArrayYears().length > 6 ? 180 : undefined;

        return (
            <CalendarStyle color={this.props.themeColor}>
                <ComboBoxStyle>
                    <ComboBox
                        scaled={true}
                        dropDownMaxHeight={180}
                        onSelect={this.selectedMonth.bind(this)}
                        selectedOption={this.getCurrentMonth()}
                        options={this.getArrayMonth()}
                        isDisabled={disabled}
                    />
                    <ComboBoxDateStyle>
                        <ComboBox
                            scaled={true}
                            dropDownMaxHeight={dropDownSize}
                            onSelect={this.selectedYear.bind(this)}
                            selectedOption={this.getCurrentYear()}
                            options={this.getArrayYears()}
                            isDisabled={disabled}
                        />
                    </ComboBoxDateStyle>
                </ComboBoxStyle>
                <Month>
                    <Weekdays>
                        {this.getWeekDays()}
                    </Weekdays>
                    <Days>
                        {this.getDays()}
                    </Days>
                </Month>
            </CalendarStyle>
        );
    }
}

Calendar.propTypes = {
    onChange: PropTypes.func,
    themeColor: PropTypes.string,
    selectedDate: PropTypes.instanceOf(Date),
    openToDate: PropTypes.instanceOf(Date),
    minDate: PropTypes.instanceOf(Date),
    maxDate: PropTypes.instanceOf(Date),
    language: PropTypes.string,
    //disabled: PropTypes.bool,
    //size: PropTypes.string
}

Calendar.defaultProps = {
    selectedDate: new Date(),
    openToDate: new Date(),
    minDate: new Date("1970/01/01"),
    maxDate: new Date("3000/01/01"),
    themeColor: '#ED7309',
    language: moment.locale(),
    //size: 'base'
}

export default Calendar;