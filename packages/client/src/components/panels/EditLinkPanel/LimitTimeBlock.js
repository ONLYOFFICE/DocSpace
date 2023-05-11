import React from "react";
import ToggleBlock from "./ToggleBlock";

const LimitTimeBlock = (props) => {
  const { isChecked, isLoading } = props;

  const DateTimePicker = () => <></>;

  return (
    <ToggleBlock {...props}>
      {isChecked ? <DateTimePicker isDisabled={isLoading} /> : <></>}
    </ToggleBlock>
  );
};

export default LimitTimeBlock;
