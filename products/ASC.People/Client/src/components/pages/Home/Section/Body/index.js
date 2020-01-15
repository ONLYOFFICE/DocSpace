import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { withTranslation, Trans } from "react-i18next";
import {
  Row,
  Avatar,
  toastr,
  EmptyScreenContainer,
  Icons,
  Link,
  RowContainer,
  Text,
} from "asc-web-components";
import UserContent from "./userContent";
import {
  selectUser,
  deselectUser,
  setSelection,
  updateUserStatus,
  resetFilter,
  fetchPeople
} from "../../../../../store/people/actions";
import {
  isUserSelected,
  getUserStatus,
  getUserRole
} from "../../../../../store/people/selectors";
import { isMobileOnly } from "react-device-detect";
import isEqual from "lodash/isEqual";
import { store, api, constants } from 'asc-web-common';
import i18n from '../../i18n';
import { ChangeEmailDialog, ChangePasswordDialog, DeleteSelfProfileDialog, DeleteProfileEverDialog } from '../../../../dialogs';
const { isAdmin, isMe } = store.auth.selectors;
const { resendUserInvites } = api.people;
const { EmployeeStatus } = constants;


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
      isEmailValid: false
    };
  }

  onEmailSentClick = email => {
    window.open("mailto:" + email);
  };

  onSendMessageClick = mobilePhone => {
    window.open(`sms:${mobilePhone}`);
  };

  onEditClick = user => {
    const { history, settings } = this.props;
    history.push(`${settings.homepage}/edit/${user.userName}`);
  };

  toggleChangePasswordDialog = (email) => {
    const checkedEmail = typeof (email) === 'string' ? email : undefined;
    this.setState({
      dialogsVisible: { ...this.state.dialogsVisible, changePassword: !this.state.dialogsVisible.changePassword },
      user: { email: checkedEmail }
    });
  };

  toggleChangeEmailDialog = (user) => {
    const checkedUser = user ? user : {};
    this.setState({
      dialogsVisible: { ...this.state.dialogsVisible, changeEmail: !this.state.dialogsVisible.changeEmail },
      user: {
        email: checkedUser.email,
        id: checkedUser.id
      }
    });
  };

  onDisableClick = user => {
    const { updateUserStatus, onLoading, t } = this.props;

    onLoading(true);
    updateUserStatus(EmployeeStatus.Disabled, [user.id])
      .then(() => toastr.success(t('SuccessChangeUserStatus')))
      .catch(error => toastr.error(error))
      .finally(() => onLoading(false));
  };

  onEnableClick = user => {
    const { updateUserStatus, onLoading, t } = this.props;

    onLoading(true);
    updateUserStatus(EmployeeStatus.Active, [user.id])
      .then(() => toastr.success(t('SuccessChangeUserStatus')))
      .catch(error => toastr.error(error))
      .finally(() => onLoading(false));
  };

  onReassignDataClick = user => {
    const { history, settings } = this.props;
    history.push(`${settings.homepage}/reassign/${user.userName}`);
  };

  onDeletePersonalDataClick = user => {
    toastr.success("Context action: Delete personal data");
  };

  toggleDeleteProfileEverDialog = user => {
    const checkedUser = user ? user : {};
    this.setState({
      dialogsVisible: { ...this.state.dialogsVisible, deleteProfileEver: !this.state.dialogsVisible.deleteProfileEver },
      user: {
        id: checkedUser.id,
        displayName: checkedUser.displayName,
        userName: checkedUser.userName,
      }
    });
  };

  toggleDeleteSelfProfileDialog = email => {
    const checkedEmail = typeof (email) === 'string' ? email : undefined;
    this.setState({
      dialogsVisible: { ...this.state.dialogsVisible, deleteSelfProfile: !this.state.dialogsVisible.deleteSelfProfile },
      user: { email: checkedEmail }
    });
  };

  onInviteAgainClick = user => {
    const { onLoading } = this.props;
    onLoading(true);
    resendUserInvites([user.id])
      .then(() =>
        toastr.success(
          <Text>
            <Trans i18nKey='MessageEmailActivationInstuctionsSentOnEmail' i18n={i18n}>
                The email activation instructions have been sent to the
                <strong>{{email: user.email}}</strong> email address
            </Trans>
          </Text>
        )
      )
      .catch(error => toastr.error(error))
      .finally(() => onLoading(false));
  };
  getUserContextOptions = (user, viewer) => {
    let status = "";
    const { t } = this.props;

    const isViewerAdmin = isAdmin(viewer);
    const isSelf = isMe(user, viewer.userName);

    if (isViewerAdmin || (!isViewerAdmin && isSelf)) {
      status = getUserStatus(user);
    }

    //console.log("getUserContextOptions", user, viewer, status);

    switch (status) {
      case "normal":
      case "unknown":
        return [
          {
            key: "send-email",
            label: t("LblSendEmail"),
            onClick: this.onEmailSentClick.bind(this, user.email)
          },
          user.mobilePhone &&
          isMobileOnly && {
            key: "send-message",
            label: t("LblSendMessage"),
            onClick: this.onSendMessageClick.bind(this, user.mobilePhone)
          },
          { key: "separator", isSeparator: true },
          {
            key: "edit",
            label: t("EditButton"),
            onClick: this.onEditClick.bind(this, user)
          },
          {
            key: "change-password",
            label: t("PasswordChangeButton"),
            onClick: this.toggleChangePasswordDialog.bind(this, user.email)
          },
          {
            key: "change-email",
            label: t("EmailChangeButton"),
            onClick: this.toggleChangeEmailDialog.bind(this, user)
          },
          isSelf
            ? viewer.isOwner
              ? {}
              : {
                key: "delete-profile",
                label: t("DeleteSelfProfile"),
                onClick: this.toggleDeleteSelfProfileDialog.bind(this, user.email)
              }
            : {
              key: "disable",
              label: t("DisableUserButton"),
              onClick: this.onDisableClick.bind(this, user)
            }
        ];
      case "disabled":
        return [
          {
            key: "enable",
            label: t("EnableUserButton"),
            onClick: this.onEnableClick.bind(this, user)
          },
          {
            key: "reassign-data",
            label: t("ReassignData"),
            onClick: this.onReassignDataClick.bind(this, user)
          },
          {
            key: "delete-personal-data",
            label: t("RemoveData"),
            onClick: this.onDeletePersonalDataClick.bind(this, user)
          },
          {
            key: "delete-profile",
            label: t("DeleteSelfProfile"),
            onClick: this.toggleDeleteProfileEverDialog.bind(this, user)
          }
        ];
      case "pending":
        return [
          {
            key: "edit",
            label: t("EditButton"),
            onClick: this.onEditClick.bind(this, user)
          },
          {
            key: "invite-again",
            label: t("LblInviteAgain"),
            onClick: this.onInviteAgainClick.bind(this, user)
          },
          !isSelf &&
          (user.status === EmployeeStatus.Active
            ? {
              key: "disable",
              label: t("DisableUserButton"),
              onClick: this.onDisableClick.bind(this, user)
            }
            : {
              key: "enable",
              label: t("EnableUserButton"),
              onClick: this.onEnableClick.bind(this, user)
            }),
          isSelf && {
            key: "delete-profile",
            label: t("DeleteSelfProfile"),
            onClick: this.toggleDeleteSelfProfileDialog.bind(this, user.email)
          }
        ];
      default:
        return [];
    }
  };

  onContentRowSelect = (checked, user) => {
    console.log("ContentRow onSelect", checked, user);
    if (checked) {
      this.props.selectUser(user);
    } else {
      this.props.deselectUser(user);
    }
  };

  onResetFilter = () => {
    const { onLoading, resetFilter } = this.props;
    onLoading(true);
    resetFilter().finally(() => onLoading(false));
  };

  needForUpdate = (currentProps, nextProps) => {
    if (currentProps.checked !== nextProps.checked) {
      return true;
    }
    if (currentProps.status !== nextProps.status) {
      return true;
    }
    if (!isEqual(currentProps.data, nextProps.data)) {
      return true;
    }
    return false;
  };

  render() {
    console.log("Home SectionBodyContent render()");
    const { users, viewer, selection, history, settings, t, filter } = this.props;
    const { dialogsVisible, user } = this.state;

    return users.length > 0 ? (
      <>
        <RowContainer useReactWindow={false}>
          {users.map(user => {
            const contextOptions = this.getUserContextOptions(user, viewer);
            const contextOptionsProps = !contextOptions.length
              ? {}
              : { contextOptions };
            const checked = isUserSelected(selection, user.id);
            const checkedProps = isAdmin(viewer) ? { checked } : {};
            const element = (
              <Avatar
                size="small"
                role={getUserRole(user)}
                userName={user.displayName}
                source={user.avatar}
              />
            );

            return (
              <Row
                key={user.id}
                status={getUserStatus(user)}
                data={user}
                element={element}
                onSelect={this.onContentRowSelect}
                {...checkedProps}
                {...contextOptionsProps}
                needForUpdate={this.needForUpdate}
              >
                <UserContent
                  user={user}
                  history={history}
                  settings={settings}
                />
              </Row>
            );
          })}
        </RowContainer>

        {dialogsVisible.changeEmail &&
          <ChangeEmailDialog
            visible={dialogsVisible.changeEmail}
            onClose={this.toggleChangeEmailDialog}
            user={user}
          />
        }
        {dialogsVisible.changePassword &&
          <ChangePasswordDialog
            visible={dialogsVisible.changePassword}
            onClose={this.toggleChangePasswordDialog}
            email={user.email}
          />
        }

        {dialogsVisible.deleteSelfProfile &&
          <DeleteSelfProfileDialog
            visible={dialogsVisible.deleteSelfProfile}
            onClose={this.toggleDeleteSelfProfileDialog}
            email={user.email}
          />
        }

        {dialogsVisible.deleteProfileEver &&
          <DeleteProfileEverDialog
            visible={dialogsVisible.deleteProfileEver}
            onClose={this.toggleDeleteProfileEverDialog}
            user={user}
            filter={filter}
            settings={settings}
            history={history}
          />
        }
      </>
    ) : (
        <EmptyScreenContainer
          imageSrc="images/empty_screen_filter.png"
          imageAlt="Empty Screen Filter image"
          headerText={t("NotFoundTitle")}
          descriptionText={t("NotFoundDescription")}
          buttons={
            <>
              <Icons.CrossIcon size="small" style={{ marginRight: "4px" }} />
              <Link type="action" isHovered={true} onClick={this.onResetFilter}>
                {t("ClearButton")}
              </Link>
            </>
          }
        />
      );
  }
}

SectionBodyContent.defaultProps = {
  users: []
};

const mapStateToProps = state => {
  return {
    selection: state.people.selection,
    selected: state.people.selected,
    users: state.people.users,
    viewer: state.auth.user,
    settings: state.auth.settings,
    filter: state.people.filter
  };
};

export default connect(
  mapStateToProps,
  { selectUser, deselectUser, setSelection, updateUserStatus, resetFilter, fetchPeople }
)(withRouter(withTranslation()(SectionBodyContent)));
