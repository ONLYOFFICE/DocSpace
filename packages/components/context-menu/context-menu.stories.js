import React, { useRef } from "react";
import CatalogFolderReactSvgUrl from "../../../public/images/catalog.folder.react.svg?url";
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
      icon: CatalogFolderReactSvgUrl,
    },
    {
      label: "Preview",
      icon: CatalogFolderReactSvgUrl,
    },
    {
      separator: true,
    },
    {
      label: "Sharing settings",
      icon: CatalogFolderReactSvgUrl,
    },
    {
      label: "Link for portal users",
      icon: CatalogFolderReactSvgUrl,
    },
    {
      label: "Copy external link",
      icon: CatalogFolderReactSvgUrl,
    },
    {
      label: "Send by e-mail",
      icon: CatalogFolderReactSvgUrl,
    },
    {
      label: "Version history",
      icon: CatalogFolderReactSvgUrl,
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
      icon: CatalogFolderReactSvgUrl,
    },
    {
      label: "Download",
      icon: CatalogFolderReactSvgUrl,
    },
    {
      label: "Download as",
      icon: CatalogFolderReactSvgUrl,
    },
    {
      label: "Move or copy",
      icon: CatalogFolderReactSvgUrl,
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
      icon: CatalogFolderReactSvgUrl,
      disabled: true,
    },
    {
      separator: true,
    },
    {
      label: "Quit",
      icon: CatalogFolderReactSvgUrl,
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
