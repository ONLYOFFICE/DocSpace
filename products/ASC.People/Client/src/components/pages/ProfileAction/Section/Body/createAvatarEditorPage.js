import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import { AvatarEditor, utils, Loader } from "asc-web-components";
import { api, toastr } from "asc-web-common";
import {
  fetchProfile,
  updateProfile,
  getUserPhoto,
  setAvatarMax,
  updateCreatedAvatar,
  setCreatedAvatar,
  setCroppedAvatar,
  resetProfile,
} from "../../../../../store/profile/actions";
import { toEmployeeWrapper } from "../../../../../store/people/selectors";
import {
  toggleAvatarEditor,
  updateProfileInUsers,
  setIsEditingForm,
} from "../../../../../store/people/actions";
import { setDocumentTitle } from "../../../../../helpers/utils";
import { isMobile } from "react-device-detect";

const { createThumbnailsAvatar, loadAvatar } = api.people;
const { isTablet } = utils.device;

const dialogsDataset = {
  changeEmail: "changeEmail",
  changePassword: "changePassword",
  changePhone: "changePhone",
};

const AvatarEditorBody = styled.div`
  margin-bottom: 24px;
`;

class CreateAvatarEditorPage extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = this.mapPropsToState(props);

    this.onLoadFileAvatar = this.onLoadFileAvatar.bind(this);
    this.onSaveAvatar = this.onSaveAvatar.bind(this);
  }

  componentDidMount() {
    const { match, fetchProfile, t, profile } = this.props;
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
    const { match, fetchProfile, profile } = this.props;
    const { userId } = match.params;
    const { avatar } = this.state;
    const prevUserId = prevProps.match.params.userId;

    if (this.props.match.params.type !== prevProps.match.params.type) {
      this.setState(this.mapPropsToState(this.props));
    }

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
    getUserPhoto(profile.id).then((userPhotoData) => {
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
    var profile = toEmployeeWrapper({
      isVisitor: props.match.params.type === "guest",
      passwordType: "link",
    });
    return {
      pageIsLoaded: true,
      visibleAvatarEditor: false,
      croppedAvatarImage: "",
      isLoading: false,
      errors: {
        firstName: false,
        lastName: false,
        email: false,
        password: false,
      },
      profile: profile,
      avatar: {
        tmpFile: "",
        image: null,
        defaultWidth: 0,
        defaultHeight: 0,
        x: 0,
        y: 0,
        width: 0,
        height: 0,
      },
      isMobile: isMobile || isTablet,
    };
  };

  setIsEdit(hasChanges) {
    const { setIsEditingForm } = this.props;
    setIsEditingForm(hasChanges);
  }

  onLoadFileAvatar(file, fileData) {
    let data = new FormData();
    let _this = this;
    data.append("file", file);
    data.append("Autosave", false);

    if (!file) {
      _this.onSaveAvatar(false);
      return;
    }

    loadAvatar(0, data)
      .then((response) => {
        var img = new Image();
        img.onload = function () {
          if (fileData) {
            fileData.avatar = {
              tmpFile: response.data,
              image: response.data,
              defaultWidth: img.width,
              defaultHeight: img.height,
            };

            var stateCopy = Object.assign({}, _this.state);

            stateCopy.avatar = {
              tmpFile: response.data,
              image: response.data,
              defaultWidth: img.width,
              defaultHeight: img.height,
            };

            _this.setState(stateCopy);

            if (fileData.existImage) {
              _this.onSaveAvatar(
                fileData.existImage,
                fileData.position,
                fileData.avatar,
                fileData.croppedImage
              );
            }
          }
        };
        img.src = response.data;
      })
      .catch((error) => toastr.error(error));
  }

  onSaveAvatar(isUpdate, result, avatar, croppedImage) {
    var stateCopy = Object.assign({}, this.state);
    const {
      setCreatedAvatar,
      toggleAvatarEditor,
      setCroppedAvatar,
      resetProfile,
    } = this.props;

    stateCopy.visibleAvatarEditor = false;
    stateCopy.croppedAvatarImage = croppedImage;
    if (isUpdate) {
      stateCopy.avatar.x = Math.round(
        result.x * avatar.defaultWidth - result.width / 2
      );
      stateCopy.avatar.y = Math.round(
        result.y * avatar.defaultHeight - result.height / 2
      );
      stateCopy.avatar.width = result.width;
      stateCopy.avatar.height = result.height;

      setCreatedAvatar(stateCopy.avatar);
      setCroppedAvatar(croppedImage);
      this.setIsEdit(true);
    } else {
      resetProfile();
      this.setIsEdit(false);
    }

    toggleAvatarEditor(false);
  }

  setUserPhotoToState = () => {
    const { profile } = this.props;

    if (!profile) {
      this.setState({ pageIsLoaded: true });
      return;
    }

    getUserPhoto(profile.id).then((userPhotoData) => {
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
    const { t, createdAvatar } = this.props;
    const { pageIsLoaded } = this.state;

    return pageIsLoaded ? (
      <AvatarEditorBody>
        <AvatarEditor
          useModalDialog={false}
          image={createdAvatar.image}
          visible={true}
          onClose={this.onCloseAvatarEditor}
          onSave={this.onSaveAvatar}
          onCancel={this.onCancel}
          onLoadFile={this.onLoadFileAvatar}
          headerLabel={t("EditPhoto")}
          selectNewPhotoLabel={t("selectNewPhotoLabel")}
          orDropFileHereLabel={t("orDropFileHereLabel")}
          unknownTypeError={t("ErrorUnknownFileImageType")}
          maxSizeFileError={t("maxSizeFileError")}
          unknownError={t("Error")}
          saveButtonLabel={
            this.state.isLoading ? t("UpdatingProcess") : t("SaveButton")
          }
          cancelButtonLabel={t("CancelButton")}
          saveButtonLoading={this.state.isLoading}
        />
      </AvatarEditorBody>
    ) : (
      <Loader className="pageLoader" type="rombs" size="40px" />
    );
  }
}

function mapStateToProps(state) {
  return {
    profile: state.profile.targetUser,
    avatarMax: state.profile.avatarMax,
    createdAvatar: state.profile.createdAvatar,
    croppedAvatar: state.profile.croppedAvatar,
    settings: state.auth.settings,
    editingForm: state.people.editingForm,
  };
}

export default connect(mapStateToProps, {
  fetchProfile,
  updateProfile,
  toggleAvatarEditor,
  setAvatarMax,
  updateCreatedAvatar,
  updateProfileInUsers,
  setCreatedAvatar,
  setCroppedAvatar,
  resetProfile,
  setIsEditingForm,
})(withTranslation()(withRouter(CreateAvatarEditorPage)));
