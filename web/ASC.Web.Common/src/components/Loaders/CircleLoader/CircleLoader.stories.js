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
import { LoaderStyle } from "../../../constants/index";

storiesOf("Components|Loaders", module)
  .addDecorator(withKnobs)
  .add("circle loader", () => (
    <Section>
      <h1>Circle Loader</h1>
      <CircleLoader
        x={text("x", "40")}
        y={text("y", "40")}
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
