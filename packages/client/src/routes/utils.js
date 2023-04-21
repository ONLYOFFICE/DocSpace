import React from "react";

export const NotFoundError = () => {
  return (
    <PrivateRoute>
      <ErrorBoundary>
        <Error404 />
      </ErrorBoundary>
    </PrivateRoute>
  );
};
