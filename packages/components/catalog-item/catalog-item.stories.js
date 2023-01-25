import React from "react";
import CatalogItem from "./";
import CatalogFolderReactSvgUrl from "../../../public/images/catalog.folder.react.svg?url";
import CatalogGuestReactSvgUrl from "../../../public/images/catalog.guest.react.svg?url";
import CatalogTrashReactSvgUrl from "../../../public/images/catalog.trash.react.svg?url";

export default {
  title: "Components/CatalogItem",
  component: CatalogItem,
  parameters: {
    docs: {
      description: {
        component:
          "Display catalog item. Can show only icon. If is it end of block - adding margin bottom.",
      },
    },
  },
};

const Template = (args) => {
  return (
    <div style={{ width: "250px" }}>
      <CatalogItem
        {...args}
        icon={args.icon}
        text={args.text}
        showText={args.showText}
        showBadge={args.showBadge}
        onClick={() => {
          console.log("clicked item");
        }}
        isEndOfBlock={args.isEndOfBlock}
        labelBadge={args.labelBadge}
        onClickBadge={() => {
          console.log("clicked badge");
        }}
      />
    </div>
  );
};

export const Default = Template.bind({});
Default.args = {
  icon: CatalogFolderReactSvgUrl,
  text: "Documents",
  showText: true,
  showBadge: true,
  isEndOfBlock: false,
  labelBadge: "2",
};

const OnlyIcon = () => {
  return (
    <div style={{ width: "52px" }}>
      <CatalogItem
        icon={CatalogFolderReactSvgUrl}
        text={"My documents"}
        showText={false}
        showBadge={false}
      />
    </div>
  );
};

export const IconWithoutBadge = OnlyIcon.bind({});

const OnlyIconWithBadge = () => {
  return (
    <div style={{ width: "52px" }}>
      <CatalogItem
        icon={CatalogGuestReactSvgUrl}
        text={"My documents"}
        showText={false}
        showBadge={true}
      />
    </div>
  );
};

export const IconWithBadge = OnlyIconWithBadge.bind({});

const InitialIcon = () => {
  return (
    <div style={{ width: "52px" }}>
      <CatalogItem
        icon={CatalogFolderReactSvgUrl}
        text={"Documents"}
        showText={false}
        showBadge={false}
        showInitial={true}
        onClick={() => {
          console.log("clicked item");
        }}
      />
    </div>
  );
};

export const IconWithInitialText = InitialIcon.bind({});

const WithBadgeIcon = () => {
  return (
    <div style={{ width: "250px" }}>
      <CatalogItem
        icon={CatalogFolderReactSvgUrl}
        text={"My documents"}
        showText={true}
        showBadge={true}
        iconBadge={CatalogTrashReactSvgUrl}
      />
    </div>
  );
};

export const ItemWithBadgeIcon = WithBadgeIcon.bind({});

const TwoItem = () => {
  return (
    <div style={{ width: "250px" }}>
      <CatalogItem
        icon={CatalogFolderReactSvgUrl}
        text={"My documents"}
        showText={true}
        showBadge={true}
        onClick={() => {
          console.log("clicked item");
        }}
        isEndOfBlock={true}
        labelBadge={3}
        onClickBadge={() => {
          console.log("clicked badge");
        }}
      />
      <CatalogItem
        icon={CatalogFolderReactSvgUrl}
        text={"Some text"}
        showText={true}
        showBadge={true}
        onClick={() => {
          console.log("clicked item");
        }}
        iconBadge={CatalogTrashReactSvgUrl}
        onClickBadge={() => {
          console.log("clicked badge");
        }}
      />
    </div>
  );
};

export const ItemIsEndOfBlock = TwoItem.bind({});
