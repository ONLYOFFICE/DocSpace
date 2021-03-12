import React, { Suspense, lazy } from "react";
import { Router, Route, Switch } from "react-router-dom";
import { connect } from "react-redux";
import {
  store as CommonStore,
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
  toastr,
} from "asc-web-common";
import Home from "./components/pages/Home";
import config from "../package.json";

const About = lazy(() => import("./components/pages/About"));
const Confirm = lazy(() => import("./components/pages/Confirm"));
const Settings = lazy(() => import("./components/pages/Settings"));
const Wizard = lazy(() => import("./components/pages/Wizard"));
const Payments = lazy(() => import("./components/pages/Payments"));
const ThirdPartyResponse = lazy(() => import("./components/pages/ThirdParty"));
const {
  setIsLoaded,
  getUser,
  getPortalSettings,
  getModules,
  getIsAuthenticated,
  setCurrentProductHomePage,
} = CommonStore.auth.actions;

class App extends React.Component {
  constructor(props) {
    super(props);

    const pathname = window.location.pathname.toLowerCase();
    this.isThirdPartyResponse = pathname.indexOf("thirdparty") !== -1;
  }
  componentDidMount() {
    const {
      setModuleInfo,
      getPortalSettings,
      getUser,
      getModules,
      setIsLoaded,
      getIsAuthenticated,
    } = this.props;

    setModuleInfo();

    getIsAuthenticated()
      .then((isAuthenticated) => {
        if (isAuthenticated) utils.updateTempContent(isAuthenticated);

        if (this.isThirdPartyResponse) {
          setIsLoaded();
          return;
        }

        const requests = [];
        if (!isAuthenticated) {
          requests.push(getPortalSettings());
        } else if (
          !window.location.pathname.includes("confirm/EmailActivation")
        ) {
          requests.push(getUser());
          requests.push(getPortalSettings());
          requests.push(getModules());
        }

        Promise.all(requests)
          .catch((e) => {
            toastr.error(e);
          })
          .finally(() => {
            utils.updateTempContent();
            setIsLoaded();
          });
      })
      .catch((err) => toastr.error(err));
  }

  render() {
    const { homepage } = this.props;
    console.log("Client App render", this.props);
    return navigator.onLine ? (
      <Router history={history}>
        {!this.isThirdPartyResponse && <NavMenu />}
        <Main>
          <Suspense fallback={null}>
            <Switch>
              <Route exact path={`${homepage}/wizard`} component={Wizard} />
              <PublicRoute
                exact
                path={[
                  `${homepage}/login`,
                  `${homepage}/login/error=:error`,
                  `${homepage}/login/confirmed-email=:confirmedEmail`,
                ]}
                component={Login}
              />
              <Route path={`${homepage}/confirm`} component={Confirm} />
              <PrivateRoute
                path={`${homepage}/thirdparty/:provider`}
                component={ThirdPartyResponse}
              />
              <PrivateRoute
                exact
                path={[`${homepage}/`, `${homepage}/error=:error`]}
                component={Home}
              />
              <PrivateRoute
                exact
                path={`${homepage}/about`}
                component={About}
              />
              <PrivateRoute
                restricted
                path={`${homepage}/settings`}
                component={Settings}
              />
              <PrivateRoute
                exact
                path={[`${homepage}/coming-soon`]}
                component={ComingSoon}
              />
              <PrivateRoute
                path={`${homepage}/payments`}
                component={Payments}
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
  const { organizationName, homepage } = settings;
  return {
    modules,
    isLoaded,
    organizationName,
    homepage: homepage || config.homepage,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    getIsAuthenticated: () => getIsAuthenticated(dispatch),
    setModuleInfo: () => dispatch(setCurrentProductHomePage(config.homepage)),
    getPortalSettings: () => getPortalSettings(dispatch),
    getUser: () => getUser(dispatch),
    getModules: () => getModules(dispatch),
    setIsLoaded: () => dispatch(setIsLoaded(true)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(App);
