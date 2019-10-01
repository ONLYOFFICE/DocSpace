import React, { useCallback } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import {
  GroupButtonsMenu,
  DropDownItem,
  Text,
  toastr,
  ContextMenuButton,
  IconButton
} from "asc-web-components";
import { connect } from "react-redux";
import {
  getSelectedGroup,
  getSelectionIds
} from "../../../../../store/people/selectors";
import { isAdmin } from "../../../../../store/auth/selectors";
import { withTranslation } from "react-i18next";
import {
  updateUserStatus,
  updateUserType,
  fetchPeople
} from "../../../../../store/people/actions";
import { EmployeeStatus, EmployeeType } from "../../../../../helpers/constants";
import {
  typeUser,
  typeGuest
} from "../../../../../helpers/../helpers/customNames";
import {
  resendUserInvites,
  deleteUsers
} from "../../../../../store/services/api";
import { deleteGroup } from "../../../../../store/group/actions";

const StyledContainer = styled.div`
  .group-button-menu-container {
    margin: 0 -16px;
  }

  .header-container {
    display: flex;
    align-items: center;
    max-width: calc(100vw - 32px);
    .add-group-button {
      margin-left: 8px;
    }
  }
`;

const SectionHeaderContent = props => {
  const {
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
    updateUserType,
    onLoading,
    filter,
    history,
    settings,
    deleteGroup
  } = props;

  const selectedUserIds = getSelectionIds(selection);
  //console.log("SectionHeaderContent render", selection, selectedUserIds);

  const onSetActive = useCallback(() => {
    updateUserStatus(EmployeeStatus.Active, selectedUserIds);
    toastr.success(t("SuccessChangeUserStatus"));
  }, [selectedUserIds, updateUserStatus, t]);

  const onSetDisabled = useCallback(() => {
    updateUserStatus(EmployeeStatus.Disabled, selectedUserIds);
    toastr.success(t("SuccessChangeUserStatus"));
  }, [selectedUserIds, updateUserStatus, t]);

  const onSetEmployee = useCallback(() => {
    updateUserType(EmployeeType.User, selectedUserIds);
    toastr.success(t("SuccessChangeUserType"));
  }, [selectedUserIds, updateUserType, t]);

  const onSetGuest = useCallback(() => {
    updateUserType(EmployeeType.Guest, selectedUserIds);
    toastr.success(t("SuccessChangeUserType"));
  }, [selectedUserIds, updateUserType, t]);

  const onSentInviteAgain = useCallback(() => {
    resendUserInvites(selectedUserIds)
      .then(() => toastr.success("The invitation was successfully sent"))
      .catch(e => toastr.error("ERROR"));
  }, [selectedUserIds]);

  const onDelete = useCallback(() => {
    onLoading(true);
    deleteUsers(selectedUserIds)
      .then(() => {
        toastr.success("Users have been removed successfully");
        return fetchPeople(filter);
      })
      .catch(e => toastr.error("ERROR"))
      .finally(() => onLoading(false));
  }, [selectedUserIds, onLoading, filter]);

  const menuItems = [
    {
      label: t("LblSelect"),
      isDropdown: true,
      isSeparator: true,
      isSelect: true,
      fontWeight: "bold",
      children: [
        <DropDownItem key="active" label={t("LblActive")} />,
        <DropDownItem key="disabled" label={t("LblTerminated")} />,
        <DropDownItem key="invited" label={t("LblInvited")} />
      ],
      onSelect: item => onSelect(item.key)
    },
    {
      label: t("CustomMakeUser", { typeUser }),
      disabled: !selection.length,
      onClick: onSetEmployee
    },
    {
      label: t("CustomMakeGuest", { typeGuest }),
      disabled: !selection.length,
      onClick: onSetGuest
    },
    {
      label: t("LblSetActive"),
      disabled: !selection.length,
      onClick: onSetActive
    },
    {
      label: t("LblSetDisabled"),
      disabled: !selection.length,
      onClick: onSetDisabled
    },
    {
      label: t("LblInviteAgain"),
      disabled: !selection.length,
      onClick: onSentInviteAgain
    },
    {
      label: t("LblSendEmail"),
      disabled: !selection.length,
      onClick: toastr.success.bind(this, "Send e-mail action")
    },
    {
      label: t("DeleteButton"),
      disabled: !selection.length,
      onClick: onDelete
    }
  ];

  const onEditGroup = useCallback(
    () => history.push(`${settings.homepage}/group/edit/${group.id}`),
    [history, settings, group]
  );

  const onDeleteGroup = useCallback(() => {
    deleteGroup(group.id).then(() =>
      toastr.success("Group has been removed successfully")
    );
  }, [deleteGroup, group]);

  const getContextOptions = useCallback(() => {
    return [
      {
        key: "edit-group",
        label: t("EditButton"),
        onClick: onEditGroup
      },
      {
        key: "delete-group",
        label: t("DeleteButton"),
        onClick: onDeleteGroup
      }
    ];
  }, [t, onEditGroup, onDeleteGroup]);

  const onAddDepartmentsClick = useCallback(() => {
    history.push(`${settings.homepage}/group/create`);
  }, [history, settings]);

  return (
    <StyledContainer>
      {isHeaderVisible ? (
        <div className="group-button-menu-container">
          <GroupButtonsMenu
            checked={isHeaderChecked}
            isIndeterminate={isHeaderIndeterminate}
            onChange={onCheck}
            menuItems={menuItems}
            visible={isHeaderVisible}
            moreLabel={t("More")}
            closeTitle={t("CloseButton")}
            onClose={onClose}
            selected={menuItems[0].label}
          />
        </div>
      ) : (
        <div className="header-container">
          {group ? (
            <>
              <Text.ContentHeader truncate={true}>{group.name}</Text.ContentHeader>
              {isAdmin && (
                <ContextMenuButton
                  directionX="right"
                  title={t("Actions")}
                  iconName="VerticalDotsIcon"
                  size={16}
                  color="#A3A9AE"
                  getData={getContextOptions.bind(this, t)}
                  isDisabled={false}
                />
              )}
            </>
          ) : (
            <>
              <Text.ContentHeader>Departments</Text.ContentHeader>
              
            </>
          )}
        </div>
      )}
    </StyledContainer>
  );
};

const mapStateToProps = state => {
  return {
    group: getSelectedGroup(state.people.groups, state.people.selectedGroup),
    selection: state.people.selection,
    isAdmin: isAdmin(state.auth.user),
    filter: state.people.filter,
    settings: state.auth.settings
  };
};

export default connect(
  mapStateToProps,
  { updateUserStatus, updateUserType, fetchPeople, deleteGroup }
)(withTranslation()(withRouter(SectionHeaderContent)));
