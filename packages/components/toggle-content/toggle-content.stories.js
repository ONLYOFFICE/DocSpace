import React, { useState } from "react";
import ToggleContent from ".";
import Button from "../button";
import TextInput from "../button";
import Text from "../text";
//import { Icons } from "../icons";
import CheckIcon from "../public/static/images/check.react.svg";

const optionsChildren = [
  "button",
  "icon",
  "text",
  "toggleContent",
  "textInput",
];

export default {
  title: "Components/ToggleContent",
  component: ToggleContent,
  parameters: {
    docs: {
      description: {
        component:
          "ToggleContent allow you to adding information, which you may hide/show by clicking header",
      },
    },
  },
  argTypes: {
    Children: {
      control: {
        type: "multi-select",
        options: optionsChildren,
      },
    },
    buttonLabel: {
      control: "text",
    },
    onClickButton: {
      action: "button clicked!",
      table: { disable: true },
    },
    text: { control: "text" },
    textInnerToggleContent: {
      control: "text",
    },
  },
};

const Template = ({
  buttonLabel,
  onClickButton,
  text,
  textInnerToggleContent,
  onChangeTextInput,
  isOpen,
  Children,
  textInputValue,
  ...args
}) => {
  const [opened, setOpened] = useState(isOpen);
  let children = [];

  Children.map((item, indx) => {
    switch (item) {
      case "button":
        children.push(
          <Button label={buttonLabel} key={indx} onClick={onClickButton} />
        );
        break;
      case "icon":
        children.push(<CheckIcon key={indx} />);
        break;
      case "text":
        children.push(<Text key={indx}>{text}</Text>);
        break;
      case "toggleContent":
        children.push(
          <ToggleContent key={indx}>{textInnerToggleContent}</ToggleContent>
        );
        break;
      case "textInput":
        children.push(
          <TextInput
            key={indx}
            value="text"
            onChange={(event) => {
              onChangeTextInput(event.target.value);
            }}
          />
        );
        break;
      default:
        break;
    }
  });

  return (
    <ToggleContent
      {...args}
      isOpen={opened}
      onChange={(checked) => {
        setOpened(checked);
      }}
    >
      {children}
    </ToggleContent>
  );
};

export const Default = Template.bind({});
Default.args = {
  Children: ["text"],
  buttonLabel: "OK",
  text:
    "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus. Vivamus elementum semper nisi.",
  textInnerToggleContent:
    "Lorem ipsum dolor sit amet, consectetuer adipiscing elit.",
  isOpen: true,
  label: "Some label",
};
