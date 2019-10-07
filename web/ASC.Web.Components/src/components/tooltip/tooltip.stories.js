import React from "react";
import { storiesOf } from "@storybook/react";
import { withKnobs, select } from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import Tooltip from "./";
import Section from "../../../.storybook/decorators/section";

const BodyStyle = { marginTop: 100, marginLeft: 150 };
const arrayTypes = ["success", "warning", "error", "info", "light"];
const arrayEffects = ["float", "solid"];
const arrayPlaces = ["top", "right", "bottom", "left"];
const tooltipContent = "tooltipContent tooltipContent tooltipContent";

storiesOf("Components|Tooltip", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => {
    return (
      <Section>
        <div style={BodyStyle}>
          <a
            data-for="tooltipContent"
            data-tip={tooltipContent}
          >
            (❂‿❂)
          </a>
        </div>
        <Tooltip
          tooltipContent={tooltipContent}
          type={select("type", arrayTypes, "light")}
          effect={select("effect", arrayEffects, "float")}
          place={select("place", arrayPlaces, "top")}
        />
      </Section>
    );
  });
