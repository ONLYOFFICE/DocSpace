import React from "react";
import PropTypes from "prop-types";
import { withTranslation } from "react-i18next";
import throttle from "lodash/throttle";

import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import Loader from "@appserver/components/loader";

import utils from "@appserver/components/utils";
import Link from "@appserver/components/link";
import { StyledBackupList } from "../styled-backup";
import {
  deleteBackup,
  deleteBackupHistory,
  getBackupHistory,
  getRestoreProgress,
  startRestore,
} from "../../../../../../../../../packages/asc-web-common/api/portal";
import BackupListBody from "./backupListBody";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import config from "../../../../../../../package.json";
import history from "@appserver/common/history";
const { desktop } = utils.device;
const homepage = config.homepage;
class BackupListModalDialog extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isLoading: false,
      filesList: [],
      hasNextPage: true,
      isNextPageLoading: false,
      displayType: this.getDisplayType(),
    };
    this.throttledResize = throttle(this.setDisplayType, 300);
  }
  componentDidMount() {
    const { isVisibleDialog } = this.props;

    if (isVisibleDialog) {
      window.addEventListener("resize", this.throttledResize);
    }

    this.setState({ isLoading: true }, function () {
      getBackupHistory()
        .then((backupList) =>
          this.setState({
            filesList: backupList,
          })
        )
        .finally(() => this.setState({ isLoading: false }));
    });
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

  onCleanListClick = () => {
    this.setState({ isLoading: true }, function () {
      deleteBackupHistory()
        .then(() => getBackupHistory())
        .then((backupList) => this.setState({ filesList: backupList }))
        .catch((error) => console.log("backup list error", error))
        .finally(() => this.setState({ isLoading: false }));
    });
  };
  onDeleteClick = (e) => {
    const { filesList } = this.state;
    const index =
      e.target.dataset.index ||
      (e.target.farthestViewportElement &&
        e.target.farthestViewportElement.dataset.index);
    if (!index) return;

    this.setState({ isLoading: true }, function () {
      deleteBackup(filesList[+index].id)
        .then(() => getBackupHistory())
        .then((backupList) => this.setState({ filesList: backupList }))
        .catch((error) => console.log("backup list error", error))
        .finally(() => this.setState({ isLoading: false }));
    });
  };
  onRestoreClick = (e) => {
    const { filesList } = this.state;
    const { isNotify } = this.props;
    const index =
      e.target.dataset.index ||
      (e.target.farthestViewportElement &&
        e.target.farthestViewportElement.dataset.index);
    if (!index) return;

    this.setState({ isLoading: true }, function () {
      const backupId = filesList[+index].id;
      const storageType = "0";
      const storageParams = [
        {
          key: "fileId",
          value: filesList[+index].id,
        },
      ];
      startRestore(backupId, storageType, storageParams, isNotify)
        .then(() =>
          history.push(
            combineUrl(
              AppServerConfig.proxyURL,
              homepage,
              "/preparation-portal"
            )
          )
        )
        .catch((error) => console.log("backup list error", error));
    });
  };
  render() {
    const { onModalClose, isVisibleDialog, t, iconUrl } = this.props;
    const { filesList, displayType, isLoading } = this.state;
    // console.log("filesList", filesList);
    return (
      <ModalDialog visible={isVisibleDialog} onClose={onModalClose}>
        <ModalDialog.Header>{t("BackupList")}</ModalDialog.Header>
        <ModalDialog.Body>
          <StyledBackupList displayType={displayType}>
            <div className="backup-list_modal-dialog_body">
              <div className="backup-list_modal-header_wrapper_description">
                <Text className="backup-list_modal-header_description">
                  {t("BackupListDeleteWarning")}
                  <Link
                    className="backup-list_clear-link"
                    onClick={this.onCleanListClick}
                  >
                    {t("ClearList")}
                  </Link>
                </Text>
              </div>
              {!isLoading ? (
                filesList.length > 0 ? (
                  <BackupListBody
                    t={t}
                    displayType={displayType}
                    needRowSelection={false}
                    filesList={filesList}
                    onIconClick={this.onDeleteClick}
                    onRestoreClick={this.onRestoreClick}
                    iconUrl={iconUrl}
                  />
                ) : (
                  <Text>{t("EmptyBackupList")}</Text>
                )
              ) : (
                <div key="loader">
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
          <Button
            className="modal-dialog-button"
            primary
            size="big"
            label={t("Common:CloseButton")}
            tabIndex={1}
            onClick={onModalClose}
          />
        </ModalDialog.Footer>
      </ModalDialog>
    );
  }
}

BackupListModalDialog.propTypes = {
  t: PropTypes.func.isRequired,
  onModalClose: PropTypes.func.isRequired,
  isVisibleDialog: PropTypes.bool.isRequired,
};

export default withTranslation(["Settings", "Common"])(BackupListModalDialog);
