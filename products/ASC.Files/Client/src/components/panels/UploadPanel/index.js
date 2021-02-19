import React from "react";
import { Backdrop, Heading, Aside, IconButton } from "asc-web-components";
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

const DownloadBodyStyle = { height: `calc(100vh - 62px)` };

class UploadPanelComponent extends React.Component {
  constructor(props) {
    super(props);

    this.ref = React.createRef();
    this.scrollRef = React.createRef();
  }

  onClose = () => {
    const {
      setUploadPanelVisible,
      uploadPanelVisible,
      uploaded,
      clearUploadData,
    } = this.props;
    setUploadPanelVisible(!uploadPanelVisible);
    if (uploaded) {
      clearUploadData();
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
    const { t, uploadPanelVisible, sharingPanelVisible, uploaded } = this.props;

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
            <StyledBody
              stype="mediumBlack"
              className="upload-panel_body"
              style={DownloadBodyStyle}
            >
              <FileList />
            </StyledBody>
          </StyledContent>
        </Aside>
        {sharingPanelVisible && <SharingPanel />}
      </StyledAsidePanel>
    );
  }
}

const UploadPanel = withTranslation("UploadPanel")(UploadPanelComponent);

export default inject(({ dialogsStore, uploadDataStore }) => {
  const { sharingPanelVisible } = dialogsStore;

  const {
    uploaded,
    clearUploadData,
    cancelUpload,
    uploadPanelVisible,
    setUploadPanelVisible,
  } = uploadDataStore;

  return {
    sharingPanelVisible,
    uploadPanelVisible,
    uploaded,

    setUploadPanelVisible,
    clearUploadData,
    cancelUpload,
  };
})(observer(UploadPanel));
