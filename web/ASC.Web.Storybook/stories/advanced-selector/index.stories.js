import React from "react";
import { storiesOf } from "@storybook/react";
import { withKnobs, text, number } from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import { AdvancedSelector } from "asc-web-components";
import Section from "../../.storybook/decorators/section";
import { boolean } from "@storybook/addon-knobs/dist/deprecated";
import { ArrayValue } from "react-values";

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
        label: "All groups",
        total: 0
      },
      {
        key: "group-dev",
        label: "Development",
        total: 0
      },
      {
        key: "group-management",
        label: "Management",
        total: 0
      },
      {
        key: "group-marketing",
        label: "Marketing",
        total: 0
      },
      {
        key: "group-mobile",
        label: "Mobile",
        total: 0
      },
      {
        key: "group-support",
        label: "Support",
        total: 0
      },
      {
        key: "group-web",
        label: "Web",
        total: 0
      }
    ];

    const options = Array.from({ length: optionsCount }, (v, index) => {
      const additional_group = groups[getRandomInt(1, 6)];
      groups[0].total++;
      additional_group.total++;
      return {
        key: `user${index}`,
        groups: ["group-all", additional_group.key],
        label: `User${index + 1} (All groups, ${additional_group.label})`
      };
    });

    return (
      <Section>
        <ArrayValue
          defaultValue={options}
          onChange={() => console.log("ArrayValue onChange")}
        >
          {({ value, set }) => (
            <AdvancedSelector
              placeholder={text("placeholder", "Search users")}
              onSearchChanged={e => console.log(e.target.value)}
              options={value}
              groups={groups}
              selectedGroups={[groups[0]]}
              isMultiSelect={boolean("isMultiSelect", true)}
              buttonLabel={text("buttonLabel", "Add members")}
              onSelect={selectedOptions =>
                console.log("onSelect", selectedOptions)
              }
              onChangeGroup={group => { 
                set(options.filter(option => {
                  return (
                    option.groups &&
                    option.groups.length > 0 &&
                    option.groups.indexOf(group.key) > -1
                  );
                }));
              }}
            />
          )}
        </ArrayValue>
      </Section>
    );
  });
