import React from "react";
import PropTypes from "prop-types";
import Backdrop from "@appserver/components/backdrop";
import Heading from "@appserver/components/heading";
import Aside from "@appserver/components/aside";
import Loaders from "@appserver/common/components/Loaders";
import FloatingButton from "@appserver/common/components/FloatingButton";
import { withTranslation } from "react-i18next";
import history from "@appserver/common/history";
import {
  StyledVersionHistoryPanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
} from "../StyledPanels";
import { SectionBodyContent } from "../../../pages/VersionHistory/Section/";
import { inject, observer } from "mobx-react";
import config from "../../../../package.json";

class PureVersionHistoryPanel extends React.Component {
  componentDidUpdate(preProps) {
    const { isTabletView, fileId } = this.props;
    if (isTabletView !== preProps.isTabletView && isTabletView) {
      this.redirectToPage(fileId);
    }
  }

  redirectToPage = (fileId) => {
    this.onClose();
    history.replace(`${this.props.homepage}/${fileId}/history`);
  };

  onClose = () => {
    this.props.setIsVerHistoryPanel(false);
  };

  render() {
    //console.log("render versionHistoryPanel");
    const { visible, isLoading, versions, showProgressBar } = this.props;
    const zIndex = 310;

    return (
      <StyledVersionHistoryPanel
        className="version-history-modal-dialog"
        visible={visible}
        isLoading={true}
      >
        <Backdrop
          onClick={this.onClose}
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
            {showProgressBar && (
              <FloatingButton
                className="layout-progress-bar"
                icon="file"
                alert={false}
              />
            )}
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
};

export default inject(({ auth, filesStore, versionHistoryStore }) => {
  const { isTabletView } = auth.settingsStore;
  const { isLoading } = filesStore;
  const {
    fileId,
    versions,
    setIsVerHistoryPanel,
    isVisible: visible,
    showProgressBar,
  } = versionHistoryStore;

  return {
    isTabletView,
    homepage: config.homepage,
    isLoading,
    fileId,
    versions,
    visible,
    showProgressBar,

    setIsVerHistoryPanel,
  };
})(observer(VersionHistoryPanel));
