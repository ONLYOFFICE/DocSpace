import VerticalDotsReactSvgUrl from "PUBLIC_DIR/images/vertical-dots.react.svg?url";
import ClearActiveReactSvgUrl from "PUBLIC_DIR/images/clear.active.react.svg?url";
import ButtonCancelReactSvgUrl from "PUBLIC_DIR/images/button.cancel.react.svg?url";
import React from "react";
import IconButton from "@docspace/components/icon-button";
import Backdrop from "@docspace/components/backdrop";
import Heading from "@docspace/components/heading";
import Aside from "@docspace/components/aside";
import { withTranslation } from "react-i18next";
import {
  StyledAsidePanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
} from "../StyledPanels";
import FileList from "./FileList";
import { inject, observer } from "mobx-react";
import Loaders from "@docspace/common/components/Loaders";
import withLoader from "../../../HOCs/withLoader";

class UploadPanelComponent extends React.Component {
  constructor(props) {
    super(props);

    this.ref = React.createRef();
    this.scrollRef = React.createRef();
  }

  onClose = () => {
    const {
      uploaded,
      converted,
      clearUploadData,
      uploadPanelVisible,
      clearUploadedFiles,
      setUploadPanelVisible,
      clearPrimaryProgressData,
    } = this.props;

    setUploadPanelVisible(!uploadPanelVisible);

    if (uploaded) {
      if (converted) {
        clearUploadData();
        clearPrimaryProgressData();
      } else {
        clearUploadedFiles();
      }
    }
  };
  componentDidMount() {
    document.addEventListener("keyup", this.onKeyPress);
  }
  componentWillUnmount() {
    document.removeEventListener("keyup", this.onKeyPress);
  }

  onKeyPress = (event) => {
    if (event.key === "Esc" || event.key === "Escape") {
      this.onClose();
    }
  };

  clearUploadPanel = () => {
    this.props.clearUploadData();
    this.onClose();
  };

  onCancelUpload = () => {
    this.props.cancelUpload(this.props.t);
  };

  render() {
    //console.log("UploadPanel render");
    const {
      t,
      uploadPanelVisible,
      /* sharingPanelVisible, */ uploaded,
      converted,
      uploadDataFiles,
      cancelConversion,
      isUploading,
      isUploadingAndConversion,
      theme,
    } = this.props;

    const visible = uploadPanelVisible;
    const zIndex = 310;

    const title = isUploading
      ? t("Uploads")
      : isUploadingAndConversion
      ? t("UploadAndConvert")
      : t("Files:Convert");

    return (
      <StyledAsidePanel visible={visible}>
        <Backdrop
          onClick={this.onClose}
          visible={visible}
          zIndex={zIndex}
          isAside={true}
        />
        <Aside
          className="header_aside-panel"
          visible={visible}
          withoutBodyScroll
          onClose={this.onClose}
        >
          <StyledContent>
            <StyledHeaderContent className="upload-panel_header-content">
              <Heading className="upload_panel-header" size="medium" truncate>
                {title}
              </Heading>
              <div className="upload_panel-icons-container">
                <div className="upload_panel-remove-icon">
                  {uploaded && converted ? (
                    <IconButton
                      size="20"
                      iconName={ClearActiveReactSvgUrl}
                      // color={theme.filesPanels.upload.color}
                      isClickable
                      onClick={this.clearUploadPanel}
                    />
                  ) : (
                    <IconButton
                      size="20"
                      iconName={ButtonCancelReactSvgUrl}
                      // color={theme.filesPanels.upload.color}
                      isClickable
                      onClick={
                        uploaded ? cancelConversion : this.onCancelUpload
                      }
                    />
                  )}
                </div>
                {/*<div className="upload_panel-vertical-dots-icon">
                  <IconButton
                    size="20"
                    iconName={VerticalDotsReactSvgUrl}
                    color="#A3A9AE"
                  />
                  </div>*/}
              </div>
            </StyledHeaderContent>
            <StyledBody stype="mediumBlack" className="upload-panel_body">
              <FileList />
            </StyledBody>
          </StyledContent>
        </Aside>
      </StyledAsidePanel>
    );
  }
}

const UploadPanel = withTranslation(["UploadPanel", "Files"])(
  withLoader(UploadPanelComponent)(<Loaders.DialogAsideLoader isPanel />)
);

export default inject(({ /* dialogsStore, */ auth, uploadDataStore }) => {
  //const { sharingPanelVisible } = dialogsStore;

  const {
    uploaded,
    converted,
    clearUploadData,
    cancelUpload,
    cancelConversion,
    clearUploadedFiles,
    uploadPanelVisible,
    setUploadPanelVisible,
    files,
    primaryProgressDataStore,
    isUploading,
    isUploadingAndConversion,
  } = uploadDataStore;

  const { clearPrimaryProgressData } = primaryProgressDataStore;

  return {
    //sharingPanelVisible,
    uploadPanelVisible,
    uploaded,
    converted,

    setUploadPanelVisible,
    clearUploadData,
    cancelUpload,
    cancelConversion,
    clearUploadedFiles,
    uploadDataFiles: files,
    clearPrimaryProgressData,
    isUploading,
    isUploadingAndConversion,

    theme: auth.settingsStore.theme,
  };
})(observer(UploadPanel));
