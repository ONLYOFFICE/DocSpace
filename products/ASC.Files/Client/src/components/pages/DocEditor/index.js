import React from "react";
import { withRouter } from "react-router";
import { Toast, Box } from "asc-web-components";
import { utils, api, toastr } from "asc-web-common";

const { getObjectByLocation, showLoader, hideLoader } = utils;

class PureEditor extends React.Component {
  async componentDidMount() {
    try {
      const urlParams = getObjectByLocation(window.location);
      const fileId = urlParams.fileId || null;
      const doc = urlParams.doc || null;

      console.log("PureEditor componentDidMount", fileId, doc);

      const vh = window.innerHeight * 0.01;
      document.documentElement.style.setProperty("--vh", `${vh}px`);

      showLoader();

      const docApiUrl = await api.files.getDocServiceUrl();

      const config = await api.files.openEdit(fileId, doc);

      this.loadDocApi(docApiUrl, () => this.onLoad(config));
    } catch (error) {
      console.log(error);
      toastr.error(error);
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
      if (window.innerWidth < 720) {
        config.type = "mobile";
      }
      if (!window.DocsAPI) throw new Error("DocsAPI is not defined");

      hideLoader();

      window.DocsAPI.DocEditor("editor", config);
    } catch (e) {
      console.log(e);
      toastr.error(e);
    }
  };

  render() {
    return (
      <Box widthProp="100vw" heightProp="calc(var(--vh, 1vh) * 100)">
        <Toast />
        <div id="editor"></div>
      </Box>
    );
  }
}

export default withRouter(PureEditor);
