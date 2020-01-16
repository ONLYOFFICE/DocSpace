import React from "react";
import { connect } from "react-redux";
import {
  Text,
  IconButton,
  ContextMenuButton,
  toastr,
  AvatarEditor,
} from "asc-web-components";
import { Headline } from 'asc-web-common';
import { withRouter } from "react-router";
import {
  getUserStatus,
  toEmployeeWrapper
} from "../../../../../store/people/selectors";
import { withTranslation } from "react-i18next";
import {
  updateUserStatus
} from "../../../../../store/people/actions";
import { fetchProfile, getUserPhoto } from "../../../../../store/profile/actions";
import styled from "styled-components";
import { store, api, constants } from "asc-web-common";
import { DeleteSelfProfileDialog, ChangePasswordDialog, ChangeEmailDialog, DeleteProfileEverDialog } from '../../../../dialogs';
const { isAdmin, isMe } = store.auth.selectors;
const {
  resendUserInvites,
  createThumbnailsAvatar,
  loadAvatar,
  deleteAvatar
} = api.people;
const { EmployeeStatus } = constants;

const StyledContainer = styled.div`
  position: relative;

  display: flex;
  align-items: center;

    .action-button {
      margin-left: 16px;

      @media (max-width: 1024px) {
        margin-left: auto;
      }
    }
    .arrow-button {
      @media (max-width: 1024px) {
        padding: 8px 0 8px 8px;
        margin-left: -8px;
      }
    }

    .header-headline {
      margin-left: 16px;
    }
`;

