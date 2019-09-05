import React, { Component } from 'react';
import PropTypes from 'prop-types'
import styled, { css } from 'styled-components';
import ComboBox from '../combobox';
import moment from 'moment/min/moment-with-locales';
import { Text } from "../text";

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
    padding-bottom: 24px !important;
`;

const ComboBoxDateStyle = styled.div`
    min-width: 80px;
    height: 32px;
    margin-left: 8px;
`;

const CalendarContainer = styled.div`
    ${props => props.size === 'base' ?
        `max-width: 293px;` : `max-width: 325px;`
    }
`;

const CalendarStyle = styled.div`
    
    ${props => props.size === 'base' ? 'width: 260px;' : 'width: 294px;'}

    padding: 16px 16px 16px 17px;
    box-sizing: content-box;
    ${props => props.isDisabled ?
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

    .calendar-month_selected-day {
        background-color: ${props => props.color};
        border-radius: 16px;
        cursor: pointer;
        color: #fff;
    }
`;

const Weekday = styled.div`
    overflow: hidden;
    flex-basis: 14.2857%; /*(1/7*100%)*/
`;

const Weekdays = styled.div`
    width: ${props => props.size === 'base' ? '265px;' : '310px;'}
    display: flex;
    margin-bottom: -5px;
`;

const Month = styled.div`
    width: ${props => props.size === 'base' ? '267px;' : '303px;'}
`;

const Days = styled.div`
    display: flex;
    flex-wrap: wrap;
    ${props => props.size === 'base' ? 'width: 270px;' : 'width: 310px;'}
`;

const Day = styled.div`
    display: flex;
    flex-basis: 14.2857%; /*(1/7*100%)*/
    text-align: center;
    line-height: 2.5em !important;
    ${props => props.size === 'base' ? 'margin-top: 3px;' : 'margin-top: 7.5px;'}
`;

const TextStyle = styled.div`
    width: 32px;
    height: 32px;
    p {
        text-align: center;
    }
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
        const currentMonth = this.state.openToDate.getMonth();
        const currentYear = this.state.openToDate.getFullYear();
        const dateInCurrentMonth = new Date(currentYear, currentMonth, day);
        const countDaysInCurrentMonth = new Date(currentYear, currentMonth + 1, 0).getDate();

        if (day < 0) {
            const countDaysInPrevMonth = new Date(currentYear, currentMonth, 0).getDate();
            const dateInPrevMonth = new Date(currentYear, currentMonth - 1, (countDaysInPrevMonth + day + 1));
            this.setState({ selectedDate: dateInPrevMonth, openToDate: dateInPrevMonth });
            this.props.onChange && this.props.onChange(dateInPrevMonth);
        }
        else if (day > countDaysInCurrentMonth) {
            const dateInNextMonth = new Date(currentYear, currentMonth + 1, (day - countDaysInCurrentMonth));
            this.setState({ selectedDate: dateInNextMonth, openToDate: dateInNextMonth });
            this.props.onChange && this.props.onChange(dateInNextMonth);
        }
        else if (this.formatSelectedDate(dateInCurrentMonth) != this.formatSelectedDate(this.state.selectedDate)) {
            this.setState({ selectedDate: dateInCurrentMonth });
            this.props.onChange && this.props.onChange(dateInCurrentMonth);
        }
    }

    getListMonth = () => {
        const minDate = this.props.minDate;
        const maxDate = this.props.maxDate;
        const months = this.state.months;
        let disabled = false;
        const listMonths = [];

        let i = 0;
        while (i <= 11) {
            listMonths.push({ key: `${i}`, label: `${months[i]}`, disabled: disabled });
            i++;
        }

        if (this.state.openToDate.getFullYear() === minDate.getFullYear()) {
            i = 0;
            while (i != minDate.getMonth()) {
                listMonths[i].disabled = true;
                i++;
            }
        }

        if (this.state.openToDate.getFullYear() === maxDate.getFullYear()) {
            i = 11;
            while (i != maxDate.getMonth()) {
                listMonths[i].disabled = true;
                i--;
            }
        }

        return listMonths;
    }

    getCurrentMonth = () => {
        const openToDate = this.state.openToDate;
        const months = this.getListMonth();
        let selectedMonth = months.find(x => x.key == openToDate.getMonth());

        if (selectedMonth.disabled === true) {
            selectedMonth = months.find(x => x.disabled === false);
            const date = new Date(openToDate.getFullYear(), selectedMonth.key, 1);
            this.state.openToDate = date;
        }
        return selectedMonth;
    }

    getArrayYears = () => {
        const minYear = this.props.minDate.getFullYear();
        const maxYear = this.props.maxDate.getFullYear();
        const yearList = [];

        let i = minYear;
        while (i <= maxYear) {
            let newDate = new Date(i, 0, 1);
            const label = moment(newDate).format('YYYY')
            const key = i;
            yearList.push({ key, label: label });
            i++;
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
        const openedDate = this.state.openToDate;
        const firstDay = moment(openedDate).locale("en").startOf("month").format("d");
        let day = firstDay - 1;
        if (day < 0) { day = 6; }
        return day;
    }

    getWeekDays = () => {
        let arrayWeekDays = [];
        const weekdays = moment.weekdaysMin();
        weekdays.push(weekdays.shift());
        let color;
        for (let i = 0; i < weekdays.length; i++) {
            (i >= 5) ? color = { color: "#A3A9AE" } : {};
            arrayWeekDays.push(
                <Weekday key={weekdays[i]}>
                    <TextStyle>
                        <Text.Body {...color} isBold={true}> {(weekdays[i])} </Text.Body>
                    </TextStyle>
                </Weekday>)
        }
        return arrayWeekDays;
    }

    getDays = () => {
        let keys = 0;
        let firstDayOfMonth = this.firstDayOfMonth();
        const currentYear = this.state.openToDate.getFullYear();
        const currentMonth = this.state.openToDate.getMonth() + 1;
        const countDaysInMonth = new Date(currentYear, currentMonth, 0).getDate();
        let countDaysInPrevMonth = new Date(currentYear, currentMonth - 1, 0).getDate();
        const arrayDays = [];
        let className = "calendar-month_neighboringMonth";
        const color = { color: "inherit;" };

        const openToDate = this.state.openToDate;
        const maxDate = this.props.maxDate;
        const minDate = this.props.minDate;

        //Disable preview month
        let disableClass = null;
        if (openToDate.getFullYear() === minDate.getFullYear() && openToDate.getMonth() === minDate.getMonth()) {
            disableClass = "calendar-month_disabled";
        }

        //Prev month
        let prevMonthDay = null;
        if (openToDate.getFullYear() === minDate.getFullYear() && openToDate.getMonth() - 1 === minDate.getMonth()) {
            prevMonthDay = minDate.getDate();
        }

        //prev month + year
        let prevYearDay = null;
        if (openToDate.getFullYear() === minDate.getFullYear() + 1 && openToDate.getMonth() === 0 && minDate.getMonth() === 11) {
            prevYearDay = minDate.getDate();
        }

        const size = this.props.size;
        // Show neighboring days in prev month
        while (firstDayOfMonth != 0) {
            if (countDaysInPrevMonth + 1 === prevMonthDay) { disableClass = "calendar-month_disabled"; }
            if (countDaysInPrevMonth + 1 === prevYearDay) { disableClass = "calendar-month_disabled"; }
            arrayDays.unshift(
                <Day size={size} key={--keys} className={disableClass} >
                    <TextStyle onClick={this.onDayClick.bind(this, keys)} className={className}>
                        <Text.Body isBold={true} {...color}>
                            {countDaysInPrevMonth--}
                        </Text.Body>
                    </TextStyle>
                </Day>
            );
            //console.log("loop");
            firstDayOfMonth--;
        }

        //Disable max days in month
        keys = 0;
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
        firstDayOfMonth = this.firstDayOfMonth();

        for (let i = 1; i <= countDaysInMonth; i++) {
            if (i === (seven - firstDayOfMonth - 1)) { className = "calendar-month_weekend"; }
            else if (i === (seven - firstDayOfMonth)) { seven += 7; className = "calendar-month_weekend"; }
            else { className = "calendar-month"; }
            if (i === dateNow && this.compareDays()) { className = "calendar-month_selected-day" }
            if (i > maxDay || i < minDay) { disableClass = "calendar-month_disabled"; className = "calendar-month_disabled" }
            else { disableClass = null; }

            arrayDays.push(
                <Day size={size} key={keys++} className={disableClass} >
                    <TextStyle onClick={this.onDayClick.bind(this, i)} className={className}>
                        <Text.Body isBold={true} {...color}>{i}</Text.Body>
                    </TextStyle>
                </Day>
            );
        }

        //Calculating neighboring days in next month
        let maxDaysInMonthTable = 42;
        const firstDay = this.firstDayOfMonth();
        if (firstDay > 5 && countDaysInMonth >= 30) { maxDaysInMonthTable += 7; }
        else if (firstDay >= 5 && countDaysInMonth > 30) { maxDaysInMonthTable += 7; }
        if (maxDaysInMonthTable > 42) { maxDaysInMonthTable -= 7; }

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
        for (let i = countDaysInMonth; i < maxDaysInMonthTable - firstDay; i++) {
            if (i - countDaysInMonth === nextYearDay) { disableClass = "calendar-month_disabled" }
            if (i - countDaysInMonth === nextMonthDay) { disableClass = "calendar-month_disabled" }
            arrayDays.push(
                <Day size={size} key={keys++} className={disableClass} >
                    <TextStyle
                        onClick={this.onDayClick.bind(this, i + 1)}
                        className={"calendar-month_neighboringMonth"}
                    >
                        <Text.Body isBold={true} {...color}>{nextDay++}</Text.Body>
                    </TextStyle>
                </Day>
            );
        }
        return arrayDays;
    }

    componentDidUpdate(prevProps) {
        moment.locale(this.props.locale);
        this.state.months = moment.months();
        if (this.props.selectedDate !== prevProps.selectedDate ||
            this.props.openToDate !== prevProps.openToDate) {
            this.setState({
                selectedDate: this.props.selectedDate,
                openToDate: this.props.openToDate
            });
        }
    }

    render() {
        //console.log("Calendar render");

        const isDisabled = this.props.isDisabled;
        const size = this.props.size;
        const dropDownSizeMonth = this.getListMonth().length > 4 ? 184 : undefined;
        const dropDownSizeYear = this.getListMonth().length > 4 ? 184 : undefined;

        return (
            <CalendarContainer size={size}>
                <CalendarStyle size={size} color={this.props.themeColor} isDisabled={isDisabled}>
                    <ComboBoxStyle>
                        <ComboBox
                            scaled={true}
                            dropDownMaxHeight={dropDownSizeMonth}
                            onSelect={this.onSelectMonth.bind(this)}
                            selectedOption={this.getCurrentMonth()}
                            options={this.getListMonth()}
                            isDisabled={isDisabled}
                        />
                        <ComboBoxDateStyle>
                            <ComboBox
                                scaled={true}
                                dropDownMaxHeight={dropDownSizeYear}
                                onSelect={this.onSelectYear.bind(this)}
                                selectedOption={this.getCurrentYear()}
                                options={this.getArrayYears().reverse()}
                                isDisabled={isDisabled}
                            />
                        </ComboBoxDateStyle>
                    </ComboBoxStyle>
                    <Month size={size}>
                        <Weekdays size={size}>
                            {this.getWeekDays()}
                        </Weekdays>
                        <Days size={size}>
                            {this.getDays()}
                        </Days>
                    </Month>
                </CalendarStyle>
            </CalendarContainer>
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
    isDisabled: PropTypes.bool,
    size: PropTypes.oneOf(['base', 'big'])
}

Calendar.defaultProps = {
    selectedDate: new Date(),
    openToDate: new Date(),
    minDate: new Date("1970/01/01"),
    maxDate: new Date(new Date().getFullYear() + 1 + "/01/01"),
    themeColor: '#ED7309',
    locale: moment.locale(),
    size: 'base'
}

export default Calendar;