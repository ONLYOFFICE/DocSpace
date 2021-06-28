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
    };
    this.throttledResize = throttle(this.setDisplayType, 300);
  }

  componentDidMount() {
    const { isPanelVisible } = this.props;
    if (isPanelVisible) {
      window.addEventListener("resize", this.throttledResize);
    }
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
    });
  };

  onSelectFile = (e) => {
    const { onSetFileName, onClose } = this.props;
    const { filesList } = this.state;
    const index = e.target.dataset.index;

    if (!index) return;
    this.setState(
      {
        selectedFile: filesList[index].id,
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
  loadNextPage = ({ startIndex = 0 }) => {
    //debugger;
    const { filterValue, filterType, withSubfolders } = this.props;
    const { selectedFolder } = this.state;

    console.log(`loadNextPage(startIndex=${startIndex}")`);

    const pageCount = 30;

    console.log("selectedFolder", selectedFolder);

    this.setState({ isNextPageLoading: true }, () => {
      getFiles(
        selectedFolder,
        filterType,
        filterValue,
        withSubfolders,
        pageCount,
        startIndex
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
      isCommonWithoutProvider,
      iconUrl,
    } = this.props;
    const {
      isVisible,
      filesList,
      isLoadingData,
      hasNextPage,
      isNextPageLoading,
      selectedFolder,
      displayType,
    } = this.state;

    return displayType === "aside" ? (
      <SelectFileDialogAsideView
        t={t}
        isPanelVisible={isPanelVisible}
        zIndex={zIndex}
        onClose={onClose}
        isVisible={isVisible}
        isCommonWithoutProvider={isCommonWithoutProvider}
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
        iconUrl={iconUrl}
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
        isCommonWithoutProvider={isCommonWithoutProvider}
        iconUrl={iconUrl}
      />
    );
  }
}

const SelectFileDialogWrapper = withTranslation([
  "SelectFile",
  "Common",
  "Home",
])(SelectFileDialogBody);
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
