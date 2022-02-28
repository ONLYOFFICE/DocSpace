import React, { Component } from "react";
import PropTypes from "prop-types";

import InputBlock from "../input-block";
import DropDown from "../drop-down";
import Calendar from "../calendar";
import moment from "moment";
import { handleAnyClick } from "../utils/event";
import isEmpty from "lodash/isEmpty";
import Aside from "../aside";
import { desktop } from "../utils/device";
import Backdrop from "../backdrop";
import Heading from "../heading";
import throttle from "lodash/throttle";
import {
  Body,
  Header,
  Content,
  DropDownStyle,
  DateInputStyle,
} from "./styled-date-picker";

class DatePicker extends Component {
  constructor(props) {
    super(props);

    moment.locale(props.locale);
    this.ref = React.createRef();

    const { isOpen, selectedDate, hasError, minDate, maxDate } = this.props;

    if (isOpen) {
      handleAnyClick(true, this.handleClick);
    }

    let newState = {
      isOpen,
      selectedDate: moment(selectedDate).toDate(),
      value: moment(selectedDate).format("L"),
      mask: this.getMask,
      hasError,
      displayType: this.getTypeByWidth(),
    };

    if (this.isValidDate(selectedDate, maxDate, minDate, hasError)) {
      newState = Object.assign({}, newState, {
        hasError: true,
        isOpen: false,
      });
    }

    this.state = newState;
    this.throttledResize = throttle(this.resize, 300);
  }

  handleClick = (e) => {
    this.state.isOpen &&
      !this.ref.current.contains(e.target) &&
      this.onClick(false);
  };

  handleChange = (e) => {
    const { value } = this.state;

    const targetValue = e.target.value;
    if (value != targetValue) {
      let newState = { value: targetValue };
      const format = moment.localeData().longDateFormat("L");
      const momentDate = moment(targetValue, format);
      const date = momentDate.toDate();

      if (
        !isNaN(date) &&
        this.compareDate(date) &&
        targetValue.indexOf("_") === -1
      ) {
        //console.log("Mask complete");
        this.props.onChange && this.props.onChange(date);
        newState = Object.assign({}, newState, {
          selectedDate: date,
          hasError: false,
        });
      } else if (targetValue.indexOf("_") !== -1 && targetValue.length !== 0) {
        //hasWarning
        newState = Object.assign({}, newState, {
          hasError: true,
        });
      } else {
        newState = Object.assign({}, newState, {
          hasError: true,
          isOpen: false,
        });
      }
      this.setState(newState);
    }
  };

  onChange = (value) => {
    const formatValue = moment(value).format("L");
    this.props.onChange && this.props.onChange(value);
    this.setState({
      selectedDate: value,
      value: formatValue,
      hasError: false,
      isOpen: !this.state.isOpen,
    });
  };

  onClick = () => {
    this.setState({ isOpen: !this.state.isOpen });
  };

  onClose = () => {
    this.setState({ isOpen: false });
  };

