import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from '@storybook/addon-actions';
import { withKnobs, text, number, boolean, select } from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import AdvancedSelector from "./";
import Section from "../../../.storybook/decorators/section";
import { ArrayValue, BooleanValue } from "react-values";
import Button from "../button";

function getRandomInt(min, max) {
  return Math.floor(Math.random() * (max - min)) + min;
}

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
  },
  {
    key: "group-1",
    label: "Group1",
    total: 0
  },
  {
    key: "group-2",
    label: "Group2",
    total: 0
  },
  {
    key: "group-3",
    label: "Group3",
    total: 0
  },
  {
    key: "group-4",
    label: "Group4",
    total: 0
  },
  {
    key: "group-5",
    label: "Group5",
    total: 0
  }
];

const sizes = ["compact", "full"];
const displayTypes = ['dropdown', 'aside'];

storiesOf("Components|AdvancedSelector", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => {
    const optionsCount = number("Users count", 1000);
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
        <BooleanValue
          defaultValue={true}
          onChange={() => action("isOpen changed")}
        >
          {({ value: isOpen, toggle }) => (
            <div style={{position: "relative"}}>
              <Button label="Toggle dropdown" onClick={toggle} />
                <ArrayValue
                  defaultValue={options}
                  onChange={() => action("options onChange")}
                >
                  {({ value, set }) => (
                    <AdvancedSelector
                      size={select("size", sizes, "full")}
                      placeholder={text("placeholder", "Search users")}
                      onSearchChanged={value => {
                        action("onSearchChanged")(value);
                        set(
                          options.filter(option => {
                            return option.label.indexOf(value) > -1;
                          })
                        );
                      }}
                      options={value}
                      groups={groups}
                      selectedGroups={[groups[0]]}
                      isMultiSelect={boolean("isMultiSelect", true)}
                      buttonLabel={text("buttonLabel", "Add members")}
                      selectAllLabel={text("selectAllLabel", "Select all")}
                      onSelect={selectedOptions => {
                        action("onSelect")(selectedOptions);
                        toggle();
                      }}
                      onCancel={toggle}
                      onChangeGroup={group => {
                        set(
                          options.filter(option => {
                            return (
                              option.groups &&
                              option.groups.length > 0 &&
                              option.groups.indexOf(group.key) > -1
                            );
                          })
                        );
                      }}
                      allowCreation={boolean("allowCreation", false)}
                      onAddNewClick={() => action("onSelect") }
                      isOpen={isOpen}
                      displayType={select("displayType", displayTypes, "dropdown")}
                    />
                  )}
                </ArrayValue>
              </div>
          )}
        </BooleanValue>
      </Section>
    );
  });
