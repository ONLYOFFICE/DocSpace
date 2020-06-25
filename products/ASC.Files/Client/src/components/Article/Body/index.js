import React from "react";
import { connect } from "react-redux";
import { toastr } from "asc-web-components";
import TreeFolders from "./TreeFolders";
import {
  setFilter,
  fetchFiles,
  setTreeFolders,
  setDragItem
} from "../../../store/files/actions";
import store from "../../../store/store";
import isEqual from "lodash/isEqual";
import { NewFilesPanel } from "../../panels";

class ArticleBodyContent extends React.Component {
  state = {
    expandedKeys: this.props.filter.treeFolders,
    showNewFilesPanel: false,
  };

  componentDidUpdate(prevProps) {
    if (
      this.props.filter.treeFolders.length !==
      prevProps.filter.treeFolders.length ||
      this.state.expandedKeys.length !== this.props.filter.treeFolders.length
    ) {
      this.setState({ expandedKeys: this.props.filter.treeFolders });
    }
  }

  onSelect = data => {
    const { selectedKeys, filter, onLoading } = this.props;
    if (selectedKeys[0] !== data[0]) {
      onLoading(true);
      const newFilter = filter.clone();
      newFilter.page = 0;
      newFilter.startIndex = 0;

      fetchFiles(data[0], newFilter, store.dispatch)
        .catch(err => toastr.error(err))
        .finally(() => onLoading(false));
    }
  };

  shouldComponentUpdate(nextProps, nextState) {
    if (!isEqual(this.state, nextState) || !isEqual(this.props, nextProps)) {
      return true;
    }

    return false;
  }

  onShowNewFilesPanel = () => {
    this.setState({showNewFilesPanel: !this.state.showNewFilesPanel});
  };

  render() {
    const {
      data,
      selectedKeys,
      filter,
      setFilter,
      setTreeFolders,
      onLoading,
      isLoading,
      dragging,
      setDragItem,
      isMy,
      myId,
      isCommon,
      commonId,
      currentId,
      isAdmin,
      isShare
    } = this.props;

    const { showNewFilesPanel, expandedKeys } = this.state;

    const fakeFiles = this.props.selection;

    //console.log("Article Body render", this.props, this.state.expandedKeys);
    return (
      <>
        {showNewFilesPanel && (
          <NewFilesPanel
            visible={showNewFilesPanel}
            onClose={this.onShowNewFilesPanel}
            files={fakeFiles}
          />
        )}
        <TreeFolders
          selectedKeys={selectedKeys}
          onSelect={this.onSelect}
          data={data}
          filter={filter}
          setFilter={setFilter}
          setTreeFolders={setTreeFolders}
          expandedKeys={expandedKeys}
          onLoading={onLoading}
          isLoading={isLoading}
          dragging={dragging}
          setDragItem={setDragItem}
          isMy={isMy}
          myId={myId}
          isCommon={isCommon}
          commonId={commonId}
          currentId={currentId}
          isAdmin={isAdmin}
          isShare={isShare}
          onBadgeClick={this.onShowNewFilesPanel}
        />
      </>
    );
  }
}

function mapStateToProps(state) {
  const { treeFolders, selectedFolder, filter, selection } = state.files;
  const currentFolderId = selectedFolder.id.toString();
  const myFolderIndex = 0;
  const shareFolderIndex = 1;
  const commonFolderIndex = 2;

  const myId = treeFolders[myFolderIndex].id;
  const shareId = treeFolders[shareFolderIndex].id;
  const commonId = treeFolders[commonFolderIndex].id;

  const isMy = selectedFolder && 
    selectedFolder.pathParts && 
    selectedFolder.pathParts[0] === myId;

  const isShare = selectedFolder && 
    selectedFolder.pathParts && 
    selectedFolder.pathParts[0] === shareId;

  const isCommon = selectedFolder && 
    selectedFolder.pathParts && 
    selectedFolder.pathParts[0] === commonId;

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
    selection
  };
}

export default connect(mapStateToProps, { setFilter, setTreeFolders, setDragItem })(
  ArticleBodyContent
);
