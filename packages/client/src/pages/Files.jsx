import React from "react";
//import { Provider as FilesProvider } from "mobx-react";
import { inject, observer } from "mobx-react";
import { Routes, useLocation, Navigate, Route } from "react-router-dom";
//import config from "PACKAGE_FILE";
import PrivateRoute from "@docspace/common/components/PrivateRoute";
import AppLoader from "@docspace/common/components/AppLoader";
import toastr from "@docspace/components/toast/toastr";
import {
  //combineUrl,
  updateTempContent,
  loadScript,
} from "@docspace/common/utils";

//import i18n from "./i18n";
import { withTranslation } from "react-i18next";
import { regDesktop } from "@docspace/common/desktop";
import Home from "./Home";
import Settings from "./Settings";
//import VersionHistory from "./VersionHistory";
import PrivateRoomsPage from "./PrivateRoomsPage";
import ErrorBoundary from "@docspace/common/components/ErrorBoundary";
import Panels from "../components/FilesPanels";
import Article from "@docspace/common/components/Article";
import {
  ArticleBodyContent,
  ArticleHeaderContent,
  ArticleMainButtonContent,
} from "../components/Article";

import GlobalEvents from "../components/GlobalEvents";
import Accounts from "./Accounts";
import Profile from "./Profile";
import NotificationComponent from "./Notifications";

// const homepage = config.homepage;

// const PROXY_HOMEPAGE_URL = combineUrl(window.DocSpaceConfig?.proxy?.url, homepage);

// const HOME_URL = combineUrl(PROXY_HOMEPAGE_URL, "/");
// const SETTINGS_URL = combineUrl(PROXY_HOMEPAGE_URL, "/settings/:setting");
// const HISTORY_URL = combineUrl(PROXY_HOMEPAGE_URL, "/:fileId/history");
// const PRIVATE_ROOMS_URL = combineUrl(PROXY_HOMEPAGE_URL, "/private");
// const FILTER_URL = combineUrl(PROXY_HOMEPAGE_URL, "/filter");
// const MEDIA_VIEW_URL = combineUrl(PROXY_HOMEPAGE_URL, "/#preview");
// const FORM_GALLERY_URL = combineUrl(
//   PROXY_HOMEPAGE_URL,
//   "/form-gallery/:folderId"
// );
// const ROOMS_URL = combineUrl(PROXY_HOMEPAGE_URL, "/rooms");

const Error404 = React.lazy(() => import("client/Error404"));

const FilesArticle = React.memo(({ withMainButton }) => {
  const location = useLocation();
  const isFormGallery = location.pathname.split("/").includes("form-gallery");

  return !isFormGallery ? (
    <Article withMainButton={withMainButton}>
      <Article.Header>
        <ArticleHeaderContent />
      </Article.Header>

      <Article.MainButton>
        <ArticleMainButtonContent />
      </Article.MainButton>

      <Article.Body>
        <ArticleBodyContent />
      </Article.Body>
    </Article>
  ) : (
    <></>
  );
});

