import React from "react";
import { connect } from "react-redux";
import { utils, toastr } from "asc-web-components";
import { getRootFolders } from "../../../store/files/selectors";
import TreeFolders from "./TreeFolders";
import { setFilter, fetchFolder } from "../../../store/files/actions";
import store from "../../../store/store";
import { api, history } from "asc-web-common";
const { files } = api;

class ArticleBodyContent extends React.Component {
  state = { defaultExpandedKeys: [] };
  componentDidMount() {
    if (history.location.hash) {
      const folderId = history.location.hash.slice(1);

      const url = `${history.location.pathname}${history.location.search}`;
      const symbol =
        history.location.hash ||
        history.location.search[history.location.search.length - 1] === "/"
          ? ""
          : "/";

      let defaultExpandedKeys = [];
      files
        .getFolder(folderId)
        .then(data => {
          let newExpandedKeys = [];
          for (let item of data.pathParts) {
            newExpandedKeys.push(item.toString());
          }
          newExpandedKeys.pop();
          const newFilter = this.props.filter.clone();
          newFilter.TreeFolders = newExpandedKeys;
          this.props.setFilter(newFilter);
          defaultExpandedKeys = newExpandedKeys;
          fetchFolder(folderId, store.dispatch)
            .then(() => {
              history.push(`${url}${symbol}#${folderId}`);
            })
            .catch(err => toastr.error("Something went wrong", err));
        })
        .catch(err => toastr.error("Something went wrong", err))
        .finally(() => this.setState({ defaultExpandedKeys }));
    }
  }
  shouldComponentUpdate(nextProps) {
    const { selectedKeys, data, fakeNewDocuments, currentModule } = this.props;
    if (!utils.array.isArrayEqual(nextProps.selectedKeys, selectedKeys)) {
      return true;
    }

    if (!utils.array.isArrayEqual(nextProps.data, data)) {
      return true;
    }

    if (fakeNewDocuments !== nextProps.fakeNewDocuments) {
      return true;
    }

    if (currentModule !== nextProps.currentModule) {
      return true;
    }

    return false;
  }

  render() {
    const {
      data,
      selectedKeys,
      fakeNewDocuments,
      currentModule,
      filter,
      setFilter
    } = this.props;

    //console.log("FilesTreeMenu", this.props);
    return (
      <TreeFolders
        selectedKeys={selectedKeys}
        fakeNewDocuments={fakeNewDocuments}
        currentModule={currentModule}
        data={data}
        filter={filter}
        setFilter={setFilter}
        defaultExpandedKeys={this.state.defaultExpandedKeys}
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

export default connect(mapStateToProps, { setFilter })(ArticleBodyContent);
