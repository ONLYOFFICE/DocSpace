import React from "react";
import { connect } from "react-redux";
import { utils } from "asc-web-components";
import { getRootFolders, setTreeFilter } from "../../../store/files/selectors";
import TreeFolders from "./TreeFolders";
import { setFilter } from "../../../store/files/actions";

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
      filter,
      setFilter
    } = this.props;

    //console.log("FilesTreeMenu", this.props);
    return (
      <TreeFolders
        selectedKeys={selectedKeys}
        fakeNewDocuments={fakeNewDocuments}
        rootFolders={rootFolders}
        currentModule={currentModule}
        data={data}
        filter={filter}
        setFilter={setFilter}
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
    filter: setTreeFilter(filter, rootFolders)
  };
}

export default connect(mapStateToProps, { setFilter })(ArticleBodyContent);
