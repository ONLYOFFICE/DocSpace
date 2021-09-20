import React from "react";

import toastr from "studio/toastr";
import Loaders from "@appserver/common/components/Loaders";
import TreeFolders from "./TreeFolders";
import TreeSettings from "./TreeSettings";
import isEmpty from "lodash/isEmpty";
import { setDocumentTitle } from "../../../helpers/utils";
import ThirdPartyList from "./ThirdPartyList";
import DownloadAppList from "./DownloadAppList";
import Banner from "./Banner";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router-dom";
import config from "../../../../package.json";
import { clickBackdrop, combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import FilesFilter from "@appserver/common/api/files/filter";
import { isDesktop, isTablet } from "react-device-detect";

class ArticleBodyContent extends React.Component {
  onSelect = (data, e) => {
    const {
      setIsLoading,
      setSelectedNode,
      fetchFiles,
      homepage,
      history,
      hideArticle,
      setFirstLoad,
    } = this.props;

    setSelectedNode(data);
    hideArticle(false);
    setIsLoading(true);
    // const selectedFolderTitle =
    //   (e.node && e.node.props && e.node.props.title) || null;

    // selectedFolderTitle
    //   ? setDocumentTitle(selectedFolderTitle)
    //   : setDocumentTitle();

    if (window.location.pathname.indexOf("/filter") > 0) {
      fetchFiles(data[0], null, true, false)
        .catch((err) => toastr.error(err))
        .finally(() => setIsLoading(false));
    } else {
      setFirstLoad(true);
      const filter = FilesFilter.getDefault();

      filter.folder = data[0];

      const urlFilter = filter.toUrlParams();

      history.push(
        combineUrl(AppServerConfig.proxyURL, homepage, `/filter?${urlFilter}`)
      );
    }
    //}
  };

  onShowNewFilesPanel = (folderId) => {
    this.props.setNewFilesPanelVisible(true, [folderId]);
  };

  render() {
    const {
      treeFolders,
      onTreeDrop,
      enableThirdParty,
      isVisitor,
      personal,
      firstLoad,
      isDesktopClient,
      FirebaseHelper,
    } = this.props;

    //console.log("Article Body render");

    const campaigns = (localStorage.getItem("campaigns") || "")
      .split(",")
      .filter((campaign) => campaign.length > 0);

    return isEmpty(treeFolders) ? (
      <Loaders.TreeFolders />
    ) : (
      <>
        <TreeFolders
          useDefaultSelectedKeys
          onSelect={this.onSelect}
          data={treeFolders}
          onBadgeClick={this.onShowNewFilesPanel}
          onTreeDrop={onTreeDrop}
        />
        {!personal && !firstLoad && <TreeSettings />}

        {!isDesktopClient && (
          <>
            {enableThirdParty && !isVisitor && <ThirdPartyList />}
            <DownloadAppList />
            {(isDesktop || isTablet) &&
              personal &&
              !firstLoad &&
              campaigns.length > 0 && (
                <Banner FirebaseHelper={FirebaseHelper} />
              )}
          </>
        )}
      </>
    );
  }
}

export default inject(
  ({
    auth,
    filesStore,
    treeFoldersStore,
    selectedFolderStore,
    dialogsStore,
    settingsStore,
  }) => {
    const { fetchFiles, setIsLoading, setFirstLoad, firstLoad } = filesStore;
    const { treeFolders, setSelectedNode, setTreeFolders } = treeFoldersStore;

    const { setNewFilesPanelVisible } = dialogsStore;

    const { personal, hideArticle, isDesktopClient } = auth.settingsStore;

    const selectedFolderTitle = selectedFolderStore.title;

    selectedFolderTitle
      ? setDocumentTitle(selectedFolderTitle)
      : setDocumentTitle();

    return {
      treeFolders,
      enableThirdParty: settingsStore.enableThirdParty,
      isVisitor: auth.userStore.user.isVisitor,
      homepage: config.homepage,
      personal,

      setIsLoading,
      setFirstLoad,
      fetchFiles,
      setSelectedNode,
      setTreeFolders,
      setNewFilesPanelVisible,
      hideArticle,
      firstLoad,
      isDesktopClient,
      FirebaseHelper: auth.settingsStore.firebaseHelper,
    };
  }
)(observer(withRouter(ArticleBodyContent)));
