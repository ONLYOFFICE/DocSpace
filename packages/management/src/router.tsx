import React from "react";
import { createBrowserRouter, Navigate } from "react-router-dom";

import App from "./App";

import Spaces from "./categories/spaces";
import Branding from "./categories/branding";
import Backup from "./categories/backup";
import Restore from "./categories/restore";
import Payments from "./categories/payments";

import Error404 from "client/Error404";

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
      {
        path: "backup",
        element: <Backup />,
      },
      {
        path: "backup/data-backup",
        element: <Backup />,
      },
      {
        path: "backup/auto-backup",
        element: <Backup />,
      },
      {
        path: "restore",
        element: <Restore />,
      },
      {
        path: "payments",
        element: <Payments />,
      },
    ],
  },
]);

export default router;
