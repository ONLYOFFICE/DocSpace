import React from "react";
import { withRouter } from "react-router";
import { /*RequestLoader,*/ Box } from "asc-web-components";
import { utils, api, toastr } from "asc-web-common";

const { getObjectByLocation, showLoader } = utils;

class PureEditor extends React.Component {
  async componentDidMount() {
    const urlParams = getObjectByLocation(window.location);
    const fileId = urlParams.fileId || null;
    const doc = urlParams.doc || null;

    console.log("PureEditor componentDidMount", fileId, doc);

    const vh = window.innerHeight * 0.01;
    document.documentElement.style.setProperty("--vh", `${vh}px`);

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

export default withRouter(PureEditor);
