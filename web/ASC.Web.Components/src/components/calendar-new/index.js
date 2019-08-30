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

const DisabledStyle = css`
    -webkit-touch-callout: none;
    -webkit-user-select: none;
    -khtml-user-select: none;
    -moz-user-select: none;
    -ms-user-select: none;
    user-select: none;
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
    width: ${props => props.scaled ? "100%;" : "325px;"}

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
    ${props => props.disabled ?
        `pointer-events: none;
        ${DisabledStyle}
        `
        : "pointer-events: auto;"
    }
    
    .calendar-month {
        ${HoverStyle}
    }

    .calendar-month_neighboringMonth {
        color: #ECEEF1;

        ${HoverStyle}
        &:hover {color: #333;}
    }

    .calendar-month_disabled {
        ${DisabledStyle}
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
    display: flex;
    overflow: hidden;
    flex-basis: 14.2857%;
    padding-left: 4px;
`;

const Weekdays = styled.div`
    display: flex;
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
        selectedDate: this.props.selectedDate
    };

    onSelectYear = (value) => {
        this.setState({ openToDate: new Date(value.key, this.state.openToDate.getMonth()) });
    }

    onSelectMonth = (value) => {
        this.setState({ openToDate: new Date(this.state.openToDate.getFullYear(), value.key) });
    }

    onDayClick = (day) => {
        let month = this.state.openToDate.getMonth() + 1;
        let year = this.state.openToDate.getFullYear();
        const date = new Date(month + "/" + day + "/" + year);
        const days = new Date(year, month, 0).getDate();

        if (day < 0) {
            if (month === 1) { month = 13, year -= 1 }
            const prevDays = new Date(year, (month - 1), 0).getDate();
            const prevDate = new Date((month - 1) + "/" + (prevDays + day + 1) + "/" + year);
            this.setState({ selectedDate: prevDate, openToDate: prevDate });
        }
        else if (day > days) {
            if (month === 12) { month = 0, year += 1 }
            const nextDate = new Date(month + 1 + "/" + (day - days) + "/" + year);
            this.setState({ selectedDate: nextDate, openToDate: nextDate });
        }
        else if (this.formatSelectedDate(date) != this.formatSelectedDate(this.state.selectedDate)) {
            this.setState({ selectedDate: date });
            this.props.onChange && this.props.onChange(date);
        }
    }

    getListMonth = () => {
        const minDate = this.props.minDate;
        const maxDate = this.props.maxDate;
        let disabled = false;
        const monthList = [];

        const month = this.state.months.map(item => item.charAt(0).toLocaleUpperCase() + item.slice(1));
        //This function is not optimal for all languages. // It'is bad...

        for (let i = 0; i <= 11; i++) {
            monthList.push({ key: `${i}`, label: `${month[i]}`, disabled: disabled });
        }

        let i = 0;
        if (this.state.openToDate.getFullYear() === minDate.getFullYear()) {
            while (i != minDate.getMonth()) {
                monthList[i].disabled = true;
                i++;
            }
        }

        i = 11;
        if (this.state.openToDate.getFullYear() === maxDate.getFullYear()) {
            while (i != maxDate.getMonth()) {
                monthList[i].disabled = true;
                i--;
            }
        }

        return monthList;
    }

    getCurrentMonth = () => {
        const openToDate = this.state.openToDate;
        const month = this.getListMonth();
        let selectedMonth = month.find(x => x.key == openToDate.getMonth());

        if (selectedMonth.disabled === true) {
            selectedMonth = month.find(x => x.disabled === false);
            const key = selectedMonth.key;
            const currentMonth = Number(key) + 1;
            const date = new Date(openToDate.getFullYear() + "/" + currentMonth + "/" + "01");
            this.state.openToDate = date;
        }
        return selectedMonth;
    }

    getArrayYears = () => {
        const minDate = this.props.minDate.getFullYear();
        const maxDate = this.props.maxDate.getFullYear();
        const yearList = [];
        for (let i = minDate; i <= maxDate; i++) {
            let newDate = new Date(i, 0, 1);
            yearList.push({ key: `${i}`, label: `${moment(newDate).format('YYYY')}` });
        }
        return yearList;
    }

    getCurrentYear = () => {
        return this.getArrayYears().find(x => x.key == this.state.openToDate.getFullYear());
    }

    formatSelectedDate = (date) => {
        return (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear();
    }

    formatDate = (date) => {
        return (date.getMonth() + 1) + "/" + 1 + "/" + date.getFullYear();
    }

    compareDays = () => {
        const openDate = this.formatDate(this.state.openToDate);
        const selectedDate = this.formatDate(this.state.selectedDate);
        return (openDate === selectedDate) ? true : false;
    }

    firstDayOfMonth = () => {
        const selectedDate = this.state.openToDate;
        const firstDay = moment(selectedDate).locale("en").startOf("month").format("d");
        let day = firstDay - 1;
        if (day < 0) { day = 6; }
        return day;
    }

    getWeekDays = () => {
        let arrayWeekDays = [];
        const weekdays = moment.weekdaysMin();
        weekdays.push(weekdays.shift());
        let className = "";
        for (let i = 0; i < weekdays.length; i++) {
            (i >= 5) ? className = "calendar-month_weekdays_weekend" : className = "calendar-month_weekdays";
            arrayWeekDays.push(<Weekday className={className} key={weekdays[i]}>
                <AbbrDay>{(weekdays[i].charAt(0).toUpperCase() + weekdays[i].slice(1))}</AbbrDay></Weekday>)
            //This function is not optimal for all languages. // It'is bad...
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
        let className = "calendar-month_neighboringMonth";

        const openToDate = this.state.openToDate;
        const maxDate = this.props.maxDate;
        const minDate = this.props.minDate;


        //Disable preview month
        let disableClass = null;
        if (openToDate.getFullYear() === minDate.getFullYear() && openToDate.getMonth() === minDate.getMonth()) {
            disableClass = "calendar-month_disabled";
        }

        //prev month
        let prevMonthDay = null;
        if (openToDate.getFullYear() === minDate.getFullYear() && openToDate.getMonth() - 1 === minDate.getMonth()) {
            prevMonthDay = minDate.getDate();
        }

        //prev month + year
        let prevYearDay = null;
        if (openToDate.getFullYear() === minDate.getFullYear() + 1 && openToDate.getMonth() === 0 && minDate.getMonth() === 11) {
            prevYearDay = minDate.getDate();
        }

        // Show neighboring days in prev month
        while (prevMonthDays != 0) {
            if (prevDays + 1 === prevMonthDay) { disableClass = "calendar-month_disabled"; }
            if (prevDays + 1 === prevYearDay) { disableClass = "calendar-month_disabled"; }
            arrayDays.unshift(
                <Day key={--keys} className={disableClass} >
                    <AbbrDay
                        onClick={this.onDayClick.bind(this, keys)}
                        className={className} >
                        {prevDays--}
                    </AbbrDay>
                </Day>
            );
            //console.log("loop");
            prevMonthDays--;
        }
        keys = 0;


        //Disable max days in month
        let maxDay, minDay;
        disableClass = null;
        if (openToDate.getFullYear() === maxDate.getFullYear() && openToDate.getMonth() >= maxDate.getMonth()) {
            if (openToDate.getMonth() === maxDate.getMonth()) { maxDay = maxDate.getDate(); }
            else { maxDay = null; }
        }

        //Disable min days in month
        if (openToDate.getFullYear() === minDate.getFullYear() && openToDate.getMonth() >= minDate.getMonth()) {
            if (openToDate.getMonth() === minDate.getMonth()) { minDay = minDate.getDate(); }
            else { minDay = null; }
        }

        // Show days in month and weekend days
        let seven = 7;
        const dateNow = this.state.selectedDate.getDate();
        const temp = 1;
        prevMonthDays = this.firstDayOfMonth();

        for (let i = 1; i <= days; i++) {
            if (i === (seven - prevMonthDays - temp)) { className = "calendar-month_weekend"; }
            else if (i === (seven - prevMonthDays)) { seven += 7; className = "calendar-month_weekend"; }
            else { className = "calendar-month"; }
            if (i === dateNow && this.compareDays()) { className = "calendar-month_selected-day" }
            if (i > maxDay || i < minDay) { disableClass = "calendar-month_disabled"; className = "calendar-month_disabled" }
            else { disableClass = null; }

            arrayDays.push(
                <Day key={keys++} className={disableClass} >
                    <AbbrDay onClick={this.onDayClick.bind(this, i)} className={className}>{i}</AbbrDay>
                </Day>
            );
        }

        //Calculating neighboring days in next month
        let maxDays = 42; // max days in month table
        const firstDay = this.firstDayOfMonth();
        if (firstDay > 5 && days >= 30) { maxDays += 7; }
        else if (firstDay >= 5 && days > 30) { maxDays += 7; }
        if (maxDays > 42) { maxDays -= 7; }

        //Disable next month days
        disableClass = null;
        if (openToDate.getFullYear() === maxDate.getFullYear() && openToDate.getMonth() >= maxDate.getMonth()) {
            disableClass = "calendar-month_disabled";
        }

        //next month + year 
        let nextYearDay = null;
        if (openToDate.getFullYear() === maxDate.getFullYear() - 1 && openToDate.getMonth() === 11 && maxDate.getMonth() === 0) {
            nextYearDay = maxDate.getDate();
        }

        //next month
        let nextMonthDay = null;
        if (openToDate.getFullYear() === maxDate.getFullYear() && openToDate.getMonth() === maxDate.getMonth() - 1) {
            nextMonthDay = maxDate.getDate();
        }

        //Show neighboring days in next month
        let nextDay = 1;
        for (let i = days; i < maxDays - firstDay; i++) {
            if (i - days === nextYearDay) { disableClass = "calendar-month_disabled" }
            if (i - days === nextMonthDay) { disableClass = "calendar-month_disabled" }
            arrayDays.push(
                <Day key={keys++} className={disableClass} >
                    <AbbrDay
                        onClick={this.onDayClick.bind(this, i + 1)}
                        className={"calendar-month_neighboringMonth"} >
                        {nextDay++}
                    </AbbrDay>
                </Day>
            );
        }
        return arrayDays;
    }

    componentDidUpdate(prevProps) {
        moment.locale(this.props.locale);
        this.state.months = moment.months();
        if (this.props.selectedDate !== prevProps.selectedDate ||
            this.props.selectedDate !== prevProps.selectedDate) {
            this.setState({
                selectedDate: this.props.selectedDate,
                openToDate: this.props.openToDate
            });
        }
    }

    render() {
        //console.log("render");

        const disabled = this.props.disabled;
        const scaled = this.props.scaled;
        const dropDownSizeMonth = this.getListMonth().length > 4 ? 180 : undefined;
        const dropDownSizeYear = this.getListMonth().length > 4 ? 180 : undefined;

        return (
            <CalendarStyle scaled={scaled} color={this.props.themeColor} disabled={disabled}>
                <ComboBoxStyle>
                    <ComboBox
                        scaled={true}
                        dropDownMaxHeight={dropDownSizeMonth}
                        onSelect={this.onSelectMonth.bind(this)}
                        selectedOption={this.getCurrentMonth()}
                        options={this.getListMonth()}
                        isDisabled={disabled}
                    />
                    <ComboBoxDateStyle>
                        <ComboBox
                            scaled={true}
                            dropDownMaxHeight={dropDownSizeYear}
                            onSelect={this.onSelectYear.bind(this)}
                            selectedOption={this.getCurrentYear()}
                            options={this.getArrayYears().reverse()}
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
    locale: PropTypes.string,
    disabled: PropTypes.bool,
    scaled: PropTypes.bool
}

Calendar.defaultProps = {
    selectedDate: new Date(),
    openToDate: new Date(),
    minDate: new Date("1970/01/01"),
    maxDate: new Date("3000/01/01"),
    themeColor: '#ED7309',
    locale: moment.locale(),
}

export default Calendar;