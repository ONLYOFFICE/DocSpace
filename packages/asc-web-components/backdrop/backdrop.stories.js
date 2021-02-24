import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import { BooleanValue } from "react-values";
import withReadme from "storybook-readme/with-readme";
import { withKnobs, number } from "@storybook/addon-knobs/react";
import Readme from "./README.md";
import Section from "../../../.storybook/decorators/section";
import Backdrop from ".";
import Button from "../button";

storiesOf("Components|Backdrop", module)
  .addDecorator(withReadme(Readme))
  .addDecorator(withKnobs)
  .add("base", () => (
    <Section>
      <BooleanValue>
        {({ value, toggle }) => (
          <div>
            <Button
              label="Show Backdrop"
              primary={true}
              onClick={(e) => {
                action("onShow")(e);
                toggle(true);
              }}
            />
            <Backdrop
              visible={value}
              zIndex={number("zIndex", 1)}
              onClick={(e) => {
                action("onHide")(e);
                toggle(false);
              }}
            />
          </div>
        )}
      </BooleanValue>
    </Section>
  ));
