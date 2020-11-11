import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";

import { Backdrop, Heading, Aside } from "asc-web-components";
import { utils } from "asc-web-common";

import { withTranslation, I18nextProvider } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";

import {
  StyledVersionHistoryPanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
} from "../StyledPanels";

const i18n = createI18N({
  page: "VersionHistory",
  localesPath: "pages/VersionHistory",
});

const { changeLanguage } = utils;

class PureVersionHistoryPanel extends React.Component {
  constructor(props) {
    super(props);
  }

  onClosePanel = () => {
    this.props.onClose();
  };

  render() {
    const { visible } = this.props;
    const zIndex = 310;
    return (
      <StyledVersionHistoryPanel visible={visible}>
        <Backdrop
          onClick={this.onClosePanel}
          visible={visible}
          zIndex={zIndex}
        />
        <Aside className="header_aside-panel version-history-panel">
          <StyledContent>
            <StyledHeaderContent>
              <Heading
                className="header_aside-panel-header"
                size="medium"
                truncate
              >
                Header
              </Heading>
            </StyledHeaderContent>

            <StyledBody>Body</StyledBody>
          </StyledContent>
        </Aside>
      </StyledVersionHistoryPanel>
    );
  }
}

const VersionHistoryPanelContainer = withTranslation()(PureVersionHistoryPanel);

const VersionHistoryPanel = (props) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);
  return (
    <I18nextProvider i18n={i18n}>
      <VersionHistoryPanelContainer {...props} />
    </I18nextProvider>
  );
};

VersionHistoryPanelContainer.propTypes = {
  history: PropTypes.object.isRequired,
  isLoaded: PropTypes.bool,
};

function mapStateToProps(state) {
  return {};
}

export default connect(mapStateToProps)(VersionHistoryPanel);
