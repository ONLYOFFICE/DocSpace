import React from "react";
import Tags from ".";

export default {
  title: "Components/Tags",
  component: Tags,
};

const Template = (args) => <Tags {...args} />;

export const Default = Template.bind({});

Default.args = {
  tags: ["tag1", "tag2"],
  id: "",
  className: "",
  columnCount: 2,
  style: {},
  onSelectTag: (tags) => console.log(tags),
};
