import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import { withKnobs, text } from "@storybook/addon-knobs/react";
import AdvancedSelector from "../advanced-selector";
import Section from "../../../.storybook/decorators/section";
import { boolean } from "@storybook/addon-knobs/dist/deprecated";
import { ArrayValue, BooleanValue } from "react-values";
import Button from "../button";

storiesOf("EXAMPLES|AdvancedSelector", module)
  .addDecorator(withKnobs)
  // To set a default viewport for all the stories for this component
  .addParameters({ viewport: { defaultViewport: "responsive" } })
  .add("people group selector", () => {
    const options = [
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

    return (
      <Section>
        <BooleanValue
          defaultValue={true}
          onChange={() => action("isOpen changed")}
        >
          {({ value: isOpen, toggle }) => (
            <div style={{ position: "relative" }}>
              <Button label="Toggle dropdown" onClick={toggle} />
              <ArrayValue
                defaultValue={options}
                onChange={() => action("options onChange")}
              >
                {({ value, set }) => (
                  <AdvancedSelector
                    isDropDown={true}
                    isOpen={isOpen}
                    maxHeight={336}
                    width={379}
                    placeholder={text("placeholder", "Search")}
                    onSearchChanged={value => {
                      action("onSearchChanged")(value);
                      set(
                        options.filter(option => {
                          return option.label.indexOf(value) > -1;
                        })
                      );
                    }}
                    options={value}
                    isMultiSelect={boolean("isMultiSelect", true)}
                    buttonLabel={text("buttonLabel", "Add departments")}
                    selectAllLabel={text("selectAllLabel", "Select all")}
                    onSelect={selectedOptions => {
                      action("onSelect")(selectedOptions);
                      toggle();
                    }}
                  />
                )}
              </ArrayValue>
            </div>
          )}
        </BooleanValue>
      </Section>
    );
  });
