import React from "react";
import { storiesOf } from "@storybook/react";
import ToggleContent from ".";
import Button from "../button";
import TextInput from "../button";
import Text from "../text";
import { Icons } from "../icons";
import Readme from "./README.md";
import withReadme from "storybook-readme/with-readme";
import { text, withKnobs, boolean, select } from "@storybook/addon-knobs/react";
import { optionsKnob as options } from "@storybook/addon-knobs";
import { action } from "@storybook/addon-actions";

storiesOf("Components|ToggleContent", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => {
    const valuesMultiSelect = {
      button: "button",
      icon: "icon",
      text: "text",
      toggleContent: "toggleContent",
      textInput: "textInput",
    };
    const optionsMultiSelect = options(
      "Children",
      valuesMultiSelect,
      ["text"],
      {
        display: "multi-select",
      }
    );

    const iconNames = Object.keys(Icons);

    let children = [];
    optionsMultiSelect.forEach(function (item, i) {
      switch (item) {
        case "button":
          children.push(
            <Button
              label={text("Button label", "OK")}
              key={i}
              onClick={action("button clicked!")}
            />
          );
          break;
        case "icon":
          {
            let iconName = `${select("iconName", iconNames, "AimIcon")}`;
            children.push(React.createElement(Icons[iconName], { key: i }));
          }
          break;
        case "text":
          children.push(
            <Text key={i}>
              {text(
                "text",
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus. Vivamus elementum semper nisi."
              )}
            </Text>
          );
          break;
        case "toggleContent":
          children.push(
            <ToggleContent key={i}>
              {text(
                "text inside another ToogleContent",
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit."
              )}
            </ToggleContent>
          );
          break;
        case "textInput":
          children.push(
            <TextInput
              key={i}
              value="text"
              onChange={(event) => alert(event.target.value)}
            />
          );
          break;
        default:
          break;
      }
    });

    let isOpen = boolean("isOpen", true);

    return (
      <>
        <ToggleContent
          label={text("label", "Some label")}
          isOpen={isOpen}
          onChange={(checked) => {
            window.__STORYBOOK_ADDONS.channel.emit("storybookjs/knobs/change", {
              name: "isOpen",
              value: checked,
            });
          }}
        >
          {children}
        </ToggleContent>
      </>
    );
  });
