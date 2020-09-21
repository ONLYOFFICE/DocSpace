import React, { Suspense } from "react";
import { connect } from "react-redux";
import axios from "axios";
import { Router, Switch, Redirect } from "react-router-dom";
import { Loader } from "asc-web-components";
import Home from "./components/pages/Home";
import DocEditor from "./components/pages/DocEditor";
import Settings from "./components/pages/Settings";
import {
  fetchMyFolder,
  fetchTreeFolders,
  fetchFiles
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
  StudioLayout,
  Offline,
  api
} from "asc-web-common";

import { getFilterByLocation } from "./helpers/converters";

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
const { FilesFilter } = api;

const VersionHistory = React.lazy(() =>
  import("./components/pages/VersionHistory")
);

const withStudioLayout = Component => props => (
  <StudioLayout>
    <Component {...props} />
  </StudioLayout>
);

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
      fetchMyFolder,
      fetchTreeFolders,
      fetchFiles,
      finalize,
      setIsLoaded
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
      fetchTreeFolders()
    ];

    axios
      .all(requests)
      .then(() => {
        const reg = new RegExp(`${config.homepage}((/?)$|/filter)`, "gm"); //TODO: Always find?
        const match = window.location.pathname.match(reg);
        let filterObj = null;

        if (match && match.length > 0) {
          filterObj = getFilterByLocation(window.location);

          if (!filterObj) {
            filterObj = FilesFilter.getDefault();
          }
        }

        return Promise.resolve(filterObj);
      })
      .then(filter => {
        let dataObj = filter;

        if (filter && filter.authorType) {
          const filterObj = filter;
          const authorType = filterObj.authorType;
          const indexOfUnderscore = authorType.indexOf("_");
          const type = authorType.slice(0, indexOfUnderscore);
          const itemId = authorType.slice(indexOfUnderscore + 1);

          if (itemId) {
            dataObj = {
              type,
              itemId,
              filter: filterObj
            };
          } else {
            filterObj.authorType = null;
            dataObj = filterObj;
          }
        }
        return Promise.resolve(dataObj);
      })
      .then(data => {
        if (!data) return Promise.resolve();
        if (data instanceof FilesFilter) return Promise.resolve(data);

        const { filter, itemId, type } = data;
        const newFilter = filter ? filter.clone() : FilesFilter.getDefault();

        switch (type) {
          case "group":
            return Promise.all([api.groups.getGroup(itemId), newFilter]);
          case "user":
            return Promise.all([api.people.getUserById(itemId), newFilter]);
          default:
            return Promise.resolve(newFilter);
        }
      })
      .catch(err => {
        Promise.resolve(FilesFilter.getDefault());
        console.warn("Filter restored by default", err);
      })
      .then(data => {
        if (!data) return Promise.resolve();
        if (data instanceof FilesFilter) return Promise.resolve(data);

        const result = data[0];
        const filter = data[1];
        const type = result.displayName ? "user" : "group";
        const selectedItem = {
          key: result.id,
          label: type === "user" ? result.displayName : result.name,
          type
        };
        filter.selectedItem = selectedItem;

        return Promise.resolve(filter);
      })
      .then(filter => {
        if (!filter) return Promise.resolve();

        const folderId = filter.folder;
        return fetchFiles(folderId, filter);
      })
      .then(() => {
        this.removeLoader();
        finalize();
      });
  }

  render() {
    const { homepage } = this.props.settings;

    return navigator.onLine ? (
      <Router history={history}>
        <Suspense
          fallback={<Loader className="pageLoader" type="rombs" size="40px" />}
        >
          <Switch>
            <Redirect exact from="/" to={`${homepage}`} />
            <PrivateRoute
              exact
              path={[homepage, `${homepage}/filter`]}
              component={withStudioLayout(Home)}
            />
            <PrivateRoute
              exact
              path={`${homepage}/settings/:setting`}
              component={withStudioLayout(Settings)}
            />
            <PrivateRoute
              exact
              path={`${homepage}/doceditor`}
              component={DocEditor}
            />
            <PrivateRoute
              exact
              path={`${homepage}/:fileId/history`}
              component={withStudioLayout(VersionHistory)}
            />
            <PublicRoute
              exact
              path={[
                "/login",
                "/login/error=:error",
                "/login/confirmed-email=:confirmedEmail"
              ]}
              component={withStudioLayout(Login)}
            />
            <PrivateRoute
              exact
              path={`/error=:error`}
              component={withStudioLayout(Error520)}
            />
            <PrivateRoute component={withStudioLayout(Error404)} />
          </Switch>
        </Suspense>
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
    getUser: () => getUser(dispatch),
    getPortalSettings: () => getPortalSettings(dispatch),
    getModules: () => getModules(dispatch),
    getPortalPasswordSettings: () => getPortalPasswordSettings(dispatch),
    getPortalCultures: () => getPortalCultures(dispatch),
    fetchMyFolder: () => fetchMyFolder(dispatch),
    fetchTreeFolders: () => fetchTreeFolders(dispatch),
    fetchFiles: (folderId, filter) => fetchFiles(folderId, filter, dispatch),
    finalize: () => {
      dispatch(setCurrentProductHomePage(config.homepage));
      dispatch(setCurrentProductId("e67be73d-f9ae-4ce1-8fec-1880cb518cb4"));
      dispatch(setIsLoaded(true));
    },
    setIsLoaded: () => dispatch(setIsLoaded(true))
  };
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(App);
