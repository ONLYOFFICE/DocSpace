import React from "react";
import IconButton from "@docspace/components/icon-button";
import ContextMenuButton from "@docspace/components/context-menu-button";
import AvatarEditor from "@docspace/components/avatar-editor";
import Headline from "@docspace/common/components/Headline";
import toastr from "client/toastr";
import { withRouter } from "react-router";
import { withTranslation, Trans } from "react-i18next";
import styled from "styled-components";
import {
  resendUserInvites,
  createThumbnailsAvatar,
  loadAvatar,
  deleteAvatar,
} from "@docspace/common/api/people";
import { AppServerConfig, EmployeeStatus } from "@docspace/common/constants";
import withCultureNames from "@docspace/common/hoc/withCultureNames";
import {
  DeleteSelfProfileDialog,
  ChangePasswordDialog,
  ChangeEmailDialog,
  DeleteProfileEverDialog,
} from "../../../../components/dialogs";
import { inject, observer } from "mobx-react";
import { toEmployeeWrapper } from "../../../../helpers/people-helpers";
import config from "PACKAGE_FILE";
import { combineUrl } from "@docspace/common/utils";

import Loaders from "@docspace/common/components/Loaders";
import withPeopleLoader from "../../../../HOCs/withPeopleLoader";

const StyledContainer = styled.div`
  position: relative;

  display: grid;
  grid-template-columns: ${(props) =>
    props.showContextButton ? "auto auto auto 1fr" : "auto 1fr"};
  align-items: center;

  @media (max-width: 1024px) {
    grid-template-columns: ${(props) =>
      props.showContextButton ? "auto 1fr auto" : "auto 1fr"};
  }

  .action-button {
    margin-left: 16px;

    @media (max-width: 1024px) {
      margin-left: auto;

      & > div:first-child {
        padding: 8px 16px 8px 0px;
        margin-right: -16px;
      }
    }
  }
  .arrow-button {
    @media (max-width: 1024px) {
      padding: 8px 16px 8px 16px;
      margin-left: -16px;
      margin-right: -16px;
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
    if (!this.props.profile && !prevProps.profile) return;
    if (
      !prevProps.profile ||
      this.props.profile.userName !== prevProps.profile.userName
    ) {
      this.setState(this.mapPropsToState(this.props));
    }
  }

  mapPropsToState = (props) => {
    let profile = toEmployeeWrapper(props.profile);

    const newState = {
      profile: profile,
      visibleAvatarEditor: false,
      avatar: {
        tmpFile: "",
        image: null,
        defaultWidth: 0,
        defaultHeight: 0,
      },
      dialogsVisible: {
        deleteSelfProfile: false,
        changePassword: false,
        changeEmail: false,
        deleteProfileEver: false,
      },
    };

    return newState;
  };

  openAvatarEditor = () => {
    this.props.getUserPhoto(this.state.profile.id).then((userPhotoData) => {
      if (userPhotoData.original) {
        let avatarDefaultSizes = /_(\d*)-(\d*)./g.exec(userPhotoData.original);
        if (avatarDefaultSizes !== null && avatarDefaultSizes.length > 2) {
          this.setState({
            avatar: {
              tmpFile: this.state.avatar.tmpFile,
              defaultWidth: avatarDefaultSizes[1],
              defaultHeight: avatarDefaultSizes[2],
              image: userPhotoData.original
                ? userPhotoData.original.indexOf("default_user_photo") !== -1
                  ? null
                  : userPhotoData.original
                : null,
            },
            visibleAvatarEditor: true,
          });
        } else {
          this.setState({
            avatar: {
              tmpFile: this.state.avatar.tmpFile,
              defaultWidth: 0,
              defaultHeight: 0,
              image: null,
            },
            visibleAvatarEditor: true,
          });
        }
      }
    });
  };

  onLoadFileAvatar = (file, callback) => {
    let data = new FormData();
    let _this = this;
    data.append("file", file);
    data.append("Autosave", false);
    loadAvatar(this.state.profile.id, data)
      .then((response) => {
        if (!response.success && response.message) {
          throw response.message;
        }
        var img = new Image();
        img.onload = function () {
          var stateCopy = Object.assign({}, _this.state);
          stateCopy.avatar = {
            tmpFile: response.data,
            image: response.data,
            defaultWidth: img.width,
            defaultHeight: img.height,
          };
          _this.setState(stateCopy);
          if (typeof callback === "function") callback();
        };
        img.src = response.data;
      })
      .catch((error) => toastr.error(error));
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
        tmpFile: this.state.avatar.tmpFile,
      })
        .then((response) => {
          let stateCopy = Object.assign({}, this.state);
          stateCopy.visibleAvatarEditor = false;
          stateCopy.avatar.tmpFile = "";
          stateCopy.profile.avatarMax =
            response.max +
            "?_=" +
            Math.floor(Math.random() * Math.floor(10000));
          toastr.success(this.props.t("ChangesApplied"));
          this.setState(stateCopy);
        })
        .catch((error) => toastr.error(error))
        .then(() => this.props.updateProfile(this.props.profile))
        .then(() => this.props.fetchProfile(this.state.profile.id));
    } else {
      deleteAvatar(this.state.profile.id)
        .then((response) => {
          let stateCopy = Object.assign({}, this.state);
          stateCopy.visibleAvatarEditor = false;
          stateCopy.profile.avatarMax = response.big;
          toastr.success(this.props.t("ChangesApplied"));
          this.setState(stateCopy);
        })
        .catch((error) => toastr.error(error));
    }
  };

  onCloseAvatarEditor = () => {
    this.setState({
      visibleAvatarEditor: false,
    });
  };

  toggleChangePasswordDialog = () =>
    this.setState({
      dialogsVisible: {
        ...this.state.dialogsVisible,
        changePassword: !this.state.dialogsVisible.changePassword,
      },
    });

  toggleChangeEmailDialog = () =>
    this.setState({
      dialogsVisible: {
        ...this.state.dialogsVisible,
        changeEmail: !this.state.dialogsVisible.changeEmail,
      },
    });

  onEditClick = () => {
    const { history, isMy, setIsEditTargetUser } = this.props;

    setIsEditTargetUser(true);

    const editUrl = isMy
      ? combineUrl(AppServerConfig.proxyURL, `/my?action=edit`)
      : combineUrl(
          AppServerConfig.proxyURL,
          config.homepage,
          `/accounts/edit/${this.state.profile.userName}`
        );

    history.push(editUrl);
  };

  onUpdateUserStatus = (status, userId) => {
    const { fetchProfile, updateUserStatus, t } = this.props;

    updateUserStatus(status, new Array(userId))
      .then(() => this.props.updateProfile(this.props.profile))
      .then(() => fetchProfile(userId))
      .then(() =>
        toastr.success(t("PeopleTranslations:SuccessChangeUserStatus"))
      )
      .catch((error) => toastr.error(error));
  };

  onDisableClick = () =>
    this.onUpdateUserStatus(EmployeeStatus.Disabled, this.state.profile.id);

  onEnableClick = () =>
    this.onUpdateUserStatus(EmployeeStatus.Active, this.state.profile.id);

  onReassignDataClick = (user) => {
    const { history } = this.props;
    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/reassign/${user.userName}`
      )
    );
  };

  onDeletePersonalDataClick = () => {
    console.log("Context action: Delete personal data");
  };

  toggleDeleteProfileEverDialog = () =>
    this.setState({
      dialogsVisible: {
        ...this.state.dialogsVisible,
        deleteProfileEver: !this.state.dialogsVisible.deleteProfileEver,
      },
    });

  toggleDeleteSelfProfileDialog = () => {
    this.setState({
      dialogsVisible: {
        ...this.state.dialogsVisible,
        deleteSelfProfile: !this.state.dialogsVisible.deleteSelfProfile,
      },
    });
  };

  onInviteAgainClick = () => {
    resendUserInvites(new Array(this.state.profile.id))
      .then(() =>
        toastr.success(
          <Trans
            i18nKey="MessageEmailActivationInstuctionsSentOnEmail"
            ns="Profile"
            t={this.props.t}
          >
            The email activation instructions have been sent to the
            <strong>{{ email: this.state.profile.email }}</strong> email address
          </Trans>
        )
      )
      .catch((error) => toastr.error(error));
  };

  getUserContextOptions = () => {
    const {
      setChangeEmailVisible,
      setChangePasswordVisible,
      setChangeAvatarVisible,
    } = this.props;

    return [
      {
        key: "change-email",
        label: "Change email",
        onClick: () => setChangeEmailVisible(true),
        disabled: false,
        icon: "/static/images/email.react.svg",
      },
      {
        key: "change-password",
        label: "Change password",
        onClick: () => setChangePasswordVisible(true),
        disabled: false,
        icon: "/static/images/security.react.svg",
      },
      {
        key: "edit-photo",
        label: "Edit photo",
        onClick: () => setChangeAvatarVisible(true),
        disabled: false,
        icon: "/static/images/image.react.svg",
      },
      { key: "separator", isSeparator: true },
      {
        key: "delete-profile",
        label: "Delete profile",
        onClick: () => console.log("Delete profile"),
        disabled: false,
        icon: "/static/images/catalog.trash.react.svg",
      },
    ];
  };

  onClickBack = () => {
    const { filter, setFilter, history, resetProfile, isMy } = this.props;

    if (isMy) {
      return history.goBack();
    }

    resetProfile();

    const url = filter.toUrlParams();
    const backUrl = combineUrl(
      AppServerConfig.proxyURL,
      config.homepage,
      `/accounts/filter?/${url}`
    );

    history.push(backUrl, url);
    setFilter(filter);
  };

  render() {
    const { profile, isAdmin, viewer, t, filter, history, isMe } = this.props;
    const { avatar, visibleAvatarEditor, dialogsVisible } = this.state;

    const contextOptions = () => this.getUserContextOptions();

    return (
      <StyledContainer
        showContextButton={(isAdmin && !profile?.isOwner) || isMe}
      >
        <IconButton
          iconName="/static/images/arrow.path.react.svg"
          size="17"
          isFill={true}
          onClick={this.onClickBack}
          className="arrow-button"
        />

        <Headline className="header-headline" type="content" truncate={true}>
          {t("Profile:MyProfile")}
          {profile.isLDAP && ` (${t("PeopleTranslations:LDAPLbl")})`}
        </Headline>
        {((isAdmin && !profile.isOwner) || isMe) && (
          <ContextMenuButton
            className="action-button"
            directionX="right"
            title={t("Common:Actions")}
            iconName="/static/images/vertical-dots.react.svg"
            size={17}
            getData={contextOptions}
            isDisabled={false}
            usePortal={false}
          />
        )}
        {visibleAvatarEditor && (
          <AvatarEditor
            image={avatar.image}
            visible={visibleAvatarEditor}
            onClose={this.onCloseAvatarEditor}
            onSave={this.onSaveAvatar}
            onLoadFile={this.onLoadFileAvatar}
            headerLabel={t("Common:EditAvatar")}
            selectNewPhotoLabel={t("PeopleTranslations:selectNewPhotoLabel")}
            orDropFileHereLabel={t("PeopleTranslations:orDropFileHereLabel")}
            unknownTypeError={t("PeopleTranslations:ErrorUnknownFileImageType")}
            maxSizeFileError={t("PeopleTranslations:maxSizeFileError")}
            unknownError={t("Common:Error")}
            saveButtonLabel={t("Common:SaveButton")}
            maxSizeLabel={t("PeopleTranslations:MaxSizeLabel")}
          />
        )}

        {dialogsVisible.deleteSelfProfile && (
          <DeleteSelfProfileDialog
            visible={dialogsVisible.deleteSelfProfile}
            onClose={this.toggleDeleteSelfProfileDialog}
            email={this.state.profile.email}
          />
        )}

        {dialogsVisible.changePassword && (
          <ChangePasswordDialog
            visible={dialogsVisible.changePassword}
            onClose={this.toggleChangePasswordDialog}
            email={this.state.profile.email}
          />
        )}

        {dialogsVisible.changeEmail && (
          <ChangeEmailDialog
            visible={dialogsVisible.changeEmail}
            onClose={this.toggleChangeEmailDialog}
            user={this.state.profile}
          />
        )}

        {dialogsVisible.deleteProfileEver && (
          <DeleteProfileEverDialog
            visible={dialogsVisible.deleteProfileEver}
            onClose={this.toggleDeleteProfileEverDialog}
            user={this.state.profile}
            filter={filter}
            history={history}
          />
        )}
      </StyledContainer>
    );
  }
}

