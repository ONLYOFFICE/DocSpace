import React, { useRef } from "react";

import ContextMenu from "./index";

export default {
  title: "Components/ContextMenu",
  component: ContextMenu,
  parameters: {
    docs: {
      description: {
        component: `ContextMenu is used for a call context actions on a page.
        Implemented as part of RowContainer component.

For use within separate component it is necessary to determine active zone and events for calling and transferring options in menu.

In particular case, state is created containing options for particular Row element and passed to component when called.
        `,
      },
    },
  },
};

const Template = (args) => {
  const cm = useRef(null);
  const items = [
    {
      label: "Edit",
      icon: "/static/images/catalog.folder.react.svg",
    },
    {
      label: "Preview",
      icon: "/static/images/catalog.folder.react.svg",
    },
    {
      separator: true,
    },
    {
      label: "Sharing settings",
      icon: "/static/images/catalog.folder.react.svg",
    },
    {
      label: "Link for portal users",
      icon: "/static/images/catalog.folder.react.svg",
    },
    {
      label: "Copy external link",
      icon: "/static/images/catalog.folder.react.svg",
    },
    {
      label: "Send by e-mail",
      icon: "/static/images/catalog.folder.react.svg",
    },
    {
      label: "Version history",
      icon: "/static/images/catalog.folder.react.svg",
      items: [
        {
          label: "Show version history",
        },
        {
          label: "Finalize version",
        },
        {
          label: "Unblock / Check-in",
        },
      ],
    },
    {
      separator: true,
    },
    {
      label: "Make as favorite",
      icon: "/static/images/catalog.folder.react.svg",
    },
    {
      label: "Download",
      icon: "/static/images/catalog.folder.react.svg",
    },
    {
      label: "Download as",
      icon: "/static/images/catalog.folder.react.svg",
    },
    {
      label: "Move or copy",
      icon: "/static/images/catalog.folder.react.svg",
      items: [
        {
          label: "Move to",
        },
        {
          label: "Copy",
        },
        {
          label: "Duplicate",
        },
      ],
    },
    {
      label: "Rename",
      icon: "/static/images/catalog.folder.react.svg",
      disabled: true,
    },
    {
      separator: true,
    },
    {
      label: "Quit",
      icon: "/static/images/catalog.folder.react.svg",
    },
  ];

  return (
    <div>
      <ContextMenu model={items} ref={cm}></ContextMenu>

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
