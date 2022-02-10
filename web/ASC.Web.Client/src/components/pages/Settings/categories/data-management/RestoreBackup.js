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
      isErrors: {},
      downloadingProgress: 100,
      isInitialLoading: true,
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
    this.formSettings = "";
  }

  checkDownloadingProgress = async () => {
    try {
      const [commonThirdPartyList, backupProgress] = await Promise.all([
        SelectFolderDialog.getCommonThirdPartyList(),
        getBackupProgress(),
      ]);

      this.commonThirdPartyList = commonThirdPartyList;

      if (backupProgress && !backupProgress.error) {
        if (backupProgress.progress !== 100) {
          this._isMounted &&
            this.setState({
              downloadingProgress: backupProgress.progress,
            });

          this.timerId = setInterval(() => this.getProgress(), 1000);
        }
      }
    } catch (error) {
      console.error(error);
    }

    this.setState({
      isInitialLoading: false,
    });
  };

  componentDidMount() {
    this._isMounted = true;

    this.checkDownloadingProgress();
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
              downloadingProgress: 100,
            });
            return;
          }

          this._isMounted &&
            this.setState({
              downloadingProgress: res.progress,
            });

          if (res.progress === 100) {
            clearInterval(this.timerId);

            this.timerId && toastr.success(`${t("SuccessCopied")}`);
            this.timerId = null;

            this._isMounted &&
              this.setState({
                downloadingProgress: 100,
              });
          }
        } else {
          clearInterval(this.timerId);
        }
      })
      .catch((err) => {
        clearInterval(this.timerId);
        this.timerId && toastr.error(err);
        console.log("err", err);
        this.timerId = null;

        this._isMounted &&
          this.setState({
            downloadingProgress: 100,
          });
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
      let errors = {};
      let firstError = false;

      for (let key of this.formNames) {
        const field = this.formSettings[key];
        //console.log("this.formNames", this.formNames, "key", key);
        if (!field) {
          if (!firstError) {
            firstError = true;
          }
          errors[key] = true;
        } else {
          if (!firstError && !field.trim()) {
            firstError = true;
          }
          errors[key] = !field.trim();
          console.log("errors[key]", errors[key]);
        }
      }

      if (firstError) {
        this.setState({
          isErrors: errors,
        });
        return false;
      }
      return true;
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
      storageParams.push({
        key: "module",
        value: this.storageId,
      });
      let tmpObj = {};
      const arraySettings = Object.entries(formSettings);
      console.log("this.state.formSettings", arraySettings);
      for (let i = 0; i < this.formNames.length; i++) {
        tmpObj = {
          key: arraySettings[i][0],
          value: arraySettings[i][1],
        };

        storageParams.push(tmpObj);
      }
    } else {
      obj.key = "filePath";
      if (isCheckedDocuments || isCheckedThirdParty) {
        obj.value = selectedFileId;
      } else {
        obj.value = ""; //TODO: added upload of file.
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

    startRestore(backupId, storageType, storageParams, isNotify)
      .then(() => getSettings())
      .then(() => (this.storageId = ""))
      .then(() =>
        history.push(
          combineUrl(
            AppServerConfig.proxyURL,
            config.homepage,
            "/preparation-portal"
          )
        )
      )
      .catch((error) => toastr.error(error));
  };

  onSetRequiredFormNames = (namesArray) => {
    this.formNames = namesArray;
  };

  onResetFormSettings = () => {
    this.formSettings = {};
    this.setState({
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

  onSetFormSettings = (name, value) => {
    this.formSettings = {
      ...this.formSettings,
      ...{ [name]: value },
    };
  };
  render() {
    const { t, history } = this.props;
    const {
      isChecked,
      isInitialLoading,
      isNotify,
      isVisibleDialog,
      isPanelVisible,
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
      isCheckedLocalFile,
      isErrors,
      downloadingProgress,
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

    const isMaxProgress = downloadingProgress === 100;

    console.log("render restore backup ", this.state, this.props);
    return isInitialLoading ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledRestoreBackup>
        <div className="restore-description">
          <Text className="restore-description">
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

        <div className="restore-backup_modules">
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
              foldersList={this.commonThirdPartyList}
              isPanelVisible={isPanelVisible}
              onClose={this.onPanelClose}
              onClickInput={this.onClickInput}
              onSelectFile={this.onSelectFile}
            />
          )}
          {isCheckedThirdPartyStorage && (
            <ThirdPartyStorages
              onSetRequiredFormNames={this.onSetRequiredFormNames}
              onResetFormSettings={this.onResetFormSettings}
              isErrors={isErrors}
              onSetStorageId={this.onSetStorageId}
              onSetFormSettings={this.onSetFormSettings}
            />
          )}
          {isCheckedLocalFile && (
            <LocalFile onSelectLocalFile={this.onSelectLocalFile} />
          )}
        </div>

        <Text className="restore-backup_list" onClick={this.onClickBackupList}>
          {t("BackupList")}
        </Text>

        {isVisibleDialog && (
          <BackupListModalDialog
            t={t}
            isVisibleDialog={isVisibleDialog}
            onModalClose={this.onModalClose}
            isNotify={isNotify}
            isCopyingToLocal={!isMaxProgress}
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

        <Text className="restore-description restore-source restore-warning">
          {t("Common:Warning")}
          {"!"}
        </Text>
        <Text className="restore-description">
          {t("RestoreSettingsWarning")}
        </Text>
        <Text className="restore-description restore-warning_link">
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
          isDisabled={!isMaxProgress || !isChecked}
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
