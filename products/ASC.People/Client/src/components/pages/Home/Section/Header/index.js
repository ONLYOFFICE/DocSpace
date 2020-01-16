import React, { useCallback, useState } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import {
  GroupButtonsMenu,
  DropDownItem,
  toastr,
  ContextMenuButton
} from "asc-web-components";
import { Headline } from 'asc-web-common';
import { connect } from "react-redux";
import {
  getSelectedGroup,
  getSelectionIds
} from "../../../../../store/people/selectors";
import { withTranslation } from "react-i18next";
import {
  updateUserStatus,
  updateUserType,
  fetchPeople
} from "../../../../../store/people/actions";
import {
  typeUser,
  typeGuest,
  department
} from "../../../../../helpers/../helpers/customNames";
import { deleteGroup } from "../../../../../store/group/actions";
import { store, api, constants } from 'asc-web-common';
import { InviteDialog } from '../../../../dialogs';

const { isAdmin } = store.auth.selectors;
const { resendUserInvites, deleteUsers } = api.people;
const { EmployeeStatus, EmployeeType } = constants;

const StyledContainer = styled.div`
  .group-button-menu-container {
    margin: 0 -16px;
  }

  .header-container {
    position: relative;

    display: flex;
    align-items: center;
    max-width: calc(100vw - 32px);

    .action-button {
      margin-left: 16px;

      @media (max-width: 1024px) {
        margin-left: auto;
        padding: 8px 0 8px 8px;
      }
    }
  }
`;

const SectionHeaderContent = props => {
  const [dialogVisible, setDialogVisible] = useState(false);

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
      .catch(error => toastr.error(error));
  }, [selectedUserIds]);

  const onDelete = useCallback(() => {
    onLoading(true);
    deleteUsers(selectedUserIds)
      .then(() => {
        toastr.success("Users have been removed successfully");
        return fetchPeople(filter);
      })
      .catch(error => toastr.error(error))
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

  const getContextOptionsGroup = useCallback(() => {
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

  const goToEmployeeCreate = useCallback(() => {
    history.push(`${settings.homepage}/create/user`);
  }, [history, settings]);

  const goToGuestCreate = useCallback(() => {
    history.push(`${settings.homepage}/create/guest`);
  }, [history, settings]);

  const goToGroupCreate = useCallback(() => {
    history.push(`${settings.homepage}/group/create`);
  }, [history, settings]);

  const onInvitationDialogClick = useCallback(() => 
    setDialogVisible(!dialogVisible), [dialogVisible]
  );

  const getContextOptionsPlus = useCallback(() => {
    return [
      {
        key: "new-employee",
        label: t("CustomNewEmployee", { typeUser }),
        onClick: goToEmployeeCreate
      },
      {
        key: "new-guest",
        label: t("CustomNewGuest", { typeGuest }),
        onClick: goToGuestCreate
      },
      {
        key: "new-group",
        label: t("CustomNewDepartment", { department }),
        onClick: goToGroupCreate
      },
      { key: 'separator', isSeparator: true },
      {
        key: "make-invitation-link",
        label: t("MakeInvitationLink"),
        onClick: onInvitationDialogClick
      }/* ,
      {
        key: "send-invitation",
        label: t("SendInvitationAgain"),
        onClick: onSentInviteAgain
      } */
    ];
  }, [t, goToEmployeeCreate, goToGuestCreate, goToGroupCreate, onInvitationDialogClick/* , onSentInviteAgain */]);

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
              <Headline className='headline-header' type="content" truncate={true}>{group.name}</Headline>
              {isAdmin && (
                <ContextMenuButton
                  className="action-button"
                  directionX="right"
                  title={t("Actions")}
                  iconName="VerticalDotsIcon"
                  size={16}
                  color="#A3A9AE"
                  getData={getContextOptionsGroup}
                  isDisabled={false}
                />
              )}
            </>
          ) : (
            <>
              <Headline className='headline-header' truncate={true} type="content">Departments</Headline>
              {isAdmin && (
                <>
                <ContextMenuButton
                  className="action-button"
                  directionX="left"
                  title={t("Actions")}
                  iconName="PlusIcon"
                  size={16}
                  color="#657077"
                  getData={getContextOptionsPlus}
                  isDisabled={false}
                />
                {dialogVisible &&
                  <InviteDialog
                    visible={dialogVisible}
                    onClose={onInvitationDialogClick}
                    onCloseButton={onInvitationDialogClick}
                  />
                }
                </>
              )}
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
