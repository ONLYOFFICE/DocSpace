import React from "react";
import { connect } from "react-redux";
import axios from "axios";
import { Router, Switch, Redirect } from "react-router-dom";
import { Loader } from "asc-web-components";
import Home from "./components/pages/Home";
import DocEditor from "./components/pages/DocEditor";
import Settings from "./components/pages/Settings";
import VersionHistory from "./components/pages/VersionHistory";
import {
  fetchMyFolder,
  fetchTreeFolders,
  fetchFiles,
} from "./store/files/actions";
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
  //PageLayout,
} from "asc-web-common";

const {
  setIsLoaded,
  getUser,
  getPortalSettings,
  getModules,
  setCurrentProductId,
  setCurrentProductHomePage,
  getPortalPasswordSettings,
  getPortalCultures,
} = commonStore.auth.actions;
const { AUTH_KEY } = constants;

// const withStudioLayout = Component => props => (
//   <StudioLayout>
//     <Component {...props} />
//   </StudioLayout>
// );

// const LoaderPage = props => {
//   console.log("LoaderPage render", props);
//   return (
//     <PageLayout>
//       <PageLayout.SectionBody>
//         <Loader className="pageLoader" type="rombs" size="40px" />
//       </PageLayout.SectionBody>
//     </PageLayout>
//   );
// };

class App extends React.Component {
  removeLoader = () => {
    const ele = document.getElementById("ipl-progress-indicator");
    if (ele) {
      // fade out
      ele.classList.add("available");
      // setTimeout(() => {
      //   // remove from DOM
      //   ele.outerHTML = "";
      // }, 2000);
    }
  };

  componentDidMount() {
    const {
      getUser,
      getPortalSettings,
      getModules,
      getPortalPasswordSettings,
      getPortalCultures,
      fetchMyFolder,
      fetchTreeFolders,
      //fetchFiles,
      finalize,
      setIsLoaded,
    } = this.props;

    const token = localStorage.getItem(AUTH_KEY);

    if (!token) {
      this.removeLoader();
      return setIsLoaded();
    }

    const requests = [
      getUser(),
      getPortalSettings(),
      getModules(),
      getPortalPasswordSettings(),
      getPortalCultures(),
      fetchMyFolder(),
      fetchTreeFolders(),
    ];

    axios.all(requests).then(() => {
      this.removeLoader();
      finalize();
    });
  }

  render() {
    const { homepage } = this.props.settings;

    return navigator.onLine ? (
      <Router history={history}>
        <NavMenu />
        <Main>
          <Switch>
            <PrivateRoute exact path="/">
              <Redirect exact from="/" to={`${homepage}`} />
            </PrivateRoute>
            <PrivateRoute exact path={`${homepage}/settings/:setting`}>
              <Settings />
            </PrivateRoute>
            <PrivateRoute exact path={`${homepage}/doceditor`}>
              <DocEditor />
            </PrivateRoute>
            <PrivateRoute exact path={`${homepage}/:fileId/history`}>
              <VersionHistory />
            </PrivateRoute>
            <PrivateRoute path={homepage}>
              <Home />
            </PrivateRoute>
            <PublicRoute
              exact
              path={[
                "/login",
                "/login/error=:error",
                "/login/confirmed-email=:confirmedEmail",
              ]}
            >
              <Login />
            </PublicRoute>
            <PrivateRoute exact path={`/error=:error`}>
              <Error520 />
            </PrivateRoute>
            <PrivateRoute>
              <Error404 />
            </PrivateRoute>
          </Switch>
        </Main>
      </Router>
    ) : (
      <Offline />
    );
  }
}

const mapStateToProps = (state) => {
  return {
    settings: state.auth.settings,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    getUser: () => getUser(dispatch),
    getPortalSettings: () => getPortalSettings(dispatch),
    getModules: () => getModules(dispatch),
    getPortalPasswordSettings: () => getPortalPasswordSettings(dispatch),
    getPortalCultures: () => getPortalCultures(dispatch),
    fetchMyFolder: () => fetchMyFolder(dispatch),
    fetchTreeFolders: () => fetchTreeFolders(dispatch),
    finalize: () => {
      dispatch(setCurrentProductHomePage(config.homepage));
      dispatch(setCurrentProductId("e67be73d-f9ae-4ce1-8fec-1880cb518cb4"));
      dispatch(setIsLoaded(true));
    },
    setIsLoaded: () => dispatch(setIsLoaded(true)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(App);
