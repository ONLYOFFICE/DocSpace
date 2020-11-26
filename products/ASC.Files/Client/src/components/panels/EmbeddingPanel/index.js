import React from "react";
import PropTypes from "prop-types";
import {
  Backdrop,
  Heading,
  Aside,
  IconButton,
  Text,
  Link,
  TextInput,
  Textarea,
} from "asc-web-components";
import { utils, toastr } from "asc-web-common";
import { withTranslation } from "react-i18next";
import {
  StyledEmbeddingPanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
} from "../StyledPanels";
import copy from "copy-to-clipboard";
import { createI18N } from "../../../helpers/i18n";
const i18n = createI18N({
  page: "EmbeddingPanel",
  localesPath: "panels/EmbeddingPanel",
});

const { changeLanguage } = utils;

class EmbeddingPanelComponent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      size: "auto",
      widthValue: "100%",
      heightValue: "100%",
      link: `<iframe src="${props.embeddingLink}" width="100%" height="100%" frameborder="0" scrolling="no" allowtransparency> </iframe>`,
    };

    changeLanguage(i18n);
  }

  onArrowClick = () => this.props.onClose();

  onClosePanels = () => {
    this.props.onClose();
    this.props.onSharingPanelClose();
  };

  onSelectSizeMiddle = () => {
    this.state.size !== "600x800" &&
      this.setState({ size: "600x800", widthValue: "600", heightValue: "800" });
  };

  onSelectSizeSmall = () => {
    this.state.size !== "400x600" &&
      this.setState({ size: "400x600", widthValue: "400", heightValue: "600" });
  };
  onSelectSizeAuto = () => {
    this.state.size !== "auto" &&
      this.setState({ size: "auto", widthValue: "100%", heightValue: "100%" });
  };

  onChangeWidth = (e) => {
    this.setState({ widthValue: e.target.value });
  };
  onChangeHeight = (e) => {
    this.setState({ heightValue: e.target.value });
  };

  onCopyLink = () => {
    copy(this.state.link);
    toastr.success(this.props.t("CodeCopySuccess"));
  };

  shouldComponentUpdate(nextProps, nextState) {
    const { size, widthValue, heightValue, link } = this.state;
    const { visible, embeddingLink } = this.props;

    if (size !== nextState.size) {
      return true;
    }

    if (widthValue !== nextState.widthValue) {
      return true;
    }

    if (heightValue !== nextState.heightValue) {
      return true;
    }

    if (visible !== nextProps.visible) {
      return true;
    }

    if (embeddingLink !== nextProps.embeddingLink) {
      return true;
    }

    if (link !== nextState.link) {
      return true;
    }

    return false;
  }

  componentDidUpdate(prevProps, prevState) {
    const { embeddingLink } = this.props;
    const { widthValue, heightValue } = this.state;

    if (
      prevProps.embeddingLink !== embeddingLink ||
      widthValue !== prevState.widthValue ||
      heightValue !== prevState.heightValue
    ) {
      const link = `<iframe src="${embeddingLink}" width="${widthValue}" height="${heightValue}" frameborder="0" scrolling="no" allowtransparency> </iframe>`;
      this.setState({ link });
    }
  }

  render() {
    const { visible, t } = this.props;
    const { size, widthValue, heightValue, link } = this.state;

    const zIndex = 310;

    //console.log("EmbeddingPanel render");
    return (
      <StyledEmbeddingPanel visible={visible}>
        <Backdrop
          onClick={this.onClosePanels}
          visible={visible}
          zIndex={zIndex}
        />
        <Aside className="header_aside-panel">
          <StyledContent>
            <StyledHeaderContent>
              <IconButton
                size="16"
                iconName="ArrowPathIcon"
                onClick={this.onArrowClick}
              />
              <Heading
                className="header_aside-panel-header"
                size="medium"
                truncate
              >
                {t("EmbeddingDocument")}
              </Heading>
            </StyledHeaderContent>
            <StyledBody size={size}>
              <Text className="embedding-panel_text">{t("Size")}:</Text>
              <div className="embedding-panel_links-container">
                <Link
                  isHovered
                  type="action"
                  className="embedding-panel_link"
                  onClick={this.onSelectSizeMiddle}
                >
                  600 x 800 px
                </Link>
                <Link
                  isHovered
                  type="action"
                  className="embedding-panel_link"
                  onClick={this.onSelectSizeSmall}
                >
                  400 x 600 px
                </Link>
                <Link
                  isHovered
                  type="action"
                  className="embedding-panel_link"
                  onClick={this.onSelectSizeAuto}
                >
                  {t("Auto")}
                </Link>
              </div>
              <div className="embedding-panel_inputs-container">
                <div>
                  <Text className="embedding-panel_text">{t("Width")}:</Text>
                  <TextInput
                    className="embedding-panel_input"
                    value={widthValue}
                    onChange={this.onChangeWidth}
                  />
                </div>
                <div>
                  <Text className="embedding-panel_text">{t("Height")}:</Text>
                  <TextInput
                    className="embedding-panel_input"
                    value={heightValue}
                    onChange={this.onChangeHeight}
                  />
                </div>
              </div>
              <div>
                <Text className="embedding-panel_text">{t("EmbedCode")}:</Text>
                <IconButton
                  className="embedding-panel_copy-icon"
                  size="16"
                  iconName="CopyIcon"
                  color="#333"
                  onClick={this.onCopyLink}
                />
                <Textarea isReadOnly value={link} />
              </div>
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

const EmbeddingPanelContainerTranslated = withTranslation()(
  EmbeddingPanelComponent
);

const EmbeddingPanel = (props) => (
  <EmbeddingPanelContainerTranslated i18n={i18n} {...props} />
);

export default EmbeddingPanel;
