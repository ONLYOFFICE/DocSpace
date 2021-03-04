import React from "react";
import EmptyScreenContainer from "./";
import Link from "../link";
import CrossIcon from "../../../public/images/cross.react.svg";

export default {
  title: "Components/EmptyScreenContainer",
  component: EmptyScreenContainer,
  argTypes: { onClick: { action: "Reset filter clicked" } },
  parameters: {
    docs: {
      description: {
        component: `Used to display empty screen page

### Properties

| Props             |      Type      | Required | Values | Default | Description                             |
| ----------------- | :------------: | :------: | :----: | :-----: | --------------------------------------- |
| buttons         |   element    |    -     |   -    |    -    | Content of EmptyContentButtonsContainer |
| className       |    string    |    -     |   -    |    -    | Accepts class                           |
| descriptionText |    string    |    -     |   -    |    -    | Description text                        |
| headerText      |    string    |    -     |   -    |    -    | Header text                             |
| subheadingText  |    string    |    -     |   -    |    -    | Subheading text                         |
| id              |    string    |    -     |   -    |    -    | Accepts id                              |
| imageAlt        |    string    |    -     |   -    |    -    | Alternative image text                  |
| imageSrc        |    string    |    -     |   -    |    -    | Image url source                        |
| style           | obj, array |    -     |   -    |    -    | Accepts css style                       |
        `,
      },
      source: {
        code: `
        import EmptyScreenContainer from "@appserver/components/empty-screen-container";

<EmptyScreenContainer
  imageSrc="empty_screen_filter.png"
  imageAlt="Empty Screen Filter image"
  headerText="No results matching your search could be found"
  subheading="No files to be displayed in this section"
  descriptionText="No results matching your search could be found"
  buttons={<a href="/">Go to home</a>}
/>`,
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
  imageSrc: "empty_screen_filter.png",
  imageAlt: "Empty Screen Filter image",
  headerText: "No results matching your search could be found",
  subheadingText: "No files to be displayed in this section",
  descriptionText:
    "No people matching your filter can be displayed in this section. Please select other filter options or clear filter to view all the people in this section.",
};
