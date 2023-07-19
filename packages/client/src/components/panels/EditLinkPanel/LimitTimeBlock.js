import React from "react";
import ToggleBlock from "./ToggleBlock";
import DateTimePicker from "@docspace/components/date-time-picker";

const LimitTimeBlock = (props) => {
  const { isLoading, expirationDate, setExpirationDate, isExpired } = props;

  const onChange = (date) => {
    setExpirationDate(date);
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
        // initialTime={minDate}
      />
    </ToggleBlock>
  );
};

export default LimitTimeBlock;
