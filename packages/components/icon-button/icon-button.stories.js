import React from "react";
import IconButton from "./";
import SearchReactSvgUrl from "PUBLIC_DIR/images/search.react.svg?url";
import EyeReactSvgUrl from "PUBLIC_DIR/images/eye.react.svg?url";
import InfoReactSvgUrl from "PUBLIC_DIR/images/info.react.svg?url";
import MailReactSvgUrl from "PUBLIC_DIR/images/mail.react.svg?url";
import CatalogPinReactSvgUrl from "PUBLIC_DIR/images/catalog.pin.react.svg?url";
import CrossReactSvgUrl from "PUBLIC_DIR/images/cross.react.svg?url";
import MediaMuteReactSvgUrl from "PUBLIC_DIR/images/media.mute.react.svg?url";
import NavLogoReactSvg from "PUBLIC_DIR/images/nav.logo.react.svg?url";
import PersonReactSvg from "PUBLIC_DIR/images/person.react.svg?url";
import QuestionReactSvg from "PUBLIC_DIR/images/question.react.svg?url";
import SettingsReactSvg from "PUBLIC_DIR/images/settings.react.svg?url";

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
      },
      options: [
        SearchReactSvgUrl,
        EyeReactSvgUrl,
        InfoReactSvgUrl,
        MailReactSvgUrl,
        CatalogPinReactSvgUrl,
        CrossReactSvgUrl,
        MediaMuteReactSvgUrl,
        NavLogoReactSvg,
        PersonReactSvg,
        QuestionReactSvg,
        SettingsReactSvg,
      ],
    },
  },
};

const Template = (args) => {
  return <IconButton {...args} />;
};

export const Default = Template.bind({});
Default.args = {
  size: 25,
  iconName: SearchReactSvgUrl,
  isFill: true,
  isDisabled: false,
};
