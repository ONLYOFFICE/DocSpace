import React, { useState, useEffect } from "react";
import RadioButton from ".";

export default {
  title: "Components/RadioButton",
  component: RadioButton,
  parameters: {
    docs: {
      description: { component: "RadioButton allow you to add radiobutton" },
    },
    design: {
      type: "figma",
      url: "https://www.figma.com/file/ZiW5KSwb4t7Tj6Nz5TducC/UI-Kit-DocSpace-1.0.0?type=design&node-id=556-3247&mode=design&t=TBNCKMQKQMxr44IZ-0",
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
