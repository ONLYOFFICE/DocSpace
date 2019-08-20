import React, { Component } from 'react';
import PropTypes from 'prop-types'
import styled, { css } from 'styled-components';
import ReactCalendar from 'react-calendar'
import ComboBox from '../combobox';
import moment from 'moment/min/moment-with-locales';

const WeekdayStyle = css`
    font-family: Open Sans;
    font-style: normal;
    font-weight: bold;
    font-size: 13px;
`;

const HoverStyle = css`
    max-width: 32px;
    max-height: 32px;
    border-radius: 16px;
`;

const CalendarStyle = styled.div`
    min-width:293px;
    ${props => props.size === 'base' ?
        `max-width: 293px;
        border-radius: 6px;
        -moz-border-radius: 6px;
        -webkit-border-radius: 6px;
        box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
        -moz-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
        -webkit-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);`
        :
        'max-width: 325px;'
    }

    padding: 16px 16px 16px 17px;
    box-sizing: content-box;

    .custom-tile-calendar {
        color: #333;
        ${WeekdayStyle}
        &:hover {
            ${HoverStyle}
            background-color: #F8F9F9;
        }
    }

    .react-calendar {
        border: none;
    }

    .react-calendar__month-view__weekdays :nth-child(6)  { color: #A3A9AE !important; }
    .react-calendar__month-view__weekdays :nth-child(7)  { color: #A3A9AE !important; }

    .react-calendar__month-view__weekdays {
        text-transform: none;
        ${WeekdayStyle}
    }

    .react-calendar__tile {
        ${HoverStyle}
        margin-top: 10px;
        background-color: none !important;
        /*flex-basis: 11.2857% !important;*/
        
        ${props => props.size === 'base' ?
            'margin-left: 9px;' :
            'margin 10px 7px 0 7px;'
        }
    }

    .react-calendar__tile:disabled {
        background-color: #fff;
    }

    .react-calendar__tile--active {
        background-color: ${props => props.color ? `${props.color} !important;` : `none !important;`}
        color: #fff !important;
        border-radius: 16px;

        &:hover {
            ${HoverStyle}
            color: #333;
        }
    }

    .react-calendar__month-view__days__day--weekend {
        color: #A3A9AE;
    }

    .react-calendar__month-view__days__day--neighboringMonth {
        color: #ECEEF1 !important;
        cursor: default !important;
        pointer-events: none;
    }
`;

const ComboBoxDateStyle = styled.div`
    min-width: 80px;
    height: 32px;
    margin-left: 8px;
`;

const ComboBoxStyle = styled.div`
    position: relative;
    display: flex;
    padding-bottom: 16px;
`;

class Calendar extends Component {
    constructor(props) {
        super(props);

        moment.locale(props.language);
        this.state = {
            selectedDate: props.selectedDate,
            openToDate: props.openToDate,
            minDate: props.minDate,
            maxDate: props.maxDate,
            months: moment.months()
        };
    }

    selectedYear = (value) => {
        this.setState({ openToDate: new Date(value.key, this.state.openToDate.getMonth()) });
    }

    selectedMonth = (value) => {
        this.setState({ openToDate: new Date(this.state.openToDate.getFullYear(), value.key) });
    }

    getArrayYears() {
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

    getListMonth(date1, date2) {
        let monthList = new Array();
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
        let selected_month = month.find(x => x.key == this.state.openToDate.getMonth());
        return (selected_month);
    }

    onChange = (date) => {
        if (!this.props.disabled && this.formatDate(this.state.selectedDate) !== this.formatDate(date)) {
            this.setState({ selectedDate: date });
            this.props.onChange && this.props.onChange(date);
        }
    }

    formatDate(date) {
        return (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear();
    }

    formatWeekday = (language, date) => {
        //console.log(date.toLocaleString(locale, {  weekday: 'short' }));
        return moment(date).locale(language).format('dd');
    }

    render() {
        const language = this.props.language;
        moment.locale(this.props.language);
        this.state.months = moment.months();
        const disabled = this.props.disabled;

        const dropDownSize = this.getArrayYears().length > 6 ? 200 : undefined;

        return (
            <CalendarStyle color={this.props.themeColor} size={this.props.size} >
                <ComboBoxStyle>
                    <ComboBox scaled={true} dropDownMaxHeight={200} onSelect={this.selectedMonth.bind(this)} selectedOption={this.getCurrentMonth()} options={this.getArrayMonth()} isDisabled={this.props.disabled} />
                    <ComboBoxDateStyle>
                        <ComboBox scaled={true} dropDownMaxHeight={dropDownSize} onSelect={this.selectedYear.bind(this)} selectedOption={this.getCurrentYear()} options={this.getArrayYears()} isDisabled={this.props.disabled} />
                    </ComboBoxDateStyle>
                </ComboBoxStyle>
                <ReactCalendar
                    onChange={this.onChange.bind(this)}
                    activeStartDate={this.state.openToDate}
                    value={this.state.selectedDate}
                    showNavigation={false}
                    locale={language}
                    minDate={this.minDate}
                    maxDate={this.maxDate}
                    tileDisabled={disabled ? ({ date }) => date.getDate() : undefined}
                    tileClassName={"custom-tile-calendar"}
                    formatShortWeekday={(value) => this.formatWeekday(language, value)}
                    //formatShortWeekday={(language, value) => this.formatWeekday(language, value)} // function to react-calendar v2.19.1
                />
            </CalendarStyle>
        );
    }
}

Calendar.propTypes = {
    onChange: PropTypes.func,
    disabled: PropTypes.bool,
    themeColor: PropTypes.string,
    selectedDate: PropTypes.instanceOf(Date),
    openToDate: PropTypes.instanceOf(Date),
    minDate: PropTypes.instanceOf(Date),
    maxDate: PropTypes.instanceOf(Date),
    language: PropTypes.string,
    size: PropTypes.string
}

Calendar.defaultProps = {
    selectedDate: new Date(),
    openToDate: new Date(),
    minDate: new Date("1970/01/01"),
    maxDate: new Date("3000/01/01"),
    themeColor: '#ED7309',
    language: moment.locale(),
    size: 'base'
}

export default Calendar;