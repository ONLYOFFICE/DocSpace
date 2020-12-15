import React from "react";
import { /*RequestLoader,*/ Box } from "asc-web-components";
import { utils, api, toastr } from "asc-web-common";
import { isIOS, deviceType } from "react-device-detect";
import { setDocumentTitle } from "../../../helpers/utils";
import { changeTitle, setFavicon } from "./utils";

import textIcon from "./icons/text.ico";
import presentationIcon from "./icons/presentation.ico";
import spreadsheetIcon from "./icons/spreadsheet.ico";

const { getObjectByLocation, showLoader } = utils;

class PureEditor extends React.Component {
  async componentDidMount() {
    const urlParams = getObjectByLocation(window.location);
    const fileId = urlParams.fileId || null;
    const doc = urlParams.doc || null;

    let docTitle = null;
    let fileType = null;

    const onDocumentStateChange = (event) => {
      changeTitle(event, docTitle);
    };

    const onMetaChange = (event) => {
      const newTitle = event.data.title;
      if (newTitle && newTitle !== docTitle) {
        setDocumentTitle(newTitle);
        docTitle = newTitle;
      }
    };

    console.log("PureEditor componentDidMount", fileId, doc);

    if (this.isIPad()) {
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

          switch (fileType) {
            case "docx":
              setFavicon(textIcon);
              break;
            case "pptx":
              setFavicon(presentationIcon);
              break;
            case "xlsx":
              setFavicon(spreadsheetIcon);
              break;

            default:
              break;
          }

          setDocumentTitle(docTitle);

          if (window.innerWidth < 720) {
            config.type = "mobile";
          }

          const events = {
            events: {
              onDocumentStateChange: onDocumentStateChange,
              onMetaChange: onMetaChange,
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

  isIPad = () => {
    return isIOS && deviceType === "tablet";
  };

  render() {
    return (
      <Box
        widthProp="100vw"
        heightProp={this.isIPad() ? "calc(var(--vh, 1vh) * 100)" : "100vh"}
      >
        <div id="editor"></div>
      </Box>
    );
  }
}

export default PureEditor;
