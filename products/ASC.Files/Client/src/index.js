import "./wdyr";

import React from "react";
import ReactDOM from "react-dom";
import "./custom.scss";
import App from "./App";

import * as serviceWorker from "./serviceWorker";
import { ErrorBoundary, store as commonStore } from "asc-web-common";
import { Provider as MobxProvider } from "mobx-react";
import initFilesStore from "./store/InitFilesStore";
import filesStore from "./store/FilesStore";
import settingsStore from "./store/SettingsStore";
import mediaViewerDataStore from "./store/MediaViewerDataStore";
import formatsStore from "./store/FormatsStore";
import versionHistoryStore from "./store/VersionHistoryStore";
import uploadDataStore from "./store/UploadDataStore";
import dialogsStore from "./store/DialogsStore";
import treeFoldersStore from "./store/TreeFoldersStore";
import selectedFolderStore from "./store/SelectedFolderStore";

const { authStore } = commonStore;

ReactDOM.render(
  <MobxProvider
    auth={authStore}
    initFilesStore={initFilesStore}
    filesStore={filesStore}
    settingsStore={settingsStore}
    mediaViewerDataStore={mediaViewerDataStore}
    formatsStore={formatsStore}
    versionHistoryStore={versionHistoryStore}
    uploadDataStore={uploadDataStore}
    dialogsStore={dialogsStore}
    treeFoldersStore={treeFoldersStore}
    selectedFolderStore={selectedFolderStore}
  >
    <ErrorBoundary>
      <App />
    </ErrorBoundary>
  </MobxProvider>,
  document.getElementById("root")
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.register();
