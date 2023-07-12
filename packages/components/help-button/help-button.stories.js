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
    <div>
      <HelpButton {...args} />
    </div>
  );
};

export const Default = Template.bind({});
Default.args = {
  offsetTop: 0,
  offsetRight: 0,
  offsetBottom: 0,
  offsetLeft: 0,
  tooltipContent: <Text fontSize="13px">Paste you tooltip content here</Text>,
  place: "right",
};

const AutoTemplate = (args) => {
  return (
    <div style={{ marginTop: "20px", marginLeft: "100px" }}>
      <HelpButton
        style={{ left: "20px" }}
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
        {...args}
      />
    </div>
  );
};

export const AutoPosition = AutoTemplate.bind({});
AutoPosition.args = {
  offsetTop: 0,
  offsetRight: 0,
  offsetBottom: 0,
  offsetLeft: 0,
};
