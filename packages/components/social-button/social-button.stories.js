import React from "react";
import SocialButton from "./";
import ShareGoogleReactSvgUrl from "PUBLIC_DIR/images/share.google.react.svg?url";
import ShareLinkedinReactSvgUrl from "PUBLIC_DIR/images/share.linkedin.react.svg?url";

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
          ShareGoogleReactSvgUrl,
          //"ShareFacebookIcon",
          //"ShareTwitterIcon",
          ShareLinkedinReactSvgUrl,
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
  iconName: ShareGoogleReactSvgUrl,
  isDisabled: false,
};
