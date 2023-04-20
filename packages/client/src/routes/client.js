import React from "react";
import { Navigate } from "react-router-dom";
import loadable from "@loadable/component";

import PrivateRoute from "@docspace/common/components/PrivateRoute";
import PublicRoute from "@docspace/common/components/PublicRoute";
import ErrorBoundary from "@docspace/common/components/ErrorBoundary";

import { NotFoundError } from "./utils";

const Client = loadable(() => import("../Client"));

const Home = loadable(() => import("../pages/Home"));
const AccountsHome = loadable(() => import("../pages/AccountsHome"));
const Settings = loadable(() => import("../pages/Settings"));
const Profile = loadable(() => import("../pages/Profile"));
const NotificationComponent = loadable(() => import("../pages/Notifications"));

const FormGallery = loadable(() => import("../pages/FormGallery"));
const About = loadable(() => import("../pages/About"));
const Wizard = loadable(() => import("../pages/Wizard"));
const PreparationPortal = loadable(() => import("../pages/PreparationPortal"));
const PortalUnavailable = loadable(() => import("../pages/PortalUnavailable"));
const ErrorUnavailable = loadable(() => import("../pages/Errors/Unavailable"));
const Error401 = loadable(() => import("client/Error401"));

const ClientRoutes = [
  {
    path: "/",
    element: (
      <PrivateRoute>
        <ErrorBoundary>
          <Client />
        </ErrorBoundary>
      </PrivateRoute>
    ),
    errorElement: <NotFoundError />,
    children: [
      {
        index: true,
        element: (
          <PrivateRoute>
            <Navigate to="/rooms/shared" replace />
          </PrivateRoute>
        ),
      },
      {
        path: "rooms",
        element: (
          <PrivateRoute>
            <Navigate to="/rooms/shared" replace />
          </PrivateRoute>
        ),
      },
      {
        path: "rooms/personal",
        element: (
          <PrivateRoute restricted withManager withCollaborator>
            <Home />
          </PrivateRoute>
        ),
      },
      {
        path: "rooms/personal/filter",
        element: (
          <PrivateRoute restricted withManager withCollaborator>
            <Home />
          </PrivateRoute>
        ),
      },
      {
        path: "files/trash",
        element: (
          <PrivateRoute restricted withManager withCollaborator>
            <Home />
          </PrivateRoute>
        ),
      },
      {
        path: "files/trash/filter",
        element: (
          <PrivateRoute restricted withManager withCollaborator>
            <Home />
          </PrivateRoute>
        ),
      },
      {
        path: "rooms/shared",
        element: (
          <PrivateRoute>
            <Home />
          </PrivateRoute>
        ),
      },
      {
        path: "rooms/shared/filter",
        element: (
          <PrivateRoute>
            <Home />
          </PrivateRoute>
        ),
      },
      {
        path: "rooms/archived",
        element: (
          <PrivateRoute>
            <Home />
          </PrivateRoute>
        ),
      },
      {
        path: "rooms/archived/filter",
        element: (
          <PrivateRoute>
            <Home />
          </PrivateRoute>
        ),
      },
      {
        path: "products/files",
        element: (
          <PrivateRoute>
            <Home />
          </PrivateRoute>
        ),
      },
      {
        path: "accounts",
        element: (
          <PrivateRoute restricted withManager>
            <Navigate to="/accounts/filter" replace />
          </PrivateRoute>
        ),
      },
      {
        path: "accounts/filter",
        element: (
          <PrivateRoute restricted withManager>
            <AccountsHome />
          </PrivateRoute>
        ),
      },
      {
        path: "accounts/view/@self",
        element: (
          <PrivateRoute>
            <Profile />
          </PrivateRoute>
        ),
      },
      {
        path: "accounts/view/@self/notification",
        element: (
          <PrivateRoute>
            <NotificationComponent />
          </PrivateRoute>
        ),
      },
      {
        path: "settings",
        element: (
          <PrivateRoute withCollaborator restricted>
            <Navigate to="/settings/common" replace />
          </PrivateRoute>
        ),
      },
      {
        path: "settings/common",
        element: (
          <PrivateRoute withCollaborator restricted>
            <Settings />
          </PrivateRoute>
        ),
      },
      {
        path: "settings/admin",
        element: (
          <PrivateRoute withCollaborator restricted>
            <Settings />
          </PrivateRoute>
        ),
      },
    ],
  },
  {
    path: "/Products/Files/",
    caseSensitive: true,
    element: <Navigate to="/rooms/shared" replace />,
  },
  {
    path: "/form-gallery/:folderId",
    element: (
      <PrivateRoute>
        <ErrorBoundary>
          <FormGallery />
        </ErrorBoundary>
      </PrivateRoute>
    ),
  },
  {
    path: "/wizard",
    element: (
      <PublicRoute>
        <ErrorBoundary>
          <Wizard />
        </ErrorBoundary>
      </PublicRoute>
    ),
  },
  {
    path: "/about",
    element: (
      <PrivateRoute>
        <ErrorBoundary>
          <About />
        </ErrorBoundary>
      </PrivateRoute>
    ),
  },
  {
    path: "/portal-unavailable",
    element: (
      <PrivateRoute>
        <ErrorBoundary>
          <PortalUnavailable />
        </ErrorBoundary>
      </PrivateRoute>
    ),
  },
  {
    path: "/unavailable",
    element: (
      <PrivateRoute>
        <ErrorBoundary>
          <ErrorUnavailable />
        </ErrorBoundary>
      </PrivateRoute>
    ),
  },
  {
    path: "/preparation-portal",
    element: (
      <PublicRoute>
        <ErrorBoundary>
          <PreparationPortal />
        </ErrorBoundary>
      </PublicRoute>
    ),
  },
  {
    path: "/error401",
    element: (
      <PrivateRoute>
        <ErrorBoundary>
          <Error401 />
        </ErrorBoundary>
      </PrivateRoute>
    ),
  },
];

export default ClientRoutes;
