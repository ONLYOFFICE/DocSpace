import React, { useRef, useState, useEffect } from "react";
import moment from "moment";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";

import Text from "@docspace/components/text";
import SelectorAddButton from "@docspace/components/selector-add-button";
import SelectedItem from "@docspace/components/selected-item";
import Calendar from "@docspace/components/calendar";

import { isMobileOnly } from "react-device-detect";

import { useTranslation } from "react-i18next";

import CalendarIconUrl from "PUBLIC_DIR/images/calendar.react.svg?url";
import CalendarIcon from "PUBLIC_DIR/images/calendar.react.svg";

const Wrapper = styled.div`
  .selectedItem {
    .calendarIcon {
      width: 12px;
      height: 12px;
      padding: 0 10px 0 2px;
      path {
        fill: #657077;
      }
    }
  }
`;

const DateSelector = styled.div`
  width: fit-content;
  cursor: pointer;

  display: flex;
  align-items: center;

  .mr-8 {
    margin-right: 8px;
  }
`;

const SelectedLabel = styled.span`
  display: flex;
  align-items: center;
`;

const StyledCalendar = styled(Calendar)`
  position: absolute;
  ${(props) =>
    props.isMobile &&
    css`
      position: fixed;
      bottom: 0;
      left: 0;
    `}
`;

const DatePicker = (props) => {
  const {
    date,
    onChange,
    selectDateText,
    className,
    id,
    minDate,
    maxDate,
    locale,
    showCalendarIcon,
  } = props;
  const { t } = useTranslation(["Webhooks"]);

  const calendarRef = useRef();
  const selectorRef = useRef();

  const [isCalendarOpen, setIsCalendarOpen] = useState(false);

  const toggleCalendar = () =>
    setIsCalendarOpen((prevIsCalendarOpen) => !prevIsCalendarOpen);

  const closeCalendar = () => {
    setIsCalendarOpen(false);
  };

  const deleteSelectedDate = (propKey, label, group, e) => {
    e.stopPropagation();
    onChange(null);
    setIsCalendarOpen(false);
  };

  const CalendarElement = () => (
    <StyledCalendar
      selectedDate={date}
      setSelectedDate={onChange}
      onChange={closeCalendar}
      isMobile={isMobileOnly}
      forwardedRef={calendarRef}
      minDate={minDate}
      maxDate={maxDate}
      locale={locale}
    />
  );

  const handleClick = (e) => {
    !selectorRef?.current?.contains(e.target) &&
      !calendarRef?.current?.contains(e.target) &&
      setIsCalendarOpen(false);
  };

  useEffect(() => {
    document.addEventListener("click", handleClick, { capture: true });
    return () =>
      document.removeEventListener("click", handleClick, { capture: true });
  }, []);

  return (
    <Wrapper className={className} id={id}>
      {date === null ? (
        <>
          <DateSelector onClick={toggleCalendar} ref={selectorRef}>
            <SelectorAddButton
              title={t("Select")}
              className="mr-8"
              iconName={CalendarIconUrl}
            />
            <Text isInline fontWeight={600} color="#A3A9AE">
              {selectDateText}
            </Text>
          </DateSelector>
          {isCalendarOpen && <CalendarElement />}
        </>
      ) : (
        <SelectedItem
          className="selectedItem"
          onClose={deleteSelectedDate}
          label={
            showCalendarIcon ? (
              <SelectedLabel>
                <CalendarIcon className="calendarIcon" />
                {date.format("DD MMM YYYY")}
              </SelectedLabel>
            ) : (
              date.format("DD MMM YYYY")
            )
          }
        />
      )}
    </Wrapper>
  );
};

DatePicker.propTypes = {
  /** Allows to change select date text */
  selectDateText: PropTypes.string,
  /** Date object */
  date: PropTypes.oneOfType([PropTypes.object.isRequired, null]),
  /** Allow you to handle changing events of component */
  onChange: PropTypes.func.isRequired,
  /** Allows to set classname */
  className: PropTypes.string,
  /** Allows to set id */
  id: PropTypes.string,
  /** Specifies min choosable calendar date */
  minDate: PropTypes.object,
  /** Specifies max choosable calendar date */
  maxDate: PropTypes.object,
  /** Specifies calendar locale */
  locale: PropTypes.string,
  /** Shows calendar icon in selected item */
  showCalendarIcon: PropTypes.bool,
};

DatePicker.defaultProps = {
  selectDateText: "Select date",
  showCalendarIcon: true,
};

export default DatePicker;
