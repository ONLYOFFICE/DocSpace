import React from "react";
import { withRouter } from "react-router";
import ModalDialogContainer from "../ModalDialogContainer";
import {
  ModalDialog,
  Button,
  Text,
  Checkbox,
  Scrollbar,
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import { api, toastr } from "asc-web-common";
import { TIMEOUT } from "../../../helpers/constants";
import { loopTreeFolders } from "../../../helpers/files-helpers";
import { inject, observer } from "mobx-react";

const { files } = api;

class DeleteDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    const foldersList = [];
    const filesList = [];
    const selection = [];

    let i = 0;
    while (props.selection.length !== i) {
      if (!(props.isRootFolder && props.selection[i].providerKey)) {
        if (
          props.selection[i].access === 0 ||
          props.selection[i].access === 1
        ) {
          const item = { ...props.selection[i], checked: true };
          selection.push(item);
          if (props.selection[i].fileExst) {
            filesList.push(item);
          } else {
            foldersList.push(item);
          }
        }
      }
      i++;
    }

    this.state = { foldersList, filesList, selection };
  }

  loopDeleteOperation = (id) => {
    const {
      currentFolderId,
      filter,
      treeFolders,
      setTreeFolders,
      isRecycleBinFolder,
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
      t,
      fetchFiles,
    } = this.props;
    const successMessage = isRecycleBinFolder
      ? t("DeleteFromTrash")
      : t("DeleteSelectedElem");
    api.files
      .getProgress()
      .then((res) => {
        const currentProcess = res.find((x) => x.id === id);
        if (currentProcess && currentProcess.progress !== 100) {
          setSecondaryProgressBarData({
            icon: "trash",
            percent: currentProcess.progress,
            label: t("DeleteOperation"),
            visible: true,
            alert: false,
          });
          setTimeout(() => this.loopDeleteOperation(id), 1000);
        } else {
          setSecondaryProgressBarData({
            icon: "trash",
            percent: 100,
            label: t("DeleteOperation"),
            visible: true,
            alert: false,
          });
          setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
          fetchFiles(currentFolderId, filter).then((data) => {
            if (!isRecycleBinFolder && !!this.state.foldersList.length) {
              const path = data.selectedFolder.pathParts.slice(0);
              const newTreeFolders = treeFolders;
              const folders = data.selectedFolder.folders;
              const foldersCount = data.selectedFolder.foldersCount;
              loopTreeFolders(path, newTreeFolders, folders, foldersCount);
              setTreeFolders(newTreeFolders);
            }
            toastr.success(successMessage);
          });
        }
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        //toastr.error(err);
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
  };

  onDelete = () => {
    const {
      isRecycleBinFolder,
      isPrivacy,
      onClose,
      t,
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.props;
    const { selection } = this.state;

    const deleteAfter = true; //Delete after finished
    const immediately = isRecycleBinFolder || isPrivacy ? true : false; //Don't move to the Recycle Bin

    const folderIds = [];
    const fileIds = [];

    let i = 0;
    while (selection.length !== i) {
      if (selection[i].fileExst && selection[i].checked) {
        fileIds.push(selection[i].id);
      } else if (selection[i].checked) {
        folderIds.push(selection[i].id);
      }
      i++;
    }

    onClose();
    if (folderIds.length || fileIds.length) {
      setSecondaryProgressBarData({
        icon: "trash",
        visible: true,
        label: t("DeleteOperation"),
        percent: 0,
        alert: false,
      });

      files
        .removeFiles(folderIds, fileIds, deleteAfter, immediately)
        .then((res) => {
          const id = res[0] && res[0].id ? res[0].id : null;
          this.loopDeleteOperation(id);
        })
        .catch((err) => {
          setSecondaryProgressBarData({
            visible: true,
            alert: true,
          });
          //toastr.error(err);
          setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
        });
    }
  };

  onChange = (event) => {
    const value = event.target.value.split("/");
    const fileType = value[0];
    const id = Number(value[1]);

    const newSelection = this.state.selection;

    if (fileType !== "undefined") {
      const selection = newSelection.find((x) => x.id === id && x.fileExst);
      selection.checked = !selection.checked;
      this.setState({ selection: newSelection });
    } else {
      const selection = newSelection.find((x) => x.id === id && !x.fileExst);
      selection.checked = !selection.checked;
      this.setState({ selection: newSelection });
    }
  };

  render() {
    const { onClose, visible, t, isLoading } = this.props;
    const { filesList, foldersList, selection } = this.state;

    const checkedSelections = selection.filter((x) => x.checked === true);

    const questionMessage =
      checkedSelections.length === 1
        ? checkedSelections[0].fileExst
          ? t("QuestionDeleteFile")
          : t("QuestionDeleteFolder")
        : t("QuestionDeleteElements");

    const accuracy = 20;
    let filesHeight = 25 * filesList.length + accuracy + 8;
    let foldersHeight = 25 * foldersList.length + accuracy;
    if (foldersList.length === 0) {
      foldersHeight = 0;
    }
    if (filesList.length === 0) {
      filesHeight = 0;
    }

    const height = filesHeight + foldersHeight;

    return (
      <ModalDialogContainer>
        <ModalDialog visible={visible} onClose={onClose}>
          <ModalDialog.Header>{t("ConfirmationTitle")}</ModalDialog.Header>
          <ModalDialog.Body>
            <div className="modal-dialog-content">
              <Text className="delete_dialog-header-text">
                {questionMessage}
              </Text>
              <Scrollbar style={{ height, maxHeight: 330 }} stype="mediumBlack">
                {foldersList.length > 0 && (
                  <Text isBold>{t("FoldersModule")}:</Text>
                )}
                {foldersList.map((item, index) => (
                  <Checkbox
                    truncate
                    className="modal-dialog-checkbox"
                    value={`${item.fileExst}/${item.id}`}
                    onChange={this.onChange}
                    key={`checkbox_${index}`}
                    isChecked={item.checked}
                    label={item.title}
                  />
                ))}

                {filesList.length > 0 && (
                  <Text isBold className="delete_dialog-text">
                    {t("FilesModule")}:
                  </Text>
                )}
                {filesList.map((item, index) => (
                  <Checkbox
                    truncate
                    className="modal-dialog-checkbox"
                    value={`${item.fileExst}/${item.id}`}
                    onChange={this.onChange}
                    key={`checkbox_${index}`}
                    isChecked={item.checked}
                    label={item.title}
                  />
                ))}
              </Scrollbar>
            </div>
          </ModalDialog.Body>
          <ModalDialog.Footer>
            <Button
              className="button-dialog-accept"
              key="OkButton"
              label={t("OKButton")}
              size="medium"
              primary
              onClick={this.onDelete}
              isLoading={isLoading}
            />
            <Button
              className="button-dialog"
              key="CancelButton"
              label={t("CancelButton")}
              size="medium"
              onClick={onClose}
              isLoading={isLoading}
            />
          </ModalDialog.Footer>
        </ModalDialog>
      </ModalDialogContainer>
    );
  }
}

const DeleteDialog = withTranslation("DeleteDialog")(DeleteDialogComponent);

export default inject(
  ({
    initFilesStore,
    filesStore,
    uploadDataStore,
    treeFoldersStore,
    selectedFolderStore,
  }) => {
    const { isLoading } = initFilesStore;
    const { secondaryProgressDataStore } = uploadDataStore;
    const { fetchFiles, selection, filter } = filesStore;

    const {
      treeFolders,
      setTreeFolders,
      isRecycleBinFolder,
      isPrivacyFolder,
    } = treeFoldersStore;

    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = secondaryProgressDataStore;

    return {
      currentFolderId: selectedFolderStore.id,
      selection,
      isLoading,
      treeFolders,
      isRecycleBinFolder,
      isPrivacy: isPrivacyFolder,
      filter,
      isRootFolder: selectedFolderStore.isRootFolder,

      fetchFiles,
      setTreeFolders,
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    };
  }
)(withRouter(observer(DeleteDialog)));
