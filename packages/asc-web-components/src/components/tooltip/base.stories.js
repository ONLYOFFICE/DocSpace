import React from "react";
import { storiesOf } from "@storybook/react";
import { withKnobs, select, number } from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import Tooltip from "./";
import Section from "../../../.storybook/decorators/section";
import Link from "../link";
import Text from "../text";

const BodyStyle = { marginTop: 100, marginLeft: 200, position: "absolute" };

const arrayEffects = ["float", "solid"];
const arrayPlaces = ["top", "right", "bottom", "left"];

storiesOf("Components|Tooltip", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => {
    return (
      <Section>
        <div style={BodyStyle}>
          <Link data-for="link" data-tip="Bob Johnston">
            Bob Johnston
          </Link>
        </div>

        <Tooltip
          id="link"
          effect={select("effect", arrayEffects, "float")}
          place={select("place", arrayPlaces, "top")}
          offsetTop={number("offsetTop", 0)}
          offsetRight={number("offsetRight", 0)}
          offsetBottom={number("offsetBottom", 0)}
          offsetLeft={number("offsetLeft", 0)}
          getContent={(dataTip) => (
            <div>
              <Text isBold={true} fontSize="16px">
                {dataTip}
              </Text>
              <Text color="#A3A9AE" fontSize="13px">
                BobJohnston@gmail.com
              </Text>
              <Text fontSize="13px">Developer</Text>
            </div>
          )}
        />
      </Section>
    );
  });
