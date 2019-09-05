import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { withTranslation } from "react-i18next";
import {
  Row,
  Avatar,
  toastr,
  EmptyScreenContainer,
  Icons,
  Link,
  RowContainer,
  ModalDialog,
  Button,
  Text,
  Label,
  TextInput
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
import { isAdmin, isMe } from "../../../../../store/auth/selectors";
import { EmployeeStatus } from "../../../../../helpers/constants";
import {
  resendUserInvites,
  sendInstructionsToDelete,
  sendInstructionsToChangePassword,
  deleteUser
} from "../../../../../store/services/api";
import { isMobileOnly } from "react-device-detect";

class SectionBodyContent extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      newEmail: null,
      dialog: {
        visible: false,
        header: "",
        body: "",
        buttons: []
      }
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

  onChangePasswordClick = email => {
    this.setState({
      dialog: {
        visible: true,
        header: "Password change",
        body: (
          <Text.Body>
            Send the password change instructions to the{" "}
            <Link type="page" href={`mailto:${email}`} isHovered title={email}>
              {email}
            </Link>{" "}
            email address
          </Text.Body>
        ),
        buttons: [
          <Button
            key="OkBtn"
            label="Send"
            primary={true}
            onClick={() => {
              const { onLoading } = this.props;
              onLoading(true);
              sendInstructionsToChangePassword(email)
              .then(() =>
                  toastr.success(
                    <Text.Body>
                      The password change instructions have been sent to the{" "}
                      <b>{email}</b> email address
                    </Text.Body>
                  )
                )
                .catch(e => toastr.error("ERROR"))
                .finally(() => onLoading(false));
              this.onDialogClose();
            }}
          />,
          <Button
            key="CancelBtn"
            label="Cancel"
            primary={false}
            onClick={this.onDialogClose}
            style={{ marginLeft: "8px" }}
          />
        ]
      }
    });
  };

  onChangeEmailClick = email => {
    this.setState({
      dialog: {
        visible: true,
        header: "Email change",
        body: (
          <>
            <Label htmlFor="new-email" text="Enter a new email address" />
            <TextInput
              id="new-email"
              scale={true}
              isAutoFocussed={true}
              value={this.state.newEmail}
              onChange={e => {
                this.setState({ newEmail: e.target.value });
              }}
            />
            <Text.Body style={{ marginTop: "16px" }}>
              The activation instructions will be sent to the entered email
            </Text.Body>
          </>
        ),
        buttons: [
          <Button
            key="OkBtn"
            label="Send"
            primary={true}
            onClick={() => {
              toastr.success(
                `Context action: Change e-mail from ${email} to ${this.state.newEmail}`
              );
              this.onDialogClose();
            }}
          />,
          <Button
            key="CancelBtn"
            label="Cancel"
            primary={false}
            onClick={this.onDialogClose}
            style={{ marginLeft: "8px" }}
          />
        ]
      }
    });
  };

  onDisableClick = user => {
    const { updateUserStatus, onLoading } = this.props;

    onLoading(true);
    updateUserStatus(EmployeeStatus.Disabled, [user.id])
      .then(() => toastr.success("SUCCESS Context action: Disable"))
      .catch(e => toastr.error("FAILED Context action: Disable", e))
      .finally(() => onLoading(false));
  };

  onEnableClick = user => {
    const { updateUserStatus, onLoading } = this.props;

    onLoading(true);
    updateUserStatus(EmployeeStatus.Active, [user.id])
      .then(() => toastr.success("SUCCESS Context action: Enable"))
      .catch(e => toastr.error("FAILED Context action: Enable", e))
      .finally(() => onLoading(false));
  };

  onReassignDataClick = () => {
    toastr.success("Context action: Reassign data");
  };

  onDeletePersonalDataClick = user => {
    toastr.success("Context action: Delete personal data");
  };

  onDeleteProfileEverClick = user => {
    this.setState({
      dialog: {
        visible: true,
        header: "Confirmation",
        body: (
          <>
            <Text.Body>
              User <b>{user.displayName}</b> will be deleted.
            </Text.Body>
            <Text.Body>Note: this action cannot be undone.</Text.Body>
            <Text.Body color="#c30" fontSize="18" style={{ margin: "20px 0" }}>
              Warning!
            </Text.Body>
            <Text.Body>
              User personal documents which are available to others will be
              deleted. To avoid this, you must start the data reassign process
              before deleting.
            </Text.Body>
          </>
        ),
        buttons: [
          <Button
            key="OkBtn"
            label="OK"
            primary={true}
            onClick={() => {
              const { onLoading, filter, fetchPeople } = this.props;
              onLoading(true);
              deleteUser(user.id)
                .then(() => {
                  toastr.success("User has been removed successfully");
                  return fetchPeople(filter);
                })
                .catch(e => toastr.error("ERROR"))
                .finally(() => onLoading(false));
              this.onDialogClose();
            }}
          />,
          <Button
            key="ReassignBtn"
            label="Reassign data"
            primary={true}
            onClick={() => {
              toastr.success("Context action: Reassign profile");
              this.onDialogClose();
            }}
            style={{ marginLeft: "8px" }}
          />,
          <Button
            key="CancelBtn"
            label="Cancel"
            primary={false}
            onClick={this.onDialogClose}
            style={{ marginLeft: "8px" }}
          />
        ]
      }
    });
  };

  onDeleteSelfProfileClick = email => {
    this.setState({
      dialog: {
        visible: true,
        header: "Delete profile dialog",
        body: (
          <Text.Body>
            Send the profile deletion instructions to the email address{" "}
            <Link type="page" href={`mailto:${email}`} isHovered title={email}>
              {email}
            </Link>
          </Text.Body>
        ),
        buttons: [
          <Button
            key="OkBtn"
            label="Send"
            primary={true}
            onClick={() => {
              const { onLoading } = this.props;
              onLoading(true);
              sendInstructionsToDelete()
                .then(() =>
                  toastr.success(
                    <Text.Body>
                      Instructions to delete your profile has been sent to{" "}
                      <b>{email}</b> email address
                    </Text.Body>
                  )
                )
                .catch(e => toastr.error("ERROR"))
                .finally(() => onLoading(false));
              this.onDialogClose();
            }}
          />,
          <Button
            key="CancelBtn"
            label="Cancel"
            primary={false}
            onClick={this.onDialogClose}
            style={{ marginLeft: "8px" }}
          />
        ]
      }
    });
  };

  onInviteAgainClick = user => {
    const { onLoading } = this.props;
    onLoading(true);
    resendUserInvites([user.id])
      .then(() =>
        toastr.success(
          <Text.Body>
            The email activation instructions have been sent to the{" "}
            <b>{user.email}</b> email address
          </Text.Body>
        )
      )
      .catch(e => toastr.error("ERROR"))
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
            onClick: this.onChangePasswordClick.bind(this, user.email)
          },
          {
            key: "change-email",
            label: t("EmailChangeButton"),
            onClick: this.onChangeEmailClick.bind(this, user.email)
          },
          isSelf
            ? viewer.isOwner 
              ? {} 
              : {
                key: "delete-profile",
                label: t("DeleteSelfProfile"),
                onClick: this.onDeleteSelfProfileClick.bind(this, user.email)
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
            onClick: this.onReassignDataClick
          },
          {
            key: "delete-personal-data",
            label: t("RemoveData"),
            onClick: this.onDeletePersonalDataClick.bind(this, user)
          },
          {
            key: "delete-profile",
            label: t("DeleteSelfProfile"),
            onClick: this.onDeleteProfileEverClick.bind(this, user)
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
            onClick: this.onDeleteSelfProfileClick.bind(this, user.email)
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

  onDialogClose = () => {
    this.setState({ 
      newEmail: null,
      dialog: { visible: false } 
    });
  };

  render() {
    console.log("Home SectionBodyContent render()");
    const { users, viewer, selection, history, settings, t } = this.props;
    const { dialog } = this.state;

    return users.length > 0 ? (
      <>
        <RowContainer>
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
        <ModalDialog
          visible={dialog.visible}
          headerContent={dialog.header}
          bodyContent={dialog.body}
          footerContent={dialog.buttons}
          onClose={this.onDialogClose}
        />
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
