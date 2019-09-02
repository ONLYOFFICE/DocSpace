import React, { Component } from 'react';
import PropTypes from 'prop-types'
import styled, { css } from 'styled-components';
import InputBlock from '../input-block';
import DropDown from '../drop-down';
import { Col } from 'reactstrap';
import NewCalendar from '../calendar-new';
import moment from 'moment/min/moment-with-locales';

class DatePicker extends Component {
    constructor(props) {
        super(props);
    }

    //date = this.state.selectedDate;
    //newDate = moment(date).format('L');

    state = {
        isOpen: this.props.isOpen,
        selectedDate: this.props.selectedDate,
        openToDate: this.props.openToDate,
        value: moment(this.props.selectedDate).format('L')
    };

    iconClick = () => {
        //console.log("Show calendar please");
        this.setState({ isOpen: !this.state.isOpen });
    };

    onChange = (value) => {
        this.setState({ selectedDate: value })
    }

    componentDidUpdate(prevProps) {
        if (this.props.selectedDate !== prevProps.selectedDate ||
            this.props.isOpen !== prevProps.isOpen) {
            this.setState({
                selectedDate: this.props.selectedDate,
                isOpen: this.props.isOpen
            });
        }
    }

    handleChange(event) {
        this.setState({ value: event.target.value });
    }


    render() {

        //console.log("Date-picker render");


        const selectedDate = this.props.selectedDate;
        const openToDate = this.props.openToDate;
        const isDisabled = this.props.isDisabled;
        const isReadOnly = this.props.isReadOnly;
        const hasError = this.props.hasError;
        const hasWarning = this.props.hasWarning;


        let date = new Date(this.state.value);
        if (date == "Invalid Date") { date = openToDate }

        return (
            <div>
                <InputBlock
                    id={"0"}
                    name={"name"}
                    isDisabled={isDisabled}
                    isReadOnly={isReadOnly}
                    hasError={hasError}
                    hasWarning={hasWarning}
                    iconName="CalendarIcon"//CalendarEmptyIcon
                    isIconFill={true}
                    iconColor="#A3A9AE"
                    onIconClick={this.iconClick}
                    value={this.state.value}
                    onChange={this.handleChange.bind(this)}
                />

                <Col>
                    <DropDown opened={this.state.isOpen}>
                        {<NewCalendar openToDate={date} selectedDate={selectedDate} onChange={this.onChange} />}
                    </DropDown>
                </Col>
            </div>
        );
    }
}

export default DatePicker;
