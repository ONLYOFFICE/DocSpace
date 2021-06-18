import React from "react";
import { Provider as MobxProvider } from "mobx-react";
import { inject, observer } from "mobx-react";
import { I18nextProvider } from "react-i18next";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";

import stores from "../../../store/index";
import i18n from "../SelectFileInput/i18n";
import SelectFileDialogModalView from "./modalView";
import SelectFileDialogAsideView from "./asideView";
import { getFiles } from "@appserver/common/api/files";

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
    };
    this.timeoutId = null;
  }

  componentDidMount() {
    window.addEventListener("resize", this.updateWidth);
  }

  updateWidth = () => {
    clearTimeout(this.timeoutId);
    this.timeoutId = setTimeout(
      () => this.setState({ width: window.innerWidth }),
      150
    );
  };

  componentWillUnmount() {
    window.removeEventListener("resize", this.updateWidth);
  }

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
    });
  };

  onSelectFile = (e) => {
    const { onSetFileName, onClose } = this.props;
    const { filesList } = this.state;
    const index = e.target.id;

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
  loadNextPage = ({ startIndex }) => {
    //debugger;
    console.log(`loadNextPage(startIndex=${startIndex}")`);
    const { selectedFolder } = this.state;
    const pageCount = 30;
    console.log("selectedFolder", selectedFolder);
    this.setState({ isNextPageLoading: true }, () => {
      getFiles(selectedFolder, pageCount, startIndex)
        .then((response) => {
          //debugger;
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
    } = this.props;
    const {
      isVisible,
      filesList,
      isLoadingData,
      width,
      hasNextPage,
      isNextPageLoading,
    } = this.state;

    return width < 1024 ? (
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
      />
    ) : (
      <SelectFileDialogModalView
        t={t}
        isPanelVisible={isPanelVisible}
        onClose={onClose}
        onSelectFolder={this.onSelectFolder}
        foldersType={foldersType}
        onClickFile={this.onClickFile}
        filesList={filesList}
        isLoadingData={isLoadingData}
        onSelectFolder={this.onSelectFolder}
        hasNextPage={hasNextPage}
        isNextPageLoading={isNextPageLoading}
        loadNextPage={this.loadNextPage}
      />
    );
  }
}

const SelectFileDialogWrapper = inject(
  ({ filesStore, treeFoldersStore, selectedFolderStore }) => {
    const { filter } = filesStore;
    const { expandedPanelKeys } = treeFoldersStore;
    return {
      expandedKeys: expandedPanelKeys
        ? expandedPanelKeys
        : selectedFolderStore.pathParts,
      filter,
    };
  }
)(
  observer(
    withTranslation(["SelectFile", "Common", "Home"])(SelectFileDialogBody)
  )
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
