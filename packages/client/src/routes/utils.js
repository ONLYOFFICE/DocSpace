import React from "react";
import PrivateRoute from "@docspace/common/components/PrivateRoute";
import ErrorBoundary from "@docspace/common/components/ErrorBoundary";
import Error404 from "SRC_DIR/pages/Errors/404";

export const NotFoundError = () => {
  return (
    <PrivateRoute>
      <ErrorBoundary>
        <Error404 />
      </ErrorBoundary>
    </PrivateRoute>
  );
};
