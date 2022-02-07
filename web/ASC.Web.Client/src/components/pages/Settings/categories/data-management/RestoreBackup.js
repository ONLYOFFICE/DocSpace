import React from "react";

import { withTranslation } from "react-i18next";

import Button from "@appserver/components/button";
import Loader from "@appserver/components/loader";
import Checkbox from "@appserver/components/checkbox";
import Text from "@appserver/components/text";
import RadioButton from "@appserver/components/radio-button";
import SelectFolderDialog from "files/SelectFolderDialog";
import { StyledRestoreBackup } from "./StyledBackup";
import BackupListModalDialog from "./sub-components-restore-backup/BackupListModalDialog";
import Documents from "./sub-components-restore-backup/DocumentsModule";
import ThirdPartyResources from "./sub-components-restore-backup/ThirdPartyResourcesModule";
import ThirdPartyStorages from "./sub-components-restore-backup/ThirdPartyStoragesModule";
import LocalFile from "./sub-components-restore-backup/LocalFileModule";
import toastr from "@appserver/components/toast/toastr";
import { getBackupProgress, startRestore } from "@appserver/common/api/portal";
import { getSettings } from "@appserver/common/api/settings";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import config from "../../../../../../package.json";
import FloatingButton from "@appserver/common/components/FloatingButton";
import { request } from "@appserver/common/api/client";

