import React from "react";
import MenuItem from ".";

export default {
  title: "Components/MenuItem",
  component: MenuItem,

  argTypes: {
    onClick: { action: "onClick" },
  },
  parameters: {
    docs: {
      description: {
        component: `Is a item of DropDown or ContextMenu component

An item can act as separator, header, line, line with arrow or container.

When used as container, it will retain all styling features and positioning. To disable hover effects in container mode, you can use _noHover_ property.`,
      },
    },
  },
};

const Template = () => {
  return (
    <div style={{ width: "250px", position: "relative" }}>
      <MenuItem
        icon={"static/images/nav.logo.react.svg"}
        label="Header(tablet or mobile)"
        isHeader={true}
        onClick={() => console.log("Header clicked")}
        noHover={true}
      />
      <MenuItem
        icon={"static/images/nav.logo.react.svg"}
        label="First item"
        onClick={() => console.log("Button 1 clicked")}
      />
      <MenuItem isSeparator={true} />
      <MenuItem
        icon={"static/images/nav.logo.react.svg"}
        label="Item after separator"
        onClick={() => console.log("Button 2 clicked")}
      />
      <MenuItem onClick={() => console.log("Button 3 clicked")}>
        <div>some child without styles</div>
      </MenuItem>
    </div>
  );
};
export const Default = Template.bind({});
