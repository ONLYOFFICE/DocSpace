import React from "react";
import { withRouter } from "react-router";
import { Toast, Box } from "asc-web-components";
import { utils, api, toastr, Loaders } from "asc-web-common";
import { setDocumentTitle } from "../../../helpers/utils";
import { changeTitle, setFavicon, isIPad } from "./utils";
import throttle from "lodash/throttle";

const { getObjectByLocation, showLoader, hideLoader } = utils;

let documentIsReady = false;

let docTitle = null;
let fileType = null;

let docSaved = null;

const throttledChangeTitle = throttle(
  () => changeTitle(docSaved, docTitle),
  500
);

class PureEditor extends React.Component {
  constructor(props) {
    super(props);

    const urlParams = getObjectByLocation(window.location);
    const fileId = urlParams ? urlParams.fileId || null : null;
    const doc = urlParams ? urlParams.doc || null : null;

    this.state = {
      fileId,
      doc,
      isLoading: true,
    };
  }
  async componentDidMount() {
    try {
      const { fileId, doc } = this.state;

      if (!fileId) return;

      showLoader();

      const docApiUrl = await api.files.getDocServiceUrl();
      const config = await api.files.openEdit(fileId, doc);

      if (isIPad()) {
        const vh = window.innerHeight * 0.01;
        document.documentElement.style.setProperty("--vh", `${vh}px`);
      }

      this.setState({ isLoading: false }, () =>
        this.loadDocApi(docApiUrl, () => this.onLoad(config))
      );
    } catch (error) {
      console.log(error);
      toastr.error(error.message, null, 0, true);
    }
  }

  loadDocApi = (docApiUrl, onLoadCallback) => {
    const script = document.createElement("script");
    script.setAttribute("type", "text/javascript");
    script.setAttribute("id", "scripDocServiceAddress");

    script.onload = onLoadCallback;

    script.src = docApiUrl;
    script.async = true;

    console.log("PureEditor componentDidMount: added script");
    document.body.appendChild(script);
  };

  onLoad = (config) => {
    try {
      docTitle = config.document.title;
      fileType = config.document.fileType;

      setFavicon(fileType);
      setDocumentTitle(docTitle);

      if (window.innerWidth < 720) {
        config.type = "mobile";
      }

      const events = {
        events: {
          onDocumentStateChange: this.onDocumentStateChange,
          onMetaChange: this.onMetaChange,
          onDocumentReady: this.onDocumentReady,
        },
      };

      const newConfig = Object.assign(config, events);

      if (!window.DocsAPI) throw new Error("DocsAPI is not defined");

      hideLoader();

      window.DocsAPI.DocEditor("editor", newConfig);
    } catch (error) {
      console.log(error);
      toastr.error(error.message, null, 0, true);
    }
  };

  onDocumentStateChange = (event) => {
    if (!documentIsReady) return;

    docSaved = !event.data;
    throttledChangeTitle();
  };

  onDocumentReady = () => {
    documentIsReady = true;
  };

  onMetaChange = (event) => {
    const newTitle = event.data.title;
    if (newTitle && newTitle !== docTitle) {
      setDocumentTitle(newTitle);
      docTitle = newTitle;
    }
  };

  render() {
    return (
      <Box
        widthProp="100vw"
        heightProp={isIPad() ? "calc(var(--vh, 1vh) * 100)" : "100vh"}
      >
        <Toast />
        {!this.state.isLoading ? (
          <div id="editor"></div>
        ) : (
          <Box paddingProp="16px">
            <Loaders.Rectangle height="96vh" />
          </Box>
        )}{" "}
      </Box>
    );
  }
}

export default withRouter(PureEditor);
