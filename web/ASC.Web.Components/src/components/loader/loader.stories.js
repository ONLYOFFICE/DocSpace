import React from "react";
import { storiesOf } from "@storybook/react";
import { withKnobs, text, select, color } from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import Loader from ".";
import Section from "../../../.storybook/decorators/section";

const typeOptions = ["base", "oval", "dual-ring", "rombs"];

storiesOf("Components|Loaders", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => (
    <Section>
      <Loader
        type={select("type", typeOptions, "base")}
        color={color("color", "#63686a")}
        size={text("size", "18px")}
        label={text("label", "Loading content, please wait...")}
      />
    </Section>
  ))
  .add("dual-ring", () => (
    <Section>
      <Loader
        type={select("type", typeOptions, "dual-ring")}
        color={color("color", "#63686a")}
        size={text("size", "40px")}
        label={text("label", "Loading content, please wait.")}
      />
    </Section>
  ))
  .add("oval", () => (
    <Section>
      <Loader
        type={select("type", typeOptions, "oval")}
        color={color("color", "#63686a")}
        size={text("size", "40px")}
        label={text("label", "Loading content, please wait.")}
      />
    </Section>
  ))
  .add("rombs", () => (
    <Section>
      <Loader
        type={select("type", typeOptions, "rombs")}
        size={text("size", "40px")}
      />
    </Section>
  ));
