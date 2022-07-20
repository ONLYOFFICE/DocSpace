import React, { useEffect } from "react";
import { Router, Switch, Route, Redirect } from "react-router-dom";
import { inject, observer } from "mobx-react";
import NavMenu from "./components/NavMenu";
import Main from "./components/Main";
import PrivateRoute from "@appserver/common/components/PrivateRoute";
import PublicRoute from "@appserver/common/components/PublicRoute";
import ErrorBoundary from "@appserver/common/components/ErrorBoundary";
import Layout from "./components/Layout";
import ScrollToTop from "./components/Layout/ScrollToTop";
import history from "@appserver/common/history";
import toastr from "studio/toastr";
import { combineUrl, updateTempContent } from "@appserver/common/utils";
import { Provider as MobxProvider } from "mobx-react";
import ThemeProvider from "@appserver/components/theme-provider";

import store from "studio/store";
import config from "../package.json";
import { I18nextProvider, useTranslation } from "react-i18next";
import i18n from "./i18n";
import AppLoader from "@appserver/common/components/AppLoader";
import System from "./components/System";
import { AppServerConfig } from "@appserver/common/constants";
import Snackbar from "@appserver/components/snackbar";
import moment from "moment";
import ReactSmartBanner from "./components/SmartBanner";
import { useThemeDetector } from "./helpers/utils";

const { proxyURL } = AppServerConfig;
const homepage = config.homepage;

const PROXY_HOMEPAGE_URL = combineUrl(proxyURL, homepage);
const HOME_URLS = [
  combineUrl(PROXY_HOMEPAGE_URL),
  combineUrl(PROXY_HOMEPAGE_URL, "/"),
  combineUrl(PROXY_HOMEPAGE_URL, "/error=:error"),
];
const WIZARD_URL = combineUrl(PROXY_HOMEPAGE_URL, "/wizard");
const ABOUT_URL = combineUrl(PROXY_HOMEPAGE_URL, "/about");
const LOGIN_URLS = [
  combineUrl(PROXY_HOMEPAGE_URL, "/login"),
  combineUrl(PROXY_HOMEPAGE_URL, "/login/confirmed-email=:confirmedEmail"),
];
const CONFIRM_URL = combineUrl(PROXY_HOMEPAGE_URL, "/confirm");
const COMING_SOON_URLS = [combineUrl(PROXY_HOMEPAGE_URL, "/coming-soon")];
const PAYMENTS_URL = combineUrl(PROXY_HOMEPAGE_URL, "/payments");
const SETTINGS_URL = combineUrl(PROXY_HOMEPAGE_URL, "/settings");
const ERROR_401_URL = combineUrl(PROXY_HOMEPAGE_URL, "/error401");
const PROFILE_MY_URL = combineUrl(PROXY_HOMEPAGE_URL, "/my");
const ENTER_CODE_URL = combineUrl(PROXY_HOMEPAGE_URL, "/code");
const INVALID_URL = combineUrl(PROXY_HOMEPAGE_URL, "/login/error=:error");
const PREPARATION_PORTAL = combineUrl(
  PROXY_HOMEPAGE_URL,
  "/preparation-portal"
);

const Payments = React.lazy(() => import("./components/pages/Payments"));
const Error404 = React.lazy(() => import("studio/Error404"));
const Error401 = React.lazy(() => import("studio/Error401"));
const Home = React.lazy(() => import("./components/pages/Home"));

const About = React.lazy(() => import("./components/pages/About"));
const Wizard = React.lazy(() => import("./components/pages/Wizard"));
const Settings = React.lazy(() => import("./components/pages/Settings"));
const ComingSoon = React.lazy(() => import("./components/pages/ComingSoon"));
const Confirm =
  !IS_PERSONAL && React.lazy(() => import("./components/pages/Confirm"));
const MyProfile = React.lazy(() => import("people/MyProfile"));
const EnterCode = !IS_PERSONAL && React.lazy(() => import("login/codeLogin"));
const InvalidError = React.lazy(() =>
  import("./components/pages/Errors/Invalid")
);
const PreparationPortal = React.lazy(() =>
  import("./components/pages/PreparationPortal")
);

const SettingsRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <Settings {...props} />
    </ErrorBoundary>
  </React.Suspense>
);
const PaymentsRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <Payments {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const Error404Route = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <Error404 {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const Error401Route = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <Error401 {...props} />
    </ErrorBoundary>
  </React.Suspense>
);
const HomeRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <Home {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const ConfirmRoute =
  !IS_PERSONAL &&
  ((props) => (
    <React.Suspense fallback={<AppLoader />}>
      <ErrorBoundary>
        <Confirm {...props} />
      </ErrorBoundary>
    </React.Suspense>
  ));

const PreparationPortalRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <PreparationPortal {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const AboutRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <About {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const WizardRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <Wizard {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const ComingSoonRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <ComingSoon {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const MyProfileRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <MyProfile {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const EnterCodeRoute =
  !IS_PERSONAL &&
  ((props) => (
    <React.Suspense fallback={<AppLoader />}>
      <ErrorBoundary>
        <EnterCode {...props} />
      </ErrorBoundary>
    </React.Suspense>
  ));

const InvalidRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <InvalidError {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const RedirectToHome = () => <Redirect to={PROXY_HOMEPAGE_URL} />;

const Shell = ({ items = [], page = "home", ...rest }) => {
  const {
    isLoaded,
    loadBaseInfo,
    modules,
    isDesktop,
    language,
    FirebaseHelper,
    personal,
    setCheckedMaintenance,
    socketHelper,
    setPreparationPortalDialogVisible,
    isBase,
    setTheme,
    setMaintenanceExist,
    roomsMode,
    setSnackbarExist,
    userTheme,
  } = rest;

  useEffect(() => {
    try {
      if (!window.AppServer) {
        window.AppServer = {};
      }

      //TEMP object, will be removed!!!
      window.AppServer.studio = {
        HOME_URLS,
        WIZARD_URL,
        ABOUT_URL,
        LOGIN_URLS,
        CONFIRM_URL,
        COMING_SOON_URLS,
        PAYMENTS_URL,
        SETTINGS_URL,
        ERROR_401_URL,
      };

      loadBaseInfo();
    } catch (err) {
      toastr.error(err);
    }
  }, []);

  useEffect(() => {
    socketHelper.emit({
      command: "subscribe",
      data: "backup-restore",
    });
    socketHelper.on("restore-backup", () => {
      setPreparationPortalDialogVisible(true);
    });
  }, [socketHelper]);

  const { t, ready } = useTranslation(["Common", "SmartBanner"]);

  let snackTimer = null;
  let fbInterval = null;
  let lastCampaignStr = null;
  const LS_CAMPAIGN_DATE = "maintenance_to_date";
  const DATE_FORMAT = "YYYY-MM-DD";
  const SNACKBAR_TIMEOUT = 10000;

  const setSnackBarTimer = (campaign) => {
    snackTimer = setTimeout(() => showSnackBar(campaign), SNACKBAR_TIMEOUT);
  };

  const clearSnackBarTimer = () => {
    if (!snackTimer) return;

    clearTimeout(snackTimer);
    snackTimer = null;
  };

  const showSnackBar = (campaign) => {
    clearSnackBarTimer();

    let skipMaintenance;

    const { fromDate, toDate, desktop } = campaign;

    console.log(
      `FB: 'bar/maintenance' desktop=${desktop} fromDate=${fromDate} toDate=${toDate}`
    );

    if (!campaign || !fromDate || !toDate) {
      console.log("Skip snackBar by empty campaign params");
      skipMaintenance = true;
    }

    const to = moment(toDate).local();

    const watchedCampaignDateStr = localStorage.getItem(LS_CAMPAIGN_DATE);

    const campaignDateStr = to.format(DATE_FORMAT);
    if (campaignDateStr == watchedCampaignDateStr) {
      console.log("Skip snackBar by already watched");
      skipMaintenance = true;
    }

    const from = moment(fromDate).local();
    const now = moment();

    if (now.isBefore(from)) {
      setSnackBarTimer(campaign);

      Snackbar.close();
      console.log(`Show snackBar has been delayed for 1 minute`, now);
      skipMaintenance = true;
    }

    if (now.isAfter(to)) {
      console.log("Skip snackBar by current date", now);
      Snackbar.close();
      skipMaintenance = true;
    }

    if (isDesktop && !desktop) {
      console.log("Skip snackBar by desktop", desktop);
      Snackbar.close();
      skipMaintenance = true;
    }

    if (skipMaintenance) {
      setCheckedMaintenance(true);
      return;
    }

    setSnackBarTimer(campaign);

    if (!document.getElementById("main-bar")) return;

    const campaignStr = JSON.stringify(campaign);
    // let skipRender = lastCampaignStr === campaignStr;

    const hasChild = document.getElementById("main-bar").hasChildNodes();

    if (hasChild) return;

    lastCampaignStr = campaignStr;

    const targetDate = to.locale(language).format("LL");

    const barConfig = {
      parentElementId: "main-bar",
      headerText: t("Attention"),
      text: `${t("BarMaintenanceDescription", {
        targetDate: targetDate,
        productName: "ONLYOFFICE Personal",
      })} ${t("BarMaintenanceDisclaimer")}`,
      isMaintenance: true,
      clickAction: () => {
        setMaintenanceExist(false);
        setSnackbarExist(false);
        Snackbar.close();
        localStorage.setItem(LS_CAMPAIGN_DATE, to.format(DATE_FORMAT));
      },
      opacity: 1,
      onLoad: () => {
        setCheckedMaintenance(true);
        setSnackbarExist(true);
        setMaintenanceExist(true);
      },
    };

    Snackbar.show(barConfig);
  };

  const fetchMaintenance = () => {
    try {
      if (!FirebaseHelper.isEnabled) return;

      FirebaseHelper.checkMaintenance()
        .then((campaign) => {
          console.log("checkMaintenance", campaign);
          if (!campaign) {
            setCheckedMaintenance(true);
            clearSnackBarTimer();
            Snackbar.close();
            return;
          }

          setTimeout(() => showSnackBar(campaign), 1000);
        })
        .catch((err) => {
          console.error(err);
        });
    } catch (e) {
      console.log(e);
    }
  };

  const fetchBanners = () => {
    if (!FirebaseHelper.isEnabled) return;

    FirebaseHelper.checkBar()
      .then((bar) => {
        localStorage.setItem("bar", bar);
      })
      .catch((err) => {
        console.log(err);
      });

    FirebaseHelper.checkCampaigns()
      .then((campaigns) => {
        localStorage.setItem("campaigns", campaigns);
      })
      .catch((err) => {
        console.error(err);
      });
  };

  useEffect(() => {
    if (!isLoaded) return;

    updateTempContent();

    if (!FirebaseHelper.isEnabled) {
      setCheckedMaintenance(true);
      localStorage.setItem("campaigns", "");
      return;
    }

    fetchMaintenance();
    fetchBanners();
    fbInterval = setInterval(fetchMaintenance, 60000);
    const bannerInterval = setInterval(fetchBanners, 60000 * 720); // get every 12 hours

    return () => {
      if (fbInterval) {
        clearInterval(fbInterval);
      }
      clearInterval(bannerInterval);
      clearSnackBarTimer();
    };
  }, [isLoaded]);

  useEffect(() => {
    console.log("Current page ", page);
  }, [page]);

  useEffect(() => {
    if (userTheme) setTheme(userTheme);
  }, [userTheme]);

  const pathname = window.location.pathname.toLowerCase();
  const isEditor = pathname.indexOf("doceditor") !== -1;

  if (!window.AppServer.studio) {
    window.AppServer.studio = {};
  }

  window.AppServer.studio.modules = {};

  const dynamicRoutes = modules.map((m) => {
    const appURL = m.link;
    const remoteEntryURL = combineUrl(
      window.location.origin,
      appURL,
      `remoteEntry.js`
    );

    const system = {
      url: remoteEntryURL,
      scope: m.appName,
      module: "./app",
    };

    window.AppServer.studio.modules[m.appName] = {
      appURL,
      remoteEntryURL,
    };

    return (
      <PrivateRoute
        key={m.id}
        path={
          m.appName === "files"
            ? [
                "/Products/Files",
                "/Products/Files/",
                "/Products/Files/?desktop=true",
                appURL,
              ]
            : appURL
        }
        component={System}
        system={system}
      />
    );
  });

  const loginRoutes = [];

  if (isLoaded && !IS_PERSONAL) {
    let module;
    if (roomsMode) {
      module = "./roomsLogin";
    } else {
      module = "./login";
    }

    const loginSystem = {
      url: combineUrl(AppServerConfig.proxyURL, "/login/remoteEntry.js"),
      scope: "login",
      module: module,
    };
    loginRoutes.push(
      <PublicRoute
        key={loginSystem.scope}
        exact
        path={LOGIN_URLS}
        component={System}
        system={loginSystem}
      />
    );
  }

  const roomsRoutes = [];

  if (!IS_PERSONAL && roomsMode) {
    roomsRoutes.push(
      <Route path={ENTER_CODE_URL} component={EnterCodeRoute} />
    );
  }

  const currentTheme = isBase ? "Base" : "Dark";
  const systemTheme = useThemeDetector();
  useEffect(() => {
    if (userTheme === "System" && currentTheme !== systemTheme)
      setTheme(systemTheme);
  }, [systemTheme]);

  return (
    <Layout>
      <Router history={history}>
        <ReactSmartBanner t={t} ready={ready} />
        {isEditor ? <></> : <NavMenu />}
        <ScrollToTop />
        <Main isDesktop={isDesktop}>
          <Switch>
            <PrivateRoute exact path={HOME_URLS} component={HomeRoute} />
            <PublicRoute exact path={WIZARD_URL} component={WizardRoute} />
            <PrivateRoute path={ABOUT_URL} component={AboutRoute} />
            {loginRoutes}
            {roomsRoutes}
            {!IS_PERSONAL && (
              <Route path={CONFIRM_URL} component={ConfirmRoute} />
            )}
            <Route path={INVALID_URL} component={InvalidRoute} />
            <PrivateRoute path={COMING_SOON_URLS} component={ComingSoonRoute} />
            <PrivateRoute path={PAYMENTS_URL} component={PaymentsRoute} />
            {!personal && (
              <PrivateRoute
                restricted
                path={SETTINGS_URL}
                component={SettingsRoute}
              />
            )}
            <PrivateRoute
              exact
              allowForMe
              path={PROFILE_MY_URL}
              component={MyProfileRoute}
            />
            <PrivateRoute
              path={PREPARATION_PORTAL}
              component={PreparationPortalRoute}
            />
            {dynamicRoutes}
            <PrivateRoute path={ERROR_401_URL} component={Error401Route} />
            <PrivateRoute
              component={!personal ? Error404Route : RedirectToHome}
            />
          </Switch>
        </Main>
      </Router>
    </Layout>
  );
};

const ShellWrapper = inject(({ auth, backup }) => {
  const { init, isLoaded, settingsStore, setProductVersion, language } = auth;

  const {
    personal,
    roomsMode,
    isDesktopClient,
    firebaseHelper,
    setModuleInfo,
    setCheckedMaintenance,
    setMaintenanceExist,
    setSnackbarExist,
    socketHelper,
    setTheme,
  } = settingsStore;
  const isBase = settingsStore.theme.isBase;
  const { setPreparationPortalDialogVisible } = backup;

  return {
    loadBaseInfo: async () => {
      await init();

      setModuleInfo(config.homepage, "home");
      setProductVersion(config.version);

      if (isDesktopClient) {
        document.body.classList.add("desktop");
      }
    },
    language,
    isLoaded,
    modules: auth.moduleStore.modules,
    isDesktop: isDesktopClient,
    FirebaseHelper: firebaseHelper,
    personal,
    setCheckedMaintenance,
    setMaintenanceExist,
    socketHelper,
    setPreparationPortalDialogVisible,
    isBase,
    setTheme,
    roomsMode,
    setSnackbarExist,
    userTheme: isDesktopClient
      ? window.RendererProcessVariable?.theme?.type === "dark"
        ? "Dark"
        : "Base"
      : auth?.userStore?.user?.theme,
  };
})(observer(Shell));

const ThemeProviderWrapper = inject(({ auth }) => {
  const { settingsStore } = auth;

  return { theme: settingsStore.theme };
})(observer(ThemeProvider));

export default () => (
  <MobxProvider {...store}>
    <I18nextProvider i18n={i18n}>
      <ThemeProviderWrapper>
        <ShellWrapper />
      </ThemeProviderWrapper>
    </I18nextProvider>
  </MobxProvider>
);
