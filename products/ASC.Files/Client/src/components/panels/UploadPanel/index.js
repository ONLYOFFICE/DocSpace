import React from "react";
import IconButton from "@appserver/components/icon-button";
import Backdrop from "@appserver/components/backdrop";
import Heading from "@appserver/components/heading";
import Aside from "@appserver/components/aside";
import { withTranslation } from "react-i18next";
import SharingPanel from "../SharingPanel";
import {
  StyledAsidePanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
} from "../StyledPanels";
import FileList from "./FileList";
import { inject, observer } from "mobx-react";
import Loaders from "@appserver/common/components/Loaders";
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

  render() {
    //console.log("UploadPanel render");
    const {
      t,
      uploadPanelVisible,
      /* sharingPanelVisible, */ uploaded,
      converted,
      uploadDataFiles,
      cancelUpload,
      cancelConversion,
    } = this.props;

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
            <StyledHeaderContent className="upload-panel_header-content">
              <Heading className="upload_panel-header" size="medium" truncate>
                {t("Uploads")}
              </Heading>
              <div className="upload_panel-icons-container">
                <div className="upload_panel-remove-icon">
                  {uploaded && converted ? (
                    <IconButton
                      size="20"
                      iconName="images/clear.active.react.svg"
                      color="#A3A9AE"
                      isClickable
                      onClick={this.clearUploadPanel}
                    />
                  ) : (
                    <IconButton
                      size="20"
                      iconName="images/button.cancel.react.svg"
                      color={"#A3A9AE"}
                      isClickable
                      onClick={uploaded ? cancelConversion : cancelUpload}
                    />
                  )}
                </div>
                {/*<div className="upload_panel-vertical-dots-icon">
                  <IconButton
                    size="20"
                    iconName="static/images/vertical-dots.react.svg"
                    color="#A3A9AE"
                  />
                  </div>*/}
              </div>
            </StyledHeaderContent>
            <StyledBody
              stype="mediumBlack"
              className="upload-panel_body"
              style={{ height: `calc(100vh - 64px)` }}
            >
              <FileList />
            </StyledBody>
          </StyledContent>
        </Aside>
        {/* sharingPanelVisible && <SharingPanel /> */}
      </StyledAsidePanel>
    );
  }
}

const UploadPanel = withTranslation("UploadPanel")(
  withLoader(UploadPanelComponent)(<Loaders.DialogAsideLoader isPanel />)
);

export default inject(({ /* dialogsStore, */ uploadDataStore }) => {
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
  };
})(observer(UploadPanel));
