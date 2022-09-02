import React from "react";

import Selector from "./";

export default {
  title: "Components/Selector",
  component: Selector,
  parameters: {
    docs: {
      description: {
        component:
          "Selector for displaying items list of people or room selector",
      },
    },
  },
};

function makeName() {
  var result = "";
  var characters =
    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
  var charactersLength = characters.length;
  for (var i = 0; i < 15; i++) {
    result += characters.charAt(Math.floor(Math.random() * charactersLength));
  }
  return result;
}

const getItems = (count) => {
  const items = [];

  for (let i = 0; i < count / 2; i++) {
    items.push({
      key: `user_${i}`,
      id: `user_${i}`,
      label: makeName(),
      avatar: "static/images/room.archive.svg",
    });
  }

  for (let i = 0; i < count / 2; i++) {
    items.push({
      key: `room_${i}`,
      id: `room_${i}`,
      label: makeName(),
      icon: "static/images/icons/32/rooms/custom.svg",
    });
  }

  return items;
};

const getAccessRights = () => {
  const accesses = [
    {
      key: "roomManager",
      label: "Room manager",
      access: 0,
    },
    {
      key: "editor",
      label: "Editor",
      access: 1,
    },
    {
      key: "formFiller",
      label: "Form filler",
      access: 2,
    },
    {
      key: "reviewer",
      label: "Reviewer",
      access: 3,
    },
    {
      key: "commentator",
      label: "Commentator",
      access: 4,
    },
    {
      key: "viewer",
      label: "Viewer",
      access: 5,
    },
  ];

  return accesses;
};

const Template = (args) => {
  return (
    <div
      style={{
        width: "480px",
        height: args.height,
        border: "1px solid red",
        margin: "auto",
      }}
    >
      <Selector {...args} />
    </div>
  );
};

export const Default = Template.bind({});

const items = getItems(1000);

const selectedItems = [items[0], items[3], items[7]];

const accessRights = getAccessRights();

const selectedAccessRight = accessRights[1];

Default.args = {
  height: "485px", // container height
  headerLabel: "Room list",
  onBackClick: () => console.log("back click"),
  searchPlaceholder: "Search",
  searchValue: "",
  items,
  onSelect: (item) => console.log("select " + item),
  isMultiSelect: false,
  selectedItems,
  acceptButtonLabel: "Add",
  onAccept: (items, access) => console.log("accept " + items + access),
  withSelectAll: false,
  selectAllLabel: "All accounts",
  selectAllIcon: "static/images/room.archive.svg",
  onSelectAll: () => console.log("select all"),
  withAccessRights: true,
  accessRights,
  selectedAccessRight,
  onAccessRightsChange: (access) =>
    console.log("access rights change " + access),
  withCancelButton: false,
  cancelButtonLabel: "Cancel",
  onCancel: () => console.log("cancel"),
};
