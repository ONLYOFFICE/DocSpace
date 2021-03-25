import React from "react";
import { storiesOf } from "@storybook/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import { withKnobs, text } from "@storybook/addon-knobs/react";
import ErrorContainer from ".";

storiesOf("Components| ErrorContainer", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => (
    <ErrorContainer
      headerText={text("headerText", "Error 404. Page not found")}
      bodyText={text(
        "bodyText",
        "This page was removed, renamed or doesn’t exist anymore."
      )}
      buttonText={text("buttonText", "Return to homepage")}
      buttonUrl={text("buttonUrl", "/")}
    />
  ));
