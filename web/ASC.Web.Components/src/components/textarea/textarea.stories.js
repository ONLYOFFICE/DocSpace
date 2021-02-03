import React from "react";
import { storiesOf } from "@storybook/react";
import { withKnobs, text, boolean, number } from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import Textarea from ".";
import Section from "../../../.storybook/decorators/section";
import { StringValue } from "react-values";
import { action } from "@storybook/addon-actions";

const textLorem =
  "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus. Vivamus elementum semper nisi. Aenean vulputate eleifend tellus. Aenean leo ligula, porttitor eu, consequat vitae";

storiesOf("Components|Input", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("textarea", () => (
    <div>
      <StringValue
        onChange={(e) => {
          action("onChange")(e);
        }}
        defaultValue={textLorem}
      >
        {({ value, set }) => (
          <Section>
            <Textarea
              placeholder={text("placeholder", "Add comment")}
              isDisabled={boolean("isDisabled", false)}
              isReadOnly={boolean("isReadOnly", false)}
              hasError={boolean("hasError", false)}
              maxLength={number("maxLength", 1000)}
              id={text("id", "")}
              name={text("name", "")}
              tabIndex={number("tabIndex", 1)}
              value={value}
              fontSize={number("fontSize", 13)}
              heightTextArea={number("heightTextArea", 89)}
              onChange={(e) => {
                set(e.target.value);
              }}
            />
          </Section>
        )}
      </StringValue>
    </div>
  ));
