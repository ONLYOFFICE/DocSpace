/* eslint-disable react/prop-types */
import React from "react";
import { storiesOf } from "@storybook/react";
//import { action } from "@storybook/addon-actions";
import {
  withKnobs
  //text,
  //number,
  //boolean,
  //select
} from "@storybook/addon-knobs/react";
import Section from "../../../.storybook/decorators/section";

import PeopleAdvancedSelector from "./";
import { BooleanValue } from "react-values";
import { Button } from "asc-web-components";
//import withReadme from "storybook-readme/with-readme";
//import Readme from "./README.md";

storiesOf("Components|PeopleSelector", module)
  .addDecorator(withKnobs)
  //.addDecorator(withReadme(Readme))
  .addParameters({ options: { addonPanelInRight: false } })
  .add("base", () => {
    return (
      <Section>
        <BooleanValue>
          {({ value, toggle }) => (
            <div style={{ position: "relative" }}>
              <Button label="Toggle dropdown" onClick={toggle} />
              <PeopleAdvancedSelector isOpen={value} />
            </div>
          )}
        </BooleanValue>
      </Section>
    );
  });
