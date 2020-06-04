import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";
import store from "./store/store";
import { fetchMyFolder, fetchTreeFolders, fetchFiles } from "./store/files/actions";
import config from "../package.json";
import "./custom.scss";
import App from "./App";

import * as serviceWorker from "./serviceWorker";
import { store as commonStore, constants, ErrorBoundary, api } from "asc-web-common";
import { getFilterByLocation } from "./helpers/converters";
const { setIsLoaded, getUserInfo, setCurrentProductId, setCurrentProductHomePage, getPortalPasswordSettings, getPortalCultures } = commonStore.auth.actions;
const { AUTH_KEY } = constants;
const { FilesFilter } = api;

const token = localStorage.getItem(AUTH_KEY);

if (token) {
  getUserInfo(store.dispatch)
    .then(() => getPortalPasswordSettings(store.dispatch))
    .then(() => getPortalCultures(store.dispatch))
    .then(() => fetchMyFolder(store.dispatch))
    .then(() => fetchTreeFolders(store.dispatch))
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
        const indexOfUnderscore = authorType.indexOf('_');
        const type = authorType.slice(0, indexOfUnderscore);
        const itemId = authorType.slice(indexOfUnderscore + 1);

        if (itemId) {
          dataObj = {
            type,
            itemId,
            filter: filterObj
          };
        }
        else {
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
        case 'group':
          return Promise.all([api.groups.getGroup(itemId), newFilter]);
        case 'user':
          return Promise.all([api.people.getUserById(itemId), newFilter]);
        default:
          return Promise.resolve(newFilter);
      }
    })
    .catch(err => {
      Promise.resolve(FilesFilter.getDefault());
      console.warn('Filter restored by default', err);
    })
    .then(data => {
      if (!data) return Promise.resolve();
      if (data instanceof FilesFilter) return Promise.resolve(data);

      const result = data[0];
      const filter = data[1];
      const type = result.displayName ? 'user' : 'group';
      const selectedItem = {
        key: result.id,
        label: type === 'user' ? result.displayName : result.name,
        type
      };
      filter.selectedItem = selectedItem;

      return Promise.resolve(filter);
    })
    .then(filter => {
      if (!filter) return Promise.resolve();

      const folderId = filter.folder;
      return fetchFiles(folderId, filter, store.dispatch);
    })
    .then(() => {
      store.dispatch(setCurrentProductHomePage(config.homepage));
      store.dispatch(setCurrentProductId("e67be73d-f9ae-4ce1-8fec-1880cb518cb4"));
      store.dispatch(setIsLoaded(true));
    });
}
else {
  store.dispatch(setIsLoaded(true));
};

ReactDOM.render(
  <Provider store={store}>
    <ErrorBoundary>
      <App />
    </ErrorBoundary>
  </Provider>,
  document.getElementById("root")
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.register();