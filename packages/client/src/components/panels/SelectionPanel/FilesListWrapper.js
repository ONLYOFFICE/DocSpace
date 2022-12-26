import React from "react";
import { inject, observer } from "mobx-react";
import toastr from "@docspace/components/toast/toastr";
import FilesListBody from "./FilesListBody";
import axios from "axios";
import { getFolder } from "@docspace/common/api/files";

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
    this._isMount = false;
  }

  componentDidMount() {
    this._isMount = true;
  }

  componentDidUpdate(prevProps) {
    const { folderId } = this.props;
    const { isNextPageLoading } = this.state;

    if (folderId !== prevProps.folderId) {
      if (isNextPageLoading) {
        this.source.cancel();

        this._isLoadNextPage = false;
        this.setState({
          isNextPageLoading: false,
        });
      }
      this.setState({
        page: 0,
        files: [],
        hasNextPage: true,
      });
    }
  }

  componentWillUnmount() {
    this._isMount = false;
  }
  _loadNextPage = () => {
    const { files, page } = this.state;
    const {
      folderId,
      setFolderTitle,
      setProviderKey,
      setResultingFolderId,
      folderSelection,
    } = this.props;

    if (this._isLoadNextPage) return;

    const pageCount = 30;
    this.newFilter.page = page;
    this.newFilter.pageCount = pageCount;
    this._isLoadNextPage = true;
    this.setState({ isNextPageLoading: true }, async () => {
      try {
        this.CancelToken = axios.CancelToken;
        this.source = this.CancelToken.source();

        const data = await getFolder(
          folderId,
          this.newFilter,
          this.source.token
        ).catch((err) => {
          if (axios.isCancel(err)) {
            console.log("Request canceled", err.message);
          } else {
            const errorBody = err.response;

            if (errorBody.data && errorBody.data.error) {
              toastr.error(errorBody.data.error.message);
            }
          }
          return;
        });

        if (!data) return;

        if (page === 0 && folderSelection) {
          setFolderTitle(data.current.title);
          setProviderKey(data.current.providerKey);
          setResultingFolderId(folderId);
        }

        const finalData = [...data.files];
        const newFilesList = [...files].concat(finalData);
        const hasNextPage = newFilesList.length < data.total - 1;
        this._isLoadNextPage = false;

        this._isMount &&
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
      maxHeight,
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
        maxHeight={maxHeight}
      />
    );
  }
}

export default inject(
  ({ selectedFolderStore, selectFolderDialogStore, auth }) => {
    const { id } = selectedFolderStore;
    const {
      setResultingFolderId,
      setFolderTitle,
      setProviderKey,
    } = selectFolderDialogStore;

    const { settingsStore } = auth;
    const { theme } = settingsStore;

    return {
      theme: theme,
      storeFolderId: id,
      setResultingFolderId,
      setFolderTitle,
      setProviderKey,
    };
  }
)(observer(FilesListWrapper));
