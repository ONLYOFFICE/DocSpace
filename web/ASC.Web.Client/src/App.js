import React, { Suspense, lazy } from "react";
import { Router, Route, Switch } from "react-router-dom";
import { connect } from "react-redux";
// import {
//   Login,
//   Error404,
//   Offline,
//   ComingSoon
// } from "@appserver/common";
import CommonStore from "@appserver/common/src/store";
import constants from "@appserver/common/src/constants";
import history from "@appserver/common/src/history";
import PrivateRoute from "@appserver/common/src/components/PrivateRoute";
import PublicRoute from "@appserver/common/src/components/PublicRoute";
import NavMenu from "@appserver/common/src/components/NavMenu";
import Main from "@appserver/common/src/components/Main";
import toastr from "@appserver/common/src/components/Toast/toastr";

import Home from "./components/pages/Home";

const About = lazy(() => import("./components/pages/About"));
const Confirm = lazy(() => import("./components/pages/Confirm"));
const Settings = lazy(() => import("./components/pages/Settings"));
const Wizard = lazy(() => import("./components/pages/Wizard"));
const Payments = lazy(() => import("./components/pages/Payments"));
const {
  setIsLoaded,
  getUser,
  getPortalSettings,
  getModules,
} = CommonStore.auth.actions;

class App extends React.Component {
  componentDidMount() {
    utils.removeTempContent();

    const { getPortalSettings, getUser, getModules, setIsLoaded } = this.props;

    const { AUTH_KEY } = constants;

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
  }

  render() {
    return navigator.onLine ? (
      <Router history={history}>
        <NavMenu />
        <Main>
          <Suspense fallback={null}>
            <Switch>
              <Route exact path="/wizard" component={Wizard} />
              <PublicRoute
                exact
                path={[
                  "/login",
                  "/login/error=:error",
                  "/login/confirmed-email=:confirmedEmail",
                ]}
                component={Login}
              />
              <Route path="/confirm" component={Confirm} />
              <PrivateRoute
                exact
                path={["/", "/error=:error"]}
                component={Home}
              />
              <PrivateRoute exact path="/about" component={About} />
              <PrivateRoute restricted path="/settings" component={Settings} />
              <PrivateRoute
                exact
                path={["/coming-soon"]}
                component={ComingSoon}
              />
              <PrivateRoute path="/payments" component={Payments} />
              <PrivateRoute component={Error404} />
            </Switch>
          </Suspense>
        </Main>
      </Router>
    ) : (
      <Offline />
    );
  }
}

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

export default connect(mapStateToProps, mapDispatchToProps)(App);
