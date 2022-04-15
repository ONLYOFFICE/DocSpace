import React from "react";
import { inject, observer } from "mobx-react";
import { getFolder } from "@appserver/common/api/files";
import toastr from "studio/toastr";
import FilesListBody from "./FilesListBody";

class FilesListWrapper extends React.Component {
  constructor(props) {
    super(props);
    const { newFilter } = this.props;
    this.newFilter = newFilter;

    this.state = {
      isNextPageLoading: false,
      page: 0,
      hasNextPage: true,
      files: [],
    };
  }

  componentDidUpdate(prevProps) {
    const { folderId } = this.props;
    if (folderId !== prevProps.folderId) {
      this.setState({
        page: 0,
        files: [],
        hasNextPage: true,
      });
    }
  }

  _loadNextPage = () => {
    const { files, page } = this.state;
    const {
      folderId,
      setFolderTitle,
      setProviderKey,
      setFolderId,
      folderSelection,
    } = this.props;

    if (this._isLoadNextPage) return;

    this._isLoadNextPage = true;

    const pageCount = 30;
    this.newFilter.page = page;
    this.newFilter.pageCount = pageCount;

    this.setState({ isNextPageLoading: true }, async () => {
      try {
        const data = await getFolder(folderId, this.newFilter);

        if (page === 0 && folderSelection) {
          setFolderTitle(data.current.title);
          setProviderKey(data.current.providerKey);
          setFolderId(folderId);
        }

        const finalData = [...data.files];
        const newFilesList = [...files].concat(finalData);
        const hasNextPage = newFilesList.length < data.total - 1;
        this._isLoadNextPage = false;
        this.setState((state) => ({
          hasNextPage: hasNextPage,
          isNextPageLoading: false,
          page: state.page + 1,
          files: newFilesList,
        }));
      } catch (e) {
        toastr.error(e);
      }
    });
  };
  render() {
    const {
      t,
      theme,
      onSelectFile,
      folderSelection = false,
      fileId,
      folderId,
    } = this.props;
    const { hasNextPage, isNextPageLoading, files, page } = this.state;

    return (
      <FilesListBody
        theme={theme}
        files={files}
        onSelectFile={onSelectFile}
        hasNextPage={hasNextPage}
        isNextPageLoading={isNextPageLoading}
        loadNextPage={this._loadNextPage}
        folderId={folderId}
        displayType={"modal"}
        folderSelection={folderSelection}
        fileId={fileId}
        page={page}
      />
    );
  }
}

export default inject(
  ({ selectedFolderStore, selectFolderDialogStore, auth }) => {
    const { id } = selectedFolderStore;
    const {
      setFolderId,
      setFolderTitle,
      setProviderKey,
    } = selectFolderDialogStore;

    const { settingsStore } = auth;
    const { theme } = settingsStore;

    return {
      theme: theme,
      storeFolderId: id,
      setFolderId,
      setFolderTitle,
      setProviderKey,
    };
  }
)(observer(FilesListWrapper));
