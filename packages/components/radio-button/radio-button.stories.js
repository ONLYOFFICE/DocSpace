import React, { useState, useEffect } from "react";
import RadioButton from ".";

export default {
  title: "Components/RadioButton",
  component: RadioButton,
  parameters: {
    docs: {
      description: { component: "RadioButton allow you to add radiobutton" },
    },
  },
  argTypes: {},
};

const Template = ({ isChecked, ...args }) => {
  const [checked, setIsChecked] = useState(isChecked);

  useEffect(() => {
    setIsChecked(isChecked);
  }, [isChecked]);

  const onChangeHandler = (e) => {
    setIsChecked(e.target.checked);
  };

  return (
    <RadioButton {...args} isChecked={checked} onChange={onChangeHandler} />
  );
};

export const Default = Template.bind({});
Default.args = {
  value: "value",
  name: "name",
  label: "Label",
  fontSize: "13px",
  fontWeight: "400",
  isDisabled: false,
  isChecked: false,
};
