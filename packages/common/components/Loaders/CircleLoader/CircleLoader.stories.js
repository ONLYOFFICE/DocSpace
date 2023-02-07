import React from "react";
import { storiesOf } from "@storybook/react";
import {
  withKnobs,
  text,
  number,
  boolean,
  color,
} from "@storybook/addon-knobs/react";
import Section from "../../../../.storybook/decorators/section";

import CircleLoader from ".";
import { LoaderStyle } from "../../../constants";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";

storiesOf("Components|Loaders", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("circle loader", () => (
    <Section>
      <h1>Circle Loader</h1>
      <CircleLoader
        title={text("title", LoaderStyle.title)}
        x={text("x", "32")}
        y={text("y", "32")}
        radius={number("radius", "32")}
        backgroundColor={color("backgroundColor", LoaderStyle.backgroundColor)}
        foregroundColor={color("foregroundColor", LoaderStyle.foregroundColor)}
        backgroundOpacity={number(
          "backgroundOpacity",
          LoaderStyle.backgroundOpacity
        )}
        foregroundOpacity={number(
          "foregroundOpacity",
          LoaderStyle.foregroundOpacity
        )}
        speed={number("speed", LoaderStyle.speed)}
        animate={boolean("animate", LoaderStyle.animate)}
      />
    </Section>
  ));
