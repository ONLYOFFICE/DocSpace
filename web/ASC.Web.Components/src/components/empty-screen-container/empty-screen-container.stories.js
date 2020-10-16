import React from "react";
import { storiesOf } from "@storybook/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import { withKnobs, text } from "@storybook/addon-knobs/react";
import { action } from "@storybook/addon-actions";
import EmptyScreenContainer from ".";
import Link from "../link";
import { Icons } from "../icons";

storiesOf("Components| EmptyScreenContainer", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => (
    <EmptyScreenContainer
      imageSrc={text("imageSrc", "empty_screen_filter.png")}
      imageAlt={text("imageAlt", "Empty Screen Filter image")}
      headerText={text(
        "headerText",
        "No results matching your search could be found"
      )}
      subheadingText={text(
        "subheaderText",
        "No files to be displayed in this section"
      )}
      descriptionText={text(
        "descriptionText",
        "No people matching your filter can be displayed in this section. Please select other filter options or clear filter to view all the people in this section."
      )}
      buttons={
        <>
          <Icons.CrossIcon size="small" style={{ marginRight: "4px" }} />
          <Link
            type="action"
            isHovered={true}
            onClick={(e) => action("Reset filter clicked")(e)}
          >
            Reset filter
          </Link>
        </>
      }
    />
  ));
