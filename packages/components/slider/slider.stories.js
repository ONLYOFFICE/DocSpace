import React, { useState } from "react";
import Slider from ".";

export default {
  title: "Components/Slider",
  component: Slider,
  parameters: {
    docs: { description: { component: "Components/Slider" } },
    design: {
      type: "figma",
      url: "https://www.figma.com/file/ZiW5KSwb4t7Tj6Nz5TducC/UI-Kit-DocSpace-1.0.0?type=design&node-id=505-4112&mode=design&t=TBNCKMQKQMxr44IZ-0",
    },
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
