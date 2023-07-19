import React from "react";
import ToggleBlock from "./ToggleBlock";
import DateTimePicker from "@docspace/components/date-time-picker";

const LimitTimeBlock = (props) => {
  const {
    isLoading,
    expirationDate,
    setExpirationDate,
    setIsExpired,
    isExpired,
  } = props;

  const onChange = (date) => {
    const isExpired = date
      ? new Date(date).getTime() <= new Date().getTime()
      : false;

    setExpirationDate(date);
    setIsExpired(isExpired);
  };

  // const minDate = new Date(new Date().getTime());
  // minDate.setDate(new Date().getDate() - 1);
  // minDate.setTime(minDate.getTime() + 60 * 60 * 1000);
  const minDate = new Date();

  return (
    <ToggleBlock {...props} withToggle={false}>
      <DateTimePicker
        className="public-room_date-picker"
        isDisabled={isLoading}
        initialDate={expirationDate}
        onChange={onChange}
        minDate={minDate}
        openDate={new Date()}
        hasError={isExpired}
      />
    </ToggleBlock>
  );
};

export default LimitTimeBlock;
