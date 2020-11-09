import React from "react";
import { storiesOf } from "@storybook/react";
import {
  withKnobs,
  boolean,
  color,
  number,
} from "@storybook/addon-knobs/react";
import Section from "../../../../.storybook/decorators/sectionBlue";

import Loaders from "..";
import { LoaderStyle } from "../../../constants/index";
import styled from "styled-components";

const StyledH1 = styled.h1`
  padding-left: 16px;
`;

storiesOf("Components|Loaders", module)
  .addDecorator(withKnobs)
  .add("header loader", () => (
    <Section>
      <StyledH1>Header Loader</StyledH1>
      <Loaders.Header
        backgroundColor={color("backgroundColor", "#fff")}
        foregroundColor={color("foregroundColor", "#fff")}
        backgroundOpacity={number("backgroundOpacity", 0.2)}
        foregroundOpacity={number("foregroundOpacity", 0.25)}
        speed={number("speed", LoaderStyle.speed)}
        animate={boolean("animate", LoaderStyle.animate)}
      />
    </Section>
  ));
