import React from "react";
import {
  Backdrop,
  Heading,
  Aside,
  IconButton,
  Icons,
} from "asc-web-components";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { utils as commonUtils, store } from "asc-web-common";
import {
  setUploadPanelVisible,
  setSharingPanelVisible,
  cancelUpload,
  cancelCurrentUpload,
  setMediaViewerData,
  setSelection,
  setSelected,
} from "../../../store/files/actions";
import {
  getUploadPanelVisible,
  getUploadDataFiles,
  getArchiveFormats,
  getImageFormats,
  getSoundFormats,
  getSharePanelVisible,
  getMediaViewerImageFormats,
  getMediaViewerMediaFormats,
  getSelected,
  getUploadData,
} from "../../../store/files/selectors";
import SharingPanel from "../SharingPanel";
import {
  StyledAsidePanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
} from "../StyledPanels";
import FileRow from "./FileRow";
import { createI18N } from "../../../helpers/i18n";
const i18n = createI18N({
  page: "UploadPanel",
  localesPath: "panels/UploadPanel",
});
const { changeLanguage } = commonUtils;

const { getCurrentUserId } = store.auth.selectors;

const DownloadBodyStyle = { height: `calc(100vh - 156px)` };

class UploadPanelComponent extends React.Component {
  constructor(props) {
    super(props);

    changeLanguage(i18n);

    this.state = {
      uploadData: [],
    };
    this.ref = React.createRef();
    this.scrollRef = React.createRef();
  }

  onClose = () => {
    this.props.setUploadPanelVisible(!this.props.uploadPanelVisible);
  };
  componentDidMount() {
    document.addEventListener("keyup", this.onKeyPress);
  }
  componentWillUnmount() {
    document.removeEventListener("keyup", this.onKeyPress);
  }

  componentDidUpdate(prevProps, prevState) {
    const { uploadDataFiles } = this.props;
    if (
      prevState.uploadData !== uploadDataFiles &&
      uploadDataFiles.length > 0
    ) {
      this.setState({
        uploadData: prevProps.uploadDataFiles,
      });
    }
  }

  onKeyPress = (event) => {
    if (event.key === "Esc" || event.key === "Escape") {
      this.onClose();
    }
  };

  clearUploadPanel = () => {
    this.setState({
      uploadData: [],
      uploaded: false,
    });
    this.onClose();
  };

  cancelCurrentUpload = (index) => {
    this.props.cancelCurrentUpload(index);
    console.log("cancel upload ", index);
  };

  onOpenSharingPanel = (item) => {
    console.log(item);
    const {
      selected,
      setSelected,
      setSelection,
      setSharingPanelVisible,
      sharingPanelVisible,
    } = this.props;
    selected === "close" && setSelected("none");
    setSelection([item]);
    setSharingPanelVisible(!sharingPanelVisible);
  };

  onMediaClick = (id) => {
    console.log("id", id);
    const item = { visible: true, id: id };
    this.props.setMediaViewerData(item);
  };

  render() {
    const {
      t,
      uploadPanelVisible,
      sharingPanelVisible,
      uploadDataFiles,
      archiveFormats,
      imageFormats,
      soundFormats,
      mediaViewerImageFormats,
      mediaViewerMediaFormats,
      getUploadData,
    } = this.props;

    const { uploadData } = this.state;
    const uploaded = getUploadData.uploaded;

    const visible = uploadPanelVisible;
    const zIndex = 310;
    return (
      <StyledAsidePanel visible={visible}>
        <Backdrop
          onClick={this.onClose}
          visible={visible}
          zIndex={zIndex}
          isAside={true}
        />
        <Aside className="header_aside-panel" visible={visible}>
          <StyledContent>
            <StyledHeaderContent>
              <Heading className="upload_panel-header" size="medium" truncate>
                {t("Uploads")}
              </Heading>
              <div className="upload_panel-icons-container">
                <div className="upload_panel-remove-icon">
                  {uploaded ? (
                    <IconButton
                      size="20"
                      iconName="ClearActiveIcon"
                      color="#A3A9AE"
                      isClickable={true}
                      onClick={this.clearUploadPanel}
                    />
                  ) : (
                    <IconButton
                      size="20"
                      iconName="ButtonCancelIcon"
                      color="#A3A9AE"
                      isClickable={true}
                      onClick={this.props.cancelUpload}
                    />
                  )}
                </div>
                {/*<div className="upload_panel-vertical-dots-icon">
                  <IconButton
                    size="20"
                    iconName="VerticalDotsIcon"
                    color="#A3A9AE"
                  />
                  </div>*/}
              </div>
            </StyledHeaderContent>
            <StyledBody stype="mediumBlack" style={DownloadBodyStyle}>
              {uploadData.map((item, index) => (
                <FileRow
                  t={t}
                  item={item}
                  key={index}
                  index={index}
                  archiveFormats={archiveFormats}
                  imageFormats={imageFormats}
                  soundFormats={soundFormats}
                  uploaded={uploaded}
                  cancelCurrentUpload={this.cancelCurrentUpload}
                  onOpenSharingPanel={this.onOpenSharingPanel}
                  mediaViewerImageFormats={mediaViewerImageFormats}
                  mediaViewerMediaFormats={mediaViewerMediaFormats}
                  onMediaClick={this.onMediaClick}
                />
              ))}
            </StyledBody>
          </StyledContent>
        </Aside>
        {sharingPanelVisible && <SharingPanel />}
      </StyledAsidePanel>
    );
  }
}

const UploadPanelContainerTranslated = withTranslation()(UploadPanelComponent);

const UploadPanel = (props) => (
  <UploadPanelContainerTranslated i18n={i18n} {...props} />
);

const mapStateToProps = (state) => {
  return {
    isMyId: getCurrentUserId(state),
    uploadPanelVisible: getUploadPanelVisible(state),
    uploadDataFiles: getUploadDataFiles(state),
    archiveFormats: getArchiveFormats(state),
    imageFormats: getImageFormats(state),
    soundFormats: getSoundFormats(state),
    sharingPanelVisible: getSharePanelVisible(state),
    mediaViewerImageFormats: getMediaViewerImageFormats(state),
    mediaViewerMediaFormats: getMediaViewerMediaFormats(state),
    selected: getSelected(state),
    getUploadData: getUploadData(state),
  };
};

export default connect(mapStateToProps, {
  setUploadPanelVisible,
  setSharingPanelVisible,
  cancelUpload,
  cancelCurrentUpload,
  setMediaViewerData,
  setSelection,
  setSelected,
})(withRouter(UploadPanel));