class SectionHeaderContent extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = this.mapPropsToState(props);
  }

  componentDidUpdate(prevProps) {
    if (this.props.profile.userName !== prevProps.profile.userName) {
      console.log(this.props.profile.userName);
      this.setState(this.mapPropsToState(this.props));
    }
  }

  mapPropsToState = props => {
    let profile = toEmployeeWrapper(props.profile);

    const newState = {
      profile: profile,
      visibleAvatarEditor: false,
      avatar: {
        tmpFile: "",
        image: null,
        defaultWidth: 0,
        defaultHeight: 0
      },
      dialogsVisible: {
        deleteSelfProfile: false,
        changePassword: false,
        changeEmail: false,
        deleteProfileEver: false
      },
    };

    return newState;
  };

  openAvatarEditor = () => {
    getUserPhoto(this.state.profile.id).then(userPhotoData => {
      if (userPhotoData.original) {
        let avatarDefaultSizes = /_(\d*)-(\d*)./g.exec(userPhotoData.original);
        if (avatarDefaultSizes !== null && avatarDefaultSizes.length > 2) {
          this.setState({
            avatar: {
              tmpFile: this.state.avatar.tmpFile,
              defaultWidth: avatarDefaultSizes[1],
              defaultHeight: avatarDefaultSizes[2],
              image: userPhotoData.original ? userPhotoData.original.indexOf('default_user_photo') !== -1 ? null : userPhotoData.original : null
            },
            visibleAvatarEditor: true
          });
        } else {
          this.setState({
            avatar: {
              tmpFile: this.state.avatar.tmpFile,
              defaultWidth: 0,
              defaultHeight: 0,
              image: null
            },
            visibleAvatarEditor: true
          });
        }
      }
    });
  };

  onLoadFileAvatar = file => {
    let data = new FormData();
    let _this = this;
    data.append("file", file);
    data.append("Autosave", false);
    loadAvatar(this.state.profile.id, data)
      .then(response => {
        var img = new Image();
        img.onload = function () {
          var stateCopy = Object.assign({}, _this.state);
          stateCopy.avatar = {
            tmpFile: response.data,
            image: response.data,
            defaultWidth: img.width,
            defaultHeight: img.height
          };
          _this.setState(stateCopy);
        };
        img.src = response.data;
      })
      .catch(error => toastr.error(error));
  };

  onSaveAvatar = (isUpdate, result) => {
    if (isUpdate) {
      createThumbnailsAvatar(this.state.profile.id, {
        x: Math.round(
          result.x * this.state.avatar.defaultWidth - result.width / 2
        ),
        y: Math.round(
          result.y * this.state.avatar.defaultHeight - result.height / 2
        ),
        width: result.width,
        height: result.height,
        tmpFile: this.state.avatar.tmpFile
      })
        .then(response => {
          let stateCopy = Object.assign({}, this.state);
          stateCopy.visibleAvatarEditor = false;
          stateCopy.avatar.tmpFile = "";
          stateCopy.profile.avatarMax =
            response.max +
            "?_=" +
            Math.floor(Math.random() * Math.floor(10000));
          toastr.success("Success");
          this.setState(stateCopy);
        })
        .catch(error => toastr.error(error))
        .then(() => this.props.fetchProfile(this.state.profile.id));
    } else {
      deleteAvatar(this.state.profile.id)
        .then(response => {
          let stateCopy = Object.assign({}, this.state);
          stateCopy.visibleAvatarEditor = false;
          stateCopy.profile.avatarMax = response.big;
          toastr.success("Success");
          this.setState(stateCopy);
        })
        .catch(error => toastr.error(error));
    }
  };

  onCloseAvatarEditor = () => {
    this.setState({
      visibleAvatarEditor: false
    });
  };

  toggleChangePasswordDialog = () => this.setState({ dialogsVisible: { ...this.state.dialogsVisible, changePassword: !this.state.dialogsVisible.changePassword } });

  toggleChangeEmailDialog = () => this.setState({ dialogsVisible: { ...this.state.dialogsVisible, changeEmail: !this.state.dialogsVisible.changeEmail } });

  onEditClick = () => {
    const { history, settings } = this.props;
    history.push(`${settings.homepage}/edit/${this.state.profile.userName}`);
  };

  onUpdateUserStatus = (status, userId) => {
    const { fetchProfile, updateUserStatus } = this.props;

    updateUserStatus(status, new Array(userId)).then(() =>
      fetchProfile(userId)
    );
  };

  onDisableClick = () =>
    this.onUpdateUserStatus(EmployeeStatus.Disabled, this.state.profile.id);

  onEnableClick = () =>
    this.onUpdateUserStatus(EmployeeStatus.Active, this.state.profile.id);

  onReassignDataClick = user => {
    const { history, settings } = this.props;
    history.push(`${settings.homepage}/reassign/${user.userName}`);
  };

  onDeletePersonalDataClick = () => {
    toastr.success("Context action: Delete personal data");
  };

  toggleDeleteProfileEverDialog = () => this.setState({ dialogsVisible: { ...this.state.dialogsVisible, deleteProfileEver: !this.state.dialogsVisible.deleteProfileEver } });


  toggleDeleteSelfProfileDialog = () => {
    this.setState(
      {
        dialogsVisible: { ...this.state.dialogsVisible, deleteSelfProfile: !this.state.dialogsVisible.deleteSelfProfile }
      });
  };

  onInviteAgainClick = () => {
    resendUserInvites(new Array(this.state.profile.id))
      .then(() =>
        toastr.success(
          <Text>
            The email activation instructions have been sent to the{" "}
            <b>{this.state.profile.email}</b> email address
          </Text>
        )
      )
      .catch(error => toastr.error(error));
  };

  getUserContextOptions = (user, viewer) => {
    let status = "";
    const { t } = this.props;

    if (isAdmin || (!isAdmin && isMe(user, viewer.userName))) {
      status = getUserStatus(user);
    }

    switch (status) {
      case "normal":
      case "unknown":
        return [
          {
            key: "edit",
            label: t("EditUserDialogTitle"),
            onClick: this.onEditClick
          },
          {
            key: "change-password",
            label: t("PasswordChangeButton"),
            onClick: this.toggleChangePasswordDialog
          },
          {
            key: "change-email",
            label: t("EmailChangeButton"),
            onClick: this.toggleChangeEmailDialog
          },
          {
            key: "edit-photo",
            label: t("EditPhoto"),
            onClick: this.openAvatarEditor
          },
          isMe(user, viewer.userName)
            ? viewer.isOwner
              ? {}
              : {
                key: "delete-profile",
                label: t("DeleteSelfProfile"),
                onClick: this.toggleDeleteSelfProfileDialog
              }
            : {
              key: "disable",
              label: t("DisableUserButton"),
              onClick: this.onDisableClick
            }
        ];
      case "disabled":
        return [
          {
            key: "enable",
            label: t("EnableUserButton"),
            onClick: this.onEnableClick
          },
          {
            key: "edit-photo",
            label: t("EditPhoto"),
            onClick: this.openAvatarEditor
          },
          {
            key: "reassign-data",
            label: t("ReassignData"),
            onClick: this.onReassignDataClick.bind(this, user)
          },
          {
            key: "delete-personal-data",
            label: t("RemoveData"),
            onClick: this.onDeletePersonalDataClick
          },
          {
            key: "delete-profile",
            label: t("DeleteSelfProfile"),
            onClick: this.toggleDeleteProfileEverDialog
          }
        ];
      case "pending":
        return [
          {
            key: "edit",
            label: t("EditButton"),
            onClick: this.onEditClick
          },
          {
            key: "invite-again",
            label: t("InviteAgainLbl"),
            onClick: this.onInviteAgainClick
          },
          {
            key: "edit-photo",
            label: t("EditPhoto"),
            onClick: this.openAvatarEditor
          },
          !isMe(user, viewer.userName) &&
          (user.status === EmployeeStatus.Active
            ? {
              key: "disable",
              label: t("DisableUserButton"),
              onClick: this.onDisableClick
            }
            : {
              key: "enable",
              label: t("EnableUserButton"),
              onClick: this.onEnableClick
            }),
          isMe(user, viewer.userName) && {
            key: "delete-profile",
            label: t("DeleteSelfProfile"),
            onClick: this.toggleDeleteSelfProfileDialog
          }
        ];
      default:
        return [];
    }
  };

  onClickBack = () => {
    const { history, settings } = this.props;

    history.push(settings.homepage);
  };

  render() {
    const { profile, isAdmin, viewer, t, filter, settings, history } = this.props;
    const { avatar, visibleAvatarEditor, dialogsVisible } = this.state;
    const contextOptions = () => this.getUserContextOptions(profile, viewer);

    return (
      <StyledContainer>
        <IconButton
          iconName="ArrowPathIcon"
          color="#A3A9AE"
          size="16"
          hoverColor="#657077"
          isFill={true}
          onClick={this.onClickBack}
          className="arrow-button"
        />
        <Headline className='header-headline' type='content' truncate={true}>
          {profile.displayName}
          {profile.isLDAP && ` (${t("LDAPLbl")})`}
        </Headline>
        {((isAdmin && !profile.isOwner) || isMe(viewer, profile.userName)) && (
          <ContextMenuButton
            className="action-button"
            directionX="right"
            title={t("Actions")}
            iconName="VerticalDotsIcon"
            size={16}
            color="#A3A9AE"
            getData={contextOptions}
            isDisabled={false}
          />
        )}

        <AvatarEditor
          image={avatar.image}
          visible={visibleAvatarEditor}
          onClose={this.onCloseAvatarEditor}
          onSave={this.onSaveAvatar}
          onLoadFile={this.onLoadFileAvatar}
          headerLabel={t("editAvatar")}
          chooseFileLabel={t("chooseFileLabel")}
          chooseMobileFileLabel={t("chooseMobileFileLabel")}
          unknownTypeError={t("unknownTypeError")}
          maxSizeFileError={t("maxSizeFileError")}
          unknownError={t("unknownError")}
        />

        {dialogsVisible.deleteSelfProfile &&
          <DeleteSelfProfileDialog
            visible={dialogsVisible.deleteSelfProfile}
            onClose={this.toggleDeleteSelfProfileDialog}
            email={this.state.profile.email}
          />
        }

        {dialogsVisible.changePassword &&
          <ChangePasswordDialog
            visible={dialogsVisible.changePassword}
            onClose={this.toggleChangePasswordDialog}
            email={this.state.profile.email}
          />
        }

        {dialogsVisible.changeEmail &&
          <ChangeEmailDialog
            visible={dialogsVisible.changeEmail}
            onClose={this.toggleChangeEmailDialog}
            user={this.state.profile}
          />
        }

        {dialogsVisible.deleteProfileEver &&
          <DeleteProfileEverDialog
            visible={dialogsVisible.deleteProfileEver}
            onClose={this.toggleDeleteProfileEverDialog}
            user={this.state.profile}
            filter={filter}
            settings={settings}
            history={history}
          />
        }
      </StyledContainer>
    );
  }
}

const mapStateToProps = state => {
  return {
    settings: state.auth.settings,
    profile: state.profile.targetUser,
    viewer: state.auth.user,
    isAdmin: isAdmin(state.auth.user),
    filter: state.people.filter
  };
};

export default connect(
  mapStateToProps,
  { updateUserStatus, fetchProfile }
)(withRouter(withTranslation()(SectionHeaderContent)));
