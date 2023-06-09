import React from "react";

import loadable from "@loadable/component";

import PrivateRoute from "@docspace/common/components/PrivateRoute";

const Profile = loadable(() => import("../pages/Profile"));
const NotificationComponent = loadable(() => import("../pages/Notifications"));

const generalRoutes = [
  {
    path: "profile",
    element: (
      <PrivateRoute>
        <Profile />
      </PrivateRoute>
    ),
  },
  {
    path: "profile/notification",
    element: (
      <PrivateRoute>
        <NotificationComponent />
      </PrivateRoute>
    ),
  },
];

export { generalRoutes };