export default withRouter(
  inject(({ auth, peopleStore }) => {
    const { targetUserStore } = peopleStore;

    const {
      setChangeEmailVisible,
      setChangePasswordVisible,
      setChangeAvatarVisible,
    } = targetUserStore;

    return {
      isAdmin: auth.isAdmin,
      isLoaded: auth.isLoaded,
      viewer: auth.userStore.user,
      filter: peopleStore.filterStore.filter,
      setFilter: peopleStore.filterStore.setFilterParams,
      setFilterUrl: peopleStore.filterStore.setFilterUrl,
      updateUserStatus: peopleStore.usersStore.updateUserStatus,
      resetProfile: peopleStore.targetUserStore.resetTargetUser,
      fetchProfile: peopleStore.targetUserStore.getTargetUser,
      setIsEditTargetUser: peopleStore.targetUserStore.setIsEditTargetUser,
      profile: peopleStore.targetUserStore.targetUser,
      isMe: peopleStore.targetUserStore.isMe,
      updateProfile: peopleStore.targetUserStore.updateProfile,
      getUserPhoto: peopleStore.targetUserStore.getUserPhoto,
      setChangeEmailVisible,
      setChangePasswordVisible,
      setChangeAvatarVisible,
    };
  })(
    observer(
      withTranslation(["Profile", "Common", "PeopleTranslations"])(
        withCultureNames(
          withPeopleLoader(SectionHeaderContent)(<Loaders.SectionHeader />)
        )
      )
    )
  )
);
