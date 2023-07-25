import React from "react";
import { createBrowserRouter, Navigate } from "react-router-dom";

//import routes from "./routes";
import Error404 from "client/Error404";

import App from "./App";
import Branding from "./categories/branding";
import Spaces from "./categories/spaces";

const router = createBrowserRouter([
  {
    path: "/management",
    element: <App />,
    errorElement: <Error404 />,
    children: [
      { index: true, element: <Navigate to="/management/spaces" replace /> },
      {
        path: "spaces",
        element: <Spaces />,
      },
      {
        path: "branding",
        element: <Branding />,
      },
    ],
  },
]);

export default router;
