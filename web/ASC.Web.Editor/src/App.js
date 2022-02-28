//import "@appserver/common/utils/wdyr";
import React from "react";
import Editor from "./Editor.js";
//import ErrorBoundary from "@appserver/common/components/ErrorBoundary";
import "@appserver/common/custom.scss";
import { AppServerConfig } from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";

const App = (initProps) => {
  const onError = () =>
    window.open(combineUrl(AppServerConfig.proxyURL, "/login"), "_self");
  return (
    // <ErrorBoundary onError={onError}>
    <Editor {...initProps} />
    // </ErrorBoundary>
  );
};

export default App;
