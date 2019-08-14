import React from 'react';
import { GroupButtonsMenu, DropDownItem, Text, toastr, ContextMenuButton } from 'asc-web-components';
import { connect } from 'react-redux';
import { getSelectedGroup } from '../../../../../store/people/selectors';
import { isAdmin } from '../../../../../store/auth/selectors';

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
      onClick: toastr.success.bind(this, "Make employee action")
    },
    {
      label: "Make guest",
      onClick: toastr.success.bind(this, "Make guest action")
    },
    {
      label: "Set active",
      onClick: toastr.success.bind(this, "Set active action")
    },
    {
      label: "Set disabled",
      onClick: toastr.success.bind(this, "Set disabled action")
    },
    {
      label: "Invite again",
      onClick: toastr.success.bind(this, "Invite again action")
    },
    {
      label: "Send e-mail",
      onClick: toastr.success.bind(this, "Send e-mail action")
    },
    {
      label: "Delete",
      onClick: toastr.success.bind(this, "Delete action")
    }
  ];

  const contextOptions = () => {
    return [
      {
        key: "edit-group",
        label: "Edit",
        onClick: toastr.success.bind(this, "Edit group action")
      },
      {
        key: "delete-group",
        label: "Delete",
        onClick: toastr.success.bind(this, "Delete group action")
      }
    ];
  };

  const wrapperStyle = {
    display: "flex",
    alignItems: "center"
  };

const SectionHeaderContent = React.memo(({
    isHeaderVisible,
    isHeaderIndeterminate,
    isHeaderChecked,
    onCheck,
    onSelect,
    onClose,
    group,
    isAdmin
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
      group 
      ? <div style={wrapperStyle}>
      <Text.ContentHeader>{group.name}</Text.ContentHeader>
      {isAdmin &&
        <ContextMenuButton 
          directionX='right' 
          title='Actions' 
          iconName='VerticalDotsIcon' 
          size={16} 
          color='#A3A9AE' 
          getData={contextOptions}
          isDisabled={false}/>
      }
      </div>
      : <Text.ContentHeader>People</Text.ContentHeader>
    )
    );
  });

  const mapStateToProps = (state) => {
    return {
      group: getSelectedGroup(state.people.groups, state.people.selectedGroup),
      isAdmin: isAdmin(state.auth.user)
    }
  }

export default connect(mapStateToProps)(SectionHeaderContent);