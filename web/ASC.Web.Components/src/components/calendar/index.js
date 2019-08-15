import React, { Component } from 'react';
import PropTypes from 'prop-types'
import styled, { css } from 'styled-components';
import DatePicker from "react-datepicker";
import ComboBox from '../combobox';
import moment from 'moment/min/moment-with-locales';
import "react-datepicker/dist/react-datepicker.css";

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
    position: absolute;
    z-index: -1;
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
    margin-bottom: 5px;
`;

const ComboboxStyled = styled.div`
    margin-left: 8px;
    width: 80px;
`;

const DayNames = styled.div`
    max-width: 293px;
    display: block;
    margin-left: 5px;
`;
const DayName = styled.label`
    width:32px;
    height:32px;
    font-size: 13px;
    font-family: Open Sans;
    font-style: normal;
    font-weight: bold;
    text-align: center;
    margin-left: 4px;
    line-height: 4.5em;    
`;

class Calendar extends Component {
    constructor(props) {
        super(props);

        moment.locale(props.location);

        this.state = {
            selectedDate: props.selectedDate,
            openToDate: props.openToDate,
            months: moment.months()
        };
    }

    getArrayDates() {
        let date1 = this.props.minDate.getFullYear();
        let date2 = this.props.maxDate.getFullYear();
        const yearList = [];
        for (let i = date1; i <= date2; i++) {
            yearList.push({ key: `${i}`, label: `${String(i).toLocaleString(this.props.location)}` });
        }
        return yearList;
    }

    getCurrentDate() {
        let year = this.getArrayDates();
        year = year.find(x => x.key == this.state.selectedDate.getFullYear());
        return (year.key);
    }

    selectedDate = (value) => {
        this.setState({ openToDate: new Date(value.key, this.state.openToDate.getMonth()) });
    }


    getListMonth(date1, date2) {
        let monthList = new Array();
        for (let i = date1; i <= date2; i++) {
            monthList.push({ key: `${i}`, label: `${this.state.months[i]}` });
        }
        return monthList;
    }

    getArrayMonth() {
        let date1 = this.props.minDate.getMonth();
        let date2 = this.props.maxDate.getMonth();
        let monthList = new Array();
        if (this.props.minDate.getFullYear() !== this.props.maxDate.getFullYear()) {
            monthList = this.getListMonth(0, 11);
        } else { monthList = this.getListMonth(date1, date2); }
        return monthList;
    }

    getCurrentMonth() {
        let month = this.getArrayMonth();
        let selectedmonth = month.find(x => x.key == this.state.selectedDate.getMonth());
        return (selectedmonth.key);
    }

    selectedMonth = (value) => {
        //console.log(value);
        this.setState({ openToDate: new Date(this.state.openToDate.getFullYear(), value.key) });
    }

    fomateDate(date) {
        return (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear();
    }

    handleChange = (date) => {
        if (this.fomateDate(this.props.selectedDate) !== this.fomateDate(date)) {
            this.setState({ selectedDate: date });
            this.props.onChange && this.props.onChange(date);
        }
    }

    componentDidUpdate(prevProps) {
        if (this.props.location !== prevProps.location) {
            moment.locale(this.props.location);
            this.state.months = moment.months()
        }
    }

    render() {
        //console.log('render');
        const weekdays = moment.weekdaysMin();
        const minDate = new Date(this.props.minDate.getFullYear(), this.props.minDate.getMonth(), 1);
        const maxDate = new Date(this.props.maxDate.getFullYear(), this.props.maxDate.getMonth() + 1, 0);
        return (
            <CalendarContainer>
                <DataSelector>
                    <div>
                        <ComboBox scaled={false} onSelect={this.selectedMonth.bind(this)} isDisabled={this.props.disabled} options={this.getArrayMonth()} selectedOption={this.getCurrentMonth.bind(this)} />
                    </div>
                    <ComboboxStyled>
                        <ComboBox onSelect={this.selectedDate.bind(this)} isDisabled={this.props.disabled} options={this.getArrayDates()} selectedOption={this.getCurrentDate()} />
                    </ComboboxStyled>
                </DataSelector>
                <DayNames>
                    <DayName>{weekdays[0]}</DayName>
                    <DayName>{weekdays[1]}</DayName>
                    <DayName>{weekdays[2]}</DayName>
                    <DayName>{weekdays[3]}</DayName>
                    <DayName>{weekdays[4]}</DayName>
                    <DayName>{weekdays[5]}</DayName>
                    <DayName>{weekdays[6]}</DayName>
                </DayNames>

                <CalendarStyle color={this.props.themeColor}>
                    <DatePicker
                        inline
                        selected={this.state.selectedDate}
                        onChange={this.handleChange.bind(this)}
                        minDate={minDate}
                        maxDate={maxDate}
                        openToDate={this.state.openToDate}
                        showDisabledMonthNavigation
                        renderCustomHeader={({ }) => { }}

                    //locale={this.props.location} 
                    //disabled={this.props.disabled}
                    //dateFormat={this.props.dateFormat}
                    //locale='language'
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
    selectedDate: PropTypes.instanceOf(Date),
    openToDate: PropTypes.instanceOf(Date),
    minDate: PropTypes.instanceOf(Date),
    maxDate: PropTypes.instanceOf(Date),
    location: PropTypes.string
}

Calendar.defaultProps = {
    selectedDate: new Date(),
    openToDate: new Date(),
    minDate: new Date("1970/01/01"),
    maxDate: new Date("3000/01/01"),
    dateFormat: "dd.MM.yyyy",
    themeColor: '#ED7309',
    location: 'en'
}

export default Calendar;
