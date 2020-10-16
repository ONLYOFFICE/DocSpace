import React, { Suspense } from "react";
import { connect } from "react-redux";
import { Router, Switch, Redirect } from "react-router-dom";
import Home from "./components/pages/Home";
import DocEditor from "./components/pages/DocEditor";
import Settings from "./components/pages/Settings";
import VersionHistory from "./components/pages/VersionHistory";
import { fetchTreeFolders } from "./store/files/actions";
import config from "../package.json";

import {
  store as commonStore,
  constants,
  history,
  PrivateRoute,
  PublicRoute,
  Login,
  Error404,
  Error520,
  Offline,
  NavMenu,
  Main,
  utils,
  toastr,
} from "asc-web-common";

const {
  setIsLoaded,
  getUser,
  getPortalSettings,
  getModules,
  setCurrentProductId,
  setCurrentProductHomePage,
  getPortalCultures,
} = commonStore.auth.actions;
const { AUTH_KEY } = constants;

class App extends React.Component {
  constructor(props) {
    super(props);

    this.isEditor = window.location.pathname.indexOf("doceditor") !== -1;
  }

  componentDidMount() {
    utils.removeTempContent();

    const {
      setModuleInfo,
      getUser,
      getPortalSettings,
      getModules,
      getPortalCultures,
      fetchTreeFolders,
      setIsLoaded,
    } = this.props;

    setModuleInfo();

    const token = localStorage.getItem(AUTH_KEY);

    if (!token) {
      return setIsLoaded();
    }

    const requests = this.isEditor
      ? [getUser()]
      : [
          getUser(),
          getPortalSettings(),
          getModules(),
          getPortalCultures(),
          fetchTreeFolders(),
        ];

    Promise.all(requests)
      .catch((e) => {
        toastr.error(e);
      })
      .finally(() => {
        setIsLoaded();
      });
  }

  render() {
    const { homepage } = this.props;

    return navigator.onLine ? (
      <Router history={history}>
        {!this.isEditor && <NavMenu />}
        <Main>
          <Suspense fallback={null}>
            <Switch>
              <Redirect exact from="/" to={`${homepage}`} />
              <PrivateRoute
                exact
                path={`${homepage}/settings/:setting`}
                component={Settings}
              />
              <PrivateRoute
                exact
                path={`${homepage}/doceditor`}
                component={DocEditor}
              />
              <PrivateRoute
                exact
                path={`${homepage}/:fileId/history`}
                component={VersionHistory}
              />
              <PrivateRoute exact path={homepage} component={Home} />
              <PrivateRoute path={`${homepage}/filter`} component={Home} />
              <PublicRoute
                exact
                path={[
                  "/login",
                  "/login/error=:error",
                  "/login/confirmed-email=:confirmedEmail",
                ]}
                component={Login}
              />
              <PrivateRoute exact path={`/error=:error`} component={Error520} />
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
  const { settings } = state.auth;
  const { homepage } = settings;
  return {
    homepage: homepage || config.homepage,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    setModuleInfo: () => {
      dispatch(setCurrentProductHomePage(config.homepage));
      dispatch(setCurrentProductId("e67be73d-f9ae-4ce1-8fec-1880cb518cb4"));
    },
    getUser: () => getUser(dispatch),
    getPortalSettings: () => getPortalSettings(dispatch),
    getModules: () => getModules(dispatch),
    getPortalCultures: () => getPortalCultures(dispatch),
    fetchTreeFolders: () => fetchTreeFolders(dispatch),
    setIsLoaded: () => dispatch(setIsLoaded(true)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(App);
