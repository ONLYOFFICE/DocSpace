import React from "react";
import { toastr, Loaders } from "asc-web-common";
import TreeFolders from "./TreeFolders";
import TreeSettings from "./TreeSettings";
import isEmpty from "lodash/isEmpty";
import { NewFilesPanel } from "../../panels";
import { setDocumentTitle } from "../../../helpers/utils";
import ThirdPartyList from "./ThirdPartyList";
import { inject, observer } from "mobx-react";

class ArticleBodyContent extends React.Component {
  constructor(props) {
    super(props);

    const { selectedFolderTitle } = props;

    selectedFolderTitle
      ? setDocumentTitle(selectedFolderTitle)
      : setDocumentTitle();

    this.state = {
      showNewFilesPanel: false,
    };
  }

  componentDidMount() {
    if (this.props.currentId) {
      const currentId = [this.props.currentId + ""];
      this.props.setSelectedNode(currentId);
    }
  }

  onSelect = (data, e) => {
    const {
      filter,
      setIsLoading,
      selectedTreeNode,
      setSelectedNode,
      fetchFiles,
    } = this.props;

    if (!selectedTreeNode || selectedTreeNode[0] !== data[0]) {
      setSelectedNode(data);
      setIsLoading(true);
      const newFilter = filter.clone();
      newFilter.page = 0;
      newFilter.startIndex = 0;

      const selectedFolderTitle =
        (e.node && e.node.props && e.node.props.title) || null;

      selectedFolderTitle
        ? setDocumentTitle(selectedFolderTitle)
        : setDocumentTitle();

      fetchFiles(data[0], newFilter)
        .catch((err) => toastr.error(err))
        .finally(() => setIsLoading(false));
    }
  };

  onShowNewFilesPanel = (folderId) => {
    const { showNewFilesPanel } = this.state;
    this.setState({
      showNewFilesPanel: !showNewFilesPanel,
      newFolderId: [folderId],
    });
  };

  setNewFilesCount = (folderPath, filesCount) => {
    const data = this.props.treeFolders;
    const dataItem = data.find((x) => x.id === folderPath[0]);
    dataItem.newItems = filesCount ? filesCount : dataItem.newItems - 1;

    this.props.setTreeFolders(data);
  };

  render() {
    const { treeFolders, onTreeDrop, selectedTreeNode } = this.props;
    const { showNewFilesPanel, newFolderId } = this.state;

    return (
      <>
        {showNewFilesPanel && (
          <NewFilesPanel
            visible={showNewFilesPanel}
            onClose={this.onShowNewFilesPanel}
            setNewFilesCount={this.setNewFilesCount}
            folderId={newFolderId}
            treeFolders={treeFolders}
          />
        )}
        {isEmpty(treeFolders) ? (
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
        )}
      </>
    );
  }
}

export default inject(
  ({ initFilesStore, filesStore, treeFoldersStore, selectedFolderStore }) => {
    const { setIsLoading } = initFilesStore;
    const { fetchFiles, filter } = filesStore;
    const { treeFolders, setSelectedNode, setTreeFolders } = treeFoldersStore;
    const selectedTreeNode = [selectedFolderStore.id + ""];

    return {
      selectedFolderTitle: selectedFolderStore.title,
      treeFolders,
      selectedTreeNode,
      filter,

      setIsLoading,
      fetchFiles,
      setSelectedNode,
      setTreeFolders,
    };
  }
)(observer(ArticleBodyContent));
