import React, { useState, useEffect } from "react";
import Paging from "./";

const disable = {
  table: {
    disable: true,
  },
};

export default {
  title: "Components/Paging",
  component: Paging,
  parameters: {
    docs: {
      description: {
        component: "Paging is used to navigate med content pages",
      },
    },
  },
  argTypes: {
    onSelectPage: { action: "onSelectPage" },
    onSelectCount: { action: "onSelectCount" },
    previousAction: { action: "onPrevious" },
    nextAction: { action: "onNext" },
    selectedCount: disable,
    pageCount: disable,
    displayItems: disable,
    displayCount: disable,
  },
};

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

const selectedCountPageHandler = (count) => {
  return countItems.filter((item) => {
    if (item.key === count) {
      return item;
    }
  });
};

const Template = ({
  pageCount,
  displayItems,
  displayCount,
  nextAction,
  previousAction,
  onSelectPage,
  onSelectCount,
  selectedCount,
  ...args
}) => {
  const pageItems = createPageItems(pageCount);
  const [selectedPageItem, setSelectedPageItems] = useState(pageItems[0]);

  useEffect(() => {
    setSelectedPageItems(pageItems[0]);
  }, [pageCount]);

  const onSelectPageNextHandler = () => {
    const currentPage = pageItems.filter(
      (item) => item.key === selectedPageItem.key + 1
    );
    if (currentPage[0]) setSelectedPageItems(currentPage[0]);
  };

  const onSelectPagePrevHandler = () => {
    const currentPage = pageItems.filter(
      (item) => item.key === selectedPageItem.key - 1
    );
    if (currentPage[0]) setSelectedPageItems(currentPage[0]);
  };

  return (
    <div style={{ height: "100px" }}>
      <Paging
        {...args}
        pageItems={displayItems ? pageItems : null}
        style={{ justifyContent: "center", alignItems: "center" }}
        countItems={displayCount ? countItems : null}
        previousAction={() => {
          previousAction("Prev");
          onSelectPagePrevHandler();
        }}
        nextAction={() => {
          onSelectPageNextHandler();
          nextAction("Next");
        }}
        onSelectPage={(a) => onSelectPage(a)}
        onSelectCount={(a) => onSelectCount(a)}
        selectedPageItem={selectedPageItem}
        selectedCountItem={selectedCountPageHandler(selectedCount)[0]}
      />
    </div>
  );
};

export const Default = Template.bind({});
Default.args = {
  previousLabel: "Previous",
  nextLabel: "Next",
  displayItems: true,
  displayCount: true,
  disablePrevious: false,
  disableNext: false,
  openDirection: "bottom",
  selectedCount: 100,
  pageCount: 10,
  selectedCountItem: {
    key: 100,
    label: "100 per page",
  },
  selectedPageItem: { key: 1, label: "1 of 10" },
};
