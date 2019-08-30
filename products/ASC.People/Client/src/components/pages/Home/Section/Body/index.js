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
  Text
} from "asc-web-components";
import UserContent from "./userContent";
import {
  selectUser,
  deselectUser,
  setSelection,
  updateUserStatus,
  resetFilter
} from "../../../../../store/people/actions";
import {
  isUserSelected,
  getUserStatus,
  getUserRole
} from "../../../../../store/people/selectors";
import { isAdmin, isMe } from "../../../../../store/auth/selectors";
import { EmployeeStatus } from "../../../../../helpers/constants";

class SectionBodyContent extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      dialog: {
        visible: false,
        header: "",
        body: "",
        successFunc: null
      }
    };
  }

  onEmailSentClick = email => {
    window.open("mailto:" + email);
  };

  onSendMessageClick = () => {
    toastr.success("Context action: Send message");
  };

  onEditClick = user => {
    const { history, settings } = this.props;
    history.push(`${settings.homepage}/edit/${user.userName}`);
  };

  onChangePasswordClick = email => {
    this.setState({dialog: {
        visible: true,
        header: "Password change",
        body: <Text.Body>
          Send the password change instructions to the <Link type="page" href={`mailto:${email}`} isHovered title={email}>{email}</Link> email address
        </Text.Body>,
        successFunc: () => {
          toastr.success("Context action: Change password");
          this.onDialogClose();
        }
    }});
  };

  onChangeEmailClick = () => {
    toastr.success("Context action: Change e-mail");
  };

  onDisableClick = user => {
    const { updateUserStatus, onLoading } = this.props;

    onLoading(true);
    updateUserStatus(EmployeeStatus.Disabled, [user.id])
      .then(() => toastr.success("SUCCESS Context action: Disable"))
      .catch((e) => toastr.error("FAILED Context action: Disable", e))
      .finally(() => onLoading(false));
  };

  onEnableClick = user => {
    const { updateUserStatus, onLoading } = this.props;

    onLoading(true);
    updateUserStatus(EmployeeStatus.Active, [user.id])
      .then(() => toastr.success("SUCCESS Context action: Enable"))
      .catch((e) => toastr.error("FAILED Context action: Enable", e))
      .finally(() => onLoading(false));
  };

  onReassignDataClick = () => {
    toastr.success("Context action: Reassign data");
  };

  onDeletePersonalDataClick = user => {
    toastr.success("Context action: Delete personal data");
  };

  onDeleteProfileEverClick = user => {
    toastr.success("Context action: Delete profile data");
  };

  onDeleteProfileClick = email => {
    this.setState({dialog: {
      visible: true,
      header: "Delete profile dialog",
      body: <Text.Body>
        Send the profile deletion instructions to the email address <Link type="page" href={`mailto:${email}`} isHovered title={email}>{email}</Link>
      </Text.Body>,
      successFunc: () => {
        toastr.success("Context action: Delete profile");
        this.onDialogClose();
      }
  }});
  };

  onInviteAgainClick = () => {
    toastr.success("Context action: Invite again");
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
          {
            key: "send-message",
            label: t("LblSendMessage"),
            onClick: this.onSendMessageClick
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
            onClick: this.onChangeEmailClick
          },
          isSelf
          ? {
            key: "delete-profile",
            label: t("PeopleResource:LblDeleteProfile"),
            onClick: this.onDeleteProfileClick.bind(this, user.email)
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
            onClick: this.onInviteAgainClick
          },
          !isSelf && (user.status === EmployeeStatus.Active 
          ? {
            key: "disable",
            label: t("DisableUserButton"),
            onClick: this.onDisableClick.bind(this, user)
          } : {
            key: "enable",
            label: t("EnableUserButton"),
            onClick: this.onEnableClick.bind(this, user)
          }),
          isSelf &&
          {
            key: "delete-profile",
            label: t("DeleteSelfProfile"),
            onClick: this.onDeleteProfileClick.bind(this, user.email)
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
    const { onLoading } = this.props;
    onLoading(true);
    resetFilter().finally(() => onLoading(false));
  };

  onDialogClose = () => {
    this.setState({dialog: { visible: false }});
  }

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
          const element = <Avatar size='small' role={getUserRole(user)} userName={user.displayName} source={user.avatar} />;
      
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
              <UserContent user={user} history={history} settings={settings} />
            </Row>
          );
        })}
      </RowContainer>
      <ModalDialog
          visible={dialog.visible}
          headerContent={dialog.header}
          bodyContent={dialog.body}
          footerContent={[
              <Button key="OkBtn" label="Send" primary={true} onClick={dialog.successFunc} />,
              <Button key="CancelBtn" label="Cancel" primary={false} onClick={this.onDialogClose} style={{ marginLeft: '8px' }} />
          ]}
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
    settings: state.auth.settings
  };
};

export default connect(
  mapStateToProps,
  { selectUser, deselectUser, setSelection, updateUserStatus, resetFilter }
)(withRouter(withTranslation()(SectionBodyContent)));
