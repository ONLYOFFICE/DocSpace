﻿import HelpReactSvgUrl from "PUBLIC_DIR/images/help.react.svg?url";
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
import Checkbox from "@docspace/components/checkbox";
import HelpButton from "@docspace/components/help-button";
import config from "PACKAGE_FILE";
import { StyledBackupList } from "../../../StyledBackup";
import BackupListBody from "./BackupListBody";
import { TenantStatus } from "@docspace/common/constants";
import styled from "styled-components";

const StyledModalDialog = styled(ModalDialog)`
  .restore_footer {
    width: 100%;
    .restore_dialog-button {
      display: flex;
      button:first-child {
        margin-right: 10px;
        width: 50%;
      }
      button:last-child {
        width: 50%;
      }
    }
    #backup-list_help {
      display: flex;
      background-color: ${(props) => props.theme.backgroundColor};
      margin-bottom: 16px;
    }

    .backup-list_agreement-text {
      user-select: none;
      div:first-child {
        display: inline-block;
      }
    }

    .backup-list_tooltip {
      margin-left: 8px;
    }
  }
`;

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
    const { isNotify, history, socketHelper, t, setTenantStatus } = this.props;

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
        .then(() => setTenantStatus(TenantStatus.PortalRestore))
        .then(() => {
          socketHelper.emit({
            command: "restore-backup",
          });
        })
        .then(() =>
          history.push(
            combineUrl(
              window.DocSpaceConfig?.proxy?.url,
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
      <StyledModalDialog
        displayType="aside"
        visible={isVisibleDialog}
        onClose={onModalClose}
        withFooterBorder
      >
        <ModalDialog.Header>
          <Text fontSize="21px" fontWeight={700}>
            {t("BackupList")}
          </Text>
        </ModalDialog.Header>
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
                      textAlign="center"
                      className="backup-restore_empty-list"
                    >
                      {t("EmptyBackupList")}
                    </Text>
                  )
                ) : (
                  <div className="loader" key="loader">
                    <Loaders.ListLoader count={7} />
                  </div>
                )}
              </div>
            </div>
          </StyledBackupList>
        </ModalDialog.Body>

        <ModalDialog.Footer>
          <div className="restore_footer">
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
                  iconName={HelpReactSvgUrl}
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
        </ModalDialog.Footer>
      </StyledModalDialog>
    );
  }
}

BackupListModalDialog.propTypes = {
  onModalClose: PropTypes.func.isRequired,
  isVisibleDialog: PropTypes.bool.isRequired,
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const { socketHelper, theme, setTenantStatus } = settingsStore;

  return {
    setTenantStatus,
    theme,
    socketHelper,
  };
})(
  withTranslation(["Settings", "Common", "Translations"])(
    observer(BackupListModalDialog)
  )
);
