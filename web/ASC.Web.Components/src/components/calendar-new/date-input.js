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

        this.ref = React.createRef();
        if (props.isOpen) {
            handleAnyClick(true, this.handleClick);
        }
    }

    state = {
        isOpen: this.props.isOpen,
        selectedDate: this.props.selectedDate,
        openToDate: this.props.openToDate,
        value: moment(this.props.selectedDate).format('L'),
        hasWarning: this.props.hasWarning,
    };

    handleClick = (e) => {
        this.state.isOpen && !this.ref.current.contains(e.target) && this.onIconClick(false);
    }

    onIconClick = (isOpen) => {
        //console.log("Show calendar please");
        this.setState({ isOpen: isOpen });
    };

    handleChange(e) {
        let date = new Date(e.target.value);
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

        if (this.props.selectedDate !== prevProps.selectedDate) {
            this.setState({
                selectedDate: this.props.selectedDate,
            });
        }
    }


    render() {

        //console.log("Date-picker render");
        const isDisabled = this.props.isDisabled;
        const isReadOnly = this.props.isReadOnly;
        const hasError = this.props.hasError;

        this.validationDate(this.state.selectedDate);

        return (
            <DateInputStyle ref={this.ref} >
                <InputBlock
                    id={"0"}
                    name={"name"}
                    isDisabled={isDisabled}
                    isReadOnly={isReadOnly}
                    hasError={hasError}
                    hasWarning={this.state.hasWarning}
                    iconName={isDisabled ? "CalendarEmptyIcon" : "CalendarIcon"}
                    isIconFill={true}
                    iconColor="#A3A9AE"
                    onIconClick={this.onIconClick.bind(this, !this.state.isOpen)}
                    value={this.state.value}
                    onChange={this.handleChange.bind(this)}
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
