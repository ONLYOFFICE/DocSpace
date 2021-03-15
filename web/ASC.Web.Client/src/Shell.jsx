import React, { useEffect } from "react";
import { Router, Switch /*, Route*/ } from "react-router-dom";
import { inject, observer } from "mobx-react";
import NavMenu from "./components/NavMenu";
import Main from "./components/Main";
import Box from "@appserver/components/box";
import PrivateRoute from "@appserver/common/components/PrivateRoute";
import PublicRoute from "@appserver/common/components/PublicRoute";
import ErrorBoundary from "@appserver/common/components/ErrorBoundary";
import Layout from "./components/Layout";
import ScrollToTop from "./components/Layout/ScrollToTop";
import history from "@appserver/common/history";
import toastr from "studio/toastr";
import { updateTempContent } from "@appserver/common/utils";
import { Provider as MobxProvider } from "mobx-react";
import ThemeProvider from "@appserver/components/theme-provider";
import { Base } from "@appserver/components/themes";
import store from "studio/store";
import config from "../package.json";
import "./custom.scss";
import { I18nextProvider } from "react-i18next";
import i18n from "./i18n";
import AppLoader from "@appserver/common/components/AppLoader";
import System from "./components/System";

const homepage = config.homepage;

const Payments = React.lazy(() => import("./components/pages/Payments"));
const Error404 = React.lazy(() => import("studio/Error404"));
const Error401 = React.lazy(() => import("studio/Error401"));
const Home = React.lazy(() => import("./components/pages/Home"));
const Login = React.lazy(() => import("login/app"));
const About = React.lazy(() => import("./components/pages/About"));
//const Wizard = React.lazy(() => import("./components/pages/Wizard"));
const Settings = React.lazy(() => import("./components/pages/Settings"));
const ComingSoon = React.lazy(() => import("./components/pages/ComingSoon"));

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

const LoginRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <Login {...props} />
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

// const WizardRoute = (props) => (
//   <React.Suspense fallback={<AppLoader />}>
//     <ErrorBoundary>
//       <Wizard {...props} />
//     </ErrorBoundary>
//   </React.Suspense>
// );

const ComingSoonRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <ComingSoon {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

// const DynamicAppRoute = ({ link, appName, ...rest }) => {
//   const system = {
//     url: `${window.location.origin}${link}remoteEntry.js`,
//     scope: appName,
//     module: "./app",
//   };
//   return (
//     <React.Suspense fallback={<AppLoader />}>
//       <ErrorBoundary>
//         <System system={system} {...rest} />
//       </ErrorBoundary>
//     </React.Suspense>
//   );
// };

const Shell = ({ items = [], page = "home", ...rest }) => {
  // useEffect(() => {
  //   //utils.removeTempContent();

  //   const {
  //     getPortalSettings,
  //     getUser,
  //     getModules,
  //     setIsLoaded,
  //     getIsAuthenticated,
  //   } = rest;

  //   getIsAuthenticated()
  //     .then((isAuthenticated) => {
  //       if (isAuthenticated) updateTempContent(isAuthenticated);

  //       const requests = [];
  //       if (!isAuthenticated) {
  //         requests.push(getPortalSettings());
  //       } else if (
  //         !window.location.pathname.includes("confirm/EmailActivation")
  //       ) {
  //         requests.push(getUser());
  //         requests.push(getPortalSettings());
  //         requests.push(getModules());
  //       }

  //       return Promise.all(requests).finally(() => {
  //         updateTempContent();
  //         setIsLoaded();
  //       });
  //     })
  //     .catch((err) => toastr.error(err.message));
  // }, []);

  const { isLoaded, loadBaseInfo, isThirdPartyResponse, modules } = rest;

  useEffect(() => {
    try {
      loadBaseInfo();
    } catch (err) {
      toastr.error(err);
    }
  }, []);

  useEffect(() => {
    if (isLoaded) updateTempContent();
  }, [isLoaded]);

  useEffect(() => {
    console.log("Current page ", page);
    // setModuleInfo(page, "e67be73d-f9ae-4ce1-8fec-1880cb518cb4");
  }, [page]);

  const pathname = window.location.pathname.toLowerCase();
  const isEditor = pathname.indexOf("doceditor") !== -1;

  const dynamicRoutes = modules
    .filter((m) => m.ready)
    .map((m) => {
      const system = {
        url: `${window.location.origin}${m.link}remoteEntry.js`,
        scope: m.appName,
        module: "./app",
      };
      return (
        <PrivateRoute
          key={m.id}
          path={`${homepage}${m.link}`}
          component={System}
          system={system}
        />
      );
    });

  return (
    <Layout>
      <Router history={history}>
        <Box>
          {isEditor ? <></> : <NavMenu />}
          <ScrollToTop />
          <Main>
            <Switch>
              <PrivateRoute
                exact
                path={[`${homepage}/`, `${homepage}/error=:error`]}
                component={HomeRoute}
              />
              {/* <Route
                exact
                path={`${homepage}/wizard`}
                component={WizardRoute}
              /> */}
              <PrivateRoute path={`${homepage}/about`} component={AboutRoute} />
              <PublicRoute
                exact
                path={[
                  `${homepage}/login`,
                  `${homepage}/login/error=:error`,
                  `${homepage}/login/confirmed-email=:confirmedEmail`,
                ]}
                component={LoginRoute}
              />
              <PrivateRoute
                path={[
                  `${homepage}/coming-soon`,
                  `${homepage}/products/mail`,
                  `${homepage}/products/projects`,
                  `${homepage}/products/crm`,
                  `${homepage}/products/calendar`,
                  `${homepage}/products/talk/`,
                ]}
                component={ComingSoonRoute}
              />
              <PrivateRoute
                path={`${homepage}/payments`}
                component={PaymentsRoute}
              />
              <PrivateRoute
                restricted
                path={`${homepage}/settings`}
                component={SettingsRoute}
              />
              {dynamicRoutes}
              <PrivateRoute
                path={`${homepage}/error401`}
                component={Error401Route}
              />
              <PrivateRoute component={Error404Route} />
            </Switch>
          </Main>
        </Box>
      </Router>
    </Layout>
  );
};

const ShellWrapper = inject(({ auth }) => {
  const { init, isLoaded } = auth;
  const pathname = window.location.pathname.toLowerCase();
  const isThirdPartyResponse = pathname.indexOf("thirdparty") !== -1;

  return {
    loadBaseInfo: () => {
      init();
      auth.settingsStore.setModuleInfo(config.homepage, "home");
      auth.setProductVersion(config.version);
    },
    isThirdPartyResponse,
    isLoaded,
    modules: auth.moduleStore.modules,
  };
})(observer(Shell));

export default () => (
  <ThemeProvider theme={Base}>
    <MobxProvider {...store}>
      <I18nextProvider i18n={i18n}>
        <ShellWrapper />
      </I18nextProvider>
    </MobxProvider>
  </ThemeProvider>
);
