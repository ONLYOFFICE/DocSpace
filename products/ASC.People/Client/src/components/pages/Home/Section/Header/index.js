import React, { useCallback } from 'react';
import { GroupButtonsMenu, DropDownItem, Text, toastr, ContextMenuButton } from 'asc-web-components';
import { connect } from 'react-redux';
import { getSelectedGroup, getSelectionIds } from '../../../../../store/people/selectors';
import { isAdmin } from '../../../../../store/auth/selectors';
import { withTranslation } from 'react-i18next';
import { updateUserStatus, updateUserType } from '../../../../../store/people/actions';
import { EmployeeStatus, EmployeeType } from '../../../../../helpers/constants';

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

const SectionHeaderContent = ({
    isHeaderVisible,
    isHeaderIndeterminate,
    isHeaderChecked,
    onCheck,
    onSelect,
    onClose,
    group,
    isAdmin,
    t,
    selection,
    updateUserStatus,
    updateUserType
  }) => {
    const selectedUserIds = getSelectionIds(selection);
    console.log("SectionHeaderContent render", selection, selectedUserIds);

    const onSetActive = useCallback(() => { 
      updateUserStatus(EmployeeStatus.Active, selectedUserIds);
      toastr.success("Set active action"); 
    }, [selectedUserIds, updateUserStatus]);

    const onSetDisabled = useCallback(() => { 
      updateUserStatus(EmployeeStatus.Disabled, selectedUserIds);
          toastr.success("Set disabled action"); 
    }, [selectedUserIds, updateUserStatus]);

    const onSetEmployee = useCallback(() => { 
      updateUserType(EmployeeType.User, selectedUserIds);
      toastr.success("Set user(s) as employees"); 
    }, [selectedUserIds, updateUserType]);

    const onSetGuest = useCallback(() => { 
      updateUserType(EmployeeType.Guest, selectedUserIds);
          toastr.success("Set user(s) as guests"); 
    }, [selectedUserIds, updateUserType]);

    const menuItems = [
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
        disabled: !selection.length,
        onClick: onSetEmployee
      },
      {
        label: "Make guest",
        disabled: !selection.length,
        onClick: onSetGuest
      },
      {
        label: "Set active",
        disabled: !selection.length,
        onClick: onSetActive
      },
      {
        label: "Set disabled",
        disabled: !selection.length,
        onClick: onSetDisabled
      },
      {
        label: "Invite again",
        disabled: !selection.length,
        onClick: toastr.success.bind(this, "Invite again action")
      },
      {
        label: "Send e-mail",
        disabled: !selection.length,
        onClick: toastr.success.bind(this, "Send e-mail action")
      },
      {
        label: t('PeopleResource:DeleteButton'),
        disabled: !selection.length,
        onClick: toastr.success.bind(this, "Delete action")
      }
    ];

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
          selected={menuItems[0].label}
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
  };

  const mapStateToProps = (state) => {
    return {
      group: getSelectedGroup(state.people.groups, state.people.selectedGroup),
      selection: state.people.selection,
      isAdmin: isAdmin(state.auth.user)
    }
  }

export default connect(mapStateToProps, { updateUserStatus, updateUserType })(withTranslation()(SectionHeaderContent));