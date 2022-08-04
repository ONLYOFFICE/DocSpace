import React, { useState } from "react";
import Box from "../box";
import ViewSelector from "./";

export default {
  title: "Components/ViewSelector",
  component: ViewSelector,
  parameters: {
    docs: {
      description: {
        component: "Actions with a button.",
      },
    },
  },
  argTypes: {
    onChangeView: {
      action: "onChangeView",
    },
  },
};

const Template = ({
  onChangeView,
  viewAs,
  viewSettings,
  isDisabled,
  isFilter,

  ...rest
}) => {
  const [view, setView] = useState(viewAs);

  return (
    <Box paddingProp="16px">
      <ViewSelector
        {...rest}
        isDisabled={isDisabled}
        viewSettings={viewSettings}
        viewAs={view}
        isFilter={isFilter}
        onChangeView={(view) => {
          onChangeView(view);
          setView(view);
        }}
      />
    </Box>
  );
};

export const Default = Template.bind({});
Default.args = {
  viewSettings: [
    {
      value: "row",
      icon: "/static/images/view-rows.react.svg",
    },
    {
      value: "tile",
      icon: "/static/images/view-tiles.react.svg",
      callback: () => console.log("callback tile click"),
    },
    {
      value: "some",
      icon: "/static/images/eye.react.svg",
      callback: () => console.log("callback some click"),
    },
  ],
  viewAs: "row",
  isDisabled: false,
  isFilter: false,
};
