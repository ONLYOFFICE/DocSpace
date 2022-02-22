import React from "react";
import PropTypes from "prop-types";
import { withTranslation } from "react-i18next";
import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import Loader from "@appserver/components/loader";
import Link from "@appserver/components/link";
import { StyledBackupList } from "../StyledBackup";
import {
  deleteBackup,
  deleteBackupHistory,
  getBackupHistory,
  startRestore,
} from "../../../../../../../../../packages/asc-web-common/api/portal";
import BackupListBody from "./BackupListBody";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import config from "../../../../../../../package.json";
import toastr from "@appserver/components/toast/toastr";

class BackupListModalDialog extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isLoading: true,
      filesList: [],
      selectedFileIndex: null,
      selectedFileId: null,
    };
  }
  componentDidMount() {
    getBackupHistory()
      .then((filesList) =>
        this.setState({
          filesList,
          isLoading: false,
        })
      )
      .catch(() => this.setState({ isLoading: false }));
  }

  onSelectFile = (e) => {
    const fileInfo = e.target.name;
    const fileArray = fileInfo.split("_");
    const id = fileArray.pop();
    const index = fileArray.shift();

    this.setState({
      selectedFileIndex: +index,
      selectedFileId: id,
    });
  };

  onCleanBackupList = () => {
    this.setState({ isLoading: true }, function () {
      deleteBackupHistory()
        .then(() => getBackupHistory())
        .then((filesList) => this.setState({ filesList, isLoading: false }))
        .catch((error) => {
          toastr.error(error);
          this.setState({ isLoading: false });
        });
    });
  };
  onDeleteBackup = (backupId) => {
    if (!backupId) return;

    this.setState({ isLoading: true }, function () {
      deleteBackup(backupId)
        .then(() => getBackupHistory())
        .then((filesList) =>
          this.setState({
            filesList,
            isLoading: false,
            selectedFileIndex: null,
            selectedFileId: null,
          })
        )
        .catch((error) => {
          toastr.error(error);
          this.setState({ isLoading: false });
        });
    });
  };
  onRestorePortal = () => {
    const { selectedFileId } = this.state;
    const { isNotify, isCopyingToLocal, history } = this.props;

    if (!selectedFileId) return;
    if (isCopyingToLocal) return;
    this.setState({ isLoading: true }, function () {
      const backupId = selectedFileId;
      const storageType = "0";
      const storageParams = [
        {
          key: "fileId",
          value: backupId,
        },
      ];

      startRestore(backupId, storageType, storageParams, isNotify)
        .then(() =>
          history.push(
            combineUrl(
              AppServerConfig.proxyURL,
              config.homepage,
              "/preparation-portal"
            )
          )
        )
        .catch((error) => toastr.error(error))
        .finally(() =>
          this.setState({
            isLoading: false,
            selectedFileIndex: null,
            selectedFileId: null,
          })
        );
    });
  };
  render() {
    const { onModalClose, isVisibleDialog, t, isCopyingToLocal } = this.props;
    const { filesList, isLoading, selectedFileIndex } = this.state;

    return (
      <ModalDialog
        visible={isVisibleDialog}
        onClose={onModalClose}
        displayType="aside"
        removeScroll
        contentHeight="100%"
      >
        <ModalDialog.Header>{t("BackupList")}</ModalDialog.Header>
        <ModalDialog.Body>
          <StyledBackupList isCopyingToLocal={isCopyingToLocal}>
            {filesList.length > 0 && (
              <div className="backup-restore_dialog-header">
                <Text fontSize="12px" style={{ marginBottom: "10px" }}>
                  {t("BackupListDeleteWarning")}
                </Text>
                <Link
                  onClick={this.onCleanBackupList}
                  fontWeight={600}
                  style={{ textDecoration: "underline dotted" }}
                >
                  {t("ClearList")}
                </Link>
              </div>
            )}
            <div className="backup-restore_dialog-scroll-body">
              {!isLoading ? (
                filesList.length > 0 ? (
                  <BackupListBody
                    filesList={filesList}
                    onDeleteBackup={this.onDeleteBackup}
                    onSelectFile={this.onSelectFile}
                    selectedFileIndex={selectedFileIndex}
                  />
                ) : (
                  <Text fontSize="12px" color="#A3A9AE" textAlign="center">
                    {t("EmptyBackupList")}
                  </Text>
                )
              ) : (
                <div className="loader" key="loader">
                  <Loader
                    type="oval"
                    size="16px"
                    style={{
                      display: "inline",
                      marginRight: "10px",
                    }}
                  />
                  <Text as="span">{`${t("Common:LoadingProcessing")} ${t(
                    "Common:LoadingDescription"
                  )}`}</Text>
                </div>
              )}
            </div>
          </StyledBackupList>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <StyledBackupList>
            <Button
              primary
              className="restore_dialog-button"
              size="medium"
              label={t("Translations:Restore")}
              onClick={this.onRestorePortal}
            />
            <Button
              className="restore_dialog-button"
              size="medium"
              label={t("Common:CloseButton")}
              onClick={onModalClose}
            />
          </StyledBackupList>
        </ModalDialog.Footer>
      </ModalDialog>
    );
  }
}

BackupListModalDialog.propTypes = {
  onModalClose: PropTypes.func.isRequired,
  isVisibleDialog: PropTypes.bool.isRequired,
};

export default withTranslation(["Settings", "Common", "Translations"])(
  BackupListModalDialog
);
