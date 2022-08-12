import styled, { css } from "styled-components";
import Base from "../themes/base";
import NoUserSelect from "../utils/commonStyles";

const HoverStyle = css`
  &:hover {
    background-color: ${(props) => props.theme.calendar.hover.backgroundColor};
    border-radius: ${(props) => props.theme.calendar.hover.borderRadius};
    cursor: ${(props) => props.theme.calendar.hover.cursor};
  }
`;

const ComboBoxStyle = styled.div`
  position: relative;
  display: flex;
  padding-bottom: 24px !important;
`;

const ComboBoxMonthStyle = styled.div`
  width: ${(props) =>
    props.size === "base"
      ? props.theme.calendar.comboBoxMonth.baseWidth
      : props.theme.calendar.comboBoxMonth.bigWidth};
  max-width: ${(props) =>
    props.size === "base"
      ? props.theme.calendar.comboBoxMonth.baseMaxWidth
      : props.theme.calendar.comboBoxMonth.bigMaxWidth};
`;
ComboBoxMonthStyle.defaultProps = { theme: Base };

const ComboBoxDateStyle = styled.div`
  min-width: ${(props) => props.theme.calendar.comboBox.minWidth};
  height: ${(props) => props.theme.calendar.comboBox.height};
  margin-left: ${(props) => props.theme.calendar.comboBox.marginLeft};
`;
ComboBoxDateStyle.defaultProps = { theme: Base };

const CalendarContainer = styled.div`
  max-width: ${(props) =>
    props.size === "base"
      ? props.theme.calendar.baseMaxWidth
      : props.theme.calendar.bigMaxWidth};
`;
CalendarContainer.defaultProps = { theme: Base };

const CalendarStyle = styled.div`
  width: ${(props) =>
    props.size === "base"
      ? props.theme.calendar.baseWidth
      : props.theme.calendar.bigWidth};

  box-sizing: content-box;

  .calendar-month {
    ${HoverStyle}
  }

  .calendar-month_neighboringMonth {
    color: ${(props) => props.theme.calendar.month.neighboringColor};

    ${HoverStyle}
    &:hover {
      color: ${(props) => props.theme.calendar.month.neighboringHoverColor};
    }
  }

  .calendar-month_disabled {
    ${NoUserSelect}
    color: ${(props) => props.theme.calendar.month.disabledColor};
    pointer-events: none;
  }

  .calendar-month_weekend {
    color: ${(props) => props.theme.calendar.month.weekendColor};
    ${HoverStyle}
  }

  .calendar-month_selected-day {
    border-radius: ${(props) => props.theme.calendar.selectedDay.borderRadius};
    cursor: ${(props) => props.theme.calendar.selectedDay.cursor};
    color: ${(props) => props.theme.calendar.selectedDay.color};
  }
`;
CalendarStyle.defaultProps = { theme: Base };

const Month = styled.div`
  width: ${(props) =>
    props.size === "base"
      ? props.theme.calendar.month.baseWidth
      : props.theme.calendar.month.bigWidth};
`;
Month.defaultProps = { theme: Base };

const StyledDay = styled.div`
  display: flex;
  flex-basis: 14.2857%; /*(1/7*100%)*/
  text-align: center;
  line-height: ${(props) => props.theme.calendar.day.lineHeight};
  user-select: none;
  margin-top: ${(props) =>
    props.size === "base"
      ? props.theme.calendar.day.baseMarginTop
      : props.theme.calendar.day.bigMarginTop};
`;
StyledDay.defaultProps = { theme: Base };

const DayContent = styled.div`
  width: ${(props) => props.theme.calendar.day.width};
  height: ${(props) => props.theme.calendar.day.height};
  .textStyle {
    text-align: center;
  }
`;
DayContent.defaultProps = { theme: Base };

const StyledDays = styled.div`
  display: flex;
  flex-wrap: wrap;
  width: ${(props) =>
    props.size === "base"
      ? props.theme.calendar.day.baseSizeWidth
      : props.theme.calendar.day.bigSizeWidth};
`;
StyledDays.defaultProps = { theme: Base };

const StyledWeekdays = styled.div`
  width: ${(props) =>
    props.size === "base"
      ? props.theme.calendar.weekdays.baseWidth
      : props.theme.calendar.weekdays.bigWidth};
  display: flex;
  margin-bottom: ${(props) => props.theme.calendar.weekdays.marginBottom};
`;
StyledWeekdays.defaultProps = { theme: Base };

const StyledWeekday = styled.div`
  overflow: hidden;
  flex-basis: 14.2857%; /*(1/7*100%)*/
  user-select: none;

  .calendar-weekday_text {
    color: ${(props) =>
      props.disable
        ? props.theme.calendar.weekdays.disabledColor
        : props.theme.calendar.weekdays.color};
    width: ${(props) => props.theme.calendar.day.width};
    height: ${(props) => props.theme.calendar.day.height};
    text-align: center;
  }
`;
StyledWeekday.defaultProps = { theme: Base };

StyledWeekday.defaultProps = { theme: Base };

export {
  Month,
  CalendarStyle,
  CalendarContainer,
  ComboBoxDateStyle,
  ComboBoxMonthStyle,
  ComboBoxStyle,
  StyledDay,
  DayContent,
  StyledDays,
  StyledWeekdays,
  StyledWeekday,
};
