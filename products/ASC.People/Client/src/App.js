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
  StudioLayout,
  Offline
} from "asc-web-common";
import { store as commonStore, constants } from "asc-web-common";
import { getFilterByLocation } from "./helpers/converters";
import { getPortalInviteLinks } from "./store/portal/actions";
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
  getPortalCultures
} = commonStore.auth.actions;
const { AUTH_KEY } = constants;

/*const Profile = lazy(() => import("./components/pages/Profile"));
const ProfileAction = lazy(() => import("./components/pages/ProfileAction"));
const GroupAction = lazy(() => import("./components/pages/GroupAction"));*/

class App extends React.Component {
  removeLoader = () => {
    const ele = document.getElementById("ipl-progress-indicator");
    if (ele) {
      // fade out
      ele.classList.add("available");
      setTimeout(() => {
        // remove from DOM
        ele.outerHTML = "";
      }, 2000);
    }
  };
  componentDidMount() {
    const {
      getUser,
      getPortalSettings,
      getModules,
      getPortalPasswordSettings,
      getPortalCultures,
      fetchGroups,
      fetchPeople,
      finalize,
      setIsLoaded
    } = this.props;

    const token = localStorage.getItem(AUTH_KEY);

    if (token) {
      const requests = [
        getUser(),
        getPortalSettings(),
        getModules(),
        getPortalPasswordSettings(),
        getPortalCultures(),
        fetchGroups(),
        fetchPeople()
      ];

      axios.all(requests).then(() => {
        this.removeLoader();
        finalize();
      });
    } else {
      this.removeLoader();
      setIsLoaded();
    }
  }

  render() {
    const { homepage } = this.props.settings;
    return navigator.onLine ? (
      <Router history={history}>
        <StudioLayout>
          <Suspense
            fallback={
              <Loader className="pageLoader" type="rombs" size="40px" />
            }
          >
            <Switch>
              <Redirect exact from="/" to={`${homepage}`} />
              <PrivateRoute
                exact
                path={[homepage, `${homepage}/filter`]}
                component={Home}
              />
              <PrivateRoute
                path={`${homepage}/view/:userId`}
                component={Profile}
              />
              <PrivateRoute
                path={`${homepage}/edit/:userId`}
                component={ProfileAction}
                restricted
                allowForMe
              />
              <PrivateRoute
                path={`${homepage}/create/:type`}
                component={ProfileAction}
                restricted
              />
              <PrivateRoute
                path={`${homepage}/group/edit/:groupId`}
                component={GroupAction}
                restricted
              />
              <PrivateRoute
                path={`${homepage}/group/create`}
                component={GroupAction}
                restricted
              />
              <PrivateRoute
                path={`${homepage}/reassign/:userId`}
                component={Reassign}
                restricted
              />
              <PublicRoute
                exact
                path={[
                  "/login",
                  "/login/error=:error",
                  "/login/confirmed-email=:confirmedEmail"
                ]}
                component={Login}
              />
              <PrivateRoute exact path={`/error=:error`} component={Error520} />
              <PrivateRoute component={Error404} />
            </Switch>
          </Suspense>
        </StudioLayout>
      </Router>
    ) : (
      <Offline />
    );
  }
}

const mapStateToProps = state => {
  return {
    settings: state.auth.settings
  };
};

const mapDispatchToProps = dispatch => {
  return {
    getUser: () =>
      getUser(dispatch).then(() => dispatch(getPortalInviteLinks())), //TODO: Try simplify
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
    setIsLoaded: () => dispatch(setIsLoaded(true))
  };
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(App);
