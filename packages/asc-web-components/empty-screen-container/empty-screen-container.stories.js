import React from "react";
import EmptyScreenContainer from "./";
import Link from "../link";
import CrossIcon from "../../../public/images/cross.react.svg";

export default {
  title: "Components/EmptyScreenContainer",
  component: EmptyScreenContainer,
  argTypes: {
    onClick: { action: "Reset filter clicked", table: { disable: true } },
  },
  parameters: {
    docs: {
      description: {
        component: "Used to display empty screen page",
      },
    },
  },
};

const Template = (args) => {
  return (
    <EmptyScreenContainer
      {...args}
      buttons={
        <>
          <CrossIcon size="small" style={{ marginRight: "4px" }} />
          <Link type="action" isHovered={true} onClick={(e) => args.onClick(e)}>
            Reset filter
          </Link>
        </>
      }
    />
  );
};

export const Default = Template.bind({});

Default.args = {
  imageSrc: "/static/images/empty_screen_filter.png",
  imageAlt: "Empty Screen Filter image",
  headerText: "No results matching your search could be found",
  subheadingText: "No files to be displayed in this section",
  descriptionText:
    "No people matching your filter can be displayed in this section. Please select other filter options or clear filter to view all the people in this section.",
};
