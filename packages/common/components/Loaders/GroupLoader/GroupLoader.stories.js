import React from "react";
import { storiesOf } from "@storybook/react";
import {
  withKnobs,
  boolean,
  text,
  color,
  number,
} from "@storybook/addon-knobs/react";
import Section from "../../../../.storybook/decorators/section";

import Loaders from "..";
import { LoaderStyle } from "../../../constants";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";

storiesOf("Components|Loaders", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("group loader", () => (
    <Section>
      <h1>Group Loader</h1>
      <Loaders.Group
        title={text("title", LoaderStyle.title)}
        borderRadius={text("borderRadius", "3")}
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
