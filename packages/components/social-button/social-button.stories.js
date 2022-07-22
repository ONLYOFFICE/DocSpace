import React from "react";
import SocialButton from "./";

export default {
  title: "Components/SocialButtons",
  component: SocialButton,
  parameters: {
    docs: {
      description: {
        component: "Button is used for sign up with help social networks",
      },
    },
  },
  argTypes: {
    onClick: { action: "onClick" },
    iconName: {
      control: {
        type: "select",
        options: [
          "static/images/share.google.react.svg",
          //"ShareFacebookIcon",
          //"ShareTwitterIcon",
          "static/images/share.linkedin.react.svg",
        ],
      },
    },
  },
};

const Template = ({ onClick, ...args }) => {
  return <SocialButton {...args} onClick={() => onClick("clicked")} />;
};

export const Default = Template.bind({});
Default.args = {
  label: "Base SocialButton",
  iconName: "static/images/share.google.react.svg",
  isDisabled: false,
};
