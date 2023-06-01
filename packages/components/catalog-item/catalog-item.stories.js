import React from "react";
import styled, { css } from "styled-components";
import CatalogItem from "./";
import CatalogFolderReactSvgUrl from "PUBLIC_DIR/images/catalog.folder.react.svg?url";
import CatalogGuestReactSvgUrl from "PUBLIC_DIR/images/catalog.guest.react.svg?url";
import CatalogTrashReactSvgUrl from "PUBLIC_DIR/images/catalog.trash.react.svg?url";

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

const CatalogWrapper = styled.div`
  background-color: ${(props) => props.theme.catalogItem.container.background};
  padding: 15px;
`;

const Template = (args) => {
  return (
    <CatalogWrapper style={{ width: "250px" }}>
      <CatalogItem
        {...args}
        icon={args.icon}
        text={args.text}
        showText={args.showText}
        showBadge={args.showBadge}
        onClick={() => {}}
        isEndOfBlock={args.isEndOfBlock}
        labelBadge={args.labelBadge}
        onClickBadge={() => {}}
      />
    </CatalogWrapper>
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
    <CatalogWrapper style={{ width: "52px" }}>
      <CatalogItem
        icon={CatalogFolderReactSvgUrl}
        text={"My documents"}
        showText={false}
        showBadge={false}
      />
    </CatalogWrapper>
  );
};

export const IconWithoutBadge = OnlyIcon.bind({});

const OnlyIconWithBadge = () => {
  return (
    <CatalogWrapper style={{ width: "52px" }}>
      <CatalogItem
        icon={CatalogGuestReactSvgUrl}
        text={"My documents"}
        showText={false}
        showBadge={true}
      />
    </CatalogWrapper>
  );
};

export const IconWithBadge = OnlyIconWithBadge.bind({});

const InitialIcon = () => {
  return (
    <CatalogWrapper style={{ width: "52px" }}>
      <CatalogItem
        icon={CatalogFolderReactSvgUrl}
        text={"Documents"}
        showText={false}
        showBadge={false}
        showInitial={true}
        onClick={() => {}}
      />
    </CatalogWrapper>
  );
};

export const IconWithInitialText = InitialIcon.bind({});

const WithBadgeIcon = () => {
  return (
    <CatalogWrapper style={{ width: "250px" }}>
      <CatalogItem
        icon={CatalogFolderReactSvgUrl}
        text={"My documents"}
        showText={true}
        showBadge={true}
        iconBadge={CatalogTrashReactSvgUrl}
      />
    </CatalogWrapper>
  );
};

export const ItemWithBadgeIcon = WithBadgeIcon.bind({});

const TwoItem = () => {
  return (
    <CatalogWrapper style={{ width: "250px" }}>
      <CatalogItem
        icon={CatalogFolderReactSvgUrl}
        text={"My documents"}
        showText={true}
        showBadge={true}
        onClick={() => {}}
        isEndOfBlock={true}
        labelBadge={3}
        onClickBadge={() => {}}
      />
      <CatalogItem
        icon={CatalogFolderReactSvgUrl}
        text={"Some text"}
        showText={true}
        showBadge={true}
        onClick={() => {}}
        iconBadge={CatalogTrashReactSvgUrl}
        onClickBadge={() => {}}
      />
    </CatalogWrapper>
  );
};

export const ItemIsEndOfBlock = TwoItem.bind({});
