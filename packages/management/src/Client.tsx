import React from "react";
import { RouterProvider } from "react-router-dom";

import ErrorBoundary from "@docspace/common/components/ErrorBoundary";
import router from "./router";

const Client = () => {
  return (
    <ErrorBoundary>
      <RouterProvider router={router} />
    </ErrorBoundary>
  );
};

export default Client;
