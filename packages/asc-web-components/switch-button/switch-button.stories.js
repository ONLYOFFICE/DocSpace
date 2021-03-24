import React, { useState } from "react";
import SwitchButton from "./";
import Box from "../box";

export default {
  title: "Components/SwitchButton",
  component: SwitchButton,
  parameters: {
    docs: {
      description: {
        component: "Actions with a button.",
      },
    },
  },
  argTypes: {
    onChange: {
      action: "onChange",
    },
  },
};

const Template = ({ checked, onChange, ...rest }) => {
  const [isChecked, setIsChecked] = useState(checked);
  return (
    <Box paddingProp="16px">
      <SwitchButton
        {...rest}
        checked={isChecked}
        onChange={(e) => {
          onChange(e.target.checked);
          setIsChecked(!isChecked);
        }}
      />
    </Box>
  );
};

export const Default = Template.bind({});
Default.args = {
  checked: false,
  disabled: false,
};