  compareDate = (date) => {
    const { minDate, maxDate } = this.props;
    if (date < minDate || date > maxDate) {
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
      /\d/,
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

  isValidDate = (selectedDate, maxDate, minDate, hasError) => {
    if (
      (this.compareDates(selectedDate, maxDate) > 0 ||
        this.compareDates(selectedDate, minDate) < 0) &&
      !hasError
    ) {
      return true;
    }
    return false;
  };

  getTypeByWidth = () => {
    if (this.props.displayType !== "auto") return this.props.displayType;
    return window.innerWidth < desktop.match(/\d+/)[0] ? "aside" : "dropdown";
  };

  resize = () => {
    if (this.props.displayType !== "auto") return;
    const type = this.getTypeByWidth();
    if (type === this.state.displayType) return;
    this.setState({ displayType: type });
  };

  popstate = () => {
    window.removeEventListener("popstate", this.popstate, false);
    this.onClose();
    window.history.go(1);
  };

  componentDidMount() {
    window.addEventListener("resize", this.throttledResize);
  }

  componentWillUnmount() {
    window.removeEventListener("resize", this.throttledResize);
    handleAnyClick(false, this.handleClick);
  }

  componentDidUpdate(prevProps, prevState) {
    const {
      locale,
      isOpen,
      selectedDate,
      maxDate,
      minDate,
      displayType,
    } = this.props;
    const { hasError, value } = this.state;
    let newState = {};

    if (locale !== prevProps.locale) {
      moment.locale(locale);
      newState = {
        mask: this.getMask(),
        value: moment(selectedDate).format("L"),
      };
    }

    if (selectedDate !== prevProps.selectedDate) {
      newState = Object.assign({}, newState, {
        selectedDate,
        value: moment(selectedDate).format("L"),
      });
    }

    if (this.state.isOpen !== prevState.isOpen) {
      handleAnyClick(this.state.isOpen, this.handleClick);
    }

    if (isOpen !== prevProps.isOpen) {
      newState = Object.assign({}, newState, {
        isOpen,
      });
    }

    if (this.props.hasError !== prevProps.hasError) {
      newState = Object.assign({}, newState, {
        hasError: this.props.hasError,
        isOpen: false,
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
        value: moment(selectedDate).format("L"),
      });
    }

    if (
      this.isValidDate(selectedDate, maxDate, minDate, hasError) &&
      this.isValidDate(this.state.selectedDate, maxDate, minDate, hasError)
    ) {
      newState = Object.assign({}, newState, {
        hasError: true,
        isOpen: false,
      });
    }

    if (displayType !== prevProps.displayType) {
      newState = Object.assign({}, newState, {
        displayType: this.getTypeByWidth(),
      });
    }

    if (isOpen && this.state.displayType === "aside") {
      window.addEventListener("popstate", this.popstate, false);
    }

    if (!isEmpty(newState)) {
      this.setState(newState);
    }
  }

  renderBody = () => {
    const { isDisabled, minDate, maxDate, locale, themeColor } = this.props;
    const { selectedDate, displayType } = this.state;

    let calendarSize;
    displayType === "aside" ? (calendarSize = "big") : (calendarSize = "base");

    return (
      <Calendar
        locale={locale}
        themeColor={themeColor}
        minDate={minDate}
        maxDate={maxDate}
        isDisabled={isDisabled}
        openToDate={selectedDate}
        selectedDate={selectedDate}
        onChange={this.onChange}
        size={calendarSize}
      />
    );
  };

  render() {
    const {
      isDisabled,
      isReadOnly,
      zIndex,
      calendarHeaderContent,
      id,
      style,
      className,
      inputClassName,
      fixedDirection,
    } = this.props;
    const { value, isOpen, mask, hasError, displayType } = this.state;

    return (
      <DateInputStyle
        ref={this.ref}
        id={id}
        className={className}
        style={style}
      >
        <InputBlock
          className={inputClassName}
          scale={true}
          isDisabled={isDisabled}
          isReadOnly={isReadOnly}
          hasError={hasError}
          iconName="/static/images/calendar.react.svg"
          iconColor="#A3A9AE"
          hoverColor="#A3A9AE"
          onIconClick={this.onClick}
          value={value}
          onChange={this.handleChange}
          mask={mask}
          keepCharPositions={true}
          //guide={true}
          //showMask={true}
        />
        {isOpen ? (
          displayType === "dropdown" ? (
            <DropDownStyle>
              <DropDown
                forwardedRef={this.ref}
                className="drop-down"
                open={isOpen}
                clickOutsideAction={this.onClose}
                isDefaultMode={false}
                fixedDirection={fixedDirection}
              >
                {this.renderBody()}
              </DropDown>
            </DropDownStyle>
          ) : (
            <>
              <Backdrop
                onClick={this.onClose}
                visible={isOpen}
                zIndex={zIndex}
                isAside={true}
              />
              <Aside visible={isOpen} scale={false} zIndex={zIndex}>
                <Content>
                  <Header>
                    <Heading className="header" size="medium" truncate={true}>
                      {calendarHeaderContent}
                    </Heading>
                  </Header>
                  <Body>{this.renderBody()}</Body>
                </Content>
              </Aside>
            </>
          )
        ) : null}
      </DateInputStyle>
    );
  }
}

DatePicker.propTypes = {
  /** Function called when the user select a day */
  onChange: PropTypes.func,
  /** Color of the selected day */
  themeColor: PropTypes.string,
  /** Selected date value */
  selectedDate: PropTypes.instanceOf(Date),
  /** Opened date value */
  openToDate: PropTypes.instanceOf(Date),
  /** Minimum date that the user can select */
  minDate: PropTypes.instanceOf(Date),
  /** Maximum date that the user can select */
  maxDate: PropTypes.instanceOf(Date),
  /** Browser locale */
  locale: PropTypes.string,
  /** Disabled react-calendar */
  isDisabled: PropTypes.bool,
  /** Set input type is read only */
  isReadOnly: PropTypes.bool,
  /** Set error date-input style */
  hasError: PropTypes.bool,
  //hasWarning: PropTypes.bool,
  /** Opens calendar */
  isOpen: PropTypes.bool,
  /** Calendar size */
  calendarSize: PropTypes.oneOf(["base", "big"]),
  /** Calendar display type */
  displayType: PropTypes.oneOf(["dropdown", "aside", "auto"]),
  /** Calendar css z-index */
  zIndex: PropTypes.number,
  /** Calendar header content (calendar opened in aside) */
  calendarHeaderContent: PropTypes.string,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts input class */
  inputClassName: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  fixedDirection: PropTypes.bool,
};

DatePicker.defaultProps = {
  minDate: new Date("1970/01/01"),
  maxDate: new Date(new Date().getFullYear() + 1, 1, 1),
  selectedDate: moment(new Date()).toDate(),
  displayType: "auto",
  zIndex: 310,
};

export default DatePicker;
