import React from "react";
import { withRouter } from "react-router";
import { /*RequestLoader,*/ Box } from "asc-web-components";
import { utils, api, toastr } from "asc-web-common";
import { setDocumentTitle } from "../../../helpers/utils";
import { changeTitleAsync, setFavicon, isIPad } from "./utils";

const { getObjectByLocation, showLoader } = utils;

class PureEditor extends React.Component {
  async componentDidMount() {
    const urlParams = getObjectByLocation(window.location);
    const fileId = urlParams.fileId || null;
    const doc = urlParams.doc || null;

    let documentIsReady = false;

    let docTitle = null;
    let fileType = null;

    let docSaved = null;
    let timeout = false;

    const onDocumentReady = () => {
      documentIsReady = true;
    };

    const onDocumentStateChange = (event) => {
      if (!documentIsReady) return;

      docSaved = !event.data;
      if (!timeout) changeTitle();
    };

    const changeTitle = () => {
      timeout = true;
      changeTitleAsync(docSaved, docTitle).then((res) => {
        timeout = false;
        if (res !== docSaved) changeTitle();
      });
    };

    const onMetaChange = (event) => {
      const newTitle = event.data.title;
      if (newTitle && newTitle !== docTitle) {
        setDocumentTitle(newTitle);
        docTitle = newTitle;
      }
    };

    console.log("PureEditor componentDidMount", fileId, doc);

    if (isIPad()) {
      const vh = window.innerHeight * 0.01;
      document.documentElement.style.setProperty("--vh", `${vh}px`);
    }

    showLoader();

    let docApiUrl = await api.files.getDocServiceUrl();

    const script = document.createElement("script");
    script.setAttribute("type", "text/javascript");
    script.setAttribute("id", "scripDocServiceAddress");

    script.onload = function () {
      console.log("PureEditor script.onload", fileId, window.DocsAPI);

      api.files
        .openEdit(fileId, doc)
        .then((config) => {
          docTitle = config.document.title;
          fileType = config.document.fileType;

          setFavicon(fileType);
          setDocumentTitle(docTitle);

          if (window.innerWidth < 720) {
            config.type = "mobile";
          }

          const events = {
            events: {
              onDocumentStateChange: onDocumentStateChange,
              onMetaChange: onMetaChange,
              onDocumentReady: onDocumentReady,
            },
          };

          const newConfig = Object.assign(config, events);

          if (!window.DocsAPI) throw new Error("DocsAPI is not defined");

          console.log("Trying to open file with DocsAPI", fileId);

          window.DocsAPI.DocEditor("editor", newConfig);
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
      <Box
        widthProp="100vw"
        heightProp={isIPad() ? "calc(var(--vh, 1vh) * 100)" : "100vh"}
      >
        <div id="editor"></div>
      </Box>
    );
  }
}

export default withRouter(PureEditor);
