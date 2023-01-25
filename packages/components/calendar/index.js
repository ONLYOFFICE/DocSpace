import React, { Component } from "react";
import PropTypes from "prop-types";

import ComboBox from "../combobox";
import moment from "moment";
import { Weekdays, Days } from "./sub-components";
import isEmpty from "lodash/isEmpty";
import {
  Month,
  CalendarStyle,
  CalendarContainer,
  ComboBoxDateStyle,
  ComboBoxMonthStyle,
  ComboBoxStyle,
  StyledRoundButton,
  StyledNextIcon,
  StyledPrevIcon,
  StyledCalendarHeader,
  StyledTitle,
} from "./styled-calendar.js";

import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

import {
  compareDates,
  compareMonths,
  getFirstMonthDay,
  getArrayYears,
  getCurrentMonth,
  getCurrentYear,
  getDays,
  getListMonth,
  getWeekDays,
} from "./utils";

class Calendar extends Component {
  constructor(props) {
    super(props);

    moment.locale(props.locale);
    this.state = this.mapPropsToState(props);
  }

  mapPropsToState = (props) => {
    const { minDate, maxDate, openToDate, selectedDate } = props;
    const months = moment.months();
    const arrayWeekdays = moment.weekdaysMin();
    const optionsMonth = getListMonth(minDate, maxDate, openToDate, months);
    const optionsYear = getArrayYears(minDate, maxDate);
    const optionsDays = getDays(minDate, maxDate, openToDate, selectedDate);
    const optionsWeekdays = getWeekDays(arrayWeekdays, this.props);
    let newOpenToDate = openToDate;
    if (
      compareDates(openToDate, maxDate) > 0 ||
      compareDates(openToDate, minDate) < 0
    ) {
      newOpenToDate = minDate;
    }

    const newState = {
      months,
      minDate,
      maxDate,
      openToDate: newOpenToDate,
      selectedDate,
      optionsMonth,
      selectedOptionMonth: getCurrentMonth(optionsMonth, newOpenToDate),
      optionsYear,
      selectedOptionYear: getCurrentYear(optionsYear, newOpenToDate),
      optionsDays,
      optionsWeekdays,
    };

    //console.log("mapPropsToState ", newState);
    return newState;
  };

  onSelectYear = (value) => {
    const openToDate = new Date(value.key, this.state.openToDate.getMonth());

    const optionsMonth = getListMonth(
      this.state.minDate,
      this.state.maxDate,
      openToDate,
      this.state.months
    );

    const openToDateMonth = openToDate.getMonth();
    const openToDateYear = openToDate.getFullYear();
    let selectedMonth = optionsMonth.find((x) => x.key == openToDateMonth);
    let newOpenToDate = openToDate;

    if (selectedMonth.disabled === true) {
      selectedMonth = optionsMonth.find((x) => x.disabled === false);
      newOpenToDate = new Date(openToDateYear, selectedMonth.key, 1);
    }

    const newState = this.mapPropsToState({
      ...this.state,
      openToDate: newOpenToDate,
      optionsMonth,
    });

    this.setState(newState);
  };

  onSelectMonth = (value) => {
    const newState = this.mapPropsToState({
      ...this.state,
      openToDate: new Date(this.state.openToDate.getFullYear(), value.key),
    });
    //console.log("onSelectMonth", newState);
    this.setState(newState);
  };

  onDayClick = (dayItem) => {
    //console.log("onDayClick", dayItem);
    const day = dayItem.value;
    const currentMonth = this.state.openToDate.getMonth();
    const currentYear = this.state.openToDate.getFullYear();
    const dateInCurrentMonth = new Date(currentYear, currentMonth, day);
    let newState;

    if (dayItem.dayState === "prev") {
      const dateInPrevMonth = new Date(currentYear, currentMonth - 1, day);
      newState = this.mapPropsToState({
        ...this.state,
        selectedDate: dateInPrevMonth,
        openToDate: dateInPrevMonth,
      });
    } else if (dayItem.dayState === "next") {
      const dateInNextMonth = new Date(currentYear, currentMonth + 1, day);
      newState = this.mapPropsToState({
        ...this.state,
        selectedDate: dateInNextMonth,
        openToDate: dateInNextMonth,
      });
    } else {
      newState = this.mapPropsToState({
        ...this.state,
        selectedDate: dateInCurrentMonth,
      });
    }

    this.setState(newState);
    this.props.onChange && this.props.onChange(newState.selectedDate);
  };

