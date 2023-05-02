import React from "react";
import ToggleBlock from "./ToggleBlock";

const LimitTimeBlock = (props) => {
  const { isChecked } = props;

  const DateTimePicker = () => <></>;

  return (
    <ToggleBlock {...props}>
      {isChecked ? <DateTimePicker /> : <></>}
    </ToggleBlock>
  );
};

export default LimitTimeBlock;
