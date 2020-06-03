import React from "react";
import { connect } from "react-redux";
import { toastr } from "asc-web-components";
import TreeFolders from "./TreeFolders";
import {
  setFilter,
  fetchFiles,
  setTreeFolders
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
      isLoading
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
      />
    );
  }
}

function mapStateToProps(state) {
  const { treeFolders, selectedFolder, filter } = state.files;
  const currentFolderId = selectedFolder.id.toString();
  const fakeNewDocuments = 8;

  return {
    data: treeFolders,
    selectedKeys: selectedFolder ? [currentFolderId] : [""],
    fakeNewDocuments,
    filter
  };
}

export default connect(mapStateToProps, { setFilter, setTreeFolders })(
  ArticleBodyContent
);
