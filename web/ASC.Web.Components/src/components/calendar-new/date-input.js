import React, { Component } from 'react';
import PropTypes from 'prop-types'
import styled, { css } from 'styled-components';
import InputBlock from '../input-block';
import DropDown from '../drop-down';
import { Col } from 'reactstrap';
import NewCalendar from '../calendar-new';
import moment from 'moment/min/moment-with-locales';
import { handleAnyClick } from '../../utils/event';


const DateInputStyle = styled.div`

`;

class DatePicker extends Component {
    constructor(props) {
        super(props);

        moment.locale(props.locale);

        this.ref = React.createRef();
        if (props.isOpen) {
            handleAnyClick(true, this.handleClick);
        }
        this.format = moment.localeData().longDateFormat('L');
    }

    state = {
        isOpen: this.props.isOpen,
        selectedDate: new Date(moment(this.props.selectedDate, this.format)),
        value: moment(this.props.selectedDate).format('L'),
    };

    handleClick = (e) => {
        this.state.isOpen && !this.ref.current.contains(e.target) && this.onIconClick(false);
    }

    onIconClick = (isOpen) => {
        //console.log("Show calendar please");
        this.setState({ isOpen: isOpen });
    };

    handleChange(e) {
        const format = moment.localeData().longDateFormat('L');
        let date = new Date(moment(e.target.value, format));

        if (!this.isValidDate(date)) { date = this.state.selectedDate; }
        if (this.validationDate(date)) {
            this.props.onChange && this.props.onChange(date);
        }
        this.setState({ value: e.target.value, selectedDate: date })
    }

    onChange = (value) => {
        this.props.onChange && this.props.onChange(value);
        this.setState({ selectedDate: value, value: moment(value).format('L') })
    }

    isValidDate = (date) => {
        return date instanceof Date && !isNaN(date);
    }

    validationDate = (date) => {
        const minDate = this.props.minDate;
        const maxDate = this.props.maxDate;
        const selectedDate = date;
        if (selectedDate < minDate || selectedDate > maxDate) {
            this.state.selectedDate = new Date();
            return false;
        }
        return true;
    }

    componentWillUnmount() {
        handleAnyClick(false, this.handleClick);
    }

    componentDidUpdate(prevProps, prevState) {
        if (this.state.isOpen !== prevState.isOpen) {
            handleAnyClick(this.state.isOpen, this.handleClick);
        }

        if (this.props.selectedDate !== prevProps.selectedDate ||
            this.props.isOpen !== prevProps.isOpen || 
            this.props.locale != prevProps.locale) {
            this.setState({
                selectedDate: this.props.selectedDate,
                isOpen: this.props.isOpen,
                value: moment(this.props.selectedDate).format('L')
            });
        }
    }

    /*formatDate = () => {

    }*/

    render() {

        //console.log("Date-picker render");
        const isDisabled = this.props.isDisabled;
        const isReadOnly = this.props.isReadOnly;
        const hasError = this.props.hasError;
        const hasWarning = this.props.hasWarning;

        this.validationDate(this.state.selectedDate);

        return (
            <DateInputStyle ref={this.ref} >
                <InputBlock
                    isDisabled={isDisabled}
                    isReadOnly={isReadOnly}
                    hasError={hasError}
                    hasWarning={hasWarning}
                    iconName={"CalendarIcon"}
                    onIconClick={this.onIconClick.bind(this, !this.state.isOpen)}
                    value={this.state.value}
                    onChange={this.handleChange.bind(this)}
                    placeholder={moment.localeData().longDateFormat('L')}
                />
                <Col>
                    <DropDown opened={this.state.isOpen}>
                        {<NewCalendar
                            {...this.props}
                            openToDate={this.state.selectedDate}
                            selectedDate={this.state.selectedDate}
                            onChange={this.onChange}
                        />}
                    </DropDown>
                </Col>
            </DateInputStyle>
        );
    }
}

DatePicker.propTypes = {
    onChange: PropTypes.func,
    themeColor: PropTypes.string,
    selectedDate: PropTypes.instanceOf(Date),
    openToDate: PropTypes.instanceOf(Date),
    minDate: PropTypes.instanceOf(Date),
    maxDate: PropTypes.instanceOf(Date),
    locale: PropTypes.string,
    isDisabled: PropTypes.bool,
    isReadOnly: PropTypes.bool,
    hasError: PropTypes.bool,
    hasWarning: PropTypes.bool,
    isOpen: PropTypes.bool
}

DatePicker.defaultProps = {
    minDate: new Date("1970/01/01"),
    maxDate: new Date(new Date().getFullYear() + 1, 1, 1),
}

export default DatePicker;
