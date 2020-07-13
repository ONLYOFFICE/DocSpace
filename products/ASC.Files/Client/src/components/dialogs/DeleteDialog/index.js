import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import ModalDialogContainer from "../ModalDialogContainer";
import {
  toastr,
  ModalDialog,
  Button,
  Text,
  Checkbox,
  Scrollbar
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import { api, utils } from "asc-web-common";
import { fetchFiles, setTreeFolders, getProgress, setProgressBarData, clearProgressData } from "../../../store/files/actions";
import { loopTreeFolders } from "../../../store/files/selectors";
import store from "../../../store/store";

const { files } = api;
const { changeLanguage } = utils;

class DeleteDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    const foldersList = [];
    const filesList = [];
    const selection = [];

    let i = 0;
    while (props.selection.length !== i) {
      selection.push({ ...props.selection[i], checked: true });
      if (selection[i].fileExst) {
        filesList.push(selection[i]);
      } else {
        foldersList.push(selection[i]);
      }
      i++;
    }
    changeLanguage(i18n);

    this.state = { foldersList, filesList, selection };
  }

  loopDeleteOperation = id => {
    const { currentFolderId, filter, treeFolders, setTreeFolders, isRecycleBinFolder, getProgress, setProgressBarData, t } = this.props;
    const successMessage = "Files and folders was deleted";
    getProgress().then(res => {
      const currentProcess = res.find(x => x.id === id);
      if(currentProcess && currentProcess.progress !== 100) {
        setProgressBarData({ percent: currentProcess.progress, label: t("DeleteOperation"), visible: true });
        setTimeout(() => this.loopDeleteOperation(id), 1000);
      } else {
        setProgressBarData({ percent: 100, label: t("DeleteOperation"), visible: true });
        fetchFiles(currentFolderId, filter, store.dispatch).then(data => {
          if (!isRecycleBinFolder) {
            const path = data.selectedFolder.pathParts.slice(0);
            const newTreeFolders = treeFolders;
            const folders = data.selectedFolder.folders;
            const foldersCount = data.selectedFolder.foldersCount;
            loopTreeFolders(path, newTreeFolders, folders, foldersCount);
            setTreeFolders(newTreeFolders);
          }
          toastr.success(successMessage);
        })
      }
    })
    .catch(err => {
      toastr.error(err);
      clearProgressData(store.dispatch);
    })
    .finally(() => setTimeout(() => clearProgressData(store.dispatch), 5000))
  }

  onDelete = () => {
    const { isRecycleBinFolder, onClose, t, setProgressBarData } = this.props;
    const { selection } = this.state;

    const deleteAfter = true; //Delete after finished
    const immediately = isRecycleBinFolder ? true : false; //Don't move to the Recycle Bin
    

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
    if(folderIds.length || fileIds.length) {
      setProgressBarData({ visible: true, label: t("DeleteOperation"), percent: 0 });

      files
        .removeFiles(folderIds, fileIds, deleteAfter, immediately)
        .then(res => {
          const id = res[0] && res[0].id ? res[0].id : null;
          this.loopDeleteOperation(id);
        })
        .catch(err => {
          toastr.error(err);
          clearProgressData(store.dispatch);
        })
    }
  };

  onChange = event => {
    const value = event.target.value.split("/");
    const fileType = value[0];
    const id = Number(value[1]);

    const newSelection = this.state.selection;

    if (fileType !== "undefined") {
      const a = newSelection.find(x => x.id === id && x.fileExst);
      a.checked = !a.checked;
      this.setState({ selection: newSelection });
    } else {
      const a = newSelection.find(x => x.id === id && !x.fileExst);
      a.checked = !a.checked;
      this.setState({ selection: newSelection });
    }
  };

  render() {
    const { onClose, visible, t, isLoading } = this.props;
    const { filesList, foldersList, selection } = this.state;

    const checkedSelections = selection.filter(x => x.checked === true);

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
        <ModalDialog
          visible={visible}
          onClose={onClose}
          headerContent={t("ConfirmationTitle")}
          bodyContent={
            <>
              <div className="modal-dialog-content">
                <Text className="delete_dialog-header-text">
                  {questionMessage}
                </Text>
                <Scrollbar
                  style={{ height, maxHeight: 330 }}
                  stype="mediumBlack"
                >
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
            </>
          }
          footerContent={
            <>
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
            </>
          }
        />
      </ModalDialogContainer>
    );
  }
}

const ModalDialogContainerTranslated = withTranslation()(DeleteDialogComponent);

const DeleteDialog = props => (
  <ModalDialogContainerTranslated i18n={i18n} {...props} />
);

const mapStateToProps = state => {
  const { selectedFolder, filter, treeFolders } = state.files;
  return {
    currentFolderId: selectedFolder.id,
    filter,
    treeFolders
  };
};

export default connect(mapStateToProps, { setTreeFolders, getProgress, setProgressBarData })(
  withRouter(DeleteDialog)
);
