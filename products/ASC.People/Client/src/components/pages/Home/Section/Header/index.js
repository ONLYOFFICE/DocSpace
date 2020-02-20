import React, { useCallback, useState } from "react";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
import {
  GroupButtonsMenu,
  DropDownItem,
  toastr,
  ContextMenuButton
} from "asc-web-components";
import { Headline } from "asc-web-common";
import { connect } from "react-redux";
import {
  getSelectedGroup,
  getSelectionIds,
  getUserType,
  getGuestType,
  getUsersStatus,
  getInactiveUsers,
  getDeleteUsers
} from "../../../../../store/people/selectors";
import { withTranslation } from "react-i18next";
import {
  updateUserStatus,
  updateUserType,
  fetchPeople,
  removeUser
} from "../../../../../store/people/actions";
import { deleteGroup } from "../../../../../store/group/actions";
import { store, constants } from "asc-web-common";
import {
  InviteDialog,
  DeleteGroupUsersDialog,
  SendInviteDialog,
  SetDisabledDialog
} from "../../../../dialogs";

const { isAdmin } = store.auth.selectors;
const { EmployeeStatus, EmployeeType } = constants;

const isRefetchPeople = true;

const StyledContainer = styled.div`
  @media (min-width: 1024px) {
    ${props =>
      props.isHeaderVisible &&
      css`
        width: calc(100% + 76px);
      `}
  }

  .group-button-menu-container {
    margin: 0 -16px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
    padding-bottom: 56px;

    @media (max-width: 1024px) {
      & > div:first-child {
        ${props =>
          props.isArticlePinned &&
          css`
            width: calc(100% - 240px);
          `}
        position: absolute;
        top: 56px;
        z-index: 180;
      }
    }

    @media (min-width: 1024px) {
      margin: 0 -24px;
    }
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

        & > div:first-child {
          padding: 8px 16px 8px 16px;
          margin-right: -16px;
        }
      }
    }
  }
`;

