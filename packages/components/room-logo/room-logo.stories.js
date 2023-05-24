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
  argTypes: {
    type: {
      options: ["Editing Room", "Custom Room"],
      control: { type: "select" },
    },
  },
};

const Template = (args) => {
  const RoomType = {
    "Editing Room": 2,
    "Custom Room": 5,
  };
  return <RoomLogo {...args} type={RoomType[args.type]} />;
};

export const Default = Template.bind({});

Default.args = {
  type: "Editing Room",
  isPrivacy: false,
  isArchive: false,
  withCheckbox: false,
  isChecked: false,
  isIndeterminate: false,
  onChange: () => console.log("checked"),
};
