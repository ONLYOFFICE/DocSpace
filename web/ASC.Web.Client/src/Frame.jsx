import React, { useEffect } from "react";
import { connect } from "react-redux";
import { Router, Switch } from "react-router-dom";
import NavMenu from "@appserver/common/src/components/NavMenu";
import Main from "@appserver/common/src/components/Main";
import Box from "@appserver/components/src/components/box";
import PrivateRoute from "@appserver/common/src/components/PrivateRoute";
import PublicRoute from "@appserver/common/src/components/PublicRoute";
import ErrorBoundary from "@appserver/common/src/components/ErrorBoundary";
import history from "@appserver/common/src/history";
import toastr from "@appserver/common/src/components/Toast/toastr";
import { AUTH_KEY } from "@appserver/common/src/constants";
import CommonStore from "@appserver/common/src/store";
const {
  setIsLoaded,
  getUser,
  getPortalSettings,
  getModules,
} = CommonStore.auth.actions;

const Home = React.lazy(() => import("studio/home"));
const Login = React.lazy(() => import("login/page"));
const People = React.lazy(() => import("people/page"));
const Files = React.lazy(() => import("files/page"));

const HomeRoute = () => (
  <React.Suspense fallback={null}>
    <ErrorBoundary>
      <Home />
    </ErrorBoundary>
  </React.Suspense>
);

const LoginRoute = () => (
  <React.Suspense fallback={null}>
    <ErrorBoundary>
      <Login />
    </ErrorBoundary>
  </React.Suspense>
);

const PeopleRoute = () => (
  <React.Suspense fallback={null}>
    <ErrorBoundary>
      <People />
    </ErrorBoundary>
  </React.Suspense>
);

const FilesRoute = () => (
  <React.Suspense fallback={null}>
    <ErrorBoundary>
      <Files />
    </ErrorBoundary>
  </React.Suspense>
);

const Frame = ({ items = [], page = "home", ...rest }) => {
  useEffect(() => {
    //utils.removeTempContent();

    const { getPortalSettings, getUser, getModules, setIsLoaded } = rest;

    const token = localStorage.getItem(AUTH_KEY);

    const requests = [];

    if (!token) {
      requests.push(getPortalSettings());
    } else if (!window.location.pathname.includes("confirm/EmailActivation")) {
      requests.push(getUser());
      requests.push(getPortalSettings());
      requests.push(getModules());
    }

    Promise.all(requests)
      .catch((e) => {
        toastr.error(e);
      })
      .finally(() => {
        setIsLoaded();
      });
  }, []);

  return (
    <Router history={history}>
      <Box>
        <NavMenu />
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
            <PublicRoute
              exact
              path={[
                "/login",
                "/login/error=:error",
                "/login/confirmed-email=:confirmedEmail",
              ]}
              component={LoginRoute}
            />
          </Switch>
        </Main>
      </Box>
    </Router>
  );
};

const mapStateToProps = (state) => {
  const { modules, isLoaded, settings } = state.auth;
  const { organizationName } = settings;
  return {
    modules,
    isLoaded,
    organizationName,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    getPortalSettings: () => getPortalSettings(dispatch),
    getUser: () => getUser(dispatch),
    getModules: () => getModules(dispatch),
    setIsLoaded: () => dispatch(setIsLoaded(true)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(Frame);
