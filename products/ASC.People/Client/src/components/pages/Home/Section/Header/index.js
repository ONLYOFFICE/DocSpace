import React, { useCallback, useState, useMemo } from "react";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
import {
  GroupButtonsMenu,
  DropDownItem,
  ContextMenuButton,
  utils,
} from "asc-web-components";
import { Headline, toastr, Loaders } from "asc-web-common";
import { connect } from "react-redux";
import {
  getSelectedGroup,
  isEmployeesSelected,
  isGuestsSelected,
  isActiveSelected,
  isDisableSelected,
  isInactiveSelected,
  isDeleteSelected,
  isAnythingSelected,
} from "../../../../../store/people/selectors";
import { withTranslation } from "react-i18next";
import {
  updateUserStatus,
  fetchPeople,
  removeUser,
  setSelected,
} from "../../../../../store/people/actions";
import { deleteGroup } from "../../../../../store/group/actions";
import { store, constants } from "asc-web-common";
import {
  InviteDialog,
  DeleteUsersDialog,
  SendInviteDialog,
  ChangeUserStatusDialog,
  ChangeUserTypeDialog,
} from "../../../../dialogs";
const { tablet, desktop } = utils.device;

const { isAdmin } = store.auth.selectors;
const { EmployeeType, EmployeeStatus } = constants;

const StyledContainer = styled.div`
  @media ${desktop} {
    ${(props) =>
      props.isHeaderVisible &&
      css`
        width: calc(100% + 76px);
      `}
  }

  .group-button-menu-container {
    margin: 0 -16px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
    padding-bottom: 56px;

    @media ${tablet} {
      & > div:first-child {
        ${(props) =>
          props.isArticlePinned &&
          css`
            width: calc(100% - 240px);
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

    display: flex;
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
    onCheck,
    onSelect,
    onClose,
    group,
    isAdmin,
    t,
    history,

    customNames,
    homepage,

    deleteGroup,

    selection,

    isAnythingSelected,
    isEmployeesSelected,
    isGuestsSelected,
    isActiveSelected,
    isDisableSelected,
    isInactiveSelected,
    isDeleteSelected,

    isLoaded,
  } = props;

  const {
    userCaption,
    guestCaption,
    groupCaption,
    groupsCaption,
  } = customNames;

  //console.log("SectionHeaderContent render");

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
        disabled: !isEmployeesSelected,
        onClick: toggleEmployeeDialog,
      },
      {
        label: t("ChangeToGuest", {
          guestCaption,
        }),
        disabled: !isGuestsSelected,
        onClick: toggleGuestDialog,
      },
      {
        label: t("LblSetActive"),
        disabled: !isActiveSelected,
        onClick: toggleActiveDialog,
      },
      {
        label: t("LblSetDisabled"),
        disabled: !isDisableSelected,
        onClick: toggleDisableDialog,
      },
      {
        label: t("LblInviteAgain"),
        disabled: !isInactiveSelected,
        onClick: toggleSendInviteDialog,
      },
      {
        label: t("LblSendEmail"),
        disabled: !isAnythingSelected,
        onClick: onSendEmail,
      },
      {
        label: t("DeleteButton"),
        disabled: !isDeleteSelected,
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
      isAnythingSelected,
      isEmployeesSelected,
      isGuestsSelected,
      isActiveSelected,
      isDisableSelected,
      isInactiveSelected,
      isDeleteSelected,
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

  const isArticlePinned = window.localStorage.getItem("asc_article_pinned_key");

  return (
    <StyledContainer
      isHeaderVisible={isHeaderVisible}
      isArticlePinned={isArticlePinned}
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
          />
        </div>
      ) : (
        <div className="header-container">
          {!isLoaded ? (
            <Loaders.Headline />
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
  );
};

const mapStateToProps = (state) => {
  const { isLoaded, settings } = state.auth;
  const { groups, selection, selectedGroup } = state.people;
  const { homepage, customNames } = settings;

  return {
    group: getSelectedGroup(groups, selectedGroup),
    isAdmin: isAdmin(state),
    homepage,
    customNames,
    selection,

    isAnythingSelected: isAnythingSelected(state),
    isEmployeesSelected: isEmployeesSelected(state),
    isGuestsSelected: isGuestsSelected(state),
    isActiveSelected: isActiveSelected(state),
    isDisableSelected: isDisableSelected(state),
    isInactiveSelected: isInactiveSelected(state),
    isDeleteSelected: isDeleteSelected(state),

    isLoaded,
  };
};

export default connect(mapStateToProps, {
  updateUserStatus,
  fetchPeople,
  deleteGroup,
  removeUser,
  setSelected,
})(withTranslation()(withRouter(SectionHeaderContent)));