const SectionHeaderContent = props => {
  const [dialogVisible, setDialogVisible] = useState(false);
  const [showDeleteDialog, setDeleteDialog] = useState(false);
  const [showSendInviteDialog, setSendInviteDialog] = useState(false);
  const [showDisableDialog, setShowDisableDialog] = useState(false);

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
    deleteGroup,
    userType,
    guestType,
    activeStatus,
    disabledUser,
    inviteLinkUsers,
    sendMessage,
    removeUsers
  } = props;

  const selectedUserIds = getSelectionIds(selection);
  //console.log("SectionHeaderContent render", selection, selectedUserIds);

  const onSetActive = useCallback(() => {
    onLoading(true);
    updateUserStatus(EmployeeStatus.Active, selectedUserIds, isRefetchPeople)
      .then(() => toastr.success(t("SuccessChangeUserStatus")))
      .catch(error => toastr.error(error))
      .finally(() => onLoading(false));
  }, [selectedUserIds, updateUserStatus, t, onLoading]);

  const onSetEmployee = useCallback(() => {
    updateUserType(EmployeeType.User, selectedUserIds);
    toastr.success(t("SuccessChangeUserType"));
  }, [selectedUserIds, updateUserType, t]);

  const onSetGuest = useCallback(() => {
    updateUserType(EmployeeType.Guest, selectedUserIds);
    toastr.success(t("SuccessChangeUserType"));
  }, [selectedUserIds, updateUserType, t]);

  const onSetDisabled = useCallback(
    () => setShowDisableDialog(!showDisableDialog),
    [showDisableDialog]
  );

  const onSendInviteAgain = useCallback(
    () => setSendInviteDialog(!showSendInviteDialog),
    [showSendInviteDialog]
  );

  const onDelete = useCallback(() => setDeleteDialog(!showDeleteDialog), [
    showDeleteDialog
  ]);

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
      label: t("ChangeToUser", {
        userCaption: settings.customNames.userCaption
      }),
      disabled: userType,
      onClick: onSetEmployee
    },
    {
      label: t("ChangeToGuest", {
        guestCaption: settings.customNames.guestCaption
      }),
      disabled: guestType,
      onClick: onSetGuest
    },
    {
      label: t("LblSetActive"),
      //disabled: activeStatus,
      onClick: onSetActive
    },
    {
      label: t("LblSetDisabled"),
      disabled: !disabledUser.length,
      onClick: onSetDisabled
    },
    {
      label: t("LblInviteAgain"),
      disabled: !inviteLinkUsers.length,
      onClick: onSendInviteAgain
    },
    {
      label: t("LblSendEmail"),
      disabled: sendMessage,
      onClick: toastr.success.bind(this, t("SendEmailAction"))
    },
    {
      label: t("DeleteButton"),
      disabled: !removeUsers.length,
      onClick: onDelete
    }
  ];

  const onEditGroup = useCallback(
    () => history.push(`${settings.homepage}/group/edit/${group.id}`),
    [history, settings, group]
  );

  const onDeleteGroup = useCallback(() => {
    deleteGroup(group.id).then(() =>
      toastr.success(t("SuccessfullyRemovedGroup"))
    );
  }, [deleteGroup, group, t]);

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

  const onInvitationDialogClick = useCallback(
    () => setDialogVisible(!dialogVisible),
    [dialogVisible]
  );

  const getContextOptionsPlus = useCallback(() => {
    const { guestCaption, userCaption, groupCaption } = settings.customNames;
    return [
      {
        key: "new-employee",
        label: userCaption,
        onClick: goToEmployeeCreate
      },
      {
        key: "new-guest",
        label: guestCaption,
        onClick: goToGuestCreate
      },
      {
        key: "new-group",
        label: groupCaption,
        onClick: goToGroupCreate
      },
      { key: "separator", isSeparator: true },
      {
        key: "make-invitation-link",
        label: t("MakeInvitationLink"),
        onClick: onInvitationDialogClick
      } /* ,
      {
        key: "send-invitation",
        label: t("SendInviteAgain"),
        onClick: onSentInviteAgain
      } */
    ];
  }, [
    settings,
    t,
    goToEmployeeCreate,
    goToGuestCreate,
    goToGroupCreate,
    onInvitationDialogClick /* , onSentInviteAgain */
  ]);

  const isArticlePinned = window.localStorage.getItem("asc_article_pinned_key");

  return (
    <StyledContainer
      isHeaderVisible={isHeaderVisible}
      isArticlePinned={isArticlePinned}
    >
      {showDisableDialog && (
        <SetDisabledDialog
          visible={showDisableDialog}
          users={disabledUser}
          onClose={onSetDisabled}
        />
      )}

      {showSendInviteDialog && (
        <SendInviteDialog
          visible={showSendInviteDialog}
          onClose={onSendInviteAgain}
          users={inviteLinkUsers}
        />
      )}

      {showDeleteDialog && (
        <DeleteGroupUsersDialog
          visible={showDeleteDialog}
          onClose={onDelete}
          users={removeUsers}
          filter={filter}
        />
      )}

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
              <Headline
                className="headline-header"
                type="content"
                truncate={true}
              >
                {group.name}
              </Headline>
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
              <Headline
                className="headline-header"
                truncate={true}
                type="content"
              >
                {settings.customNames.groupsCaption}
              </Headline>
              {isAdmin && (
                <>
                  <ContextMenuButton
                    className="action-button"
                    directionX="right"
                    title={t("Actions")}
                    iconName="PlusIcon"
                    size={16}
                    color="#657077"
                    getData={getContextOptionsPlus}
                    isDisabled={false}
                  />
                  {dialogVisible && (
                    <InviteDialog
                      visible={dialogVisible}
                      onClose={onInvitationDialogClick}
                      onCloseButton={onInvitationDialogClick}
                    />
                  )}
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
  const selection = state.people.selection;
  const activeUsers = 1;
  const disabledUsers = 2;
  const currentUserId = state.auth.user.id;

  return {
    group: getSelectedGroup(state.people.groups, state.people.selectedGroup),
    selection,
    isAdmin: isAdmin(state.auth.user),
    filter: state.people.filter,
    settings: state.auth.settings,

    userType: getUserType(selection),
    guestType: getGuestType(selection),
    activeStatus: getUsersStatus(selection, activeUsers, currentUserId),
    disabledUser: getUsersStatus(selection, disabledUsers, currentUserId),
    inviteLinkUsers: getInactiveUsers(selection),
    sendMessageUsers: !selection.length,
    removeUsers: getDeleteUsers(selection)
  };
};

export default connect(mapStateToProps, {
  updateUserStatus,
  updateUserType,
  fetchPeople,
  deleteGroup,
  removeUser
})(withTranslation()(withRouter(SectionHeaderContent)));
