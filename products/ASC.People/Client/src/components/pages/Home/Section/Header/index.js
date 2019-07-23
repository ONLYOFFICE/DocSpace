import React from 'react';
import { GroupButtonsMenu, DropDownItem, Text } from 'asc-web-components';

const getPeopleItems = (onSelect) => [
    {
      label: "Select",
      isDropdown: true,
      isSeparator: true,
      fontWeight: "bold",
      children: [
        <DropDownItem key="active" label="Active" onClick={() => onSelect("active")} />,
        <DropDownItem key="disabled" label="Disabled" onClick={() => onSelect("disabled")} />,
        <DropDownItem key="invited" label="Invited" onClick={() => onSelect("invited")} />
      ]
    },
    {
      label: "Make employee",
      onClick: () => console.log("Make employee action")
    },
    {
      label: "Make guest",
      onClick: () => console.log("Make guest action")
    },
    {
      label: "Set active",
      onClick: () => console.log("Set active action")
    },
    {
      label: "Set disabled",
      onClick: () => console.log("Set disabled action")
    },
    {
      label: "Invite again",
      onClick: () => console.log("Invite again action")
    },
    {
      label: "Send e-mail",
      onClick: () => console.log("Send e-mail action")
    },
    {
      label: "Delete",
      onClick: () => console.log("Delete action")
    }
  ];

const SectionHeaderContent = ({
    isHeaderVisible,
    isHeaderIndeterminate,
    isHeaderChecked,
    onCheck,
    onSelect,
    onClose
  }) =>
    isHeaderVisible ? (
      <div style={{ margin: "0 -16px" }}>
        <GroupButtonsMenu
          checked={isHeaderChecked}
          isIndeterminate={isHeaderIndeterminate}
          onChange={onCheck}
          menuItems={getPeopleItems(onSelect)}
          visible={isHeaderVisible}
          moreLabel="More"
          closeTitle="Close"
          onClose={onClose}
        />
      </div>
    ) : (
      <Text.ContentHeader>People</Text.ContentHeader>
    );

export default SectionHeaderContent;