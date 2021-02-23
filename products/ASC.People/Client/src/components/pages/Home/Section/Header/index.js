import React, { useCallback, useState, useMemo } from "react";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
import {
  GroupButtonsMenu,
  DropDownItem,
  ContextMenuButton,
  utils,
} from "asc-web-components";
import { Headline, toastr, Loaders, constants } from "asc-web-common";
import { withTranslation } from "react-i18next";
import {
  InviteDialog,
  DeleteUsersDialog,
  SendInviteDialog,
  ChangeUserStatusDialog,
  ChangeUserTypeDialog,
} from "../../../../dialogs";
import { isMobile } from "react-device-detect";
import { inject, observer } from "mobx-react";

const { tablet, desktop } = utils.device;
const { Consumer } = utils.context;

const { EmployeeType, EmployeeStatus } = constants;

const StyledContainer = styled.div`
  .group-button-menu-container {
    margin: 0 -16px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
    padding-bottom: 56px;
    ${isMobile &&
    css`
      position: sticky;
    `}
    ${(props) =>
      !props.isTabletView
        ? props.width &&
          isMobile &&
          css`
            width: ${props.width + 40 + "px"};
          `
        : props.width &&
          isMobile &&
          css`
            width: ${props.width + 24 + "px"};
          `}

    @media ${tablet} {
      padding-bottom: 0;
      ${!isMobile &&
      css`
        height: 56px;
      `}
      & > div:first-child {
        ${(props) =>
          !isMobile &&
          props.width &&
          css`
            width: ${props.width + 16 + "px"};
          `}

        position: absolute;
        top: 56px;
        z-index: 180;
      }
    }

    @media ${desktop} {
      margin: 0 -24px;
    }
  }

  .header-container {
    position: relative;
    ${(props) =>
      props.isLoaded &&
      css`
        display: grid;
        grid-template-columns: auto auto 1fr;

        @media ${tablet} {
          grid-template-columns: 1fr auto;
        }
      `}

    align-items: center;
    max-width: calc(100vw - 32px);

    .action-button {
      margin-left: 16px;

      @media ${tablet} {
        margin-left: auto;

        & > div:first-child {
          padding: 8px 16px 8px 16px;
          margin-right: -16px;
        }
      }
    }
  }
`;

