import React from "react";
import { inject, observer } from "mobx-react";
import toastr from "client/toastr";
import FilesListBody from "./FilesListBody";
import axios from "axios";
import { combineUrl, getFolderOptions } from "@docspace/common/utils";
import AppServerConfig from "@docspace/common/constants/AppServerConfig";

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
      setFolderId,
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
        const options = getFolderOptions(folderId, this.newFilter);

        const response = await axios
          .get(combineUrl(AppServerConfig.apiPrefixURL, options.url), {
            cancelToken: this.source.token,
          })
          .catch((thrown) => {
            if (axios.isCancel(thrown)) {
              console.log("Request canceled", thrown.message);
            } else {
              console.error(thrown);
            }
            return;
          });
        if (!response) return;
        const data = response.data.response;

        if (page === 0 && folderSelection) {
          setFolderTitle(data.current.title);
          setProviderKey(data.current.providerKey);
          setFolderId(folderId);
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
