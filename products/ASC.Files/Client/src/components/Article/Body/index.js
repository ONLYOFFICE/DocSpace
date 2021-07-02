import React from "react";

import toastr from "studio/toastr";
import Loaders from "@appserver/common/components/Loaders";
import TreeFolders from "./TreeFolders";
import TreeSettings from "./TreeSettings";
import isEmpty from "lodash/isEmpty";
import { setDocumentTitle } from "../../../helpers/utils";
import ThirdPartyList from "./ThirdPartyList";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router-dom";
import config from "../../../../package.json";
import { clickBackdrop, combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import FilesFilter from "@appserver/common/api/files/filter";

class ArticleBodyContent extends React.Component {
  constructor(props) {
    super(props);

    const { selectedFolderTitle } = props;

    selectedFolderTitle
      ? setDocumentTitle(selectedFolderTitle)
      : setDocumentTitle();
  }

  /*componentDidMount() {
    if (this.props.currentId) {
      const currentId = [this.props.currentId + ""];
      this.props.setSelectedNode(currentId);
    }
  }*/

  onSelect = (data, e) => {
    const {
      filter,
      setIsLoading,
      selectedTreeNode,
      setSelectedNode,
      fetchFiles,
      homepage,
      history,
    } = this.props;

    //if (!selectedTreeNode || selectedTreeNode[0] !== data[0]) {
    setSelectedNode(data);
    setIsLoading(true);

    const selectedFolderTitle =
      (e.node && e.node.props && e.node.props.title) || null;

    selectedFolderTitle
      ? setDocumentTitle(selectedFolderTitle)
      : setDocumentTitle();

    if (window.location.pathname.indexOf("/filter") > 0) {
      fetchFiles(data[0])
        .catch((err) => toastr.error(err))
        .finally(() => setIsLoading(false));
    } else {
      const urlFilter = FilesFilter.getDefault().toUrlParams();
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
      selectedTreeNode,
      enableThirdParty,
      isVisitor,
    } = this.props;

    return isEmpty(treeFolders) ? (
      <Loaders.TreeFolders />
    ) : (
      <>
        <TreeFolders
          selectedKeys={selectedTreeNode}
          onSelect={this.onSelect}
          data={treeFolders}
          onBadgeClick={this.onShowNewFilesPanel}
          onTreeDrop={onTreeDrop}
        />
        <TreeSettings />
        {enableThirdParty && !isVisitor && <ThirdPartyList />}
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
    const { fetchFiles, filter, setIsLoading } = filesStore;
    const { treeFolders, setSelectedNode, setTreeFolders } = treeFoldersStore;

    const selectedNode = treeFoldersStore.selectedTreeNode;

    const selectedTreeNode =
      selectedNode.length > 0 &&
      selectedNode[0] !== "@my" &&
      selectedNode[0] !== "@common"
        ? selectedNode
        : [selectedFolderStore.id + ""];

    const { setNewFilesPanelVisible } = dialogsStore;

    return {
      selectedFolderTitle: selectedFolderStore.title,
      treeFolders,
      selectedTreeNode,
      filter,
      enableThirdParty: settingsStore.enableThirdParty,
      isVisitor: auth.userStore.user.isVisitor,

      setIsLoading,
      fetchFiles,
      setSelectedNode,
      setTreeFolders,
      setNewFilesPanelVisible,

      homepage: config.homepage,
    };
  }
)(observer(withRouter(ArticleBodyContent)));
