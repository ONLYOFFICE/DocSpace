import React, { useState } from "react";
import PhoneInput from ".";

const Template = ({ onChange, ...args }) => {
  const [value, setValue] = useState("");

  const onChangeHandler = (e) => {
    onChange(e.currentTarget.value);
    setValue(e.currentTarget.value);
  };

  return (
    <div style={{ height: "500px", display: "grid", gridGap: "24px" }}>
      <PhoneInput {...args} value={value} onChange={onChangeHandler} />
    </div>
  );
};

export const basic = Template.bind({});
basic.args = {
  locale: "GB",
};


