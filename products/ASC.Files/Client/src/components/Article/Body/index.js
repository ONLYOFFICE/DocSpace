import React from "react";
import { connect } from "react-redux";
import { utils, toastr } from "asc-web-components";
import { getRootFolders } from "../../../store/files/selectors";
import TreeFolders from "./TreeFolders";
import { setFilter, fetchFiles } from "../../../store/files/actions";
import store from "../../../store/store";
import { api, history } from "asc-web-common";
const { files } = api;

class ArticleBodyContent extends React.Component {
  state = { expandedKeys: this.props.filter.treeFolders };

  componentDidMount() {
    if (history.location.hash) {
      const folderId = history.location.hash.slice(1);

      const url = `${history.location.pathname}${history.location.search}`;
      const symbol =
        history.location.hash ||
        history.location.search[history.location.search.length - 1] === "/"
          ? ""
          : "/";

      let expandedKeys = [];
      files
        .getFolder(folderId)
        .then(data => {
          for (let item of data.pathParts) {
            expandedKeys.push(item.toString());
          }

          expandedKeys.pop();

          fetchFiles(folderId, this.props.filter, store.dispatch)
            // .then(() => {
            //   history.push(`${url}${symbol}#${folderId}`);
            // })
            // .catch(err => toastr.error("Something went wrong", err));
        })
        .catch(err => toastr.error("Something went wrong", err))
        .finally(() => this.setState({ expandedKeys }));
    }
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
  const parentId = selectedFolder.parentId;

  return {
    data: getRootFolders(rootFolders),
    selectedKeys: selectedFolder ? [currentFolderId] : [""],
    fakeNewDocuments,
    currentModule: currentFolderId,
    filter,
    parentId
  };
}

export default connect(mapStateToProps, { setFilter })(ArticleBodyContent);
