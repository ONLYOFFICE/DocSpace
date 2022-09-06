import React, { useEffect } from "react";
import { Router, Switch, Route, Redirect } from "react-router-dom";
import { inject, observer } from "mobx-react";
import NavMenu from "./components/NavMenu";
import Main from "./components/Main";
import PrivateRoute from "@docspace/common/components/PrivateRoute";
import PublicRoute from "@docspace/common/components/PublicRoute";
import ErrorBoundary from "@docspace/common/components/ErrorBoundary";
import Layout from "./components/Layout";
import ScrollToTop from "./components/Layout/ScrollToTop";
import history from "@docspace/common/history";
import Toast from "@docspace/components/toast";
import toastr from "client/toastr";
import { combineUrl, updateTempContent } from "@docspace/common/utils";
import { Provider as MobxProvider } from "mobx-react";
import ThemeProvider from "@docspace/components/theme-provider";
import store from "client/store";

import config from "PACKAGE_FILE";
import { I18nextProvider, useTranslation } from "react-i18next";
import i18n from "./i18n";
import AppLoader from "@docspace/common/components/AppLoader";
import System from "./components/System";
import { AppServerConfig } from "@docspace/common/constants";
import Snackbar from "@docspace/components/snackbar";
import moment from "moment";
import ReactSmartBanner from "./components/SmartBanner";
import { useThemeDetector } from "SRC_DIR/helpers/utils";
import { isMobileOnly } from "react-device-detect";
import IndicatorLoader from "./components/IndicatorLoader";
import DialogsWrapper from "./components/dialogs/DialogsWrapper";

// const { proxyURL } = AppServerConfig;
// const homepage = config.homepage;

// const PROXY_HOMEPAGE_URL = combineUrl(proxyURL, homepage);
// const HOME_URLS = [
//   combineUrl(PROXY_HOMEPAGE_URL),
//   combineUrl(PROXY_HOMEPAGE_URL, "/"),
//   combineUrl(PROXY_HOMEPAGE_URL, "/error=:error"),
// ];
// const WIZARD_URL = combineUrl(PROXY_HOMEPAGE_URL, "/wizard");
// const ABOUT_URL = combineUrl(PROXY_HOMEPAGE_URL, "/about");
// const CONFIRM_URL = combineUrl(PROXY_HOMEPAGE_URL, "/confirm");

// const PAYMENTS_URL = combineUrl(PROXY_HOMEPAGE_URL, "/payments");
// const SETTINGS_URL = combineUrl(PROXY_HOMEPAGE_URL, "/portal-settings");
// const ERROR_401_URL = combineUrl(PROXY_HOMEPAGE_URL, "/error401");
// const PROFILE_MY_URL = combineUrl(PROXY_HOMEPAGE_URL, "/my");
// const ENTER_CODE_URL = combineUrl(PROXY_HOMEPAGE_URL, "/code");
// const PREPARATION_PORTAL = combineUrl(
//   PROXY_HOMEPAGE_URL,
//   "/preparation-portal"
// );

const Payments = React.lazy(() => import("./pages/Payments"));
const Error404 = React.lazy(() => import("client/Error404"));
const Error401 = React.lazy(() => import("client/Error401"));
const Files = React.lazy(() => import("./pages/Files")); //import("./components/pages/Home"));

const About = React.lazy(() => import("./pages/About"));
const Wizard = React.lazy(() => import("./pages/Wizard"));
const PortalSettings = React.lazy(() => import("./pages/PortalSettings"));

const Confirm = !IS_PERSONAL && React.lazy(() => import("./pages/Confirm"));
// const MyProfile = React.lazy(() => import("./pages/My"));
const PreparationPortal = React.lazy(() => import("./pages/PreparationPortal"));

const FormGallery = React.lazy(() => import("./pages/FormGallery"));

const PortalSettingsRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <PortalSettings {...props} />
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
const FilesRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <Files {...props} />
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

// const MyProfileRoute = (props) => (
//   <React.Suspense fallback={<AppLoader />}>
//     <ErrorBoundary>
//       <MyProfile {...props} />
//     </ErrorBoundary>
//   </React.Suspense>
// );

const FormGalleryRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <FormGallery {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

// const RedirectToHome = () => <Redirect to={PROXY_HOMEPAGE_URL} />;

const Shell = ({ items = [], page = "home", ...rest }) => {
  const {
    isLoaded,
    loadBaseInfo,

    isDesktop,
    language,
    FirebaseHelper,
    // personal,
    setCheckedMaintenance,
    socketHelper,
    setPreparationPortalDialogVisible,
    isBase,
    setTheme,
    setMaintenanceExist,
    roomsMode,
    setSnackbarExist,
    userTheme,
    //user,
  } = rest;

  useEffect(() => {
    try {
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

  const currentTheme = isBase ? "Base" : "Dark";
  const systemTheme = useThemeDetector();
  useEffect(() => {
    if (userTheme === "System" && currentTheme !== systemTheme)
      setTheme(systemTheme);
  }, [systemTheme]);

  return (
    <Layout>
      <Router history={history}>
        <Toast />
        <ReactSmartBanner t={t} ready={ready} />
        {isEditor || !isMobileOnly ? <></> : <NavMenu />}
        <IndicatorLoader />
        <ScrollToTop />
        <DialogsWrapper t={t} />
        <Main isDesktop={isDesktop}>
          <Switch>
            <PrivateRoute
              exact
              path={[
                "/",

                "/rooms/personal",
                "/rooms/personal/filter",

                "/rooms/shared",
                "/rooms/shared/filter",
                "/rooms/shared/:room",
                "/rooms/shared/:room/filter",

                "/rooms/archived",
                "/rooms/archived/filter",
                "/rooms/archived/:room",
                "/rooms/archived/:room/filter",

                "/files/favorite",
                "/files/favorite/filter",

                "/files/recent",
                "/files/recent/filter",

                "/files/trash",
                "/files/trash/filter",

                "/accounts",
                "/accounts/filter",

                "/accounts/create/:type",
                "/accounts/edit/:userId",
                "/accounts/view/:userId",
                "/accounts/view/@self",

                "/settings",
                "/settings/common",
                "/settings/admin",
                "/settings/connected-clouds",
              ]}
              component={FilesRoute}
            />
            <PrivateRoute
              path={"/form-gallery/:folderId"}
              component={FormGalleryRoute}
            />
            <PublicRoute exact path={"/wizard"} component={WizardRoute} />
            <PrivateRoute path={"/about"} component={AboutRoute} />
            <Route path={"/confirm"} component={ConfirmRoute} />
            <PrivateRoute path={"/payments"} component={PaymentsRoute} />
            <PrivateRoute
              restricted
              path={"/portal-settings"}
              component={PortalSettingsRoute}
            />
            <PrivateRoute
              path={"/preparation-portal"}
              component={PreparationPortalRoute}
            />
            <PrivateRoute path={"/error401"} component={Error401Route} />
            <PrivateRoute component={Error404Route} />
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
