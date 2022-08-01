import React from "react";
import { withRouter } from "react-router";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import AvatarEditor from "@docspace/components/avatar-editor";
import Loader from "@docspace/components/loader";
import { isTablet, isMobile } from "@docspace/components/utils/device";
import {
  createThumbnailsAvatar,
  loadAvatar,
  deleteAvatar,
} from "@docspace/common/api/people";
import toastr from "client/toastr";
import { inject, observer } from "mobx-react";
import { toEmployeeWrapper } from "../../../../helpers/people-helpers";

const dialogsDataset = {
  changeEmail: "changeEmail",
  changePassword: "changePassword",
  changePhone: "changePhone",
};

const AvatarEditorBody = styled.div`
  ${isMobile() && "margin-left: -8px"}
  margin-bottom: 24px;
`;

class AvatarEditorPage extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = this.mapPropsToState(props);
  }

  componentDidMount() {
    const { match, fetchProfile, t, profile, setDocumentTitle } = this.props;
    const { avatar } = this.state;
    const { userId } = match.params;

    setDocumentTitle(t("ProfileAction"));
    if (userId && !profile) {
      fetchProfile(userId);
    }

    if (avatar.image === undefined) {
      this.setUserPhotoToState();
    }
  }

  componentDidUpdate(prevProps) {
    const { match, fetchProfile /*, profile*/ } = this.props;
    const { userId } = match.params;
    const { avatar } = this.state;
    const prevUserId = prevProps.match.params.userId;

    if (userId !== undefined && userId !== prevUserId) {
      fetchProfile(userId);
    }

    if (avatar.image === undefined) {
      this.setUserPhotoToState();
    }
  }

  onCancel = () => {
    const { toggleAvatarEditor } = this.props;
    toggleAvatarEditor(false);
  };

  updateUserPhotoInState = () => {
    var profile = toEmployeeWrapper(this.props.profile);
    this.props.getUserPhoto(profile.id).then((userPhotoData) => {
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
          });
        }
      }
    });
  };

  mapPropsToState = (props) => {
    var profile = toEmployeeWrapper(props.profile);

    this.props.getUserPhoto(profile.id).then((userPhotoData) => {
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
                  ? undefined
                  : userPhotoData.original
                : undefined,
            },
          });
        }
      }
    });

    const newState = {
      isLoading: false,
      pageIsLoaded: true,
      errors: {
        firstName: false,
        lastName: false,
      },
      profile: profile,
      visibleAvatarEditor: false,
      avatar: {
        tmpFile: "",
        image: null,
        defaultWidth: 0,
        defaultHeight: 0,
      },
      dialogsVisible: {
        [dialogsDataset.changePassword]: false,
        [dialogsDataset.changePhone]: false,
        [dialogsDataset.changeEmail]: false,
        currentDialog: "",
      },
      isMobile: isMobile || isTablet,
    };

    //Set unique contacts id
    const now = new Date().getTime();

    newState.profile.contacts.forEach((contact, index) => {
      contact.id = (now + index).toString();
    });

    return newState;
  };

  onSaveAvatar = (isUpdate, result, avatar) => {
    this.setState({ isLoading: true });
    const { profile, toggleAvatarEditor, setAvatarMax, personal } = this.props;

    if (isUpdate) {
      createThumbnailsAvatar(profile.id, {
        x: Math.round(result.x * avatar.defaultWidth - result.width / 2),
        y: Math.round(result.y * avatar.defaultHeight - result.height / 2),
        width: result.width,
        height: result.height,
        tmpFile: avatar.tmpFile,
      })
        .then((response) => {
          const avatarMax =
            response.max +
            "?_=" +
            Math.floor(Math.random() * Math.floor(10000));

          let stateCopy = Object.assign({}, this.state);
          setAvatarMax(avatarMax);
          toastr.success(this.props.t("ChangesSavedSuccessfully"));
          this.setState(stateCopy);
        })
        .catch((error) => {
          toastr.error(error);
          this.setState({ isLoading: false });
        })
        .then(() => {
          this.props.updateProfile(this.props.profile);
        })
        .then(() => {
          this.setState(this.mapPropsToState(this.props));
        })
        .then(() => !personal && this.props.fetchProfile(profile.id))
        .then((res) => {
          //this.props.isMe && this.props.setUser(res);
          return toggleAvatarEditor(false);
        });
    } else {
      deleteAvatar(profile.id)
        .then((response) => {
          let stateCopy = Object.assign({}, this.state);
          setAvatarMax(response.big);
          toastr.success(this.props.t("ChangesSavedSuccessfully"));
          this.setState(stateCopy);
        })
        .catch((error) => toastr.error(error))
        .then(() => this.props.updateProfile(this.props.profile))
        .then(() => {
          this.setState(this.mapPropsToState(this.props));
        })
        .then(() => !personal && this.props.fetchProfile(profile.id))
        .then((res) => {
          //this.props.isMe && this.props.setUser(res);
          return toggleAvatarEditor(false);
        });
    }
  };

  onSaveClick = () => this.setState({ isLoading: true });

  onLoadFileAvatar = (file, fileData) => {
    const { profile } = this.props;

    let data = new FormData();
    let _this = this;

    if (!file) {
      _this.onSaveAvatar(false);
      return;
    }

    data.append("file", file);
    data.append("Autosave", false);
    loadAvatar(profile.id, data)
      .then((response) => {
        if (!response.success && response.message) {
          throw response.message;
        }
        var img = new Image();
        img.onload = () => {
          if (fileData) {
            fileData.avatar = {
              tmpFile: response.data,
              image: response.data,
              defaultWidth: img.width,
              defaultHeight: img.height,
            };
            if (!fileData.existImage) {
              _this.onSaveAvatar(fileData.existImage); // saving empty avatar
            } else {
              _this.onSaveAvatar(
                fileData.existImage,
                fileData.position,
                fileData.avatar
              );
            }
          }
        };
        img.src = response.data;
      })
      .catch((error) => {
        toastr.error(error);
        this.setState({ isLoading: false });
      });
  };

  setUserPhotoToState = () => {
    const { profile } = this.props;

    if (!profile) {
      this.setState({ pageIsLoaded: true });
      return;
    }

    this.props.getUserPhoto(profile.id).then((userPhotoData) => {
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
          });
        }
      }

      this.setState({ pageIsLoaded: true });
    });
  };

  render() {
    const { t } = this.props;
    const { pageIsLoaded } = this.state;

    return pageIsLoaded ? (
      <AvatarEditorBody>
        <AvatarEditor
          useModalDialog={false}
          image={this.state.avatar.image}
          visible={true}
          onSave={this.onSaveClick}
          onCancel={this.onCancel}
          onLoadFile={this.onLoadFileAvatar}
          headerLabel={t("EditPhoto")}
          selectNewPhotoLabel={t("PeopleTranslations:selectNewPhotoLabel")}
          orDropFileHereLabel={t("PeopleTranslations:orDropFileHereLabel")}
          unknownTypeError={t("PeopleTranslations:ErrorUnknownFileImageType")}
          maxSizeFileError={t("PeopleTranslations:maxSizeFileError")}
          unknownError={t("Common:Error")}
          saveButtonLabel={
            this.state.isLoading ? t("UpdatingProcess") : t("Common:SaveButton")
          }
          cancelButtonLabel={t("Common:CancelButton")}
          saveButtonLoading={this.state.isLoading}
          maxSizeLabel={t("PeopleTranslations:MaxSizeLabel")}
        />
      </AvatarEditorBody>
    ) : (
      <Loader className="pageLoader" type="rombs" size="40px" />
    );
  }
}

export default withRouter(
  inject(({ auth, peopleStore }) => ({
    setDocumentTitle: auth.setDocumentTitle,
    //setUser: auth.userStore.setUser,
    toggleAvatarEditor: peopleStore.avatarEditorStore.toggleAvatarEditor,
    fetchProfile: peopleStore.targetUserStore.getTargetUser,
    profile: peopleStore.targetUserStore.targetUser,
    //isMe: peopleStore.targetUserStore.isMe,
    avatarMax: peopleStore.avatarEditorStore.avatarMax,
    setAvatarMax: peopleStore.avatarEditorStore.setAvatarMax,
    updateProfile: peopleStore.targetUserStore.updateProfile,
    getUserPhoto: peopleStore.targetUserStore.getUserPhoto,
    personal: auth.settingsStore.personal,
  }))(
    observer(
      withTranslation(["ProfileAction", "Common", "PeopleTranslations"])(
        AvatarEditorPage
      )
    )
  )
);
