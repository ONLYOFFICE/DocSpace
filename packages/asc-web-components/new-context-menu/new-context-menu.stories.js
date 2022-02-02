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

const Template = (args) => {
  const cm = React.useRef(null);
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
    {
      disabled: false,
      icon: "/static/images/catalog.shared.react.svg",
      key: "sharing-settings1",
      label: "Sharing settings",
      onClick: () => console.log("item 3 clicked"),
    },
    {
      disabled: false,
      icon: "/static/images/catalog.shared.react.svg",
      key: "sharing-settings2",
      label: "Sharing settings",
      onClick: () => console.log("item 3 clicked"),
    },
    {
      disabled: false,
      icon: "/static/images/catalog.shared.react.svg",
      key: "sharing-settings3",
      label: "Sharing settings",
      onClick: () => console.log("item 3 clicked"),
    },
    {
      disabled: false,
      icon: "/static/images/catalog.shared.react.svg",
      key: "sharing-settings4",
      label: "Sharing settings",
      onClick: () => console.log("item 3 clicked"),
    },
    {
      disabled: false,
      icon: "/static/images/catalog.shared.react.svg",
      key: "sharing-settings5",
      label: "Sharing settings",
      onClick: () => console.log("item 3 clicked"),
    },
    {
      disabled: false,
      icon: "/static/images/catalog.shared.react.svg",
      key: "sharing-settings6",
      label: "Sharing settings",
      onClick: () => console.log("item 3 clicked"),
    },
    {
      disabled: false,
      icon: "/static/images/catalog.shared.react.svg",
      key: "sharing-settings7",
      label: "Sharing settings",
      onClick: () => console.log("item 3 clicked"),
    },
    {
      disabled: false,
      icon: "/static/images/catalog.shared.react.svg",
      key: "sharing-settings8",
      label: "Sharing settings",
      onClick: () => console.log("item 3 clicked"),
    },
    {
      disabled: false,
      icon: "/static/images/catalog.shared.react.svg",
      key: "sharing-settings9",
      label: "Sharing settings",
      onClick: () => console.log("item 3 clicked"),
    },
    {
      disabled: false,
      icon: "/static/images/catalog.shared.react.svg",
      key: "sharing-settings10",
      label: "Sharing settings",
      options: [
        {
          key: "key1",
          icon: "static/images/nav.logo.react.svg",
          label: "Item after separator",
          onClick: () => console.log("Button 1 clicked"),
        },
        {
          key: "key2",
          icon: "static/images/nav.logo.react.svg",
          label: "Item after separator",
          onClick: () => console.log("Button 2 clicked"),
        },
      ],
      onClick: () => console.log("item 3 clicked"),
    },
    {
      disabled: false,
      icon: "/static/images/catalog.shared.react.svg",
      key: "sharing-settings11",
      label: "Sharing settings",
      onClick: () => console.log("item 3 clicked"),
    },
    {
      disabled: false,
      icon: "/static/images/catalog.shared.react.svg",
      key: "sharing-settings12",
      label: "Sharing settings",
      onClick: () => console.log("item 3 clicked"),
    },
    {
      disabled: false,
      icon: "/static/images/catalog.shared.react.svg",
      key: "sharing-settings13",
      label: "Sharing settings",
      onClick: () => console.log("item 3 clicked"),
    },
  ];

  const headerOptions = {
    icon: "/static/images/catalog.shared.react.svg",
    title: "File name",
  };

  return (
    <div>
      <NewContextMenu
        model={defaultModel}
        header={headerOptions}
        ref={cm}
      ></NewContextMenu>

      <div
        style={{
          width: "200px",
          height: "200px",
          backgroundColor: "red",
          display: "inline-block",
        }}
        onContextMenu={(e) => cm.current.show(e)}
      >
        {""}
      </div>
    </div>
  );
};
export const Default = Template.bind({});
