import React, { useEffect, Suspense } from "react";
//import { Box, Text } from "@appserver/components/src";
// import { useStore } from "react-redux";
// import dynamic from "@redux-dynostore/react-redux";
// import { attachReducer } from "@redux-dynostore/core";
//import rootReducer from "./store/rootReducer";
//import portalReducer from "./store/portal/reducers";
import { inject, observer } from "mobx-react";

import { Router, Switch, Redirect, Route } from "react-router-dom";
import Home from "./components/pages/Home";
import DocEditor from "./components/pages/DocEditor";
import Settings from "./components/pages/Settings";
import VersionHistory from "./components/pages/VersionHistory";
import config from "../package.json";
import "./i18n";

import Layout from "@appserver/common/src/components/Layout";
import history from "@appserver/common/src/history";
import PrivateRoute from "@appserver/common/src/components/PrivateRoute";
import PublicRoute from "@appserver/common/src/components/PublicRoute";
import NavMenu from "@appserver/common/src/components/NavMenu";
import Main from "@appserver/common/src/components/Main";
import ScrollToTop from "@appserver/common/src/components/Layout/ScrollToTop";
import toastr from "@appserver/common/src/components/Toast/toastr";
import { updateTempContent } from "@appserver/common/src/utils";
//import { regDesktop } from "@appserver/common/src/desktop";

const Error520 = React.lazy(() =>
  import("@appserver/common/src/pages/errors/520")
);
const Error404 = React.lazy(() =>
  import("@appserver/common/src/pages/errors/404")
);
const Offline = React.lazy(() =>
  import("@appserver/common/src/pages/errors/offline")
);
//const Login = React.lazy(() => import("login/page"));

class FilesContent extends React.Component {
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
        updateTempContent();
      });
  }

  //   componentDidUpdate(prevProps) {
  //     const {
  //       isAuthenticated,
  //       user,
  //       isEncryption,
  //       encryptionKeys,
  //       setEncryptionKeys,
  //       isLoaded,
  //     } = this.props;
  //     //console.log("componentDidUpdate: ", this.props);
  //     if (isAuthenticated && !this.isDesktopInit && isEncryption && isLoaded) {
  //       this.isDesktopInit = true;
  //       regDesktop(
  //         user,
  //         isEncryption,
  //         encryptionKeys,
  //         setEncryptionKeys,
  //         this.isEditor
  //       );
  //       console.log(
  //         "%c%s",
  //         "color: green; font: 1.2em bold;",
  //         "Current keys is: ",
  //         encryptionKeys
  //       );
  //     }
  //   }

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
                {/* <PublicRoute
                  exact
                  path={[
                    "/login",
                    "/login/error=:error",
                    "/login/confirmed-email=:confirmedEmail",
                  ]}
                  component={Login}
                /> */}
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

export default inject(({ auth, initFilesStore }) => {
  const homepage = "/products/files"; //TODO: add homepage to config?
  return {
    isDesktop: auth.settingsStore.isDesktopClient,
    user: auth.userStore.user,
    isAuthenticated: auth.isAuthenticated,
    homepage: auth.settingsStore.homepage || homepage,
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
  };
})(observer(FilesContent));
