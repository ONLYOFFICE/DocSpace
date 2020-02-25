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
  getUserType,
  getUsersStatus,
  getInactiveUsers,
  getDeleteUsers,
  getUsersIds
} from "../../../../../store/people/selectors";
import { withTranslation } from "react-i18next";
import {
  updateUserStatus,
  fetchPeople,
  removeUser,
  setSelected
} from "../../../../../store/people/actions";
import { deleteGroup } from "../../../../../store/group/actions";
import { store, constants } from "asc-web-common";
import {
  InviteDialog,
  DeleteGroupUsersDialog,
  SendInviteDialog,
  ChangeUserStatusDialog,
  ChangeUserTypeDialog
} from "../../../../dialogs";

const { isAdmin } = store.auth.selectors;
const { EmployeeType, EmployeeStatus } = constants;

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
  const [showActiveDialog, setShowActiveDialog] = useState(false);
  const [showGuestDialog, setShowGuestDialog] = useState(false);
  const [showEmployeeDialog, setShowEmployeeDialog] = useState(false);

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
    filter,
    history,
    settings,
    deleteGroup,
    usersWithEmployeeType,
    usersWithGuestType,
    usersWithActiveStatus,
    usersWithDisableStatus,
    usersToInvite,
    usersToSendMessage,
    usersToRemove,
    setSelected,
    selection
  } = props;

  //console.log("SectionHeaderContent render");

  const onSetEmployee = useCallback(
    () => setShowEmployeeDialog(!showEmployeeDialog),
    [showEmployeeDialog]
  );

  const onSetGuest = useCallback(() => setShowGuestDialog(!showGuestDialog), [
    showGuestDialog
  ]);

  const onSetActive = useCallback(
    () => setShowActiveDialog(!showActiveDialog),
    [showActiveDialog]
  );

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
      disabled: !usersWithEmployeeType.length,
      onClick: onSetEmployee
    },
    {
      label: t("ChangeToGuest", {
        guestCaption: settings.customNames.guestCaption
      }),
      disabled: !usersWithGuestType.length,
      onClick: onSetGuest
    },
    {
      label: t("LblSetActive"),
      disabled: !usersWithActiveStatus.length,
      onClick: onSetActive
    },
    {
      label: t("LblSetDisabled"),
      disabled: !usersWithDisableStatus.length,
      onClick: onSetDisabled
    },
    {
      label: t("LblInviteAgain"),
      disabled: !usersToInvite.length,
      onClick: onSendInviteAgain
    },
    {
      label: t("LblSendEmail"),
      disabled: usersToSendMessage,
      onClick: toastr.success.bind(this, t("SendEmailAction"))
    },
    {
      label: t("DeleteButton"),
      disabled: !usersToRemove.length,
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
      {showEmployeeDialog && (
        <ChangeUserTypeDialog
          visible={showEmployeeDialog}
          userIds={getUsersIds(usersWithEmployeeType)}
          selectedUsers={selection}
          onClose={onSetEmployee}
          userType={EmployeeType.User}
          setSelected={setSelected}
        />
      )}

      {showGuestDialog && (
        <ChangeUserTypeDialog
          visible={showGuestDialog}
          userIds={getUsersIds(usersWithGuestType)}
          selectedUsers={selection}
          onClose={onSetGuest}
          userType={EmployeeType.Guest}
          setSelected={setSelected}
        />
      )}
      {showActiveDialog && (
        <ChangeUserStatusDialog
          visible={showActiveDialog}
          userIds={getUsersIds(usersWithActiveStatus)}
          selectedUsers={selection}
          onClose={onSetActive}
          userStatus={EmployeeStatus.Active}
          setSelected={setSelected}
        />
      )}
      {showDisableDialog && (
        <ChangeUserStatusDialog
          visible={showDisableDialog}
          userIds={getUsersIds(usersWithDisableStatus)}
          selectedUsers={selection}
          onClose={onSetDisabled}
          userStatus={EmployeeStatus.Disabled}
          setSelected={setSelected}
        />
      )}

      {showSendInviteDialog && (
        <SendInviteDialog
          visible={showSendInviteDialog}
          onClose={onSendInviteAgain}
          userIds={getUsersIds(usersToInvite)}
          selectedUsers={selection}
          setSelected={setSelected}
        />
      )}

      {showDeleteDialog && (
        <DeleteGroupUsersDialog
          visible={showDeleteDialog}
          onClose={onDelete}
          userIds={getUsersIds(usersToRemove)}
          selectedUsers={selection}
          filter={filter}
          setSelected={setSelected}
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
  const employeeStatus = true;
  const guestStatus = false;

  return {
    group: getSelectedGroup(state.people.groups, state.people.selectedGroup),
    selection,
    isAdmin: isAdmin(state.auth.user),
    filter: state.people.filter,
    settings: state.auth.settings,

    usersWithEmployeeType: getUserType(
      selection,
      employeeStatus,
      currentUserId
    ),
    usersWithGuestType: getUserType(selection, guestStatus, currentUserId),
    usersWithActiveStatus: getUsersStatus(
      selection,
      activeUsers,
      currentUserId
    ),
    usersWithDisableStatus: getUsersStatus(
      selection,
      disabledUsers,
      currentUserId
    ),
    usersToInvite: getInactiveUsers(selection),
    usersToSendMessage: !selection.length,
    usersToRemove: getDeleteUsers(selection)
  };
};

export default connect(mapStateToProps, {
  updateUserStatus,
  fetchPeople,
  deleteGroup,
  removeUser,
  setSelected
})(withTranslation()(withRouter(SectionHeaderContent)));
