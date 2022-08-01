//import "@docspace/common/utils/wdyr";
import React from "react";
import ErrorBoundary from "@docspace/common/components/ErrorBoundary";
import Shell from "client/shell";
import "@docspace/common/custom.scss";

const App = () => {
  return (
    <ErrorBoundary>
      <Shell />
    </ErrorBoundary>
  );
};

export default App;
