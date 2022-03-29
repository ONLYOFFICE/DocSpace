//import "@appserver/common/utils/wdyr";
import React from "react";
import Editor from "./Editor";
import ErrorBoundary from "@appserver/common/components/ErrorBoundary";
import "@appserver/common/custom.scss";
import { AppServerConfig } from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";

const App = () => {
  const onError = () =>
    window.open(combineUrl(AppServerConfig.proxyURL, "/login"), "_self");
  return (
    <ErrorBoundary onError={onError}>
      <Editor />
    </ErrorBoundary>
  );
};

export default App;
