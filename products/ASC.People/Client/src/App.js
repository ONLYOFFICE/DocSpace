//import "@appserver/common/utils/wdyr";
import React from "react";
import Shell from "studio/shell";
import ErrorBoundary from "@appserver/common/components/ErrorBoundary";
import "@appserver/common/custom.scss";

const App = () => {
  return (
    <ErrorBoundary>
      <Shell />
    </ErrorBoundary>
  );
};

export default App;
