import React from "react";
import { Provider as MobxProvider } from "mobx-react";
import { I18nextProvider } from "react-i18next";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import throttle from "lodash/throttle";

import stores from "../../../store/index";
import i18n from "../SelectFileInput/i18n";
import SelectFileDialogModalView from "./modalView";
import SelectFileDialogAsideView from "./asideView";
import { getFiles } from "@appserver/common/api/files";
import utils from "@appserver/components/utils";

const { desktop } = utils.device;
class SelectFileDialogBody extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isLoadingData: false,
      isVisible: false,
      selectedFolder: "",
      selectedFile: "",
      defaultSelectedFile: "",
      fileName: "",
      defaultFileName: "",
      filesList: [],
      width: window.innerWidth,
      isChecked: false,
      hasNextPage: true,
      isNextPageLoading: false,
      displayType: this.getDisplayType(),
      selectedKeys: "",
      filterParams: this.getFilterParameters(),
    };
    this.throttledResize = throttle(this.setDisplayType, 300);
  }

  getFilterParameters = () => {
    const {
      isImageOnly,
      isDocumentsOnly,
      isArchiveOnly,
      isPresentationOnly,
      isTablesOnly,
      isMediaOnly,
      searchParam = "",
    } = this.props;

    if (isImageOnly) {
      return { filterType: "7", filterValue: searchParam };
    }
    if (isDocumentsOnly) {
      return { filterType: "3", filterValue: searchParam };
    }
    if (isArchiveOnly) {
      return { filterType: "10", filterValue: searchParam };
    }
    if (isPresentationOnly) {
      return { filterType: "4", filterValue: searchParam };
    }
    if (isTablesOnly) {
      return { filterType: "5", filterValue: searchParam };
    }
    if (isMediaOnly) {
      return { filterType: "12", filterValue: searchParam };
    }
    return { filterType: "1", filterValue: "" };
  };

  componentDidMount() {
    const { isPanelVisible } = this.props;

    window.addEventListener("resize", this.throttledResize);
  }
  componentWillUnmount() {
    if (this.throttledResize) {
      this.throttledResize && this.throttledResize.cancel();
      window.removeEventListener("resize", this.throttledResize);
    }
  }

  getDisplayType = () => {
    const displayType =
      window.innerWidth < desktop.match(/\d+/)[0] ? "aside" : "modal";

    return displayType;
  };

  setDisplayType = () => {
    const displayType = this.getDisplayType();

    this.setState({ displayType: displayType });
  };

  onClickInput = () => {
    this.setState({
      isVisible: true,
    });
  };

  onCloseSelectFolderDialog = () => {
    this.setState({
      isVisible: false,
    });
  };

  onSelectFolder = (id) => {
    this.setState({
      selectedFolder: id,
      hasNextPage: true,
      filesList: [],
      selectedKeys: id,
    });
  };

  onSelectFile = (e) => {
    const { onSetFileName, onClose } = this.props;
    const { filesList } = this.state;
    const index = e.target.dataset.index;

    if (!index) return;
    this.setState(
      {
        selectedFile: filesList[index].id, //object sent
      },
      function () {
        onClose && onClose();
        onSetFileName & onSetFileName(filesList[index].title);
      }
    );
  };

  onClickFile = (e) => {
    const { filesList } = this.state;
    const index = +e.target.id;

    this.setState({
      selectedFile: filesList[index].id,
      fileName: filesList[index].title,
    });
  };
  onClickSave = () => {
    const { onSetFileName, onClose, onSetFileId } = this.props;
    const { fileName, selectedFile } = this.state;
    onSetFileName & onSetFileName(fileName);
    onSetFileId & onSetFileId(selectedFile);
    onClose && onClose();
  };

  onCloseModalView = () => {
    this.setState({
      isChecked: false,
    });
  };

  onSetLoadingData = (loading) => {
    this.setState({
      isLoadingData: loading,
    });
  };
  loadNextPage = ({ startIndex }) => {
    //debugger;
    const { withSubfolders } = this.props;
    const { selectedFolder, filterParams } = this.state;

    console.log(`loadNextPage(startIndex=${startIndex}")`);

    const pageCount = 30;

    this.setState({ isNextPageLoading: true }, () => {
      getFiles(
        selectedFolder,
        filterParams.filterType,
        filterParams.filterValue,
        pageCount,
        startIndex,
        withSubfolders
      )
        .then((response) => {
          let newFilesList = startIndex
            ? this.state.filesList.concat(response.files)
            : response.files;
          console.log("newFilesList", newFilesList);

          this.setState({
            hasNextPage: newFilesList.length < response.total,
            isNextPageLoading: false,
            filesList: newFilesList,
          });
        })
        .catch((error) => console.log(error));
    });
  };
  render() {
    const {
      t,
      isPanelVisible,
      onClose,
      zIndex,
      foldersType,
      withoutProvider,
      folderId,
      header,
      modalHeightContent,
      loadingLabel,
    } = this.props;
    const {
      isVisible,
      filesList,
      isLoadingData,
      hasNextPage,
      isNextPageLoading,
      selectedFolder,
      displayType,
      selectedKeys,
    } = this.state;
    const loadingText = loadingLabel
      ? loadingLabel
      : `${t("Common:LoadingProcessing")} ${t("Common:LoadingDescription")}`;
    return displayType === "aside" ? (
      <SelectFileDialogAsideView
        t={t}
        isPanelVisible={isPanelVisible}
        zIndex={zIndex}
        onClose={onClose}
        isVisible={isVisible}
        withoutProvider={withoutProvider}
        foldersType={foldersType}
        filesList={filesList}
        isLoadingData={isLoadingData}
        onSelectFile={this.onSelectFile}
        onClickInput={this.onClickInput}
        onCloseSelectFolderDialog={this.onCloseSelectFolderDialog}
        onSelectFolder={this.onSelectFolder}
        onSetLoadingData={this.onSetLoadingData}
        hasNextPage={hasNextPage}
        isNextPageLoading={isNextPageLoading}
        loadNextPage={this.loadNextPage}
        selectedFolder={selectedFolder}
        header={header}
        loadingText={loadingText}
      />
    ) : (
      <SelectFileDialogModalView
        t={t}
        isPanelVisible={isPanelVisible}
        onClose={onClose}
        onSelectFolder={this.onSelectFolder}
        onSelectFile={this.onSelectFile}
        foldersType={foldersType}
        onClickFile={this.onClickFile}
        filesList={filesList}
        isLoadingData={isLoadingData}
        hasNextPage={hasNextPage}
        isNextPageLoading={isNextPageLoading}
        loadNextPage={this.loadNextPage}
        selectedFolder={selectedFolder}
        withoutProvider={withoutProvider}
        selectedKeys={selectedKeys}
        folderId={folderId}
        header={header}
        modalHeightContent={modalHeightContent}
        loadingText={loadingText}
      />
    );
  }
}
SelectFileDialogBody.propTypes = {
  onClose: PropTypes.func.isRequired,
  isPanelVisible: PropTypes.bool.isRequired,
  foldersType: PropTypes.oneOf(["common", "third-party"]),
  folderId: PropTypes.string,
  withoutProvider: PropTypes.bool,
  header: PropTypes.string,
  zIndex: PropTypes.number,
};

SelectFileDialogModalView.defaultProps = {
  folderId: "",
  header: "",
  withoutProvider: false,
  zIndex: 310,
};

const SelectFileDialogWrapper = withTranslation(["SelectFile", "Common"])(
  SelectFileDialogBody
);
class SelectFileDialog extends React.Component {
  render() {
    return (
      <MobxProvider {...stores}>
        <I18nextProvider i18n={i18n}>
          <SelectFileDialogWrapper {...this.props} />
        </I18nextProvider>
      </MobxProvider>
    );
  }
}

export default SelectFileDialog;
