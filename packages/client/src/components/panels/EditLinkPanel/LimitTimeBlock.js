import React from "react";
import ToggleBlock from "./ToggleBlock";
import DateTimePicker from "@docspace/components/date-time-picker";

const LimitTimeBlock = (props) => {
  const { isLoading, expirationDate, setExpirationDate } = props;

  const onChange = (date) => {
    setExpirationDate(date);
  };

  return (
    <ToggleBlock {...props} withToggle={false}>
      <DateTimePicker
        className="public-room_date-picker"
        isDisabled={isLoading}
        initialDate={expirationDate}
        onChange={onChange}
      />
    </ToggleBlock>
  );
};

export default LimitTimeBlock;
