import React from "react";

import RoomLogo from ".";

export default {
  title: "Components/RoomLogo",
  component: RoomLogo,
  parameters: {
    docs: {
      description: {
        component:
          "Room logo allow you display default room logo depend on type and private",
      },
    },
  },
  argTypes: {},
};

const Template = (args) => <RoomLogo {...args} />;

export const Default = Template.bind({});

Default.args = {
  type: "custom",
  isPrivacy: false,
  isArchive: false,
  withCheckbox: false,
  isChecked: false,
  isIndeterminate: false,
  onChange: () => console.log("checked"),
};
