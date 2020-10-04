import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import styled from 'styled-components';
import { withTranslation } from "react-i18next";
import { AvatarEditor, utils } from "asc-web-components";
import { api, toastr } from "asc-web-common";
import { fetchProfile, updateProfile, getUserPhoto } from "../../../../../store/profile/actions";
import { toEmployeeWrapper} from "../../../../../store/people/selectors";
import { setDocumentTitle } from "../../../../../helpers/utils";
import { isMobile } from "react-device-detect";

const { createThumbnailsAvatar, loadAvatar, deleteAvatar } = api.people;
const { isTablet } = utils.device;


const AvatarEditorBody = styled.div`
  margin-bottom: 24px;
`;

class SectionBodyContent extends React.PureComponent{
  constructor(props) {
    super(props);

    this.state = {
      avatar: {
        tmpFile: "",
        image: null,
        defaultWidth: 0,
        defaultHeight: 0
      },
      isMobile: isMobile || isTablet,
      isLoading: false
    }
  }

  componentDidMount() {
    const { match, fetchProfile, t } = this.props;
    const { userId } = match.params;

    setDocumentTitle(t("ProfileAction"));
    if (userId) {
      fetchProfile(userId);
    }
  }

  componentDidUpdate(prevProps) {
    const { match, fetchProfile, profile } = this.props;
    const { userId } = match.params;
    const { avatar } = this.state;
    const prevUserId = prevProps.match.params.userId;

    if (userId !== undefined && userId !== prevUserId) {
      fetchProfile(userId);
    }

    if(profile && !avatar.image){
      this.setUserPhotoToState();
    }
  }

  onBackClick = () => {
    const {profile, settings} = this.props

    this.props.history.push(`${settings.homepage}/edit/${profile.userName}`)
  }

  onCloseAvatarEditor = () => {
    console.log("onCloseAvatarEditor")
  }

  onSaveAvatar = (isUpdate, result) => {
    this.setState({ isLoading: true });
    const { profile } = this.props
    if (isUpdate) {
      createThumbnailsAvatar(profile.id, {
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
          //let stateCopy = Object.assign({}, this.state);
          //stateCopy.avatar.tmpFile = "";
       /*   stateCopy.profile.avatarMax =
            response.max +
            "?_=" +
            Math.floor(Math.random() * Math.floor(10000));*/
          toastr.success(this.props.t("ChangesSavedSuccessfully"));
          this.setState({ isLoading: false });
          //this.setState(stateCopy);
        })
        .catch(error => {
          toastr.error(error);
          this.setState({ isLoading: false });
        })
        .then(() => this.props.updateProfile(this.props.profile))
        .then(() => this.props.fetchProfile(profile.id));
    } else {
      deleteAvatar(profile.id)
        .then(response => {
          //let stateCopy = Object.assign({}, this.state);
          //stateCopy.visibleAvatarEditor = false;
          //stateCopy.profile.avatarMax = response.big;
          toastr.success(this.props.t("ChangesSavedSuccessfully"));
          //this.setState(stateCopy);
        })
        .catch(error => toastr.error(error));
    }
  }

  onLoadFileAvatar = (file, callback) => {
    const { profile } = this.props

    this.setState({ isLoading: true });
    let data = new FormData();
    let _this = this;
    data.append("file", file);
    data.append("Autosave", false);
    loadAvatar(profile.id, data)
      .then(response => {
        var img = new Image();
        img.onload = function() {
          var stateCopy = Object.assign({}, _this.state);
          stateCopy.avatar = {
            tmpFile: response.data,
            image: response.data,
            defaultWidth: img.width,
            defaultHeight: img.height
          };
          _this.setState(stateCopy);
          _this.setState({ isLoading: false });
          if (typeof callback === "function") callback();
        };
        img.src = response.data;
      })
      .catch(error => {
        toastr.error(error);
        this.setState({ isLoading: false });
      });
  }

  setUserPhotoToState = () => {
    const {profile} = this.props

    getUserPhoto(profile.id).then(userPhotoData => {
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
                : null
            }
          });
        }
      }
    })
  }

  render(){

    const {t, profile} = this.props

    return(
      <AvatarEditorBody>
        {t("UploadNewPhoto")}
        <AvatarEditor
          useModalDialog={false}
          image={this.state.avatar.image}
          visible={true}
          onClose={this.onCloseAvatarEditor}
          onSave={this.onSaveAvatar}
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
          saveButtonLoading={this.state.isLoading}
        />

      </AvatarEditorBody>
      
    )
  }
}

function mapStateToProps(state) {
  return {
    profile: state.profile.targetUser,
    settings: state.auth.settings
  };
}

export default connect(
  mapStateToProps,
  { fetchProfile,updateProfile }
  )(withTranslation()(withRouter(SectionBodyContent)));
