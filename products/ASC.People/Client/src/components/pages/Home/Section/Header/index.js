import React, { useMemo } from 'react';
import { GroupButtonsMenu, DropDownItem, Text } from 'asc-web-components';

const getPeopleItems = (onSelect) => [
    {
      label: "Select",
      isDropdown: true,
      isSeparator: true,
      isSelect: true,
      fontWeight: "bold",
      children: [
        <DropDownItem key="active" label="Active" />,
        <DropDownItem key="disabled" label="Disabled" />,
        <DropDownItem key="invited" label="Invited" />
      ],
      onSelect: (item) => onSelect(item.key)
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

const SectionHeaderContent = React.memo(({
    isHeaderVisible,
    isHeaderIndeterminate,
    isHeaderChecked,
    onCheck,
    onSelect,
    onClose
  }) => {
    console.log("SectionHeaderContent render");
    const menuItems = getPeopleItems(onSelect);
    return (
    isHeaderVisible ? (
      <div style={{ margin: "0 -16px" }}>
        <GroupButtonsMenu
          checked={isHeaderChecked}
          isIndeterminate={isHeaderIndeterminate}
          onChange={onCheck}
          menuItems={menuItems}
          visible={isHeaderVisible}
          moreLabel="More"
          closeTitle="Close"
          onClose={onClose}
          selected={getPeopleItems(onSelect)[0].label}
        />
      </div>
    ) : (
      <Text.ContentHeader>People</Text.ContentHeader>
    )
    );
  });

export default SectionHeaderContent;