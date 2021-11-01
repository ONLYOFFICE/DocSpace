import React from "react";
import PropTypes from "prop-types";
import Backdrop from "@appserver/components/backdrop";
import Heading from "@appserver/components/heading";
import Aside from "@appserver/components/aside";
import IconButton from "@appserver/components/icon-button";
import { withTranslation, I18nextProvider } from "react-i18next";
import i18n from "./i18n";
import {
  StyledEmbeddingPanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
} from "../StyledPanels";

import EmbeddingBody from "./EmbeddingBody";

class EmbeddingPanelComponent extends React.Component {
  onArrowClick = () => this.props.onClose();

  onClosePanels = () => {
    this.props.onClose();
    this.props.onSharingPanelClose();
  };

  render() {
    const { visible, t } = this.props;
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
        <Aside className="header_aside-panel">
          <StyledContent>
            <StyledHeaderContent>
              <IconButton
                size="16"
                iconName="/static/images/arrow.path.react.svg"
                onClick={this.onArrowClick}
                color="#A3A9AE"
              />
              <Heading
                className="header_aside-panel-header"
                size="medium"
                truncate
              >
                {t("EmbeddingDocument")}
              </Heading>
            </StyledHeaderContent>
            <StyledBody>
              <EmbeddingBody />
            </StyledBody>
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

const EmbeddingBodyWrapper = withTranslation("EmbeddingPanel")(
  EmbeddingPanelComponent
);

export default (props) => (
  <I18nextProvider i18n={i18n}>
    <EmbeddingBodyWrapper {...props} />
  </I18nextProvider>
);
