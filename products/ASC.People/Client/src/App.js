import React, { Suspense } from "react";
import { connect } from "react-redux";
import { Router, Switch, Redirect } from "react-router-dom";
import axios from "axios";
import { Loader } from "asc-web-components";
import Home from "./components/pages/Home";
import Profile from "./components/pages/Profile";
import ProfileAction from "./components/pages/ProfileAction";
import GroupAction from "./components/pages/GroupAction";
import Reassign from "./components/pages/Reassign";
import {
  history,
  PrivateRoute,
  PublicRoute,
  Login,
  Error404,
  Error520,
  Offline,
  utils,
  store as commonStore,
  constants,
  NavMenu,
  Main,
} from "asc-web-common";
import { getFilterByLocation } from "./helpers/converters";
import { fetchGroups, fetchPeople } from "./store/people/actions";
import config from "../package.json";

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

/*const Profile = lazy(() => import("./components/pages/Profile"));
const ProfileAction = lazy(() => import("./components/pages/ProfileAction"));
const GroupAction = lazy(() => import("./components/pages/GroupAction"));*/

class App extends React.Component {
  componentDidMount() {
    utils.removeTempContent();

    const {
      getUser,
      getPortalSettings,
      getModules,
      getPortalPasswordSettings,
      getPortalCultures,
      fetchGroups,
      fetchPeople,
      finalize,
      setIsLoaded,
    } = this.props;

    const token = localStorage.getItem(AUTH_KEY);

    if (!token) {
      utils.hideLoader();
      return setIsLoaded();
    }

    const requests = [
      getUser(),
      getPortalSettings(),
      getModules(),
      getPortalPasswordSettings(),
      getPortalCultures(),
      fetchGroups(),
      fetchPeople(),
    ];

    axios.all(requests).then(() => {
      utils.hideLoader();
      finalize();
    });
  }

  render() {
    const { homepage } = this.props.settings;
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
              <Redirect exact from="/" to={`${homepage}`} />
              <PrivateRoute path={`${homepage}/view/:userId`}>
                <Profile />
              </PrivateRoute>
              <PrivateRoute
                path={`${homepage}/edit/:userId`}
                restricted
                allowForMe
              >
                <ProfileAction />
              </PrivateRoute>
              <PrivateRoute path={`${homepage}/create/:type`} restricted>
                <ProfileAction />
              </PrivateRoute>
              <PrivateRoute
                path={[
                  `${homepage}/group/edit/:groupId`,
                  `${homepage}/group/create`,
                ]}
                restricted
              >
                <GroupAction />
              </PrivateRoute>
              <PrivateRoute path={`${homepage}/reassign/:userId`} restricted>
                <Reassign />
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
          </Suspense>
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
    fetchGroups: () => fetchGroups(dispatch),
    fetchPeople: () => {
      var re = new RegExp(`${config.homepage}((/?)$|/filter)`, "gm");
      const match = window.location.pathname.match(re);

      if (match && match.length > 0) {
        const newFilter = getFilterByLocation(window.location);
        return fetchPeople(newFilter, dispatch);
      }

      return Promise.resolve();
    },
    finalize: () => {
      dispatch(setCurrentProductHomePage(config.homepage));
      dispatch(setCurrentProductId("f4d98afd-d336-4332-8778-3c6945c81ea0"));
      dispatch(setIsLoaded(true));
    },
    setIsLoaded: () => dispatch(setIsLoaded(true)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(App);
