import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import Backdrop from "@docspace/components/backdrop";
import Heading from "@docspace/components/heading";
import Aside from "@docspace/components/aside";
import IconButton from "@docspace/components/icon-button";
import { withTranslation, I18nextProvider } from "react-i18next";
import i18n from "./i18n";
import { StyledEmbeddingPanel, StyledContent } from "../StyledPanels";

import { StyledHeaderContent } from "../SharingPanel/StyledSharingPanel";

import EmbeddingBody from "./EmbeddingBody";

class EmbeddingPanelComponent extends React.Component {
  onArrowClick = () => this.props.onClose();

  onClosePanels = () => {
    this.props.onClose();
    this.props.onSharingPanelClose();
  };

  render() {
    const { visible, t, theme, embeddingLink } = this.props;
    const zIndex = 310;

    //console.log("EmbeddingPanel render");
    return (
      <StyledEmbeddingPanel visible={visible}>
        <Backdrop
          onClick={this.onClosePanels}
          visible={visible}
          zIndex={zIndex}
          isAside={true}
        />
        <Aside
          className="header_aside-panel"
          visible={visible}
          onClose={this.onClosePanels}
        >
          <StyledContent>
            <StyledHeaderContent isEmbedding={true}>
              <IconButton
                size="16"
                iconName="/static/images/arrow.path.react.svg"
                onClick={this.onArrowClick}
                // color={theme.filesPanels.embedding.color}
              />
              <Heading
                className="header_aside-panel-header"
                size="medium"
                truncate
              >
                {t("EmbeddingDocument")}
              </Heading>
            </StyledHeaderContent>

            <EmbeddingBody embeddingLink={embeddingLink} theme={theme} />
          </StyledContent>
        </Aside>
      </StyledEmbeddingPanel>
    );
  }
}

EmbeddingPanelComponent.propTypes = {
  visible: PropTypes.bool,
  onSharingPanelClose: PropTypes.func,
  onClose: PropTypes.func,
};

const EmbeddingBodyWrapper = inject(({ auth }) => {
  return { theme: auth.settingsStore.theme };
})(observer(withTranslation("EmbeddingPanel")(EmbeddingPanelComponent)));

export default (props) => (
  <I18nextProvider i18n={i18n}>
    <EmbeddingBodyWrapper {...props} />
  </I18nextProvider>
);
