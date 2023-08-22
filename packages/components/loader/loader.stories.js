import React from "react";
import Loader from "./";

export default {
  title: "Components/Loader",
  component: Loader,
  parameters: {
    docs: {
      description: {
        component:
          "Loader component is used for displaying loading actions on a page",
      },
    },
    design: {
      type: "figma",
      url: "https://www.figma.com/file/ZiW5KSwb4t7Tj6Nz5TducC/UI-Kit-DocSpace-1.0.0?type=design&node-id=419-1989&mode=design&t=TBNCKMQKQMxr44IZ-0",
    },
  },
  argTypes: {
    color: { control: "color" },
  },
};

const Template = (args) => {
  return (
    <div style={{ height: "100px" }}>
      <Loader {...args} />
    </div>
  );
};

export const Default = Template.bind({});
Default.args = {
  type: "base",
  color: "#63686a",
  size: "18px",
  label: "Loading content, please wait...",
};

const ExamplesTemplate = (args) => {
  return (
    <div
      style={{
        display: "grid",
        gridTemplateColumns: "1fr 1fr 1fr 1fr 1fr",
        height: "100px",
      }}
    >
      <Loader
        type={"base"}
        color={"#63686a"}
        size={"18px"}
        label={"Loading content, please wait..."}
      />
      <Loader
        type={"dual-ring"}
        color={"#63686a"}
        size={"40px"}
        label={"Loading content, please wait."}
      />
      <Loader
        type={"oval"}
        color={"#63686a"}
        size={"40px"}
        label={"Loading content, please wait."}
      />
      <Loader type={"rombs"} size={"40px"} />
      <Loader type="track" style={{ width: "30px" }} />
    </div>
  );
};

export const Examples = ExamplesTemplate.bind({});
