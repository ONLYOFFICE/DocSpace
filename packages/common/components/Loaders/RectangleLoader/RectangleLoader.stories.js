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
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";

import Loaders from "..";
import { LoaderStyle } from "../../../constants";

storiesOf("Components|Loaders", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("rectangle loader", () => (
    <Section>
      <h1>Rectangle Loader</h1>
      <Loaders.Rectangle
        title={text("title", LoaderStyle.title)}
        x={text("x", "0")}
        y={text("y", "0")}
        width={text("width", LoaderStyle.width)}
        height={text("height", LoaderStyle.height)}
        borderRadius={number("borderRadius", LoaderStyle.borderRadius)}
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
