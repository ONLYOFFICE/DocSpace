import React from "react";
//import { Provider as FilesProvider } from "mobx-react";
import { inject, observer } from "mobx-react";
import { Switch, withRouter, Redirect } from "react-router-dom";
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

const FilesArticle = React.memo(({ history, withMainButton }) => {
  const isFormGallery = history.location.pathname
    .split("/")
    .includes("form-gallery");

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

const FilesSection = React.memo(({ withMainButton }) => {
  return (
    <Switch>
      {/*<PrivateRoute exact path={HISTORY_URL} component={VersionHistory} />*/}
      {/* <PrivateRoute path={"/private"} component={PrivateRoomsPage} /> */}
      <PrivateRoute
        exact
        path={"/settings"}
        component={() => <Redirect to="/settings/common" />}
      />
      <PrivateRoute
        exact
        path={["/", "/rooms"]}
        component={() => <Redirect to="/rooms/shared" />}
      />
      <PrivateRoute
        restricted
        withManager
        withCollaborator
        path={[
          "/rooms/personal",
          "/rooms/personal/filter",

          "/files/trash",
          "/files/trash/filter",
        ]}
        component={Home}
      />

      <PrivateRoute
        path={[
          "/rooms/shared",
          "/rooms/shared/filter",
          "/rooms/shared/:room",
          "/rooms/shared/:room/filter",

          "/rooms/archived",
          "/rooms/archived/filter",
          "/rooms/archived/:room",
          "/rooms/archived/:room/filter",

          // "/files/favorite",
          // "/files/favorite/filter",

          // "/files/recent",
          // "/files/recent/filter",
          "/products/files/",
        ]}
        component={Home}
      />
      {/* <PrivateRoute path={["/rooms/personal/filter"]} component={Home} /> */}
      {/* <PrivateRoute path={"/#preview"} component={Home} /> */}
      {/* <PrivateRoute path={"/rooms"} component={Home} /> */}
      {/* <PrivateRoute path={ROOMS_URL} component={VirtualRooms} /> */}

      <PrivateRoute
        exact
        restricted
        withManager
        path={[
          "/accounts",
          "/accounts/changeOwner",
          "/accounts/filter",
          "/accounts/create/:type",
        ]}
        component={Accounts}
      />

      <PrivateRoute
        exact
        path={["/accounts/view/@self", "/accounts/view/@self/notification"]}
        component={Accounts}
      />

      <PrivateRoute
        exact
        restricted
        path={"/settings/admin"}
        component={Settings}
      />
      {withMainButton && (
        <PrivateRoute exact path={"/settings/common"} component={Settings} />
      )}
      <PrivateRoute component={Error404Route} />
    </Switch>
  );
});

const Error404Route = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <Error404 {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

class FilesContent extends React.Component {
  constructor(props) {
    super(props);

    const pathname = window.location.pathname.toLowerCase();
    this.isEditor = pathname.indexOf("doceditor") !== -1;
    this.isDesktopInit = false;
  }

  componentDidMount() {
    loadScript("/static/scripts/tiff.min.js", "img-tiff-script");

    this.props
      .loadFilesInfo()
      .catch((err) => toastr.error(err))
      .finally(() => {
        this.props.setIsLoaded(true);

        updateTempContent();
      });
  }

  componentWillUnmount() {
    const script = document.getElementById("img-tiff-script");
    document.body.removeChild(script);
  }

  componentDidUpdate(prevProps) {
    const {
      isAuthenticated,
      user,
      isEncryption,
      encryptionKeys,
      setEncryptionKeys,
      isLoaded,
      isDesktop,
    } = this.props;
    // console.log("componentDidUpdate: ", this.props);
    if (isAuthenticated && !this.isDesktopInit && isDesktop && isLoaded) {
      this.isDesktopInit = true;
      regDesktop(
        user,
        isEncryption,
        encryptionKeys,
        setEncryptionKeys,
        this.isEditor,
        null,
        this.props.t
      );
      console.log(
        "%c%s",
        "color: green; font: 1.2em bold;",
        "Current keys is: ",
        encryptionKeys
      );
    }
  }

  render() {
    const { showMenu, isFrame, withMainButton, history } = this.props;

    return (
      <>
        <GlobalEvents />
        <Panels />
        {isFrame ? (
          showMenu && (
            <FilesArticle history={history} withMainButton={withMainButton} />
          )
        ) : (
          <FilesArticle history={history} withMainButton={withMainButton} />
        )}
        <FilesSection withMainButton={withMainButton} />
      </>
    );
  }
}

const Files = inject(({ auth, filesStore }) => {
  const {
    frameConfig,
    isFrame,
    isDesktopClient,
    encryptionKeys,
    setEncryptionKeys,
    isEncryptionSupport,
  } = auth.settingsStore;

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
})(withTranslation("Common")(observer(withRouter(FilesContent))));

export default () => <Files />;