class RestoreBackup extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isChecked: false,
      isNotify: true,
      isVisibleDialog: false,
      isPanelVisible: false,
      isCheckedDocuments: true,
      isCheckedThirdParty: false,
      isCheckedThirdPartyStorage: false,
      isCheckedLocalFile: false,
      selectedFileId: "",
      selectedFile: "",
      formSettings: {},
      isErrors: {},
      isCopyingToLocal: true,
      downloadingProgress: 100,
    };
    this._isMounted = false;
    this.timerId = null;
    this.switches = [
      "isCheckedLocalFile",
      "isCheckedDocuments",
      "isCheckedThirdParty",
      "isCheckedThirdPartyStorage",
    ];

    this.storageId = "";
  }

  componentDidMount() {
    this._isMounted = true;
    this.setState(
      {
        isLoading: true,
      },
      function () {
        getBackupProgress().then((response) => {
          if (response) {
            if (!response.error) {
              if (response.progress === 100)
                this.setState({
                  isCopyingToLocal: false,
                });
              if (response.progress !== 100)
                this.timerId = setInterval(() => this.getProgress(), 1000);
            } else {
              this.setState({
                isCopyingToLocal: false,
              });
            }
          } else {
            this.setState({
              isCopyingToLocal: false,
            });
          }
        });

        SelectFolderDialog.getCommonThirdPartyList()
          .then(
            (thirdPartyArray) => (this.commonThirdPartyList = thirdPartyArray)
          )
          .finally(() =>
            this.setState({
              isLoading: false,
            })
          );
      }
    );
  }
  componentWillUnmount() {
    this._isMounted = false;
    clearInterval(this.timerId);
  }
  getProgress = () => {
    const { t } = this.props;

    getBackupProgress()
      .then((res) => {
        if (res) {
          if (res.error) {
            clearInterval(this.timerId);
            this.timerId && toastr.error(`${res.error}`);
            console.log("error", res.error);
            this.timerId = null;
            this.setState({
              isCopyingToLocal: false,
              downloadingProgress: 100,
            });
            return;
          }
          if (this._isMounted) {
            this.setState({
              downloadingProgress: res.progress,
            });
          }
          if (res.progress === 100) {
            clearInterval(this.timerId);

            this.timerId && toastr.success(`${t("SuccessCopied")}`);
            this.timerId = null;
            if (this._isMounted) {
              this.setState({
                isCopyingToLocal: false,
              });
            }
          }
        }
      })
      .catch((err) => {
        clearInterval(timerId);
        timerId && toastr.error(err);
        console.log("err", err);
        timerId = null;
        if (this._isMounted) {
          this.setState({
            downloadingProgress: 100,
            isCopyingToLocal: false,
          });
        }
      });
  };

  onChangeCheckbox = () => {
    this.setState({
      isChecked: !this.state.isChecked,
    });
  };
  onChangeCheckboxNotify = () => {
    this.setState({
      isNotify: !this.state.isNotify,
    });
  };
  onClickBackupList = () => {
    this.setState({
      isVisibleDialog: !this.state.isVisibleDialog,
    });
  };

  onModalClose = () => {
    this.setState({
      isVisibleDialog: false,
    });
  };

  onClickInput = () => {
    this.setState({
      isPanelVisible: true,
    });
  };

  onPanelClose = () => {
    this.setState({
      isPanelVisible: false,
    });
  };
  onClickShowStorage = (e) => {
    let newStateObj = {};
    const name = e.target.name;
    newStateObj[name] = true;

    const newState = this.switches.filter((el) => el !== name);
    newState.forEach((name) => (newStateObj[name] = false));

    this.setState({
      ...newStateObj,
    });
  };

  onSelectFile = (file) => {
    this.setState({
      selectedFileId: file.id,
    });
  };

  onSelectLocalFile = (data) => {
    this.setState({
      selectedFile: data,
    });
  };

  canRestore = () => {
    const {
      isCheckedDocuments,
      isCheckedLocalFile,
      selectedFileId,
      selectedFile,
      isCheckedThirdPartyStorage,
      isCheckedThirdParty,
      formSettings,
    } = this.state;
    if (isCheckedDocuments || isCheckedThirdParty) {
      if (!selectedFileId) return false;
      return true;
    }

    if (isCheckedLocalFile) {
      if (!selectedFile) return false;
      return true;
    }

    if (isCheckedThirdPartyStorage) {
      let errorObject = {};

      for (let key of this.formNames) {
        errorObject = {
          ...errorObject,
          [key]: this.state.formSettings[key]
            ? !this.state.formSettings[key].trim()
            : true,
        };
      }
      if (Object.keys(errorObject).length !== 0) {
        this.setState({
          isErrors: errorObject,
        });
        return;
      }
    }
  };
  onRestoreClick = async () => {
    const {
      isNotify,
      isCheckedDocuments,
      isCheckedLocalFile,
      selectedFileId,
      selectedFile,
      isCheckedThirdPartyStorage,
      isCheckedThirdParty,
      formSettings,
    } = this.state;
    const { history } = this.props;

    if (!this.canRestore()) return;

    let storageParams = [];
    let obj = {};

    const backupId = "";
    const storageType = isCheckedDocuments
      ? "0"
      : isCheckedThirdParty
      ? "1"
      : isCheckedLocalFile
      ? "3"
      : "5";

    if (isCheckedThirdPartyStorage) {
      //TODO: add to add storageParams
    } else {
      obj.key = "filePath";
      if (isCheckedDocuments || isCheckedThirdParty) {
        obj.value = selectedFileId;
      } else {
        obj.value = selectedFile; //TODO: added upload of file.
      }
      storageParams.push(obj);
    }

    console.log(
      "backupId",
      backupId,
      "storageType",
      storageType,
      "storageParams",
      storageParams,
      "isNotify",
      isNotify,
      selectedFile
    );

    try {
      if (isCheckedLocalFile) {
        const data = await request({
          baseURL: combineUrl(AppServerConfig.proxyURL, config.homepage),
          method: "post",
          url: `/backupFileUpload.ashx`,
          responseType: "text",
          data: selectedFile,
        });
        console.error("data", data);
      }
    } catch (e) {
      console.error("error");
    }

    // startRestore(backupId, storageType, storageParams, isNotify)
    //   .then(() => getSettings())
    //   .then(() => (this.storageId = ""))
    //   .then(() =>
    //     history.push(
    //       combineUrl(
    //         AppServerConfig.proxyURL,
    //         config.homepage,
    //         "/preparation-portal"
    //       )
    //     )
    //   )
    //   .catch((error) => toastr.error(error));
  };

  onSetFormNames = (namesArray) => {
    this.formNames = namesArray;
  };

  onChange = (event) => {
    const { target } = event;
    const value = target.value;
    const name = target.name;
    const { formSettings } = this.state;

    this.setState({ formSettings: { ...formSettings, ...{ [name]: value } } });
  };

  onResetFormSettings = () => {
    this.setState({
      formSettings: {},
      isErrors: {},
    });
  };

  onClickFloatingButton = () => {
    const { history } = this.props;
    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        "/settings/datamanagement/backup/manual-backup"
      )
    );
  };

  onSetStorageId = (storageId) => {
    this.storageId = storageId;
  };
  render() {
    const { t, history } = this.props;
    const {
      isChecked,
      isLoading,
      isNotify,
      isVisibleDialog,
      isPanelVisible,
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
      isCheckedLocalFile,
      formSettings,
      isErrors,
      isCopyingToLocal,
      downloadingProgress,
      storageId,
    } = this.state;

    const commonRadioButtonProps = {
      fontSize: "13px",
      fontWeight: "400",
      value: "value",
      className: "backup_radio-button",
      onClick: this.onClickShowStorage,
    };

    const isDisabledThirdParty =
      this.commonThirdPartyList && this.commonThirdPartyList.length === 0;
    console.log("render child ");
    return isLoading ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledRestoreBackup>
        <div className="restore-description">
          <Text className="backup-description">
            {t("DataRestoreDescription")}
          </Text>
        </div>
        <RadioButton
          label={t("DocumentsModule")}
          name={"isCheckedDocuments"}
          key={1}
          isChecked={isCheckedDocuments}
          isDisabled={false}
          {...commonRadioButtonProps}
        />

        <RadioButton
          label={t("ThirdPartyResource")}
          name={"isCheckedThirdParty"}
          key={2}
          isChecked={isCheckedThirdParty}
          isDisabled={isDisabledThirdParty}
          {...commonRadioButtonProps}
        />

        <RadioButton
          label={t("ThirdPartyStorage")}
          name={"isCheckedThirdPartyStorage"}
          key={3}
          isChecked={isCheckedThirdPartyStorage}
          isDisabled={false}
          {...commonRadioButtonProps}
        />

        <RadioButton
          label={t("LocalFile")}
          name={"isCheckedLocalFile"}
          key={4}
          isChecked={isCheckedLocalFile}
          isDisabled={false}
          {...commonRadioButtonProps}
        />

        {isCheckedDocuments && (
          <Documents
            isPanelVisible={isPanelVisible}
            onClose={this.onPanelClose}
            onClickInput={this.onClickInput}
            onSelectFile={this.onSelectFile}
          />
        )}
        {isCheckedThirdParty && (
          <ThirdPartyResources
            isPanelVisible={isPanelVisible}
            onClose={this.onPanelClose}
            onClickInput={this.onClickInput}
            onSelectFile={this.onSelectFile}
          />
        )}
        {isCheckedThirdPartyStorage && (
          <ThirdPartyStorages
            onSetStorageForm={this.onSetStorageForm}
            onSetFormNames={this.onSetFormNames}
            onChange={this.onChange}
            formSettings={formSettings}
            onResetFormSettings={this.onResetFormSettings}
            isErrors={isErrors}
            onSetStorageId={this.onSetStorageId}
          />
        )}
        {isCheckedLocalFile && (
          <LocalFile onSelectLocalFile={this.onSelectLocalFile} />
        )}

        <Text className="restore-backup_list" onClick={this.onClickBackupList}>
          {t("BackupList")}
        </Text>

        {isVisibleDialog && (
          <BackupListModalDialog
            t={t}
            isVisibleDialog={isVisibleDialog}
            isLoading={isLoading}
            onModalClose={this.onModalClose}
            isNotify={isNotify}
            isCopyingToLocal={isCopyingToLocal}
            history={history}
          />
        )}
        <Checkbox
          truncate
          className="restore-backup-checkbox_notification"
          onChange={this.onChangeCheckboxNotify}
          isChecked={isNotify}
          label={t("UserNotification")}
        />

        <Text className="backup-description restore-source restore-warning">
          {t("Common:Warning")}
          {"!"}
        </Text>
        <Text className="backup-description ">
          {t("RestoreSettingsWarning")}
        </Text>
        <Text className="backup-description restore-warning_link">
          {t("RestoreSettingsLink")}
        </Text>

        <Checkbox
          truncate
          className="restore-backup-checkbox"
          onChange={this.onChangeCheckbox}
          isChecked={isChecked}
          label={t("UserAgreement")}
        />

        <Button
          label={t("RestoreButton")}
          onClick={this.onRestoreClick}
          primary
          isDisabled={isCopyingToLocal || !isChecked}
          size="medium"
          tabIndex={10}
        />

        {downloadingProgress > 0 && downloadingProgress !== 100 && (
          <FloatingButton
            className="layout-progress-bar"
            icon="file"
            alert={false}
            percent={downloadingProgress}
            onClick={this.onClickFloatingButton}
          />
        )}
      </StyledRestoreBackup>
    );
  }
}

export default withTranslation(["Settings", "Common"])(RestoreBackup);
