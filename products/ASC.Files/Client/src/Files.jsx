import React from "react";
import { Provider as FilesProvider } from "mobx-react";
import { inject, observer } from "mobx-react";
import { Switch, withRouter } from "react-router-dom";
import config from "../package.json";
import PrivateRoute from "@appserver/common/components/PrivateRoute";
import AppLoader from "@appserver/common/components/AppLoader";
import toastr from "studio/toastr";
import {
  combineUrl,
  updateTempContent,
  loadScript,
} from "@appserver/common/utils";
import stores from "./store/index";
import i18n from "./i18n";
import { I18nextProvider, withTranslation } from "react-i18next";
import { regDesktop } from "@appserver/common/desktop";
import Home from "./pages/Home";
import Settings from "./pages/Settings";
import VersionHistory from "./pages/VersionHistory";
import PrivateRoomsPage from "./pages/PrivateRoomsPage";
import ErrorBoundary from "@appserver/common/components/ErrorBoundary";
import Panels from "./components/FilesPanels";
import { AppServerConfig } from "@appserver/common/constants";
import Article from "@appserver/common/components/Article";
import {
  ArticleBodyContent,
  ArticleHeaderContent,
  ArticleMainButtonContent,
} from "./components/Article";
import FormGallery from "./pages/FormGallery";
import GlobalEvents from "./components/GlobalEvents";

const { proxyURL } = AppServerConfig;
const homepage = config.homepage;

const PROXY_HOMEPAGE_URL = combineUrl(proxyURL, homepage);

const HOME_URL = combineUrl(PROXY_HOMEPAGE_URL, "/");
const SETTINGS_URL = combineUrl(PROXY_HOMEPAGE_URL, "/settings/:setting");
const HISTORY_URL = combineUrl(PROXY_HOMEPAGE_URL, "/:fileId/history");
const PRIVATE_ROOMS_URL = combineUrl(PROXY_HOMEPAGE_URL, "/private");
const FILTER_URL = combineUrl(PROXY_HOMEPAGE_URL, "/filter");
const MEDIA_VIEW_URL = combineUrl(PROXY_HOMEPAGE_URL, "/#preview");
const FORM_GALLERY_URL = combineUrl(
  PROXY_HOMEPAGE_URL,
  "/form-gallery/:folderId"
);

if (!window.AppServer) {
  window.AppServer = {};
}

window.AppServer.files = {
  HOME_URL,
  SETTINGS_URL,
  HISTORY_URL,
  PRIVATE_ROOMS_URL,
  FILTER_URL,
  MEDIA_VIEW_URL,
};

const Error404 = React.lazy(() => import("studio/Error404"));

const FilesArticle = React.memo(({ history }) => {
  const isFormGallery = history.location.pathname
    .split("/")
    .includes("form-gallery");

  return !isFormGallery ? (
    <Article>
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

const FilesSection = React.memo(() => {
  return (
    <Switch>
      <PrivateRoute exact path={SETTINGS_URL} component={Settings} />
      {/*<PrivateRoute exact path={HISTORY_URL} component={VersionHistory} />*/}
      <PrivateRoute path={PRIVATE_ROOMS_URL} component={PrivateRoomsPage} />
      <PrivateRoute exact path={HOME_URL} component={Home} />
      <PrivateRoute path={FILTER_URL} component={Home} />
      <PrivateRoute path={MEDIA_VIEW_URL} component={Home} />
      <PrivateRoute path={FORM_GALLERY_URL} component={FormGallery} />
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
    //const { /*, isDesktop*/ } = this.props;

    return (
      <>
        <GlobalEvents />
        <Panels />
        <FilesArticle history={this.props.history} />
        <FilesSection />
      </>
    );
  }
}

const Files = inject(({ auth, filesStore }) => {
  return {
    isDesktop: auth.settingsStore.isDesktopClient,
    user: auth.userStore.user,
    isAuthenticated: auth.isAuthenticated,
    encryptionKeys: auth.settingsStore.encryptionKeys,
    isEncryption: auth.settingsStore.isEncryptionSupport,
    isLoaded: auth.isLoaded && filesStore.isLoaded,
    setIsLoaded: filesStore.setIsLoaded,

    setEncryptionKeys: auth.settingsStore.setEncryptionKeys,
    loadFilesInfo: async () => {
      //await auth.init();
      await filesStore.initFiles();
      auth.setProductVersion(config.version);
    },
  };
})(withTranslation("Common")(observer(withRouter(FilesContent))));

export default () => (
  <FilesProvider {...stores}>
    <I18nextProvider i18n={i18n}>
      <Files />
    </I18nextProvider>
  </FilesProvider>
);
