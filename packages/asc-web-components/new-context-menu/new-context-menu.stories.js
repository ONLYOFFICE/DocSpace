import React from "react";
import NewContextMenu from ".";

export default {
  title: "Components/NewContextMenu",
  component: NewContextMenu,

  parameters: {
    docs: {
      description: {
        component: `Is a  ContextMenu component.
        ContextMenu contain MenuItem component and can take from the props model(all view)
        and header(show only tablet or mobile, when view changed).
`,
      },
    },
  },
};

const defaultModel = [
  {
    disabled: false,
    icon: "/static/images/access.edit.react.svg",
    key: "edit",
    label: "Edit",
    onClick: () => console.log("item 1 clicked"),
  },
  {
    disabled: false,
    icon: "/static/images/eye.react.svg",
    key: "preview",
    label: "Preview",
    onClick: () => console.log("item 2 clicked"),
  },
  { isSeparator: true, key: "separator0" },
  {
    disabled: false,
    icon: "/static/images/catalog.shared.react.svg",
    key: "sharing-settings",
    label: "Sharing settings",
    onClick: () => console.log("item 3 clicked"),
  },
];

const Template = () => {
  return <NewContextMenu model={defaultModel} />;
};
export const Default = Template.bind({});
