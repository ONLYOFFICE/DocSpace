import React from "react";
import styled from "styled-components";

import Selector from "./";
import CustomSvgUrl from "PUBLIC_DIR/images/icons/32/room/custom.svg?url";
import ArchiveSvgUrl from "PUBLIC_DIR/images/room.archive.svg?url";
import EmptyScreenFilter from "PUBLIC_DIR/images/empty_screen_filter.png";

const StyledRowLoader = styled.div`
  width: 100%;
  height: 48px;
`;

const StyledSearchLoader = styled.div`
  width: 100%;
  height: 32px;
  background: black;
`;

export default {
  title: "Components/Selector",
  component: Selector,
  parameters: {
    docs: {
      description: {
        component:
          "Selector for displaying items list of people or room selector",
      },
    },
  },
  argTypes: {
    height: {
      table: {
        disable: true,
      },
    },
  },
};

function makeName() {
  var result = "";
  var characters =
    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
  var charactersLength = characters.length;
  for (var i = 0; i < 15; i++) {
    result += characters.charAt(Math.floor(Math.random() * charactersLength));
  }
  return result;
}

const getItems = (count) => {
  const items = [];

  for (let i = 0; i < count / 2; i++) {
    items.push({
      key: `user_${i}`,
      id: `user_${i}`,
      label: makeName() + " " + i,
      icon: CustomSvgUrl,
    });
  }

  for (let i = 0; i < count / 2; i++) {
    items.push({
      key: `room_${i}`,
      id: `room_${i}`,
      label: makeName() + " " + i + " room",
      icon: CustomSvgUrl,
    });
  }

  return items;
};

const getAccessRights = () => {
  const accesses = [
    {
      key: "roomManager",
      label: "Room manager",
      access: 0,
    },
    {
      key: "editor",
      label: "Editor",
      access: 1,
    },
    {
      key: "formFiller",
      label: "Form filler",
      access: 2,
    },
    {
      key: "reviewer",
      label: "Reviewer",
      access: 3,
    },
    {
      key: "commentator",
      label: "Commentator",
      access: 4,
    },
    {
      key: "viewer",
      label: "Viewer",
      access: 5,
    },
  ];

  return accesses;
};

const items = getItems(100000);

const selectedItems = [items[0], items[3], items[7]];

const accessRights = getAccessRights();

const selectedAccessRight = accessRights[1];

const renderedItems = items.slice(0, 100);
const totalItems = items.length;

const Template = (args) => {
  const [rendItems, setRendItems] = React.useState(renderedItems);

  const loadNextPage = (index) => {
    setRendItems((val) => [...val, ...items.slice(index, index + 100)]);
  };

  const rowLoader = <StyledRowLoader />;
  const searchLoader = <StyledSearchLoader className="search-loader" />;

  return (
    <div
      style={{
        width: "480px",
        height: args.height,
        border: "1px solid #eee",
        margin: "auto",
      }}
    >
      <Selector
        {...args}
        items={rendItems}
        loadNextPage={loadNextPage}
        searchLoader={searchLoader}
        rowLoader={rowLoader}
      />
    </div>
  );
};

export const Default = Template.bind({});

Default.args = {
  height: "485px", // container height
  headerLabel: "Room list",
  onBackClick: () => {},
  searchPlaceholder: "Search",
  searchValue: "",
  items: renderedItems,
  onSelect: (item) => {},
  isMultiSelect: false,
  selectedItems: selectedItems,
  acceptButtonLabel: "Add",
  onAccept: (items, access) => {},
  withSelectAll: false,
  selectAllLabel: "All accounts",
  selectAllIcon: ArchiveSvgUrl,
  onSelectAll: () => {},
  withAccessRights: false,
  accessRights,
  selectedAccessRight,
  onAccessRightsChange: (access) => {},
  withCancelButton: false,
  cancelButtonLabel: "Cancel",
  onCancel: () => {},
  emptyScreenImage: EmptyScreenFilter,
  emptyScreenHeader: "No other accounts here yet",
  emptyScreenDescription:
    "The list of users previously invited to DocSpace or separate rooms will appear here. You will be able to invite these users for collaboration at any time.",
  searchEmptyScreenImage: EmptyScreenFilter,
  searchEmptyScreenHeader: "No other accounts here yet search",
  searchEmptyScreenDescription:
    " SEARCH !!! The list of users previously invited to DocSpace or separate rooms will appear here. You will be able to invite these users for collaboration at any time.",
  totalItems,
  hasNextPage: true,
  isNextPageLoading: false,
  isLoading: false,
};
