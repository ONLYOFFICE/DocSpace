import React, { useState } from "react";
import Box from "../box";
import ViewSelector from "./";

import ViewRowsReactSvg from "PUBLIC_DIR/images/view-rows.react.svg?url";
import ViewTilesReactSvg from "PUBLIC_DIR/images/view-tiles.react.svg?url";
import EyeReactSvg from "PUBLIC_DIR/images/eye.react.svg?url";

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
      icon: ViewRowsReactSvg,
    },
    {
      value: "tile",
      icon: ViewTilesReactSvg,
      callback: () => console.log("callback tile click"),
    },
    {
      value: "some",
      icon: EyeReactSvg,
      callback: () => console.log("callback some click"),
    },
  ],
  viewAs: "row",
  isDisabled: false,
  isFilter: false,
};