const SectionHeaderContent = (props) => {
  const [invitationDialogVisible, setInvitationDialogVisible] = useState(false);
  const [deleteDialogVisible, setDeleteDialogVisible] = useState(false);
  const [sendInviteDialogVisible, setSendInviteDialogVisible] = useState(false);
  const [disableDialogVisible, setDisableDialogVisible] = useState(false);
  const [activeDialogVisible, setActiveDialogVisible] = useState(false);
  const [guestDialogVisible, setGuestDialogVisible] = useState(false);
  const [employeeDialogVisible, setEmployeeDialogVisible] = useState(false);

  const {
    isHeaderVisible,
    isHeaderIndeterminate,
    isHeaderChecked,
    //onCheck,
    //onSelect,
    //clearSelection,
    group,
    isAdmin,
    t,
    history,
    customNames,
    homepage,
    deleteGroup,
    selection,
    hasAnybodySelected,
    hasUsersToMakeEmployees,
    hasUsersToMakeGuests,
    hasUsersToActivate,
    hasUsersToDisable,
    hasUsersToInvite,
    hasUsersToRemove,
    isLoaded,
    isTabletView,
    //selectAll,
    setSelected,
    //selectByStatus,
  } = props;

  const {
    userCaption,
    guestCaption,
    groupCaption,
    groupsCaption,
  } = customNames;

  //console.log("SectionHeaderContent render", props.isTabletView);

  const onCheck = (checked) => {
    setSelected(checked ? "all" : "close");
  };
  const onSelect = useCallback((selected) => setSelected(selected), [
    setSelected,
  ]);

  const onClose = () => {
    setSelected("none");
  };

  const toggleEmployeeDialog = useCallback(
    () => setEmployeeDialogVisible(!employeeDialogVisible),
    [employeeDialogVisible]
  );

  const toggleGuestDialog = useCallback(
    () => setGuestDialogVisible(!guestDialogVisible),
    [guestDialogVisible]
  );

  const toggleActiveDialog = useCallback(
    () => setActiveDialogVisible(!activeDialogVisible),
    [activeDialogVisible]
  );

  const toggleDisableDialog = useCallback(
    () => setDisableDialogVisible(!disableDialogVisible),
    [disableDialogVisible]
  );

  const toggleSendInviteDialog = useCallback(
    () => setSendInviteDialogVisible(!sendInviteDialogVisible),
    [sendInviteDialogVisible]
  );

  const toggleDeleteDialog = useCallback(
    () => setDeleteDialogVisible(!deleteDialogVisible),
    [deleteDialogVisible]
  );

  const onSendEmail = useCallback(() => {
    let str = "";
    for (let item of selection) {
      str += `${item.email},`;
    }
    window.open(`mailto: ${str}`, "_self");
  }, [selection]);

  const onSelectorSelect = useCallback(
    (item) => {
      onSelect && onSelect(item.key);
    },
    [onSelect]
  );

  const menuItems = useMemo(
    () => [
      {
        label: t("LblSelect"),
        isDropdown: true,
        isSeparator: true,
        isSelect: true,
        fontWeight: "bold",
        children: [
          <DropDownItem key="active" label={t("LblActive")} data-index={0} />,
          <DropDownItem
            key="disabled"
            label={t("LblTerminated")}
            data-index={1}
          />,
          <DropDownItem key="invited" label={t("LblInvited")} data-index={2} />,
        ],
        onSelect: onSelectorSelect,
      },
      {
        label: t("ChangeToUser", {
          userCaption,
        }),
        disabled: !hasUsersToMakeEmployees,
        onClick: toggleEmployeeDialog,
      },
      {
        label: t("ChangeToGuest", {
          guestCaption,
        }),
        disabled: !hasUsersToMakeGuests,
        onClick: toggleGuestDialog,
      },
      {
        label: t("LblSetActive"),
        disabled: !hasUsersToActivate,
        onClick: toggleActiveDialog,
      },
      {
        label: t("LblSetDisabled"),
        disabled: !hasUsersToDisable,
        onClick: toggleDisableDialog,
      },
      {
        label: t("LblInviteAgain"),
        disabled: !hasUsersToInvite,
        onClick: toggleSendInviteDialog,
      },
      {
        label: t("LblSendEmail"),
        disabled: !hasAnybodySelected,
        onClick: onSendEmail,
      },
      {
        label: t("DeleteButton"),
        disabled: !hasUsersToRemove,
        onClick: toggleDeleteDialog,
      },
    ],
    [
      t,
      userCaption,
      guestCaption,
      onSelectorSelect,
      toggleEmployeeDialog,
      toggleGuestDialog,
      toggleActiveDialog,
      toggleDisableDialog,
      toggleSendInviteDialog,
      onSendEmail,
      toggleDeleteDialog,
      hasAnybodySelected,
      hasUsersToMakeEmployees,
      hasUsersToMakeGuests,
      hasUsersToActivate,
      hasUsersToDisable,
      hasUsersToInvite,
      hasUsersToRemove,
    ]
  );

  const onEditGroup = useCallback(
    () => history.push(`${homepage}/group/edit/${group.id}`),
    [history, homepage, group]
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
        onClick: onEditGroup,
      },
      {
        key: "delete-group",
        label: t("DeleteButton"),
        onClick: onDeleteGroup,
      },
    ];
  }, [t, onEditGroup, onDeleteGroup]);

  const goToEmployeeCreate = useCallback(() => {
    history.push(`${homepage}/create/user`);
  }, [history, homepage]);

  const goToGuestCreate = useCallback(() => {
    history.push(`${homepage}/create/guest`);
  }, [history, homepage]);

  const goToGroupCreate = useCallback(() => {
    history.push(`${homepage}/group/create`);
  }, [history, homepage]);

  const onInvitationDialogClick = useCallback(
    () => setInvitationDialogVisible(!invitationDialogVisible),
    [invitationDialogVisible]
  );

  const getContextOptionsPlus = useCallback(() => {
    return [
      {
        key: "new-employee",
        label: userCaption,
        onClick: goToEmployeeCreate,
      },
      {
        key: "new-guest",
        label: guestCaption,
        onClick: goToGuestCreate,
      },
      {
        key: "new-group",
        label: groupCaption,
        onClick: goToGroupCreate,
      },
      { key: "separator", isSeparator: true },
      {
        key: "make-invitation-link",
        label: t("MakeInvitationLink"),
        onClick: onInvitationDialogClick,
      } /* ,
      {
        key: "send-invitation",
        label: t("SendInviteAgain"),
        onClick: onSentInviteAgain
      } */,
    ];
  }, [
    userCaption,
    guestCaption,
    groupCaption,
    t,
    goToEmployeeCreate,
    goToGuestCreate,
    goToGroupCreate,
    onInvitationDialogClick /* , onSentInviteAgain */,
  ]);

  return (
    <Consumer>
      {(context) => (
        <StyledContainer
          isHeaderVisible={isHeaderVisible}
          isLoaded={isLoaded}
          width={context.sectionWidth}
          isTabletView={isTabletView}
        >
          {employeeDialogVisible && (
            <ChangeUserTypeDialog
              visible={employeeDialogVisible}
              onClose={toggleEmployeeDialog}
              userType={EmployeeType.User}
            />
          )}

          {guestDialogVisible && (
            <ChangeUserTypeDialog
              visible={guestDialogVisible}
              onClose={toggleGuestDialog}
              userType={EmployeeType.Guest}
            />
          )}
          {activeDialogVisible && (
            <ChangeUserStatusDialog
              visible={activeDialogVisible}
              onClose={toggleActiveDialog}
              userStatus={EmployeeStatus.Active}
            />
          )}
          {disableDialogVisible && (
            <ChangeUserStatusDialog
              visible={disableDialogVisible}
              onClose={toggleDisableDialog}
              userStatus={EmployeeStatus.Disabled}
            />
          )}

          {sendInviteDialogVisible && (
            <SendInviteDialog
              visible={sendInviteDialogVisible}
              onClose={toggleSendInviteDialog}
            />
          )}

          {deleteDialogVisible && (
            <DeleteUsersDialog
              visible={deleteDialogVisible}
              onClose={toggleDeleteDialog}
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
                sectionWidth={context.sectionWidth}
              />
            </div>
          ) : (
            <div className="header-container">
              {!isLoaded ? (
                <Loaders.SectionHeader />
              ) : group ? (
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
                      size={17}
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
                    {groupsCaption}
                  </Headline>
                  {isAdmin && (
                    <>
                      <ContextMenuButton
                        className="action-button"
                        directionX="right"
                        title={t("Actions")}
                        iconName="PlusIcon"
                        size={17}
                        color="#657077"
                        getData={getContextOptionsPlus}
                        isDisabled={false}
                      />
                      {invitationDialogVisible && (
                        <InviteDialog
                          visible={invitationDialogVisible}
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
      )}
    </Consumer>
  );
};

export default inject(({ auth, peopleStore }) => ({
  customNames: auth.settingsStore.customNames,
  homepage: auth.settingsStore.homepage,
  isLoaded: auth.isLoaded,
  isAdmin: auth.isAdmin,
  fetchPeople: peopleStore.usersStore.getUsersList,
  selection: peopleStore.selectionStore.selection,
  setSelected: peopleStore.selectionStore.setSelected,
  selectByStatus: peopleStore.selectionStore.selectByStatus,
  isHeaderVisible: peopleStore.headerMenuStore.isHeaderVisible,
  isHeaderIndeterminate: peopleStore.headerMenuStore.isHeaderIndeterminate,
  isHeaderChecked: peopleStore.headerMenuStore.isHeaderChecked,
  clearSelection: peopleStore.selectionStore.clearSelection,
  selectAll: peopleStore.selectionStore.selectAll,
  hasAnybodySelected: peopleStore.selectionStore.hasAnybodySelected,
  hasUsersToMakeEmployees: peopleStore.selectionStore.hasUsersToMakeEmployees,
  hasUsersToMakeGuests: peopleStore.selectionStore.hasUsersToMakeGuests,
  hasUsersToActivate: peopleStore.selectionStore.hasUsersToActivate,
  hasUsersToDisable: peopleStore.selectionStore.hasUsersToDisable,
  hasUsersToInvite: peopleStore.selectionStore.hasUsersToInvite,
  hasUsersToRemove: peopleStore.selectionStore.hasUsersToRemove,
  deleteGroup: peopleStore.groupsStore.deleteGroup,
  removeUser: peopleStore.usersStore.removeUser,
  updateUserStatus: peopleStore.usersStore.updateUserStatus,
  group: peopleStore.selectedGroupStore.group,
  isTabletView: auth.settingsStore.isTabletView, 
}))(observer(withTranslation("Home")(withRouter(SectionHeaderContent))));
