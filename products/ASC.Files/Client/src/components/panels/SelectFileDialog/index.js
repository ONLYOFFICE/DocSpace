import React from "react";
import { Provider as MobxProvider } from "mobx-react";
import { inject, observer } from "mobx-react";
import { I18nextProvider } from "react-i18next";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import SelectFolderInput from "../SelectFolderInput";
import stores from "../../../store/index";
import i18n from "../SelectFileInput/i18n";
import { StyledAsidePanel, StyledSelectFilePanel } from "../StyledPanels";
import ModalDialog from "@appserver/components/modal-dialog";
import IconButton from "@appserver/components/icon-button";
import { getBackupFiles, getFolderInfo } from "@appserver/common/api/files";
import SelectFolderDialog from "../SelectFolderDialog";
import Text from "@appserver/components/text";
class SelectFileDialogBody extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isLoadingData: false,
      isAvailableFolders: true,
      certainFolders: true,
      isVisible: false,
      selectedFolder: "",
      filesList: [],
    };
    this.backupList;
    this.convertedData = [];
  }

  componentDidMount() {}
  componentDidUpdate(prevProps, prevState) {
    const { selectedFolder } = this.state;
    const { foldersType, getBackupFiles } = this.props;
    if (selectedFolder !== prevState.selectedFolder) {
      if (foldersType === "common") {
        //debugger;
        getBackupFiles(selectedFolder).then((filesList) =>
          this.setState({ filesList: filesList })
        );
        console.log("selectedFolder", selectedFolder);
      }
    }
  }

  onClickInput = () => {
    this.setState({
      isVisible: !this.state.isVisible,
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
      }
    );
    onSetFileName & onSetFileName(filesList[index].title);
  };
  render() {
    const { t, isPanelVisible, onClose, zIndex, foldersType } = this.props;
    const { isVisible, filesList, selectedFolder } = this.state;
    console.log("filesList", filesList);
    return (
      <StyledAsidePanel visible={isPanelVisible}>
        <ModalDialog visible={isPanelVisible} zIndex={zIndex} onClose={onClose}>
          <ModalDialog.Header>{t("SelectFile")}</ModalDialog.Header>
          <ModalDialog.Body>
            <StyledSelectFilePanel>
              <Text fontWeight="600">{t("ChooseByUser")}</Text>
              <SelectFolderInput
                onClickInput={this.onClickInput}
                onClose={this.onClose}
                onSelectFolder={this.onSelectFolder}
                isPanelVisible={isVisible}
                foldersType={foldersType}
                isNeedArrowIcon
              />
              <div className="modal-dialog_body-files-list">
                <Text fontWeight="600"> {"Список файлов:"}</Text>
                {filesList &&
                  filesList.map((data, index) => (
                    <div className="file-name">
                      <div
                        id={`${index}`}
                        key={`${index}`}
                        className="entry-title"
                        onClick={this.onFileClick}
                      >
                        {data.title.substring(0, data.title.indexOf(".gz"))}
                      </div>
                      <div className="file-exst">{".gz"}</div>
                    </div>
                  ))}
              </div>
            </StyledSelectFilePanel>
          </ModalDialog.Body>
        </ModalDialog>
      </StyledAsidePanel>
    );
  }
}

const SelectFileDialogWrapper = inject(
  ({ filesStore, treeFoldersStore, selectedFolderStore }) => {
    const { getBackupFiles, filter } = filesStore;
    const { expandedPanelKeys } = treeFoldersStore;
    return {
      getBackupFiles,
      expandedKeys: expandedPanelKeys
        ? expandedPanelKeys
        : selectedFolderStore.pathParts,
      filter,
    };
  }
)(observer(withTranslation(["SelectFile", "Common"])(SelectFileDialogBody)));

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
