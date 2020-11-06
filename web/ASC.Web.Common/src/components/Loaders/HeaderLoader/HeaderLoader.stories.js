import React from "react";
import { storiesOf } from "@storybook/react";
import {
  withKnobs,
  boolean,
  color,
  number,
} from "@storybook/addon-knobs/react";
import Section from "../../../../.storybook/decorators/section";

import Loaders from "..";
import { LoaderStyle } from "../../../constants/index";

storiesOf("Components|Loaders", module)
  .addDecorator(withKnobs)
  .add("header loader", () => (
    <Section>
      <h1>Header Loader</h1>
      <Loaders.Header
        backgroundColor={color("backgroundColor", "#fff")}
        foregroundColor={color("foregroundColor", "#fff")}
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
