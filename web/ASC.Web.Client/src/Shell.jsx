import React, { useEffect } from "react";
import styled from "styled-components";
import { Router, Switch } from "react-router-dom";
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
import Loader from "@appserver/components/loader";
import Grid from "@appserver/components/grid";
import PageLayout from "@appserver/common/components/PageLayout";
import { updateTempContent } from "@appserver/common/utils";
import { Provider as MobxProvider } from "mobx-react";
import ThemeProvider from "@appserver/components/theme-provider";
import { Base } from "@appserver/components/themes";
import store from "studio/store";
import config from "../package.json";
import "./custom.scss";
import "./i18n";

const Payments = React.lazy(() => import("./components/pages/Payments"));
const Error404 = React.lazy(() => import("studio/Error404"));
const Error401 = React.lazy(() => import("studio/Error401"));
const Home = React.lazy(() => import("./components/pages/Home"));
const Login = React.lazy(() => import("login/app"));
const People = React.lazy(() => import("people/app"));
const Files = React.lazy(() => import("files/app"));
const About = React.lazy(() => import("./components/pages/About"));
const Settings = React.lazy(() => import("./components/pages/Settings"));
const ComingSoon = React.lazy(() => import("./components/pages/ComingSoon"));

const LoadingShell = () => (
  <PageLayout>
    <PageLayout.SectionBody>
      <Box displayProp="flex" alignItems="center" justifyContent="center">
        <Loader type="rombs" size="40px" />
      </Box>
    </PageLayout.SectionBody>
  </PageLayout>
);

const SettingsRoute = (props) => (
  <React.Suspense fallback={<LoadingShell />}>
    <ErrorBoundary>
      <Settings {...props} />
    </ErrorBoundary>
  </React.Suspense>
);
const PaymentsRoute = (props) => (
  <React.Suspense fallback={<LoadingShell />}>
    <ErrorBoundary>
      <Payments {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const Error404Route = (props) => (
  <React.Suspense fallback={<LoadingShell />}>
    <ErrorBoundary>
      <Error404 {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const Error401Route = (props) => (
  <React.Suspense fallback={<LoadingShell />}>
    <ErrorBoundary>
      <Error401 {...props} />
    </ErrorBoundary>
  </React.Suspense>
);
const HomeRoute = (props) => (
  <React.Suspense fallback={<LoadingShell />}>
    <ErrorBoundary>
      <Home {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const LoginRoute = (props) => (
  <React.Suspense fallback={<LoadingShell />}>
    <ErrorBoundary>
      <Login {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const PeopleRoute = (props) => {
  return (
    <React.Suspense fallback={<LoadingShell />}>
      <ErrorBoundary>
        <People {...props} />
      </ErrorBoundary>
    </React.Suspense>
  );
};

const FilesRoute = (props) => {
  return (
    <React.Suspense fallback={<LoadingShell />}>
      <ErrorBoundary>
        <Files {...props} />
      </ErrorBoundary>
    </React.Suspense>
  );
};

const AboutRoute = (props) => (
  <React.Suspense fallback={<LoadingShell />}>
    <ErrorBoundary>
      <About {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const ComingSoonRoute = (props) => (
  <React.Suspense fallback={<LoadingShell />}>
    <ErrorBoundary>
      <ComingSoon {...props} />
    </ErrorBoundary>
  </React.Suspense>
);
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

  const { isLoaded, loadBaseInfo, isThirdPartyResponse } = rest;

  useEffect(() => {
    try {
      loadBaseInfo();
    } catch (err) {
      toastr.error(err);
    }
  }, [loadBaseInfo]);

  useEffect(() => {
    if (isLoaded) updateTempContent();
  }, [isLoaded]);

  useEffect(() => {
    console.log("Current page ", page);
    // setModuleInfo(page, "e67be73d-f9ae-4ce1-8fec-1880cb518cb4");
  }, [page]);

  return (
    <Layout>
      <Router history={history}>
        <Box>
          <NavMenu />
          <ScrollToTop />
          <Main>
            <Switch>
              <PrivateRoute
                exact
                path={["/", "/error=:error"]}
                component={HomeRoute}
              />
              <PrivateRoute
                path={["/products/people", "/products/people/filter"]}
                component={PeopleRoute}
              />
              <PrivateRoute
                path={["/products/files", "/products/files/filter"]}
                component={FilesRoute}
              />
              <PrivateRoute path={["/about"]} component={AboutRoute} />
              <PublicRoute
                exact
                path={[
                  "/login",
                  "/login/error=:error",
                  "/login/confirmed-email=:confirmedEmail",
                ]}
                component={LoginRoute}
              />
              <PrivateRoute
                exact
                path={[
                  "/coming-soon",
                  "/products/mail",
                  "/products/projects",
                  "/products/crm",
                  "/products/calendar",
                  "/products/talk/",
                ]}
                component={ComingSoonRoute}
              />
              <PrivateRoute path="/payments" component={PaymentsRoute} />
              <PrivateRoute
                restricted
                path="/settings"
                component={SettingsRoute}
              />
              <PrivateRoute path="/error401" component={Error401Route} />
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
      auth.setProductVersion(config.version);
    },
    isThirdPartyResponse,
    isLoaded,
  };
})(observer(Shell));

export default () => (
  <ThemeProvider theme={Base}>
    <MobxProvider {...store}>
      <ShellWrapper />
    </MobxProvider>
  </ThemeProvider>
);
