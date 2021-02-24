import React, { Suspense } from "react";
import { Router, Switch, Redirect, Route } from "react-router-dom";
import Home from "./components/pages/Home";
import DocEditor from "./components/pages/DocEditor";
import Settings from "./components/pages/Settings";
import VersionHistory from "./components/pages/VersionHistory";
import config from "../package.json";
import "./i18n";

import {
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
  Layout,
  ScrollToTop,
  regDesktop,
} from "asc-web-common";
import { inject, observer } from "mobx-react";

class App extends React.Component {
  constructor(props) {
    super(props);

    const pathname = window.location.pathname.toLowerCase();
    this.isEditor = pathname.indexOf("doceditor") !== -1;
    this.isDesktopInit = false;
  }

  componentDidMount() {
    this.props
      .loadFilesInfo()
      .catch((err) => toastr.error(err))
      .finally(() => {
        this.props.setIsLoaded(true);
        utils.updateTempContent();
      });
  }

  /*
  componentDidMount() {
    const {
      setModuleInfo,
      getUser,
      getPortalSettings,
      getModules,
      getPortalCultures,
      fetchTreeFolders,
      setIsLoaded,
      getIsEncryptionSupport,
      getEncryptionKeys,
      isDesktop,
      getIsAuthenticated,
      setProductVersion,
    } = this.props;

    setModuleInfo();
    setProductVersion();

    if (this.isEditor) {
      setIsLoaded();
      return;
    }

    getIsAuthenticated().then((isAuthenticated) => {
      if (!isAuthenticated) {
        utils.updateTempContent();
        return setIsLoaded();
      } else {
        utils.updateTempContent(isAuthenticated);
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
          requests.push(getIsEncryptionSupport(), getEncryptionKeys());
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
          utils.updateTempContent();
          setIsLoaded();
        });
    });
  }
*/
  componentDidUpdate(prevProps) {
    const {
      isAuthenticated,
      user,
      isEncryption,
      encryptionKeys,
      setEncryptionKeys,
      isLoaded,
    } = this.props;
    //console.log("componentDidUpdate: ", this.props);
    if (isAuthenticated && !this.isDesktopInit && isEncryption && isLoaded) {
      this.isDesktopInit = true;
      regDesktop(
        user,
        isEncryption,
        encryptionKeys,
        setEncryptionKeys,
        this.isEditor
      );
      console.log(
        "%c%s",
        "color: green; font: 1.2em bold;",
        "Current keys is: ",
        encryptionKeys
      );
    }
  }

  render() {
    const { homepage, isDesktop } = this.props;

    return navigator.onLine ? (
      <Layout>
        <Router history={history}>
          <ScrollToTop />
          {!this.isEditor && <NavMenu />}
          <Main isDesktop={isDesktop}>
            <Suspense fallback={null}>
              <Switch>
                <Redirect exact from="/" to={`${homepage}`} />
                <PrivateRoute
                  exact
                  path={`${homepage}/settings/:setting`}
                  component={Settings}
                />
                <Route
                  exact
                  path={[
                    `${homepage}/doceditor`,
                    `/Products/Files/DocEditor.aspx`,
                  ]}
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
                <PrivateRoute
                  exact
                  path={`/error=:error`}
                  component={Error520}
                />
                <PrivateRoute component={Error404} />
              </Switch>
            </Suspense>
          </Main>
        </Router>
      </Layout>
    ) : (
      <Offline />
    );
  }
}

export default inject(({ auth, initFilesStore }) => ({
  isDesktop: auth.settingsStore.isDesktopClient,
  user: auth.userStore.user,
  isAuthenticated: auth.isAuthenticated,
  homepage: auth.settingsStore.homepage || config.homepage,
  encryptionKeys: auth.settingsStore.encryptionKeys,
  isEncryption: auth.settingsStore.isEncryptionSupport,
  isLoaded: initFilesStore.isLoaded,
  setIsLoaded: initFilesStore.setIsLoaded,
  setEncryptionKeys: auth.settingsStore.setEncryptionKeys,
  loadFilesInfo: async () => {
    await auth.init();
    await initFilesStore.initFiles();
    auth.setProductVersion(config.version);
  },
}))(observer(App));
