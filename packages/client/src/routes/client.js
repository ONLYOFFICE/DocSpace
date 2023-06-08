import React from "react";
import { Navigate } from "react-router-dom";
import loadable from "@loadable/component";

import PrivateRoute from "@docspace/common/components/PrivateRoute";
import PublicRoute from "@docspace/common/components/PublicRoute";
import ErrorBoundary from "@docspace/common/components/ErrorBoundary";

import Error404 from "SRC_DIR/pages/Errors/404";
import FilesView from "SRC_DIR/pages/Home/View/Files";
import AccountsView from "SRC_DIR/pages/Home/View/Accounts";
import SettingsView from "SRC_DIR/pages/Home/View/Settings";

const Client = loadable(() => import("../Client"));

const Home = loadable(() => import("../pages/Home"));

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
    errorElement: <Error404 />,
    children: [
      {
        path: "/",
        element: <Home />,
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
        path: "archived",
        element: (
          <PrivateRoute>
            <Navigate to="/rooms/archived" replace />
          </PrivateRoute>
        ),
      },
      {
        path: "rooms/personal",
        element: (
          <PrivateRoute restricted withManager withCollaborator>
                <FilesView />
          </PrivateRoute>
        ),
      },
      {
        path: "rooms/personal/filter",
        element: (
          <PrivateRoute restricted withManager withCollaborator>
                <FilesView />
          </PrivateRoute>
        ),
      },
      {
        path: "files/trash",
        element: (
          <PrivateRoute restricted withManager withCollaborator>
                <FilesView />
          </PrivateRoute>
        ),
      },
      {
        path: "files/trash/filter",
        element: (
          <PrivateRoute restricted withManager withCollaborator>
                <FilesView />
          </PrivateRoute>
        ),
      },
      {
        path: "rooms/shared",
        element: (
          <PrivateRoute>
                <FilesView />
          </PrivateRoute>
        ),
      },
      {
        path: "rooms/shared/filter",
        element: (
          <PrivateRoute>
                <FilesView />
          </PrivateRoute>
        ),
      },
      {
        path: "rooms/shared/:room",
        element: (
          <PrivateRoute>
                <FilesView />
          </PrivateRoute>
        ),
      },
      {
        path: "rooms/shared/:room/filter",
        element: (
          <PrivateRoute>
                <FilesView />
          </PrivateRoute>
        ),
      },
      {
        path: "rooms/archived",
        element: (
          <PrivateRoute>
                <FilesView />
          </PrivateRoute>
        ),
      },
      {
        path: "rooms/archived/filter",
        element: (
          <PrivateRoute>
                <FilesView />
          </PrivateRoute>
        ),
      },
      {
        path: "rooms/archived/:room",
        element: (
          <PrivateRoute>
                <FilesView />
          </PrivateRoute>
        ),
      },
      {
        path: "rooms/archived/:room/filter",
        element: (
          <PrivateRoute>
                <FilesView />
          </PrivateRoute>
        ),
      },
      {
        path: "products/files",
        element: (
          <PrivateRoute>
                <FilesView />
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
                <AccountsView />
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
                <SettingsView />
          </PrivateRoute>
        ),
      },
      {
            path: "settings/admin",
        element: (
          <PrivateRoute withCollaborator restricted>
                <SettingsView />
          </PrivateRoute>
        ),
      },
        ],
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