const Error404Route = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <Error404 {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const FilesSection = React.memo(({ withMainButton }) => {
  return (
    <Routes>
      <Route
        path="/"
        element={
          <PrivateRoute location={location}>
            <Navigate to="/rooms/shared" replace />
          </PrivateRoute>
        }
      />

      <Route
        path="/rooms"
        element={
          <PrivateRoute location={location}>
            <Navigate to="/rooms/shared" replace />
          </PrivateRoute>
        }
      />

      <Route
        path="/rooms/personal/*"
        element={
          <PrivateRoute
            restricted
            withManager
            withCollaborator
            location={location}
          >
            <Home />
          </PrivateRoute>
        }
      />

      <Route
        path="/files/trash/*"
        element={
          <PrivateRoute
            restricted
            withManager
            withCollaborator
            location={location}
          >
            <Home />
          </PrivateRoute>
        }
      />

      <Route
        path="/rooms/shared/*"
        element={
          <PrivateRoute location={location}>
            <Home />
          </PrivateRoute>
        }
      />

      <Route
        path="/rooms/archived/*"
        element={
          <PrivateRoute location={location}>
            <Home />
          </PrivateRoute>
        }
      />

      <Route
        path="/products/files/"
        element={
          <PrivateRoute location={location}>
            <Home />
          </PrivateRoute>
        }
      />

      <Route
        path="/accounts/*"
        element={
          <PrivateRoute restricted withManager location={location}>
            <Accounts />
          </PrivateRoute>
        }
      />

      <Route
        path="/accounts/create/:type"
        element={
          <PrivateRoute restricted withManager location={location}>
            <Accounts />
          </PrivateRoute>
        }
      />

      <Route
        path="/accounts/view/@self"
        element={
          <PrivateRoute location={location}>
            <Profile />
          </PrivateRoute>
        }
      />

      <Route
        path="accounts/view/@self/notification"
        element={
          <PrivateRoute location={location}>
            <NotificationComponent />
          </PrivateRoute>
        }
      />

      <Route
        path={"/settings"}
        element={
          <PrivateRoute withCollaborator restricted location={location}>
            <Navigate to="/settings/common" replace />
          </PrivateRoute>
        }
      />

      <Route
        path={"/settings/*"}
        element={
          <PrivateRoute withCollaborator restricted location={location}>
            <Settings />
          </PrivateRoute>
        }
      />

      <Route
        element={
          <PrivateRoute location={location}>
            <Error404Route />
          </PrivateRoute>
        }
      />
    </Routes>
  );
});

const FilesContent = (props) => {
  const pathname = window.location.pathname.toLowerCase();
  const isEditor = pathname.indexOf("doceditor") !== -1;
  const [isDesktopInit, setIsDesktopInit] = React.useState(false);

  const {
    loadFilesInfo,
    setIsLoaded,
    isAuthenticated,
    user,
    isEncryption,
    encryptionKeys,
    setEncryptionKeys,
    isLoaded,
    isDesktop,
    showMenu,
    isFrame,
    withMainButton,

    t,
  } = props;

  React.useEffect(() => {
    loadScript("/static/scripts/tiff.min.js", "img-tiff-script");

    loadFilesInfo()
      .catch((err) => toastr.error(err))
      .finally(() => {
        setIsLoaded(true);

        updateTempContent();
      });

    return () => {
      const script = document.getElementById("img-tiff-script");
      document.body.removeChild(script);
    };
  }, []);

  React.useEffect(() => {
    if (isAuthenticated && !isDesktopInit && isDesktop && isLoaded) {
      setIsDesktopInit(true);
      regDesktop(
        user,
        isEncryption,
        encryptionKeys,
        setEncryptionKeys,
        isEditor,
        null,
        t
      );
      console.log(
        "%c%s",
        "color: green; font: 1.2em bold;",
        "Current keys is: ",
        encryptionKeys
      );
    }
  }, [
    t,
    isAuthenticated,
    user,
    isEncryption,
    encryptionKeys,
    setEncryptionKeys,
    isLoaded,
    isDesktop,
    isDesktopInit,
  ]);

  return (
    <>
      <GlobalEvents />
      <Panels />
      {isFrame ? (
        showMenu && <FilesArticle />
      ) : (
        <FilesArticle withMainButton={withMainButton} />
      )}
      <FilesSection withMainButton={withMainButton} />
    </>
  );
};

const Files = inject(({ auth, filesStore }) => {
  const {
    frameConfig,
    isFrame,
    isDesktopClient,
    encryptionKeys,
    setEncryptionKeys,
    isEncryptionSupport,
  } = auth.settingsStore;

  if (!auth.userStore.user) return;

  const { isVisitor } = auth.userStore.user;

  const withMainButton = !isVisitor;

  return {
    isDesktop: isDesktopClient,
    isFrame,
    showMenu: frameConfig?.showMenu,
    user: auth.userStore.user,
    isAuthenticated: auth.isAuthenticated,
    encryptionKeys: encryptionKeys,
    isEncryption: isEncryptionSupport,
    isLoaded: auth.isLoaded && filesStore.isLoaded,
    setIsLoaded: filesStore.setIsLoaded,
    withMainButton,

    setEncryptionKeys: setEncryptionKeys,
    loadFilesInfo: async () => {
      await filesStore.initFiles();
      //auth.setProductVersion(config.version);
    },
  };
})(withTranslation("Common")(observer(FilesContent)));

export default () => <Files />;
