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
      label: "File",
      items: [
        {
          label: "New",
          items: [
            {
              label: "Bookmark",
            },
            {
              label: "Video",
            },
          ],
        },
        {
          label: "Delete",
        },
        {
          separator: true,
        },
        {
          label: "Export",
        },
      ],
    },
    {
      label: "Edit",
      items: [
        {
          label: "Left",
        },
        {
          label: "Right",
        },
        {
          label: "Center",
        },
        {
          label: "Justify",
        },
      ],
    },
    {
      label: "Users",
      items: [
        {
          label: "New",
        },
        {
          label: "Delete",
        },
        {
          label: "Search",
          items: [
            {
              label: "Filter",
              items: [
                {
                  label: "Print",
                },
              ],
            },
            {
              label: "List",
            },
          ],
        },
      ],
    },
    {
      label: "Events",
      items: [
        {
          label: "Edit",
          items: [
            {
              label: "Save",
            },
            {
              label: "Delete",
            },
          ],
        },
        {
          label: "Archieve",
          items: [
            {
              label: "Remove",
            },
          ],
        },
      ],
    },
    {
      separator: true,
    },
    {
      label: "Quit",
    },
  ];

  return (
    <div>
      <ContextMenu model={items} ref={cm}></ContextMenu>

      <img
        src="https://cdn2.thecatapi.com/images/ac8.jpg"
        alt="Logo"
        onContextMenu={(e) => cm.current.show(e)}
      />
    </div>
  );
};

export const Default = Template.bind({});
