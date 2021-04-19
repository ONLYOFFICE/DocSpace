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

    if (!selectedTreeNode || selectedTreeNode[0] !== data[0]) {
      setSelectedNode(data);
      setIsLoading(true);

      const newFilter = filter.clone();
      newFilter.page = 0;
      newFilter.startIndex = 0;
      newFilter.folder = data[0];

      const selectedFolderTitle =
        (e.node && e.node.props && e.node.props.title) || null;

      selectedFolderTitle
        ? setDocumentTitle(selectedFolderTitle)
        : setDocumentTitle();

      if (window.location.pathname.indexOf("/filter") > 0) {
        fetchFiles(data[0], newFilter)
          .catch((err) => toastr.error(err))
          .finally(() => {

            setIsLoading(false);
          });
      } else {

        newFilter.startIndex = 0;
        const urlFilter = newFilter.toUrlParams();
        history.push(
          combineUrl(AppServerConfig.proxyURL, homepage, `/filter?${urlFilter}`)
        );
      }
    }
  };

  onShowNewFilesPanel = (folderId) => {
    this.props.setNewFilesPanelVisible(true);
    this.props.setNewFilesIds([folderId]);
  };

  render() {
    const { treeFolders, onTreeDrop, selectedTreeNode } = this.props;

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
        <ThirdPartyList />
      </>
    );
  }
}

export default inject(
  ({ filesStore, treeFoldersStore, selectedFolderStore, dialogsStore }) => {
    const { fetchFiles, filter, setIsLoading } = filesStore;
    const { treeFolders, setSelectedNode, setTreeFolders } = treeFoldersStore;
    const selectedTreeNode =
      treeFoldersStore.selectedTreeNode.length > 0 &&
      treeFoldersStore.selectedTreeNode[0] !== "@my"
        ? treeFoldersStore.selectedTreeNode
        : [selectedFolderStore.id + ""];

    const { setNewFilesPanelVisible, setNewFilesIds } = dialogsStore;

    return {
      selectedFolderTitle: selectedFolderStore.title,
      treeFolders,
      selectedTreeNode,
      filter,

      setIsLoading,
      fetchFiles,
      setSelectedNode,
      setTreeFolders,
      setNewFilesPanelVisible,
      setNewFilesIds,

      homepage: config.homepage,
    };
  }
)(observer(withRouter(ArticleBodyContent)));
