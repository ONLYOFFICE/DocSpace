import React from "react";

import Text from "../text";
import Link from "../link";
import HelpButton from ".";

export default {
  title: "Components/HelpButton",
  component: HelpButton,
  subcomponents: { Text, Link },
  argTypes: {},
  parameters: {
    docs: {
      description: {
        component: "HelpButton is used for a action on a page",
      },
    },
  },
};

const Template = (args) => {
  return (
    <>
      <HelpButton {...args} />
    </>
  );
};

export const Default = Template.bind({});
Default.args = {
  displayType: "auto",
  offsetTop: 0,
  offsetRight: 0,
  offsetBottom: 0,
  offsetLeft: 0,
  tooltipContent: <Text fontSize="13px">Paste you tooltip content here</Text>,
};

const DropDownTemplate = (args) => {
  return (
    <HelpButton
      displayType="dropdown"
      offsetTop={0}
      offsetRight={0}
      offsetBottom={0}
      offsetLeft={0}
      tooltipContent={
        <Text fontSize="13px">Paste you tooltip content here</Text>
      }
    />
  );
};

export const DropDownPosition = DropDownTemplate.bind({});

const AsideTemplate = (args) => {
  return (
    <HelpButton
      displayType="aside"
      helpButtonHeaderContent="Aside position HelpButton"
      tooltipContent={
        <Text>
          You tooltip content with{" "}
          <Link
            isHovered={true}
            href="http://localhost:6006/?path=/story/components-helpbutton--default"
          >
            link
          </Link>
        </Text>
      }
    />
  );
};

export const AsidePosition = AsideTemplate.bind({});

const AutoTemplate = (args) => {
  return (
    <HelpButton
      displayType="auto"
      helpButtonHeaderContent="Auto position HelpButton"
      tooltipContent={
        <>
          <p>You can put every thing here</p>
          <ul style={{ marginBottom: 0 }}>
            <li>Word</li>
            <li>Chart</li>
            <li>Else</li>
          </ul>
        </>
      }
    />
  );
};

export const AutoPosition = AutoTemplate.bind({});
