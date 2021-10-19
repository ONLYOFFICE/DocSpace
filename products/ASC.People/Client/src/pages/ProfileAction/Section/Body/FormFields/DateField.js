import React from "react";
import equal from "fast-deep-equal/react";
import FieldContainer from "@appserver/components/field-container";
import DatePicker from "@appserver/components/date-picker";

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
      locale,
      maxLabelWidth,
    } = this.props;

    return (
      <FieldContainer
        isRequired={isRequired}
        hasError={hasError}
        labelText={labelText}
        maxLabelWidth={maxLabelWidth}
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
          locale={locale}
        />
      </FieldContainer>
    );
  }
}

export default DateField;
