import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { /*RequestLoader,*/ Box } from "asc-web-components";
import { utils, api, toastr } from "asc-web-common";
import { withTranslation, I18nextProvider } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";

const i18n = createI18N({
  page: "DocEditor",
  localesPath: "pages/DocEditor",
});

const { changeLanguage, getObjectByLocation, showLoader } = utils;
const { files } = api;

class PureEditor extends React.Component {
  async componentDidMount() {
    const urlParams = getObjectByLocation(window.location);
    const fileId = urlParams.fileId || null;

    console.log("PureEditor componentDidMount", fileId);

    const vh = window.innerHeight * 0.01;
    document.documentElement.style.setProperty("--vh", `${vh}px`);

    showLoader();

    let docApiUrl = await files.getDocServiceUrl();

    const script = document.createElement("script");
    script.setAttribute("type", "text/javascript");
    script.setAttribute("id", "scripDocServiceAddress");

    script.onload = function () {
      console.log("PureEditor script.onload", fileId, window.DocsAPI);

      files
        .openEdit(fileId)
        .then((config) => {
          if (window.innerWidth < 720) {
            config.type = "mobile";
          }
          if (!window.DocsAPI) throw new Error("DocsAPI is not defined");

          console.log("Trying to open file with DocsAPI", fileId);
          window.DocsAPI.DocEditor("editor", config);
        })
        .catch((e) => {
          console.log(e);
          toastr.error(e);
        });
    };

    script.src = docApiUrl;
    script.async = true;

    console.log("PureEditor componentDidMount: added script");
    document.body.appendChild(script);
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

export default withRouter(DocEditor);
