import React, { Component } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import InputBlock from "../input-block";
import DropDown from "../drop-down";
import { Col } from "reactstrap";
import NewCalendar from "../calendar-new";
import moment from "moment/min/moment-with-locales";
import { handleAnyClick } from "../../utils/event";
import isEmpty from "lodash/isEmpty";

const DateInputStyle = styled.div``;

class DatePicker extends Component {
  constructor(props) {
    super(props);

    moment.locale(props.locale);
    this.ref = React.createRef();
    this.newRef = React.createRef();

    if (props.isOpen) {
      handleAnyClick(true, this.handleClick);
    }

    const { isOpen, selectedDate } = this.props;

    this.state = {
      isOpen,
      selectedDate: moment(selectedDate).toDate(),
      value: moment(selectedDate).format("L"),
      mask: this.getMask
    };
  }

  handleClick = e => {
    this.state.isOpen &&
      !this.ref.current.contains(e.target) &&
      this.onIconClick(false);
  };

  onIconClick = isOpen => {
    this.setState({ isOpen });
  };

  handleChange = e => {
    if (this.state.value != e.target.value) {
      let newState = { value: e.target.value };
      const format = moment.localeData().longDateFormat("L");
      const momentDate = moment(e.target.value, format);
      const date = momentDate.toDate();

      if (!isNaN(date) && this.compareDates(date)) {
        this.props.onChange && this.props.onChange(date);
        newState = Object.assign({}, newState, {
          selectedDate: date
        });
      }
      this.setState(newState);
    }
  };

  onChange = value => {
    const formatValue = moment(value).format("L");
    this.props.onChange && this.props.onChange(value);
    this.setState({ selectedDate: value, value: formatValue });
  };

  compareDates = date => {
    const { minDate, maxDate } = this.props;
    const selectedDate = date;

    if (selectedDate < minDate || selectedDate > maxDate) {
      return false;
    }
    return true;
  };

  getMask = () => {
    let symbol = ".";
    let localeMask = moment.localeData().longDateFormat("L");

    if (localeMask.indexOf("/") + 1) {
      symbol = "/";
    } else if (localeMask.indexOf(".") + 1) {
      symbol = ".";
    } else if (localeMask.indexOf("-") + 1) {
      symbol = "-";
    }

    let mask = [/\d/, /\d/, symbol, /\d/, /\d/, symbol, /\d/, /\d/, /\d/, /\d/];

    if (localeMask[0] === "Y") {
      mask = mask.reverse();
    }
    if (this.props.locale === "ko" || this.props.locale === "lv") {
      mask.push(symbol);
    }
    return mask;
  };

  componentWillUnmount() {
    handleAnyClick(false, this.handleClick);
  }

  componentDidUpdate(prevProps, prevState) {
    const { locale, isOpen, selectedDate } = this.props;
    let newState = {};

    if (locale !== prevProps.locale) {
      moment.locale(locale);
      newState = {
        mask: this.getMask(),
        value: moment(selectedDate).format("L")
      };
    }

    if (selectedDate !== prevProps.selectedDate) {
      newState = Object.assign({}, newState, {
        selectedDate
      });
    }

    if (this.state.isOpen !== prevState.isOpen) {
      handleAnyClick(this.state.isOpen, this.handleClick);
    }

    if (isOpen !== prevProps.isOpen) {
      newState = Object.assign({}, newState, {
        isOpen
      });
    }

    if (!isEmpty(newState)) {
      this.setState(newState);
    }
  }

  render() {
    const {
      isDisabled,
      isReadOnly,
      hasError,
      hasWarning,
      minDate,
      maxDate,
      locale,
      themeColor
    } = this.props;

    const { value, isOpen, mask, selectedDate } = this.state;

    return (
      <DateInputStyle ref={this.ref}>
        <InputBlock
          isDisabled={isDisabled}
          isReadOnly={isReadOnly}
          hasError={hasError}
          hasWarning={hasWarning}
          iconName={"CalendarIcon"}
          onIconClick={this.onIconClick.bind(this, !isOpen)}
          value={value}
          onChange={this.handleChange}
          placeholder={moment.localeData().longDateFormat("L")}
          mask={mask}
          keepCharPositions={true}
          //guide={true}
          //showMask={true}
        />
        {isOpen ? (
          <Col>
            <DropDown opened={isOpen}>
              {
                <NewCalendar
                  locale={locale}
                  themeColor={themeColor}
                  minDate={minDate}
                  maxDate={maxDate}
                  isDisabled={isDisabled}
                  openToDate={selectedDate}
                  selectedDate={selectedDate}
                  onChange={this.onChange}
                />
              }
            </DropDown>
          </Col>
        ) : null}
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
};

DatePicker.defaultProps = {
  minDate: new Date("1970/01/01"),
  maxDate: new Date(new Date().getFullYear() + 1, 1, 1)
};

export default DatePicker;