  componentDidUpdate(prevProps) {
    const { selectedDate, openToDate, minDate, maxDate, locale } = this.props;

    let newState = {};

    if (compareDates(selectedDate, prevProps.selectedDate) !== 0) {
      newState = { selectedDate };
    }

    if (compareDates(openToDate, prevProps.openToDate) !== 0) {
      newState = Object.assign({}, newState, {
        openToDate,
      });
    }

    if (compareDates(minDate, prevProps.minDate) !== 0) {
      newState = Object.assign({}, newState, {
        minDate,
      });
    }

    if (compareDates(maxDate, prevProps.maxDate) !== 0) {
      newState = Object.assign({}, newState, {
        maxDate,
      });
    }

    if (!isEmpty(newState) || locale !== prevProps.locale) {
      moment.locale(locale);
      newState = this.mapPropsToState({
        ...this.state,
        ...newState,
      });

      //console.log("componentDidUpdate newState", newState);
      this.setState(newState);
    }
  }

  render() {
    //console.log("Calendar render");

    const { size, themeColor, style, id, className } = this.props;
    const {
      optionsMonth,
      selectedOptionMonth,
      selectedOptionYear,
      optionsYear,
      optionsDays,
      optionsWeekdays,
    } = this.state;

    const dropDownSizeMonth = optionsMonth.length > 4 ? 184 : undefined;
    const dropDownSizeYear = optionsYear.length > 4 ? 184 : undefined;

    return (
      <CalendarContainer
        className={className}
        id={id}
        style={style}
        size={size}
      >
        <ColorTheme size={size} color={themeColor} themeId={ThemeType.Calendar}>
          <ComboBoxStyle>
            <StyledCalendarHeader>
              <StyledRoundButton>
                <StyledPrevIcon />
              </StyledRoundButton>

              <StyledTitle>March 2021</StyledTitle>

              <StyledRoundButton>
                <StyledNextIcon />
              </StyledRoundButton>
            </StyledCalendarHeader>

            {/* <ComboBoxMonthStyle size={size}>
              <ComboBox
                isDefaultMode={false}
                scaled={true}
                scaledOptions={true}
                dropDownMaxHeight={dropDownSizeMonth}
                onSelect={this.onSelectMonth}
                selectedOption={selectedOptionMonth}
                options={optionsMonth}
                isDisabled={false}
                name="month-button"
                displaySelectedOption
                fixedDirection={true}
                directionY="bottom"
              />
            </ComboBoxMonthStyle>
            <ComboBoxDateStyle>
              <ComboBox
                isDefaultMode={false}
                scaled={true}
                scaledOptions={true}
                dropDownMaxHeight={dropDownSizeYear}
                onSelect={this.onSelectYear}
                selectedOption={selectedOptionYear}
                options={optionsYear}
                isDisabled={false}
                showDisabledItems
                name="year-button"
                displaySelectedOption
                fixedDirection={true}
                directionY="bottom"
              />
            </ComboBoxDateStyle> */}
          </ComboBoxStyle>

          <Month size={size}>
            <Weekdays optionsWeekdays={optionsWeekdays} size={size} />
            <Days
              optionsDays={optionsDays}
              size={size}
              onDayClick={this.onDayClick}
            />
          </Month>
        </ColorTheme>
      </CalendarContainer>
    );
  }
}

Calendar.propTypes = {
  /** Color of the selected day */
  themeColor: PropTypes.string,
  /** Selected date value */
  selectedDate: PropTypes.instanceOf(Date),
  /** The beginning of a period that shall be displayed by default */
  openToDate: PropTypes.instanceOf(Date),
  /** Minimum date that the user can select. */
  minDate: PropTypes.instanceOf(Date),
  /** Maximum date that the user can select */
  maxDate: PropTypes.instanceOf(Date),
  /** Browser locale */
  locale: PropTypes.string,
  /** Calendar size */
  size: PropTypes.oneOf(["base", "big"]),
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Function called when the user select a day */
  onChange: PropTypes.func,
};

Calendar.defaultProps = {
  selectedDate: new Date(),
  openToDate: new Date(),
  minDate: new Date("1970/01/01"),
  maxDate: new Date(new Date().getFullYear() + 1 + "/01/01"),
  locale: moment.locale(),
  size: "base",
};

export default Calendar;
