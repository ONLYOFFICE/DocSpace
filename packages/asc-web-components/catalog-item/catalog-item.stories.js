import React from "react";
import CatalogItem from "./";

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
  icon: "/static/images/catalog.folder.react.svg",
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
        icon={"/static/images/catalog.folder.react.svg"}
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
        icon={"/static/images/catalog.guest.react.svg"}
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
        icon={"/static/images/catalog.folder.react.svg"}
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
        icon={"/static/images/catalog.folder.react.svg"}
        text={"My documents"}
        showText={true}
        showBadge={true}
        iconBadge={"/static/images/catalog.trash.react.svg"}
      />
    </div>
  );
};

export const ItemWithBadgeIcon = WithBadgeIcon.bind({});

const TwoItem = () => {
  return (
    <div style={{ width: "250px" }}>
      <CatalogItem
        icon={"/static/images/catalog.folder.react.svg"}
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
        icon={"/static/images/catalog.folder.react.svg"}
        text={"Some text"}
        showText={true}
        showBadge={true}
        onClick={() => {
          console.log("clicked item");
        }}
        iconBadge={"/static/images/catalog.trash.react.svg"}
        onClickBadge={() => {
          console.log("clicked badge");
        }}
      />
    </div>
  );
};

export const ItemIsEndOfBlock = TwoItem.bind({});
