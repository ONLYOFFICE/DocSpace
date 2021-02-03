import React from "react";
import equal from "fast-deep-equal/react";
import { FieldContainer, DatePicker } from "asc-web-components";

class DateField extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    console.log("DateField render");

    const {
      isRequired,
      hasError,
      labelText,
      calendarHeaderContent,
      inputName,
      inputValue,
      inputIsDisabled,
      inputOnChange,
      inputTabIndex,
      calendarMinDate,
    } = this.props;

    return (
      <FieldContainer
        isRequired={isRequired}
        hasError={hasError}
        labelText={labelText}
      >
        <DatePicker
          name={inputName}
          selectedDate={inputValue}
          isDisabled={inputIsDisabled}
          onChange={inputOnChange}
          hasError={hasError}
          tabIndex={inputTabIndex}
          displayType="auto"
          calendarHeaderContent={calendarHeaderContent}
          minDate={calendarMinDate ? calendarMinDate : new Date("1900/01/01")}
          maxDate={new Date()}
        />
      </FieldContainer>
    );
  }
}

export default DateField;
