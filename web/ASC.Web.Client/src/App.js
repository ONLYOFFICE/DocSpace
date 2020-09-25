import React, { Suspense, lazy } from "react";
import { Router, Route, Switch } from "react-router-dom";
import axios from "axios";
import { connect } from "react-redux";
import { Loader } from "asc-web-components";
import {
  store as CommonStore,
  constants,
  history,
  PrivateRoute,
  PublicRoute,
  Login,
  Error404,
  Offline,
  ComingSoon,
  NavMenu,
  Main,
  utils,
} from "asc-web-common";
import Home from "./components/pages/Home";

const About = lazy(() => import("./components/pages/About"));
const Confirm = lazy(() => import("./components/pages/Confirm"));
const Settings = lazy(() => import("./components/pages/Settings"));
const Wizard = lazy(() => import("./components/pages/Wizard"));

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

    axios
      .all(requests)
      .catch((e) => {
        console.log("INIT REQUESTS FAILED", e);
      })
      .finally(() => {
        utils.hideLoader();
        setIsLoaded(true);
      });
  }

  render() {
    return navigator.onLine ? (
      <Router history={history}>
        <NavMenu />
        <Main>
          <Suspense
            fallback={
              <Loader className="pageLoader" type="rombs" size="40px" />
            }
          >
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
