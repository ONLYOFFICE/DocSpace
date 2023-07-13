import React from "react";
import Tags from ".";

export default {
  title: "Components/Tags",
  component: Tags,
  parameters: {
    design: {
      type: "figma",
      url: "https://www.figma.com/file/ZiW5KSwb4t7Tj6Nz5TducC/UI-Kit-DocSpace-1.0.0?type=design&node-id=62-2597&mode=design&t=TBNCKMQKQMxr44IZ-0",
    },
  },
};

const Template = (args) => <Tags {...args} />;

export const Default = Template.bind({});

Default.args = {
  tags: ["tag1", "tag2"],
  id: "",
  className: "",
  columnCount: 2,
  style: {},
  onSelectTag: (tags) => {},
};
