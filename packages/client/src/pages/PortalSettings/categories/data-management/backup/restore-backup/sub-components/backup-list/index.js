import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import { withTranslation } from "react-i18next";
import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import Link from "@docspace/components/link";
import {
  deleteBackup,
  deleteBackupHistory,
  getBackupHistory,
  startRestore,
} from "@docspace/common/api/portal";
import toastr from "@docspace/components/toast/toastr";
import Loaders from "@docspace/common/components/Loaders";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import Checkbox from "@docspace/components/checkbox";
import HelpButton from "@docspace/components/help-button";
import config from "PACKAGE_FILE";
import { StyledBackupList } from "../../../StyledBackup";
import BackupListBody from "./BackupListBody";

class BackupListModalDialog extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isLoading: true,
      filesList: [],
      selectedFileIndex: null,
      selectedFileId: null,
      isChecked: false,
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
    const { isNotify, history, socketHelper, t } = this.props;

    if (!selectedFileId) {
      toastr.error(t("RecoveryFileNotSelected"));
      return;
    }
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
        .then(() => {
          socketHelper.emit({
            command: "restore-backup",
          });
        })
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

  onChangeCheckbox = () => {
    this.setState({
      isChecked: !this.state.isChecked,
    });
  };

  render() {
    const {
      onModalClose,
      isVisibleDialog,
      t,
      isCopyingToLocal,
      theme,
    } = this.props;
    const { filesList, isLoading, selectedFileIndex, isChecked } = this.state;

    const helpContent = () => (
      <>
        <Text className="restore-backup_warning-description">
          {t("RestoreBackupWarningText")}{" "}
          <Text as="span" className="restore-backup_warning-link">
            {t("RestoreBackupResetInfoWarningText")}
          </Text>
        </Text>
      </>
    );

    return (
      <ModalDialog
        visible={isVisibleDialog}
        onClose={onModalClose}
        displayType="aside"
        withoutBodyScroll
        contentHeight="100%"
        contentPaddingBottom="0px"
      >
        <ModalDialog.Header>{t("BackupList")}</ModalDialog.Header>
        <ModalDialog.Body>
          <StyledBackupList
            isCopyingToLocal={isCopyingToLocal}
            isEmpty={filesList?.length === 0}
            theme={theme}
          >
            <div className="backup-list_content">
              {filesList.length > 0 && (
                <div className="backup-restore_dialog-header">
                  <Text fontSize="12px" style={{ marginBottom: "10px" }}>
                    {t("BackupListWarningText")}
                  </Text>
                  <Link
                    onClick={this.onCleanBackupList}
                    fontWeight={600}
                    style={{ textDecoration: "underline dotted" }}
                  >
                    {t("ClearBackupList")}
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
                    <Text
                      fontSize="12px"
                      color="#A3A9AE"
                      textAlign="center"
                      className="backup-restore_empty-list"
                    >
                      {t("EmptyBackupList")}
                    </Text>
                  )
                ) : (
                  <div className="loader" key="loader">
                    <Loaders.ListLoader />
                  </div>
                )}
              </div>

              <div className="backup-list_footer">
                {filesList.length > 0 && (
                  <div>
                    <div id="backup-list_help">
                      <Checkbox
                        truncate
                        className="backup-list_checkbox"
                        onChange={this.onChangeCheckbox}
                        isChecked={isChecked}
                      />
                      <Text as="span" className="backup-list_agreement-text">
                        {t("UserAgreement")}
                        <HelpButton
                          className="backup-list_tooltip"
                          offsetLeft={100}
                          iconName={"/static/images/help.react.svg"}
                          getContent={helpContent}
                          tooltipMaxWidth={"286px"}
                        />
                      </Text>
                    </div>
                    <div className="restore_dialog-button">
                      <Button
                        primary
                        size="normal"
                        label={t("Common:Restore")}
                        onClick={this.onRestorePortal}
                        isDisabled={isCopyingToLocal || !isChecked}
                      />
                      <Button
                        size="normal"
                        label={t("Common:CloseButton")}
                        onClick={onModalClose}
                      />
                    </div>
                  </div>
                )}
              </div>
            </div>
          </StyledBackupList>
        </ModalDialog.Body>
      </ModalDialog>
    );
  }
}

BackupListModalDialog.propTypes = {
  onModalClose: PropTypes.func.isRequired,
  isVisibleDialog: PropTypes.bool.isRequired,
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const { socketHelper, theme } = settingsStore;

  return {
    theme,
    socketHelper,
  };
})(
  withTranslation(["Settings", "Common", "Translations"])(
    observer(BackupListModalDialog)
  )
);
