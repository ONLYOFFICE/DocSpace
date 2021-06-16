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
      filesList: [],
      width: window.innerWidth,
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

  componentDidUpdate(prevProps, prevState) {
    const { selectedFolder } = this.state;

    if (selectedFolder !== prevState.selectedFolder && selectedFolder) {
      //debugger;
      this.setState({ isLoadingData: true }, function () {
        getFiles(selectedFolder)
          .then((filesList) => {
            this.setState({ filesList: filesList.files });
          })
          .finally(() => this.setState({ isLoadingData: false }));
      });
      console.log("selectedFolder", selectedFolder);
    }
  }

  componentWillUnmount() {
    window.removeEventListener("resize", this.updateWidth);
  }

  onClickInput = () => {
    this.setState({
      isVisible: true,
    });
  };
  onClose = () => {
    this.setState({
      isVisible: false,
    });
  };
  onSelectFolder = (id) => {
    this.setState({
      selectedFolder: id,
    });
  };

  onFileClick = (e) => {
    console.log("e", e.target.id);
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
  onSetLoadingData = (loading) => {
    this.setState({
      isLoadingData: loading,
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
    const { isVisible, filesList, isLoadingData, width } = this.state;

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
        onFileClick={this.onFileClick}
        onClickInput={this.onClickInput}
        onCloseSelectFolderDialog={this.onClose}
        onSelectFolder={this.onSelectFolder}
        onSetLoadingData={this.onSetLoadingData}
      />
    ) : (
      <SelectFileDialogModalView
        t={t}
        isPanelVisible={isPanelVisible}
        onClose={onClose}
        onSelectFolder={this.onSelectFolder}
        foldersType={foldersType}
        onFileClick={this.onFileClick}
        filesList={filesList}
        isLoadingData={isLoadingData}
        onSelectFolder={this.onSelectFolder}
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
