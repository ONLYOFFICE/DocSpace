import React, { useEffect } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { RequestLoader } from "asc-web-components";
import { utils, api } from "asc-web-common";
import { withTranslation, I18nextProvider } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";
const i18n = createI18N({
  page: "DocEditor",
  localesPath: "pages/DocEditor",
});

const { changeLanguage, getObjectByLocation } = utils;
const { files } = api;

class PureEditor extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isLoading: false,
    };
  }

  onLoading = (status) => {
    this.setState({ isLoading: status });
  };

  render() {
    const { isLoading } = this.state;
    const { t } = this.props;

    const urlParams = getObjectByLocation(window.location);
    const fileId = urlParams.fileId || null;

    const vh = window.innerHeight * 0.01;
    document.documentElement.style.setProperty("--vh", `${vh}px`);

    const wrapperStyle = {
      //height: "100vh",
      height: "calc(var(--vh, 1vh) * 100)",
      width: "100vw",
    };

    files.openEdit(fileId).then((config) => {
      if (window.innerWidth < 720) {
        config.type = "mobile";
      }

      window.DocsAPI.DocEditor("editor", config);
    });

    return (
      <div style={wrapperStyle}>
        <RequestLoader
          visible={isLoading}
          zIndex={256}
          loaderSize="16px"
          loaderColor={"#999"}
          label={`${t("LoadingProcessing")} ${t("LoadingDescription")}`}
          fontSize="12px"
          fontColor={"#999"}
        />
        <div id="editor"></div>
      </div>
    );
  }
}

const EditorContainer = withTranslation()(PureEditor);

const DocEditor = (props) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);
  return (
    <I18nextProvider i18n={i18n}>
      <EditorContainer {...props} />
    </I18nextProvider>
  );
};

DocEditor.propTypes = {
  files: PropTypes.array,
  history: PropTypes.object,
  isLoaded: PropTypes.bool,
};

function mapStateToProps(state) {
  return {
    files: state.files.files,
    folders: state.files.folders,
    isLoaded: state.auth.isLoaded,
  };
}

export default connect(mapStateToProps)(withRouter(DocEditor));
