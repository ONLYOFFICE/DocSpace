import React from "react";
import PropTypes from "prop-types";
import { Backdrop, Heading, Aside } from "asc-web-components";
import { Loaders } from "asc-web-common";
import { withTranslation } from "react-i18next";
import {
  StyledVersionHistoryPanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
} from "../StyledPanels";
import { SectionBodyContent } from "../../pages/VersionHistory/Section/";
import { inject, observer } from "mobx-react";

class PureVersionHistoryPanel extends React.Component {
  componentDidUpdate(preProps) {
    const { isTabletView, fileId } = this.props;
    if (isTabletView !== preProps.isTabletView && isTabletView) {
      this.redirectToPage(fileId);
    }
  }

  redirectToPage = (fileId) => {
    const { history, homepage, setIsVerHistoryPanel } = this.props;
    setIsVerHistoryPanel(false);

    history.replace(`${homepage}/${fileId}/history`);
  };

  onClosePanelHandler = () => {
    this.props.onClose();
  };

  render() {
    //console.log("render versionHistoryPanel");
    const { visible, isLoading, versions } = this.props;
    const zIndex = 310;

    return (
      <StyledVersionHistoryPanel
        className="version-history-modal-dialog"
        visible={visible}
        isLoading={true}
      >
        <Backdrop
          onClick={this.onClosePanelHandler}
          visible={visible}
          zIndex={zIndex}
          isAside={true}
        />
        <Aside className="version-history-aside-panel">
          <StyledContent>
            <StyledHeaderContent className="version-history-panel-header">
              {versions && !isLoading ? (
                <Heading
                  className="version-history-panel-heading"
                  size="medium"
                  truncate
                >
                  {versions[0].title}
                </Heading>
              ) : (
                <Loaders.ArticleHeader
                  className="loader-version-history"
                  height="28"
                  width="688"
                  title="version-history-header-loader"
                />
              )}
            </StyledHeaderContent>

            <StyledBody className="version-history-panel-body">
              <SectionBodyContent />
            </StyledBody>
          </StyledContent>
        </Aside>
      </StyledVersionHistoryPanel>
    );
  }
}

const VersionHistoryPanel = withTranslation("VersionHistory")(
  PureVersionHistoryPanel
);

VersionHistoryPanel.propTypes = {
  fileId: PropTypes.string,
  visible: PropTypes.bool,
  onClose: PropTypes.func,
};

export default inject(({ auth, initFilesStore, versionHistoryStore }) => {
  const { isTabletView, homepage } = auth.settingsStore;
  const { isLoading } = initFilesStore;
  const {
    fileId,
    versions,
    setIsVerHistoryPanel,
    setVerHistoryFileId,
  } = versionHistoryStore;

  return {
    isTabletView,
    homepage,
    isLoading,
    fileId,
    versions,

    setIsVerHistoryPanel,
    setVerHistoryFileId,
  };
})(observer(VersionHistoryPanel));
