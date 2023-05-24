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
      options: ["EditingRoom", "CustomRoom"],
      control: { type: "select" },
    },
  },
};

const Template = (args) => {
  const RoomType = {
    EditingRoom: 2,
    CustomRoom: 5,
  };
  return <RoomLogo {...args} type={RoomType[args.type]} />;
};

export const Default = Template.bind({});

Default.args = {
  type: "EditingRoom",
  isPrivacy: false,
  isArchive: false,
  withCheckbox: false,
  isChecked: false,
  isIndeterminate: false,
  onChange: () => console.log("checked"),
};
