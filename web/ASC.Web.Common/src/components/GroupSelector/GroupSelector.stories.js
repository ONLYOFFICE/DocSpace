/* eslint-disable react/prop-types */
import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import {
  withKnobs,
  boolean
} from "@storybook/addon-knobs/react";
import Section from "../../../.storybook/decorators/section";
import withProvider from "../../../.storybook/decorators/redux";
import GroupSelector from ".";
import { BooleanValue } from "react-values";
import { Button } from "asc-web-components";
//import withReadme from "storybook-readme/with-readme";
//import Readme from "./README.md";


storiesOf("Components|GroupSelector", module)
  .addDecorator(withProvider)
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
              <GroupSelector 
                isOpen={value} 
                useFake={true} 
                isMultiSelect={boolean("isMultiSelect", true)}
                onSelect={(group) => {
                  action("onSelect", group);
                  toggle();
                }} 
              />
            </div>
          )}
        </BooleanValue>
      </Section>
    );
  });
