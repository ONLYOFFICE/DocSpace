import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import { withKnobs, boolean, text, select } from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import SocialButton from ".";

storiesOf("Components|Buttons|SocialButtons", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("social button", () => {
    const socialNetworks = [
      "static/images/share.google.react.svg",
      "ShareFacebookIcon",
      "ShareTwitterIcon",
      "static/images/share.linkedin.react.svg",
    ];
    const iconName = select(
      "iconName",
      ["", ...socialNetworks],
      "static/images/share.google.react.svg"
    );

    return (
      <SocialButton
        label={text("label", "Base SocialButton")}
        iconName={iconName}
        isDisabled={boolean("isDisabled", false)}
        onClick={action("clicked")}
      />
    );
  });
