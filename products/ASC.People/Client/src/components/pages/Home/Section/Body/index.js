import React from "react";
import { withRouter } from "react-router";
import { withTranslation, Trans } from "react-i18next";
import { Row, Avatar, RowContainer, utils } from "asc-web-components";
import UserContent from "./userContent";
import equal from "fast-deep-equal/react";
import { api, constants, toastr, Loaders } from "asc-web-common";
import {
  ChangeEmailDialog,
  ChangePasswordDialog,
  DeleteSelfProfileDialog,
  DeleteProfileEverDialog,
} from "../../../../dialogs";

import EmptyScreen from "./sub-components/EmptyScreen";
import { inject, observer } from "mobx-react";

const { Consumer } = utils.context;
const { isArrayEqual } = utils.array;
const { resendUserInvites } = api.people;
const { EmployeeStatus } = constants;

const isRefetchPeople = true;

class SectionBodyContent extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      dialogsVisible: {
        changeEmail: false,
        changePassword: false,
        deleteSelfProfile: false,
        deleteProfileEver: false,
      },
      isEmailValid: false,
      isLoadedSection: true,
    };
  }

  componentDidMount() {
    const { isLoaded, fetchPeople, filter, peopleList } = this.props;
    if (!isLoaded || peopleList.length > 0) return;

    this.setState({ isLoadedSection: false });

    fetchPeople(filter)
      .catch((error) => {
        toastr.error(error);
      })
      .finally(() => this.setState({ isLoadedSection: isLoaded }));
  }

  findUserById = (id) => this.props.peopleList.find((man) => man.id === id);

  onEmailSentClick = (e) => {
    const user = this.findUserById(e.currentTarget.dataset.id);
    window.open("mailto:" + user.email);
  };

  onSendMessageClick = (e) => {
    const user = this.findUserById(e.currentTarget.dataset.id);
    window.open(`sms:${user.mobilePhone}`);
  };

  onEditClick = (e) => {
    const { history, settings } = this.props;
    const user = this.findUserById(e.currentTarget.dataset.id);
    history.push(`${settings.homepage}/edit/${user.userName}`);
  };

  onDisableClick = (e) => {
    const user = this.findUserById(e.currentTarget.dataset.id);
    const { updateUserStatus, onLoading, t } = this.props;

    onLoading(true);
    updateUserStatus(EmployeeStatus.Disabled, [user.id], isRefetchPeople)
      .then(() => toastr.success(t("SuccessChangeUserStatus")))
      .catch((error) => toastr.error(error))
      .finally(() => onLoading(false));
  };

  onEnableClick = (e) => {
    const user = this.findUserById(e.currentTarget.dataset.id);
    const { updateUserStatus, onLoading, t } = this.props;

    onLoading(true);
    updateUserStatus(EmployeeStatus.Active, [user.id], isRefetchPeople)
      .then(() => toastr.success(t("SuccessChangeUserStatus")))
      .catch((error) => toastr.error(error))
      .finally(() => onLoading(false));
  };

  onReassignDataClick = (e) => {
    const user = this.findUserById(e.currentTarget.dataset.id);
    const { history, settings } = this.props;
    history.push(`${settings.homepage}/reassign/${user.userName}`);
  };

  onDeletePersonalDataClick = (e) => {
    //const user = this.findUserById(e.currentTarget.dataset.id);
    toastr.success("Context action: Delete personal data");
  };

  onCloseDialog = () => {
    this.setState({
      dialogsVisible: {
        changeEmail: false,
        changePassword: false,
        deleteSelfProfile: false,
        deleteProfileEver: false,
      },
    });
  };

  toggleChangeEmailDialog = (e) => {
    const user = this.findUserById(e.currentTarget.dataset.id);

    if (!user) return;

    const { id, email } = user;

    this.setState({
      dialogsVisible: {
        changeEmail: true,
      },
      user: {
        email,
        id,
      },
    });
  };

  toggleChangePasswordDialog = (e) => {
    const user = this.findUserById(e.currentTarget.dataset.id);

    if (!user) return;

    const { email } = user;

    this.setState({
      dialogsVisible: {
        changePassword: true,
      },
      user: { email },
    });
  };

  toggleDeleteSelfProfileDialog = (e) => {
    this.onCloseDialog();

    const user = this.findUserById(e.currentTarget.dataset.id);

    if (!user) return;

    const { email } = user;
    this.setState({
      dialogsVisible: {
        deleteSelfProfile: true,
      },
      user: { email },
    });
  };

  toggleDeleteProfileEverDialog = (e) => {
    this.onCloseDialog();
    const user = this.findUserById(e.currentTarget.dataset.id);

    if (!user) return;

    const { id, displayName, userName } = user;
    this.setState({
      dialogsVisible: {
        deleteProfileEver: true,
      },
      user: {
        id,
        displayName,
        userName,
      },
    });
  };

  onInviteAgainClick = (e) => {
    const user = this.findUserById(e.currentTarget.dataset.id);
    const { onLoading } = this.props;

    onLoading(true);
    resendUserInvites([user.id])
      .then(() =>
        toastr.success(
          <Trans
            i18nKey="MessageEmailActivationInstuctionsSentOnEmail"
            ns="Home"
          >
            The email activation instructions have been sent to the
            <strong>{{ email: user.email }}</strong> email address
          </Trans>
        )
      )
      .catch((error) => toastr.error(error))
      .finally(() => onLoading(false));
  };

  getUserContextOptions = (options, id) => {
    const { t } = this.props;

    return options.map((option) => {
      switch (option) {
        case "send-email":
          return {
            key: option,
            label: t("LblSendEmail"),
            "data-id": id,
            onClick: this.onEmailSentClick,
          };
        case "send-message":
          return {
            key: option,
            label: t("LblSendMessage"),
            "data-id": id,
            onClick: this.onSendMessageClick,
          };
        case "separator":
          return { key: option, isSeparator: true };
        case "edit":
          return {
            key: option,
            label: t("EditButton"),
            "data-id": id,
            onClick: this.onEditClick,
          };
        case "change-password":
          return {
            key: option,
            label: t("PasswordChangeButton"),
            "data-id": id,
            onClick: this.toggleChangePasswordDialog,
          };
        case "change-email":
          return {
            key: option,
            label: t("EmailChangeButton"),
            "data-id": id,
            onClick: this.toggleChangeEmailDialog,
          };
        case "delete-self-profile":
          return {
            key: option,
            label: t("DeleteSelfProfile"),
            "data-id": id,
            onClick: this.toggleDeleteSelfProfileDialog,
          };
        case "disable":
          return {
            key: option,
            label: t("DisableUserButton"),
            "data-id": id,
            onClick: this.onDisableClick,
          };
        case "enable":
          return {
            key: option,
            label: t("EnableUserButton"),
            "data-id": id,
            onClick: this.onEnableClick,
          };
        case "reassign-data":
          return {
            key: option,
            label: t("ReassignData"),
            "data-id": id,
            onClick: this.onReassignDataClick,
          };
        case "delete-personal-data":
          return {
            key: option,
            label: t("RemoveData"),
            "data-id": id,
            onClick: this.onDeletePersonalDataClick,
          };
        case "delete-profile":
          return {
            key: option,
            label: t("DeleteSelfProfile"),
            "data-id": id,
            onClick: this.toggleDeleteProfileEverDialog,
          };
        case "invite-again":
          return {
            key: option,
            label: t("LblInviteAgain"),
            "data-id": id,
            onClick: this.onInviteAgainClick,
          };
        default:
          break;
      }

      return undefined;
    });
  };

  onContentRowSelect = (checked, user) => {
    if (checked) {
      this.props.selectUser(user);
    } else {
      this.props.deselectUser(user);
    }
  };

  onResetFilter = () => {
    const { onLoading, resetFilter } = this.props;
    onLoading(true);
    resetFilter(true).finally(() => onLoading(false));
  };

  needForUpdate = (currentProps, nextProps) => {
    if (currentProps.checked !== nextProps.checked) {
      return true;
    }
    if (currentProps.status !== nextProps.status) {
      return true;
    }
    if (currentProps.sectionWidth !== nextProps.sectionWidth) {
      return true;
    }
    if (!equal(currentProps.data, nextProps.data)) {
      return true;
    }
    if (!isArrayEqual(currentProps.contextOptions, nextProps.contextOptions)) {
      return true;
    }
    return false;
  };

  render() {
    //console.log("Home SectionBodyContent render()");
    const {
      isLoaded,
      peopleList,
      history,
      settings,
      t,
      filter,
      widthProp,
      isMobile,
      selectGroup,
      isLoading,
      isAdmin,
      currentUserId,
      isEmptyGroup,
    } = this.props;

    const { dialogsVisible, user, isLoadedSection } = this.state;

    return !isLoaded || (isMobile && isLoading) || !isLoadedSection ? (
      <Loaders.Rows isRectangle={false} />
    ) : peopleList.length > 0 ? (
      <>
        <Consumer>
          {(context) => (
            <RowContainer
              className="people-row-container"
              useReactWindow={false}
            >
              {peopleList.map((man) => {
                const {
                  checked,
                  role,
                  displayName,
                  avatar,
                  id,
                  status,
                  options,
                } = man;
                const sectionWidth = context.sectionWidth;
                const showContextMenu = options && options.length > 0;
                const contextOptionsProps =
                  (isAdmin && showContextMenu) ||
                  (showContextMenu && id === currentUserId)
                    ? {
                        contextOptions: this.getUserContextOptions(options, id),
                      }
                    : {};

                const checkedProps =
                  checked !== null && isAdmin ? { checked } : {};

                const element = (
                  <Avatar
                    size="min"
                    role={role}
                    userName={displayName}
                    source={avatar}
                  />
                );

                return (
                  <Row
                    key={id}
                    status={status}
                    data={man}
                    element={element}
                    onSelect={this.onContentRowSelect}
                    {...checkedProps}
                    {...contextOptionsProps}
                    needForUpdate={this.needForUpdate}
                    sectionWidth={sectionWidth}
                  >
                    <UserContent
                      isMobile={isMobile}
                      widthProp={widthProp}
                      user={man}
                      history={history}
                      settings={settings}
                      selectGroup={selectGroup}
                      sectionWidth={sectionWidth}
                    />
                  </Row>
                );
              })}
            </RowContainer>
          )}
        </Consumer>

        {dialogsVisible.changeEmail && (
          <ChangeEmailDialog
            visible={dialogsVisible.changeEmail}
            onClose={this.onCloseDialog}
            user={user}
          />
        )}
        {dialogsVisible.changePassword && (
          <ChangePasswordDialog
            visible={dialogsVisible.changePassword}
            onClose={this.onCloseDialog}
            email={user.email}
          />
        )}

        {dialogsVisible.deleteSelfProfile && (
          <DeleteSelfProfileDialog
            visible={dialogsVisible.deleteSelfProfile}
            onClose={this.onCloseDialog}
            email={user.email}
          />
        )}

        {dialogsVisible.deleteProfileEver && (
          <DeleteProfileEverDialog
            visible={dialogsVisible.deleteProfileEver}
            onClose={this.onCloseDialog}
            user={user}
            filter={filter}
            settings={settings}
            history={history}
          />
        )}
      </>
    ) : (
      <EmptyScreen
        t={t}
        onResetFilter={this.onResetFilter}
        isEmptyGroup={isEmptyGroup}
      />
    );
  }
}

export default inject(({ auth, peopleStore }) => ({
  settings: auth.settingsStore,
  isLoaded: auth.isLoaded,
  isAdmin: auth.isAdmin,
  currentUserId: auth.userStore.user.id,
  fetchPeople: peopleStore.usersStore.getUsersList,
  peopleList: peopleStore.usersStore.peopleList,
  filter: peopleStore.filterStore.filter,
  resetFilter: peopleStore.resetFilter,
  selectUser: peopleStore.selectionStore.selectUser,
  deselectUser: peopleStore.selectionStore.deselectUser,
  selectGroup: peopleStore.selectedGroupStore.selectGroup,
  updateUserStatus: peopleStore.usersStore.updateUserStatus,
  isLoading: peopleStore.isLoading,
  isEmptyGroup: peopleStore.selectedGroupStore.isEmptyGroup,
}))(observer(withRouter(withTranslation("Home")(SectionBodyContent))));
