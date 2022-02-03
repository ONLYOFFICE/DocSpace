import React from "react";
import IconButton from "./";

export default {
  title: "Components/IconButton",
  component: IconButton,
  parameters: {
    docs: {
      description: { component: "IconButton is used for a action on a page" },
    },
  },
  argTypes: {
    color: { control: "color" },
    clickColor: { control: "color" },
    hoverColor: { control: "color" },
    onClick: { action: "onClick" },
    iconName: {
      control: {
        type: "select",
        options: [
          "static/images/search.react.svg",
          "static/images/eye.react.svg",
          "static/images/info.react.svg",
          "static/images/mail.react.svg",
          "static/images/catalog.pin.react.svg",
          "static/images/cross.react.svg",
          "static/images/media.mute.react.svg",
          "static/images/nav.logo.react.svg",
          "static/images/people.react.svg",
          "static/images/question.react.svg",
          "static/images/settings.react.svg",
        ],
      },
    },
  },
};

const Template = (args) => {
  return <IconButton {...args} />;
};

export const Default = Template.bind({});
Default.args = {
  size: 25,
  iconName: "static/images/search.react.svg",
  isFill: true,
  isDisabled: false,
};
