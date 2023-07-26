//import "@docspace/common/utils/wdyr";
import React from "react";
import { RouterProvider } from "react-router-dom";

import ErrorBoundary from "@docspace/common/components/ErrorBoundary";
import "@docspace/common/custom.scss";

import router from "./router";

const App = () => {
  return (
    <ErrorBoundary>
      <RouterProvider router={router} />
    </ErrorBoundary>
  );
};

export default App;
