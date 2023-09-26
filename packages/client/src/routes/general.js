import React from "react";
import { Navigate } from "react-router-dom";

import loadable from "@loadable/component";

import PrivateRoute from "@docspace/common/components/PrivateRoute";

const Profile = loadable(() => import("../pages/Profile"));

const generalRoutes = [
  {
    path: "profile",
    element: <Navigate to="/profile/login" />,
  },
  {
    path: "profile/login",
    element: (
      <PrivateRoute>
        <Profile />
      </PrivateRoute>
    ),
  },
  {
    path: "profile/notifications",
    element: (
      <PrivateRoute>
        <Profile />
      </PrivateRoute>
    ),
  },
  {
    path: "profile/file-management",
    element: (
      <PrivateRoute withCollaborator restricted>
        <Profile />
      </PrivateRoute>
    ),
  },
  {
    path: "profile/interface-theme",
    element: (
      <PrivateRoute>
        <Profile />
      </PrivateRoute>
    ),
  },
];

export { generalRoutes };
