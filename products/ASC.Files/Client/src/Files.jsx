import React from "react";
import { Provider as FilesProvider } from "mobx-react";
import { inject, observer } from "mobx-react";
import { Switch } from "react-router-dom";
import config from "../package.json";
import PrivateRoute from "@appserver/common/components/PrivateRoute";
import AppLoader from "@appserver/common/components/AppLoader";
import toastr from "studio/toastr";
import { updateTempContent } from "@appserver/common/utils";
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
import filesActionsStore from "./store/FilesActionsStore";
import "./custom.scss";
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";
//import { regDesktop } from "@appserver/common/src/desktop";
import Home from "./components/pages/Home";
import Settings from "./components/pages/Settings";
import VersionHistory from "./components/pages/VersionHistory";

const homepage = config.homepage;

const Error404 = React.lazy(() => import("studio/Error404"));

const Error404Route = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <Error404 {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

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
    //const { /*, isDesktop*/ } = this.props;

    return (
      <Switch>
        <PrivateRoute
          exact
          path={`${homepage}/settings/:setting`}
          component={Settings}
        />
        <PrivateRoute
          exact
          path={[`${homepage}/:fileId/history`]}
          component={VersionHistory}
        />
        <PrivateRoute exact path={homepage} component={Home} />
        <PrivateRoute path={`${homepage}/filter`} component={Home} />
        <PrivateRoute component={Error404Route} />
      </Switch>
    );
  }
}

const Files = inject(({ auth, initFilesStore }) => {
  return {
    //isDesktop: auth.settingsStore.isDesktopClient,
    user: auth.userStore.user,
    isAuthenticated: auth.isAuthenticated,
    encryptionKeys: auth.settingsStore.encryptionKeys,
    isEncryption: auth.settingsStore.isEncryptionSupport,
    isLoaded: auth.isLoaded && initFilesStore.isLoaded,
    setIsLoaded: initFilesStore.setIsLoaded,
    setEncryptionKeys: auth.settingsStore.setEncryptionKeys,
    loadFilesInfo: async () => {
      //await auth.init();
      await initFilesStore.initFiles();
      auth.setProductVersion(config.version);
    },
  };
})(observer(FilesContent));

export default () => (
  <FilesProvider
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
    filesActionsStore={filesActionsStore}
  >
    <I18nextProvider i18n={i18n}>
      <Files />
    </I18nextProvider>
  </FilesProvider>
);
