import React from "react";
import { storiesOf } from "@storybook/react";
import { withKnobs, text, number } from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import { AdvancedSelector } from "asc-web-components";
import Section from "../../.storybook/decorators/section";
import { boolean } from "@storybook/addon-knobs/dist/deprecated";

function getRandomInt(min, max) {
  return Math.floor(Math.random() * (max - min)) + min;
}

storiesOf("Components|AdvancedSelector", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => {

    const optionsCount = number("Users count", 10000);

    const groups = [
      {
        key: "group-all",
        label: "All groups"
      },
      {
        key: "group-dev",
        label: "Development"
      },
      {
        key: "group-management",
        label: "Management"
      },
      {
        key: "group-marketing",
        label: "Marketing"
      },
      {
        key: "group-mobile",
        label: "Mobile"
      },
      {
        key: "group-support",
        label: "Support"
      },
      {
        key: "group-web",
        label: "Web"
      }
    ];
    
    const options = Array.from({ length: optionsCount }, (v, index) => {
      const additional_group = groups[getRandomInt(1, 6)];
      return {
        key: `user${index}`,
        groups: ["group-all", additional_group.key],
        label: `User${index + 1} (All groups, ${additional_group.label})`
      };
    });

    return (
      <Section>
        <AdvancedSelector
          placeholder={text("placeholder", "Search users")}
          onSearchChanged={e => console.log(e.target.value)}
          options={options}
          groups={groups}
          selectedGroups={[groups[0]]}
          isMultiSelect={boolean("isMultiSelect", true)}
          buttonLabel={text("buttonLabel", "Add members")}
          onSelect={selectedOptions => console.log("onSelect", selectedOptions)}
        />
      </Section>
    );
  });
