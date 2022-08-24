//import "@docspace/common/utils/wdyr";
import React from "react";
import Shell from "client/shell";
import ErrorBoundary from "@docspace/common/components/ErrorBoundary";
import "@docspace/common/custom.scss";

const App = () => {
  return (
    <ErrorBoundary>
      <Shell />
    </ErrorBoundary>
  );
};

export default App;
