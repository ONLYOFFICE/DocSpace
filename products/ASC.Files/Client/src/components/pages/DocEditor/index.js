import React from "react";
import { withRouter } from "react-router";
import { Toast, Box } from "asc-web-components";
import { utils, api, toastr, Loaders } from "asc-web-common";

const { getObjectByLocation, showLoader, hideLoader } = utils;

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

      console.log("PureEditor componentDidMount", fileId, doc);

      const vh = window.innerHeight * 0.01;
      document.documentElement.style.setProperty("--vh", `${vh}px`);

      showLoader();

      const docApiUrl = await api.files.getDocServiceUrl();

      const config = await api.files.openEdit(fileId, doc);

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
      if (window.innerWidth < 720) {
        config.type = "mobile";
      }
      if (!window.DocsAPI) throw new Error("DocsAPI is not defined");

      hideLoader();

      window.DocsAPI.DocEditor("editor", config);
    } catch (error) {
      console.log(error);
      toastr.error(error.message, null, 0, true);
    }
  };

  render() {
    return (
      <Box widthProp="100vw" heightProp="100vh">
        <Toast />

        {!this.state.isLoading ? (
          <div id="editor"></div>
        ) : (
          <Box paddingProp="16px">
            <Loaders.Rectangle height="96vh" />
          </Box>
        )}
      </Box>
    );
  }
}

export default withRouter(PureEditor);
