import React from "react";
import ToggleBlock from "./ToggleBlock";
import DateTimePicker from "@docspace/components/date-time-picker";

const LimitTimeBlock = (props) => {
  const { isLoading, expirationDate, setExpirationDate, isExpired } = props;

  const onChange = (date) => {
    setExpirationDate(date);
  };

  const minDate = new Date(new Date().getTime());
  minDate.setDate(new Date().getDate() - 1);

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
