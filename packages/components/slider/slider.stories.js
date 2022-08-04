import React, { useState } from "react";
import Slider from ".";

export default {
  title: "Components/Slider",
  component: Slider,
  parameters: {
    docs: { description: { component: "Components/Slider" } },
  },
};

const Template = ({ ...args }) => {
  const [value, setValue] = useState(0);

  const handleChange = (e) => {
    const target = e.target;
    setValue(target.value);
  };

  return (
    <div style={{ width: "400px", height: "50px" }}>
      <Slider {...args} value={value} onChange={handleChange} />
    </div>
  );
};

export const Default = Template.bind({});
Default.args = {
  min: 0,
  max: 5,
  step: 0.1,
  withPouring: false,
};
