//import "@appserver/common/utils/wdyr";
import React from "react";
import Editor from "./Editor";
import ErrorBoundary from "@appserver/common/components/ErrorBoundary";
import "@appserver/common/custom.scss";

const App = () => {
  return (
    <ErrorBoundary>
      <Editor />
    </ErrorBoundary>
  );
};

export default App;
