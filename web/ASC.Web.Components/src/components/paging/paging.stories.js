import React from "react";
import { storiesOf } from "@storybook/react";
import {
  withKnobs,
  text,
  boolean,
  select,
  number,
} from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import Paging from ".";
import Section from "../../../.storybook/decorators/section";

storiesOf("Components|Paging", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => {
    const createPageItems = (count) => {
      let pageItems = [];
      for (let i = 1; i <= count; i++) {
        pageItems.push({
          key: i,
          label: i + " of " + count,
        });
      }
      return pageItems;
    };

    const countItems = [
      {
        key: 25,
        label: "25 per page",
      },
      {
        key: 50,
        label: "50 per page",
      },
      {
        key: 100,
        label: "100 per page",
      },
    ];

    const displayItems = boolean("Display pageItems", true);
    const displayCount = boolean("Display countItems", true);
    const selectedCount = select("selectedCount", [25, 50, 100], 100);
    const pageCount = number("Count of pages", 10);
    const pageItems = createPageItems(pageCount);
    const selectedPageItem = pageItems[0];
    const selectedCountItem = countItems[0];

    return (
      <Section>
        <Paging
          previousLabel={text("previousLabel", "Previous")}
          nextLabel={text("nextLabel", "Next")}
          pageItems={displayItems ? pageItems : undefined}
          selectedPageItem={selectedPageItem}
          selectedCountItem={selectedCountItem}
          countItems={displayCount ? countItems : undefined}
          disablePrevious={boolean("disablePrevious", false)}
          disableNext={boolean("disableNext", false)}
          previousAction={() => console.log("Prev")}
          nextAction={() => console.log("Next")}
          openDirection="bottom"
          selectedCount={selectedCount}
          onSelectPage={(a) => console.log(a)}
          onSelectCount={(a) => console.log(a)}
        />
      </Section>
    );
  });
