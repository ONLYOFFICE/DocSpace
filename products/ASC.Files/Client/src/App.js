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
  regDesktop,
} from "asc-web-common";

const {
  setIsLoaded,
  getUser,
  getPortalSettings,
  getModules,
  setCurrentProductId,
  setCurrentProductHomePage,
  getPortalCultures,
  setEncryptionKeys,
  getEncryptionSupport,
} = commonStore.auth.actions;
const {
  getCurrentUser,
  isEncryptionSupport,
  isDesktopClient,
  getIsLoaded,
} = commonStore.auth.selectors;
const { AUTH_KEY } = constants;

class App extends React.Component {
  constructor(props) {
    super(props);

    this.isEditor = window.location.pathname.indexOf("doceditor") !== -1;
    this.isDesktopInit = false;
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
      getEncryptionSupport,
      isDesktop,
    } = this.props;

    setModuleInfo();

    const token = localStorage.getItem(AUTH_KEY);

    if (!token) {
      return setIsLoaded();
    }

    const requests = [getUser()];
    if (!this.isEditor) {
      requests.push(
        getPortalSettings(),
        getModules(),
        getPortalCultures(),
        fetchTreeFolders()
      );
      if (isDesktop) {
        requests.push(getEncryptionSupport());
      }
    }

    Promise.all(requests)
      .then(() => {
        if (this.isEditor) return Promise.resolve();
      })
      .catch((e) => {
        toastr.error(e);
      })
      .finally(() => {
        setIsLoaded();
      });
  }

  componentDidUpdate(prevProps) {
    const {
      isAuthenticated,
      user,
      isEncryption,
      keys,
      setEncryptionKeys,
      isLoaded,
    } = this.props;
    console.log("componentDidUpdate: ", this.props);
    if (isAuthenticated && !this.isDesktopInit && isEncryption && isLoaded) {
      this.isDesktopInit = true;
      regDesktop(user, isEncryption, keys, setEncryptionKeys);
      console.log("%c%s", "font: 1.1em/1 bold;", "Current keys is: ", keys);
    }
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
    user: getCurrentUser(state),
    isAuthenticated: state.auth.isAuthenticated,
    isLoaded: getIsLoaded(state),
    isEncryption: isEncryptionSupport(state),
    isDesktop: isDesktopClient(state),
    keys: settings.encryptionKeys,
    settings: settings,
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
    getEncryptionSupport: () => getEncryptionSupport(dispatch),
    setEncryptionKeys: (keys) => dispatch(setEncryptionKeys(keys)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(App);
