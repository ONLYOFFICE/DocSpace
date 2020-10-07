import React, { useEffect } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { /*RequestLoader,*/ Box } from "asc-web-components";
import { utils, api, store } from "asc-web-common";
import { withTranslation, I18nextProvider } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";
import { getFiles, getFolders } from "../../../store/files/selectors";

const { getIsLoaded } = store.auth.selectors;

const i18n = createI18N({
  page: "DocEditor",
  localesPath: "pages/DocEditor",
});

const { changeLanguage, getObjectByLocation, hideLoader, showLoader } = utils;
const { files } = api;

class PureEditor extends React.Component {
  componentDidMount() {
    const urlParams = getObjectByLocation(window.location);
    const fileId = urlParams.fileId || null;

    const vh = window.innerHeight * 0.01;
    document.documentElement.style.setProperty("--vh", `${vh}px`);

    showLoader();

    files
      .openEdit(fileId)
      .then((config) => {
        if (window.innerWidth < 720) {
          config.type = "mobile";
        }

        window.DocsAPI.DocEditor("editor", config);
      })
      .catch((e) => {
        console.log(e);
        hideLoader();
      });
  }

  render() {
    return (
      <Box widthProp="100vw" heightProp="calc(var(--vh, 1vh) * 100)">
        <div id="editor"></div>
      </Box>
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
    files: getFiles(state),
    folders: getFolders(state),
    isLoaded: getIsLoaded(state),
  };
}

export default connect(mapStateToProps)(withRouter(DocEditor));
