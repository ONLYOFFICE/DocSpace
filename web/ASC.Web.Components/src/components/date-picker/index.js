import React, { Component } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import InputBlock from "../input-block";
import DropDown from "../drop-down";
import NewCalendar from "../calendar-new";
import moment from "moment";
import { handleAnyClick } from "../../utils/event";
import isEmpty from "lodash/isEmpty";

const DateInputStyle = styled.div`
  max-width: 110px;
  width: 110px;
`;

const DropDownStyle = styled.div`
  position: relative;
`;

class DatePicker extends Component {
  constructor(props) {
    super(props);

    moment.locale(props.locale);
    this.ref = React.createRef();

    const { isOpen, selectedDate, hasError } = this.props;

    if (isOpen) {
      handleAnyClick(true, this.handleClick);
    }

    this.state = {
      isOpen,
      selectedDate: moment(selectedDate).toDate(),
      value: moment(selectedDate).format("L"),
      mask: this.getMask,
      hasError
    };
  }

  handleClick = e => {
    this.state.isOpen &&
      !this.ref.current.contains(e.target) &&
      this.onClick(false);
  };

  handleChange = e => {
    const { value } = this.state;

    const targetValue = e.target.value;
    if (value != targetValue) {
      let newState = { value: targetValue };
      const format = moment.localeData().longDateFormat("L");
      const momentDate = moment(targetValue, format);
      const date = momentDate.toDate();

      if (
        !isNaN(date) &&
        this.compareDates(date) &&
        targetValue.indexOf("_") === -1
      ) {
        //console.log("Mask complete");
        this.props.onChange && this.props.onChange(date);
        newState = Object.assign({}, newState, {
          selectedDate: date,
          hasError: false
        });
      } else if (targetValue.indexOf("_") !== -1 && targetValue.length !== 0) {
        //hasWarning
        newState = Object.assign({}, newState, {
          hasError: true
        });
      } else {
        newState = Object.assign({}, newState, {
          hasError: true,
          isOpen: false
        });
      }
      this.setState(newState);
    }
  };

  onChange = value => {
    this.onClick(!this.state.isOpen);
    const formatValue = moment(value).format("L");
    this.props.onChange && this.props.onChange(value);
    this.setState({ selectedDate: value, value: formatValue });
  };

  onClick = isOpen => {
    if (!this.state.hasError) {
      this.setState({ isOpen });
    }
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
    const localeMask = moment.localeData().longDateFormat("L");
    const { locale } = this.props;

    if (localeMask.indexOf("/") + 1) {
      symbol = "/";
    } else if (localeMask.indexOf(".") + 1) {
      symbol = ".";
    } else if (localeMask.indexOf("-") + 1) {
      symbol = "-";
    }

    const mask = [
      /\d/,
      /\d/,
      symbol,
      /\d/,
      /\d/,
      symbol,
      /\d/,
      /\d/,
      /\d/,
      /\d/
    ];

    if (localeMask[0] === "Y") {
      mask.reverse();
    }
    if (locale === "ko" || locale === "lv") {
      mask.push(symbol);
    }
    return mask;
  };

  compareDates = (date1, date2) => {
    return moment(date1)
      .startOf("day")
      .diff(moment(date2).startOf("day"), "days");
  };

  componentWillUnmount() {
    handleAnyClick(false, this.handleClick);
  }

  componentDidUpdate(prevProps, prevState) {
    const { locale, isOpen, selectedDate, maxDate, minDate } = this.props;
    const { hasError, value } = this.state;
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
        selectedDate,
        value: moment(selectedDate).format("L")
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

    if (this.props.hasError !== prevProps.hasError) {
      newState = Object.assign({}, newState, {
        hasError: this.props.hasError,
        isOpen: false
      });
    }

    const date = new Date(value);
    if (
      this.compareDates(selectedDate, maxDate) <= 0 &&
      this.compareDates(selectedDate, minDate) >= 0 &&
      hasError &&
      this.compareDates(date, maxDate) <= 0 &&
      this.compareDates(date, minDate) >= 0 &&
      !this.props.hasError
    ) {
      newState = Object.assign({}, newState, {
        hasError: false,
        selectedDate,
        value: moment(selectedDate).format("L")
      });
    }

    if (
      (this.compareDates(selectedDate, maxDate) > 0 ||
        this.compareDates(selectedDate, minDate) < 0) &&
      !hasError
    ) {
      newState = Object.assign({}, newState, {
        hasError: true,
        isOpen: false
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
      //hasWarning,
      minDate,
      maxDate,
      locale,
      themeColor
    } = this.props;

    const { value, isOpen, mask, selectedDate, hasError } = this.state;

    return (
      <DateInputStyle ref={this.ref}>
        <InputBlock
          scale={true}
          isDisabled={isDisabled}
          isReadOnly={isReadOnly}
          hasError={hasError}
          onFocus={this.onClick.bind(this, true)}
          //hasWarning={hasWarning}
          iconName={"CalendarIcon"}
          onIconClick={this.onClick.bind(this, !isOpen)}
          value={value}
          onChange={this.handleChange}
          mask={mask}
          keepCharPositions={true}
          //guide={true}
          //showMask={true}
        />
        {isOpen ? (
          <DropDownStyle>
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
          </DropDownStyle>
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
  maxDate: new Date(new Date().getFullYear() + 1, 1, 1),
  selectedDate: moment(new Date()).toDate()
};

export default DatePicker;
