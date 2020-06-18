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

class ArticleBodyContent extends React.Component {
  state = { expandedKeys: this.props.filter.treeFolders };

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

      fetchFiles(data[0], newFilter, store.dispatch).catch(err =>
        toastr.error(err)
      ).finally(() => onLoading(false));
    }
  };

  render() {
    const {
      data,
      selectedKeys,
      fakeNewDocuments,
      filter,
      setFilter,
      setTreeFolders,
      onLoading,
      isLoading,
      dragging,
      setDragItem,
      dragItem,
      isMy,
      myId,
      isCommon,
      commonId,
      currentId
    } = this.props;

    //console.log("Article Body render", this.props, this.state.expandedKeys);
    return (
      <TreeFolders
        selectedKeys={selectedKeys}
        fakeNewDocuments={fakeNewDocuments}
        onSelect={this.onSelect}
        data={data}
        filter={filter}
        setFilter={setFilter}
        setTreeFolders={setTreeFolders}
        expandedKeys={this.state.expandedKeys}
        onLoading={onLoading}
        isLoading={isLoading}
        dragging={dragging}
        setDragItem={setDragItem}
        dragItem={dragItem}
        isMy={isMy}
        myId={myId}
        isCommon={isCommon}
        commonId={commonId}
        currentId={currentId}
      />
    );
  }
}

function mapStateToProps(state) {
  const { treeFolders, selectedFolder, filter, dragItem } = state.files;
  const currentFolderId = selectedFolder.id.toString();
  const fakeNewDocuments = 8;
  const myFolderIndex = 0;
  const commonFolderIndex = 2;

  const myId = treeFolders[myFolderIndex].id;
  const commonId = treeFolders[commonFolderIndex].id

  const isMy = selectedFolder.pathParts[0] === myId;
  const isCommon = selectedFolder.pathParts[0] === commonId;

  return {
    data: treeFolders,
    selectedKeys: selectedFolder ? [currentFolderId] : [""],
    fakeNewDocuments,
    filter,
    isMy,
    isCommon,
    myId,
    commonId,
    dragItem,
    currentId: selectedFolder.id
  };
}

export default connect(mapStateToProps, { setFilter, setTreeFolders, setDragItem })(
  ArticleBodyContent
);
