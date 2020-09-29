import React from "react";
import { connect } from "react-redux";
import { utils } from "asc-web-components";
import { store as initStore, toastr, Loaders } from "asc-web-common";
import TreeFolders from "./TreeFolders";
import TreeSettings from "./TreeSettings";
import isEmpty from "lodash/isEmpty";
import {
  setFilter,
  fetchFiles,
  setTreeFolders,
  setDragItem,
  setDragging,
  setNewTreeFilesBadge,
  setIsLoading,
  setSelectedNode,
} from "../../../store/files/actions";
import store from "../../../store/store";
import { NewFilesPanel } from "../../panels";
import { setDocumentTitle } from "../../../helpers/utils";

const { getCurrentModule } = initStore.auth.selectors;

class ArticleBodyContent extends React.Component {
  constructor(props) {
    super(props);

    const { selectedFolderTitle, filter, data } = props;

    selectedFolderTitle
      ? setDocumentTitle(selectedFolderTitle)
      : setDocumentTitle();

    this.state = {
      expandedKeys: filter.treeFolders,
      data,
      showNewFilesPanel: false,
    };
  }

  componentDidUpdate(prevProps) {
    const { filter, data } = this.props;

    if (
      filter.treeFolders.length !== prevProps.filter.treeFolders.length ||
      this.state.expandedKeys.length !== filter.treeFolders.length
    ) {
      this.setState({ expandedKeys: filter.treeFolders });
    }

    //console.log(prevProps.data);
    //console.log(data);

    if (!utils.array.isArrayEqual(prevProps.data, data)) {
      this.setState({ data });
    }
  }

  // shouldComponentUpdate(nextProps, nextState) {
  //   if (this.props.updateTreeNew) {
  //     this.props.setNewTreeFilesBadge(false);
  //     return true;
  //   }

  //   if (!isEqual(this.state, nextState) || !isEqual(this.props, nextProps)) {
  //     return true;
  //   }

  //   return false;
  // }

  onSelect = (data, e) => {
    const {
      filter,
      setIsLoading,
      selectedTreeNode,
      setSelectedNode,
      fetchFiles,
    } = this.props;

    if (selectedTreeNode[0] !== data[0]) {
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

      fetchFiles(data[0], newFilter, store.dispatch)
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
    const data = this.state.data;
    const dataItem = data.find((x) => x.id === folderPath[0]);
    dataItem.newItems = filesCount ? filesCount : dataItem.newItems - 1;
    this.setState({ data });
  };

  render() {
    const {
      data,
      filter,
      setFilter,
      setTreeFolders,
      dragging,
      setDragItem,
      isMy,
      myId,
      isCommon,
      commonId,
      currentId,
      isAdmin,
      isShare,
      setDragging,
      onTreeDrop,
      selectedTreeNode,
      setIsLoading,
    } = this.props;

    const { showNewFilesPanel, expandedKeys, newFolderId } = this.state;

    //console.log("Article Body render", this.props, this.state.expandedKeys);
    return (
      <>
        {showNewFilesPanel && (
          <NewFilesPanel
            visible={showNewFilesPanel}
            onClose={this.onShowNewFilesPanel}
            setNewFilesCount={this.setNewFilesCount}
            folderId={newFolderId}
            treeFolders={data}
            setTreeFolders={setTreeFolders}
            setIsLoading={setIsLoading}
            //setNewItems={this.setNewItems}
          />
        )}
        {isEmpty(data) ? (
          <Loaders.TreeFolders />
        ) : (
          <>
            <TreeFolders
              selectedKeys={selectedTreeNode}
              onSelect={this.onSelect}
              data={data}
              filter={filter}
              setFilter={setFilter}
              setTreeFolders={setTreeFolders}
              expandedKeys={expandedKeys}
              dragging={dragging}
              setDragging={setDragging}
              setDragItem={setDragItem}
              isMy={isMy}
              myId={myId}
              isCommon={isCommon}
              commonId={commonId}
              currentId={currentId}
              isAdmin={isAdmin}
              isShare={isShare}
              onBadgeClick={this.onShowNewFilesPanel}
              onTreeDrop={onTreeDrop}
            />
            <TreeSettings />{" "}
          </>
        )}
      </>
    );
  }
}

function mapStateToProps(state) {
  const { modules, settings } = state.auth;
  const { organizationName, currentProductId } = settings;

  const currentModule = getCurrentModule(modules, currentProductId);

  const {
    treeFolders,
    selectedFolder,
    filter,
    selection,
    dragging,
    updateTreeNew,
    selectedTreeNode,
  } = state.files;

  const currentFolderId =
    selectedFolder && selectedFolder.id && selectedFolder.id.toString();
  const myFolderIndex = 0;
  const shareFolderIndex = 1;
  const commonFolderIndex = 2;

  const myId = treeFolders.length && treeFolders[myFolderIndex].id;
  const shareId = treeFolders.length && treeFolders[shareFolderIndex].id;
  const commonId = treeFolders.length && treeFolders[commonFolderIndex].id;

  const isMy =
    selectedFolder &&
    selectedFolder.pathParts &&
    selectedFolder.pathParts[0] === myId;

  const isShare =
    selectedFolder &&
    selectedFolder.pathParts &&
    selectedFolder.pathParts[0] === shareId;

  const isCommon =
    selectedFolder &&
    selectedFolder.pathParts &&
    selectedFolder.pathParts[0] === commonId;

  const selected =
    selectedTreeNode.length > 0 ? selectedTreeNode : [currentFolderId];

  return {
    data: treeFolders,
    selectedKeys: selectedFolder ? [currentFolderId] : [""],
    filter,
    isMy,
    isCommon,
    isShare,
    myId,
    commonId,
    currentId: selectedFolder.id,
    isAdmin: state.auth.user.isAdmin,
    selection,
    dragging,
    updateTreeNew,
    selectedTreeNode: selected,
    currentModuleName: (currentModule && currentModule.title) || "",
    selectedFolderTitle: (selectedFolder && selectedFolder.title) || "",
    organizationName,
  };
}

const mapDispatchToProps = (dispatch) => {
  return {
    setFilter: (filter) => dispatch(setFilter(filter)),
    setTreeFolders: (treeFolders) => dispatch(setTreeFolders(treeFolders)),
    setDragItem: (dragItem) => dispatch(setDragItem(dragItem)),
    setDragging: (dragging) => dispatch(setDragging(dragging)),
    setNewTreeFilesBadge: (updateTreeNew) =>
      dispatch(setNewTreeFilesBadge(updateTreeNew)),
    setIsLoading: (isLoading) => dispatch(setIsLoading(isLoading)),
    setSelectedNode: (node) => dispatch(setSelectedNode(node)),
    fetchFiles: (folderId, filter) => fetchFiles(folderId, filter, dispatch),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(ArticleBodyContent);
