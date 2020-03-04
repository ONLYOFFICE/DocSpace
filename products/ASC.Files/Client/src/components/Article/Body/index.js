import React from "react";
import { connect } from "react-redux";
import { utils } from "asc-web-components";
import {
  selectFolder,
  fetchSharedFolder,
  fetchCommonFolder,
  fetchProjectsFolder,
  fetchTrashFolder,
  fetchFolder,
  setRootFolders,
  testUpdateMyFolder
} from "../../../store/files/actions";
import { getRootFolders } from "../../../store/files/selectors";
import TreeFolders from "./TreeFolders";

class ArticleBodyContent extends React.Component {
  /*shouldComponentUpdate(nextProps) {
    const { selectedKeys, data } = this.props;
    if (!utils.array.isArrayEqual(nextProps.selectedKeys, selectedKeys)) {
      return true;
    }

    if (!utils.array.isArrayEqual(nextProps.data, data)) {
      return true;
    }

    return false;
  }*/

  render() {
    const {
      data,
      selectedKeys,
      fakeNewDocuments,
      rootFolders,
      currentModule,
      filter
    } = this.props;

    //console.log("FilesTreeMenu", this.props);
    return (
      <TreeFolders
        selectedKeys={selectedKeys}
        fakeNewDocuments={fakeNewDocuments}
        rootFolders={rootFolders}
        currentModule={currentModule}
        state={this.props.state}
        testUpdateMyFolder={this.props.testUpdateMyFolder}
        data={data}
        filter={filter}
      />
    );
  }
}

function mapStateToProps(state) {
  const { rootFolders, selectedFolder, filter } = state.files;
  const currentFolderId = selectedFolder.id.toString();
  const fakeNewDocuments = 8;

  return {
    data: getRootFolders(rootFolders),
    selectedKeys: selectedFolder ? [currentFolderId] : [""],
    fakeNewDocuments,
    currentModule: currentFolderId,
    rootFolders,
    state,
    filter
  };
}

export default connect(mapStateToProps, {
  selectFolder,
  fetchSharedFolder,
  fetchCommonFolder,
  fetchProjectsFolder,
  fetchTrashFolder,
  fetchFolder,
  setRootFolders,
  testUpdateMyFolder
})(ArticleBodyContent);
