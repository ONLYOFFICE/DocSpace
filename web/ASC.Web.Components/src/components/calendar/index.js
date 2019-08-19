import React, { Component } from 'react';
import PropTypes from 'prop-types'
import styled, { css } from 'styled-components';
//import ReactCalendar from 'react-calendar'
import ReactCalendar from 'react-calendar';
//import Calendar from 'react-calendar/dist/entry.nostyle';
import ComboBox from '../combobox';
import moment from 'moment/min/moment-with-locales';
//import moment from 'moment';

const WeekdayStyle = css`
    font-family: Open Sans;
    font-style: normal;
    font-weight: bold;
    font-size: 13px;
`;

const CalendarStyle = styled.div`
    max-width: 325px;

    border-radius: 6px;
    -moz-border-radius: 6px;
    -webkit-border-radius: 6px;
    box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -moz-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -webkit-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);   
    padding: 16px 16px 16px 17px;

    .custom-tile-calendar {
        color: #333;
        ${WeekdayStyle}
        &:hover {
            border-radius: 16px;
            background-color: ${props => props.color ? `${props.color} !important` : `none !important`};
            color: #fff !important;
        }
    }

    .react-calendar {
        border: none;
    }

    .react-calendar__month-view__weekdays {
        text-transform: none;
    }

    .react-calendar__month-view__weekdays__weekday {     
        abbr {
            text-decoration: none;
            ${WeekdayStyle}
            cursor: auto;
       }
    }

    .react-calendar__tile {
        height: 32px;
        width: 32px;
        margin-top: 10px;
    }


    .react-calendar__tile--active {
        background-color: #F8F9F9 !important;
        border-radius: 16px;
    }

    .react-calendar__month-view__days__day--weekend {
        color: #A3A9AE;
    }

    .react-calendar__month-view__days__day--neighboringMonth {
        color: #ECEEF1;
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

        moment.locale(props.location);
        this.state = {
            selectedDate: props.selectedDate,
            openToDate: props.minDate,
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
            yearList.push({ key: `${i}`, label: `${String(i).toLocaleString(this.props.location)}` });
        }
        return yearList;
    }

    getCurrentYear = () => {
        let year = this.getArrayYears();
        year = year.find(x => x.key == this.state.selectedDate.getFullYear());
        return (year.key);
    }

    getListMonth (date1, date2) {
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
        let selectedmonth = month.find(x => x.key == this.state.selectedDate.getMonth());
        return (selectedmonth.key);
    }

    onChange = (date) => {
        if (this.formatDate(this.state.selectedDate) !== this.formatDate(date)) {
            this.setState({ selectedDate: date });
            this.props.onChange && this.props.onChange(date);
        }
    }

    formatDate(date) {
        return (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear();
    }

    formatWeekday = (locale, date) => {
        return moment(date).locale(locale).format('dd');
        //console.log(new Date().toLocaleString('en-us', {  weekday: 'short' }));
    }

    render() {

        const location = this.props.location;
        this.state.months = moment.months();

        return (
            <CalendarStyle color={this.props.themeColor}>
                <ComboBoxStyle>
                    <ComboBox scaled={true} onSelect={this.selectedMonth.bind(this)} selectedOption={this.getCurrentMonth()} options={this.getArrayMonth()} isDisabled={this.props.disabled} />
                    <ComboBoxDateStyle>
                        <ComboBox scaled={true} onSelect={this.selectedYear.bind(this)} selectedOption={this.getCurrentYear()} options={this.getArrayYears()} isDisabled={this.props.disabled} />
                    </ComboBoxDateStyle>
                </ComboBoxStyle>
                <ReactCalendar
                    onClickDay={this.onChange.bind(this)}
                    activeStartDate={this.state.openToDate}
                    value={this.state.selectedDate}
                    locale={location}
                    minDate={this.minDate}
                    maxDate={this.maxDate}

                    

                    //showNavigation={false}
                    className={"custom-calendar"}
                    tileClassName={"custom-tile-calendar"}
                    //formatShortWeekday={(locale, value) => this.formatWeekday(locale, value)}
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
    //openToDate: PropTypes.instanceOf(Date),
    minDate: PropTypes.instanceOf(Date),
    maxDate: PropTypes.instanceOf(Date),
    location: PropTypes.string
}

Calendar.defaultProps = {
    selectedDate: new Date(),
    openToDate: new Date(),
    minDate: new Date("1970/01/01"),
    maxDate: new Date("3000/01/01"),
    themeColor: '#ED7309'
}

export default Calendar;