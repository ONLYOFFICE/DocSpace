import React from "react";
import { storiesOf } from "@storybook/react";
import withReadme from "storybook-readme/with-readme";
import styled from "@emotion/styled";
import {
  withKnobs,
  select,
  color,
  boolean,
} from "@storybook/addon-knobs/react";
import Readme from "./README.md";

import { Icons } from ".";

const IconList = styled.div`
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr;
`;

const IconItem = styled.div`
  padding: 16px;
  display: flex;
  flex-direction: column;
  align-items: center;
  flex: 1;
`;

const IconContainer = styled.div`
  margin: 16px 0;
`;

const iconNames = Object.keys(Icons);

storiesOf("Components|Icons", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("all icons", () => (
    <IconList>
      {Object.values(Icons).map((Icon, index) => {
        const sizeValue = select(
          "size",
          ["small", "medium", "big", "scale"],
          "big"
        );

        let isFill = boolean("isfill", false);
        let iconColor = isFill ? { color: `${color("color", "dimgray")}` } : {};

        let isStroke = boolean("isStroke", false);
        let iconStroke = isStroke
          ? { stroke: `${color("stroke", "dimgray")}` }
          : {};

        const containerWidth =
          sizeValue === "scale"
            ? {
                width: `${select(
                  "container width",
                  ["100", "200", "300", "400"],
                  "100"
                )}px`,
              }
            : {};
        return (
          <IconItem key={index}>
            <IconContainer style={containerWidth}>
              <Icon
                isfill={isFill}
                isStroke={isStroke}
                size={sizeValue}
                {...iconColor}
                {...iconStroke}
              />
            </IconContainer>
            <span>{iconNames[index]}</span>
          </IconItem>
        );
      })}
    </IconList>
  ));
