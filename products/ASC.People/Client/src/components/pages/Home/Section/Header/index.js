import React from 'react';
import { GroupButtonsMenu, DropDownItem } from 'asc-web-components';

const peopleItems = [
    {
      label: "Select",
      isDropdown: true,
      isSeparator: true,
      fontWeight: "bold",
      children: [
        <DropDownItem key="active" label="Active" />,
        <DropDownItem key="disabled" label="Disabled" />,
        <DropDownItem key="invited" label="Invited" />
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
    onCheck
  }) =>
    isHeaderVisible ? (
      <div style={{ margin: "0 -16px" }}>
        <GroupButtonsMenu
          checked={isHeaderChecked}
          isIndeterminate={isHeaderIndeterminate}
          onChange={onCheck}
          menuItems={peopleItems}
          visible={isHeaderVisible}
          moreLabel="More"
          closeTitle="Close"
        />
      </div>
    ) : (
      "People"
    );

export default SectionHeaderContent;