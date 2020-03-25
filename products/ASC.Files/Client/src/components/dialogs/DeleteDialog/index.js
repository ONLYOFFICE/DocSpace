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
import { fetchFiles, setTreeFolders } from "../../../store/files/actions";
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

    this.state = { isLoading: false, foldersList, filesList, selection };
  }

  loop = (path, item, folderId, folders, foldersCount) => {
    const newPath = path;
    while (path.length !== 0) {
      const newItems = item.find(x => x.id === path[0]);
      newPath.shift();
      if (path.length === 0) {
        const currentItem = item.find(x => x.id === folderId);
        currentItem.folders = folders;
        currentItem.foldersCount = foldersCount;
        return;
      }
      this.loop(newPath, newItems.folders, folderId, folders, foldersCount);
    }
  };

  onDelete = () => {
    const { isRecycleBinFolder, onClose } = this.props;
    const { selection } = this.state;

    const deleteAfter = true; //Delete after finished
    const immediately = isRecycleBinFolder ? true : false; //Don't move to the Recycle Bin
    const successMessage = "Files and folders was deleted";

    const folderIds = [];
    const fileIds = [];

    let i = 0;
    while (selection.length !== i) {
      if (selection[i].fileExst && selection[i].checked) {
        fileIds.push(selection[i].id.toString());
      } else if (selection[i].checked) {
        folderIds.push(selection[i].id.toString());
      }
      i++;
    }

    this.setState({ isLoading: true }, () => {
      const {
        currentFolderId,
        filter,
        treeFolders,
        setTreeFolders
      } = this.props;

      files
        .removeFiles(folderIds, fileIds, deleteAfter, immediately)

        .then(() =>
          fetchFiles(currentFolderId, filter, store.dispatch).then(data => {
            if (!isRecycleBinFolder) {
              const path = data.selectedFolder.pathParts;
              const newTreeFolders = treeFolders;
              const folders = data.selectedFolder.folders;
              const foldersCount = data.selectedFolder.foldersCount;
              this.loop(
                path,
                newTreeFolders,
                currentFolderId,
                folders,
                foldersCount
              );
              setTreeFolders(newTreeFolders);
            }
          })
        )
        .then(() => toastr.success(successMessage))

        .catch(err => {
          toastr.error(err);
        })
        .finally(() => {
          this.setState({ isLoading: false }, () => onClose());
        });
    });
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
    const { onClose, visible, t } = this.props;
    const { isLoading, filesList, foldersList, selection } = this.state;

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

export default connect(mapStateToProps, { setTreeFolders })(
  withRouter(DeleteDialog)
);
