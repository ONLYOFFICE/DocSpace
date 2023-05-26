import React from "react";
import PropTypes from "prop-types";
import Backdrop from "@docspace/components/backdrop";
import Heading from "@docspace/components/heading";
import Aside from "@docspace/components/aside";
import Loaders from "@docspace/common/components/Loaders";
import FloatingButton from "@docspace/components/floating-button";
import { withTranslation } from "react-i18next";
import {
  StyledVersionHistoryPanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
} from "../StyledPanels";
import { SectionBodyContent } from "../../../pages/VersionHistory/Section/";
import { inject, observer } from "mobx-react";
import config from "PACKAGE_FILE";

class PureVersionHistoryPanel extends React.Component {
  onClose = () => {
    const { setIsVerHistoryPanel } = this.props;
    setIsVerHistoryPanel(false);
  };

  componentDidMount() {
    document.addEventListener("keyup", this.onKeyPress);
  }

  componentWillUnmount() {
    document.removeEventListener("keyup", this.onKeyPress);
  }

  onKeyPress = (e) => (e.key === "Esc" || e.key === "Escape") && this.onClose();

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
        <Aside
          className="version-history-aside-panel"
          visible={visible}
          onClose={this.onClose}
          withoutBodyScroll
        >
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
