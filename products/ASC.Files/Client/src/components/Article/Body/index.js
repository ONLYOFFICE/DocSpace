import React from "react";
import { connect } from "react-redux";
import { utils, toastr } from "asc-web-components";
import { getRootFolders } from "../../../store/files/selectors";
import TreeFolders from "./TreeFolders";
import {
  setFilter,
  fetchFiles,
  setRootFolders
} from "../../../store/files/actions";
import store from "../../../store/store";
import { api } from "asc-web-common";
const { files } = api;

class ArticleBodyContent extends React.Component {
  state = { expandedKeys: this.props.filter.treeFolders };

  componentDidMount() {
    const newFilter = this.props.filter.clone();
    const folderId = newFilter.folder;

    let expandedKeys = [];
    files
      .getFolder(folderId)
      .then(data => {
        for (let item of data.pathParts) {
          expandedKeys.push(item.toString());
        }

        expandedKeys.pop();

        fetchFiles(folderId, newFilter, store.dispatch).catch(err =>
          toastr.error("Something went wrong", err)
        );
      })
      .catch(err => toastr.error("Something went wrong", err))
      .finally(() => this.setState({ expandedKeys }));
  }

  componentDidUpdate(prevProps) {
    if (
      this.props.filter.treeFolders.length !==
      prevProps.filter.treeFolders.length ||
      this.state.expandedKeys.length !== this.props.filter.treeFolders.length
    ) {
      this.setState({ expandedKeys: this.props.filter.treeFolders });
    }
  }

  render() {
    const {
      data,
      selectedKeys,
      fakeNewDocuments,
      currentModule,
      filter,
      setFilter,
      setRootFolders,
      onLoading,
      isLoading
    } = this.props;

    //console.log("Article Body render", this.props, this.state.expandedKeys);
    return (
      <TreeFolders
        selectedKeys={selectedKeys}
        fakeNewDocuments={fakeNewDocuments}
        currentModule={currentModule}
        data={data}
        filter={filter}
        setFilter={setFilter}
        setRootFolders={setRootFolders}
        expandedKeys={this.state.expandedKeys}
        onLoading={onLoading}
        isLoading={isLoading}
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
    filter
  };
}

export default connect(mapStateToProps, { setFilter, setRootFolders })(
  ArticleBodyContent
);
