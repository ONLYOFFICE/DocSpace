import React from "react";
import { storiesOf } from "@storybook/react";
import Section from "../../../.storybook/decorators/section";

import Loaders from ".";

storiesOf("Components|FilterLoader", module).add("base", () => (
  <Section>
    <Loaders.Filter />
  </Section>
));
