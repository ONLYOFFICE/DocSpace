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
  .add("tree folder loader", () => (
    <Section>
      <h1>Tree Folder Loader</h1>
      <Loaders.TreeFolders
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
