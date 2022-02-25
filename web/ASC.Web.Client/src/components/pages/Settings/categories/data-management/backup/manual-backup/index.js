import React from "react";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";

import Button from "@appserver/components/button";
import { getBackupProgress, startBackup } from "@appserver/common/api/portal";
import toastr from "@appserver/components/toast/toastr";
import ThirdPartyModule from "./sub-components/ThirdPartyModule";
import DocumentsModule from "./sub-components/DocumentsModule";
import ThirdPartyStorageModule from "./sub-components/ThirdPartyStorageModule";

import FloatingButton from "@appserver/common/components/FloatingButton";
import RadioButton from "@appserver/components/radio-button";
import { StyledModules, StyledManualBackup } from "./../StyledBackup";
import SelectFolderDialog from "files/SelectFolderDialog";
import Loader from "@appserver/components/loader";
import { saveToSessionStorage, getFromSessionStorage } from "../../../../utils";
import { isDesktop } from "@appserver/components/utils/device";

let selectedStorageType = "";

class ManualBackup extends React.Component {
  constructor(props) {
    super(props);

    const { isDesktop } = this.props;

    selectedStorageType = getFromSessionStorage("LocalCopyStorageType");

    const checkedDocuments = selectedStorageType
      ? selectedStorageType === "Documents"
      : false;
    const checkedTemporary = selectedStorageType
      ? selectedStorageType === "TemporaryStorage"
      : true;
    const checkedThirdPartyResource = selectedStorageType
      ? selectedStorageType === "ThirdPartyResource"
      : false;
    const checkedThirdPartyStorage = selectedStorageType
      ? selectedStorageType === "ThirdPartyStorage"
      : false;

    this.state = {
      downloadingProgress: 100,
      link: "",
      selectedFolder: "",
      isPanelVisible: false,
      isInitialLoading: isDesktop ? false : true,
      isCheckedTemporaryStorage: checkedTemporary,
      isCheckedDocuments: checkedDocuments,
      isCheckedThirdParty: checkedThirdPartyResource,
      isCheckedThirdPartyStorage: checkedThirdPartyStorage,
    };

    this.switches = [
      "isCheckedTemporaryStorage",
      "isCheckedDocuments",
      "isCheckedThirdParty",
      "isCheckedThirdPartyStorage",
    ];

    this._isMounted = false;
    this.timerId = null;
  }

  clearSessionStorage = () => {
    saveToSessionStorage("LocalCopyStorageType", "");

    getFromSessionStorage("LocalCopyPath") &&
      saveToSessionStorage("LocalCopyPath", "");

    getFromSessionStorage("LocalCopyFolder") &&
      saveToSessionStorage("LocalCopyFolder", "");

    getFromSessionStorage("LocalCopyStorage") &&
      saveToSessionStorage("LocalCopyStorage", "");

    getFromSessionStorage("LocalCopyThirdPartyStorageType") &&
      saveToSessionStorage("LocalCopyThirdPartyStorageType", "");
  };

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

          this.setIntervalProcess();
        } else {
          backupProgress.link &&
            backupProgress.link.slice(0, 1) === "/" &&
            this.setState({
              link: backupProgress.link,
              downloadingProgress: 100,
            });

          this.clearSessionStorage();
        }
      } else {
        this.clearSessionStorage();
      }
    } catch (error) {
      console.error(error);
      this.clearSessionStorage();
    }

    this.setState({
      isInitialLoading: false,
    });
  };

  componentDidMount() {
    const { isDesktop } = this.props;

    this._isMounted = true;
    !isDesktop && this.checkDownloadingProgress();
  }

  componentWillUnmount() {
    this._isMounted = false;
    clearInterval(this.timerId);
  }

  setIntervalProcess = () => {
    this.timerId = setInterval(() => this.getProgress(), 1000);
  };

  onMakeTemporaryBackup = () => {
    const { isDesktop, setBackupProgress } = this.props;

    saveToSessionStorage("LocalCopyStorageType", "TemporaryStorage");

    if (isDesktop) {
      startBackup("4", null)
        .then(() => setBackupProgress())
        .catch((err) => {
          toastr.error(`${t("CopyingError")}`);
          console.error(err);
        });
    } else {
      startBackup("4", null)
        .then(() =>
          this.setState({
            downloadingProgress: 1,
          })
        )
        .then(() => !this.timerId && this.setIntervalProcess())
        .catch((err) => {
          toastr.error(`${t("CopyingError")}`);
          console.error(err);
        });
    }
  };

  getProgress = () => {
    const { downloadingProgress } = this.state;
    const { t } = this.props;

    getBackupProgress()
      .then((response) => {
        if (response) {
          const { progress, link, error } = response;
          if (error.length > 0 && progress !== 100) {
            console.log("error", error);

            this.clearSessionStorage();
            clearInterval(this.timerId);

            this.timerId && toastr.error(`${t("CopyingError")}`);
            this.timerId = null;

            this.setState({
              downloadingProgress: 100,
            });

            return;
          }

          if (progress === 100) {
            this.clearSessionStorage();
            clearInterval(this.timerId);

            this._isMounted &&
              this.setState({
                downloadingProgress: 100,
                ...(link && link.slice(0, 1) === "/" && { link: link }),
              });

            this.timerId && toastr.success(`${t("SuccessCopied")}`);
            this.timerId = null;

            return;
          }

          this._isMounted &&
            downloadingProgress !== progress &&
            this.setState({
              downloadingProgress: progress,
            });
        } else {
          clearInterval(this.timerId);
        }
      })
      .catch((err) => {
        console.log("error", err);

        this.clearSessionStorage();
        clearInterval(this.timerId);

        this.timerId && toastr.error(`${t("CopyingError")}`);
        this.timerId = null;

        this._isMounted &&
          this.setState({
            downloadingProgress: 100,
          });
      });
  };

  onClickDownloadBackup = () => {
    const { link } = this.state;
    const url = window.location.origin;
    const downloadUrl = `${url}` + `${link}`;
    window.open(downloadUrl, "_self");
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

  onMakeCopy = async (
    selectedFolder,
    moduleName,
    moduleType,
    key,
    selectedId,
    storageValues,
    selectedStorageId,
    selectedStorage
  ) => {
    const { isCheckedDocuments, isCheckedThirdParty } = this.state;
    const { t, isDesktop, setBackupProgress } = this.props;

    this.setState({
      downloadingProgress: 1,
    });

    const storageValue =
      isCheckedDocuments || isCheckedThirdParty ? selectedFolder : selectedId;

    const storageParams = [
      {
        key: `${key}`,
        value: storageValue,
      },
    ];

    saveToSessionStorage("LocalCopyStorageType", moduleName);

    if (isCheckedDocuments || isCheckedThirdParty) {
      saveToSessionStorage("LocalCopyFolder", `${selectedFolder}`);

      SelectFolderDialog.getFolderPath(selectedFolder).then((folderPath) => {
        saveToSessionStorage("LocalCopyPath", `${folderPath}`);
      });
    } else {
      saveToSessionStorage("LocalCopyStorage", `${selectedStorageId}`);
      saveToSessionStorage(
        "LocalCopyThirdPartyStorageType",
        `${selectedStorage}`
      );

      for (let i = 0; i < storageValues.length; i++) {
        storageParams.push(storageValues[i]);
      }
    }

    try {
      await startBackup(moduleType, storageParams);
      if (isDesktop) {
        setBackupProgress();
      } else {
        !this.timerId && this.setIntervalProcess();
      }
    } catch (err) {
      toastr.error(`${t("CopyingError")}`);
      console.error(err);

      this.clearSessionStorage();
      this.setState({
        downloadingProgress: 100,
      });
    }
  };
  render() {
    const { t, isDesktop, isCopyingLocal } = this.props;
    const {
      downloadingProgress,
      link,
      isInitialLoading,
      isCheckedTemporaryStorage,
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
    } = this.state;

    const isMaxProgress =
      (isDesktop ? isCopyingLocal : downloadingProgress) === 100;

    const isDisabledThirdParty =
      this.commonThirdPartyList && this.commonThirdPartyList.length === 0;

    const commonRadioButtonProps = {
      fontSize: "13px",
      fontWeight: "400",
      value: "value",
      className: "backup_radio-button",
      onClick: this.onClickShowStorage,
    };

    const commonModulesProps = {
      isMaxProgress,
      onMakeCopy: this.onMakeCopy,
    };

    return isInitialLoading ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledManualBackup>
        <StyledModules>
          <RadioButton
            label={t("TemporaryStorage")}
            name={"isCheckedTemporaryStorage"}
            key={0}
            isChecked={isCheckedTemporaryStorage}
            isDisabled={!isMaxProgress}
            {...commonRadioButtonProps}
          />
          <Text className="backup-description">
            {t("TemporaryStorageDescription")}
          </Text>
          {isCheckedTemporaryStorage && (
            <div className="category-item-wrapper temporary-storage">
              <div className="manual-backup_buttons">
                <Button
                  label={t("MakeCopy")}
                  onClick={this.onMakeTemporaryBackup}
                  primary
                  isDisabled={!isMaxProgress}
                  size="medium"
                />
                {link.length > 0 && isMaxProgress && (
                  <Button
                    label={t("DownloadBackup")}
                    onClick={this.onClickDownloadBackup}
                    isDisabled={false}
                    size="medium"
                    style={{ marginLeft: "8px" }}
                  />
                )}
                {!isMaxProgress && (
                  <Button
                    label={t("Copying")}
                    onClick={() => console.log("click")}
                    isDisabled={true}
                    size="medium"
                    style={{ marginLeft: "8px" }}
                  />
                )}
              </div>
            </div>
          )}
        </StyledModules>

        <StyledModules>
          <RadioButton
            label={t("DocumentsModule")}
            name={"isCheckedDocuments"}
            key={1}
            isChecked={isCheckedDocuments}
            isDisabled={!isMaxProgress}
            {...commonRadioButtonProps}
          />

          <Text className="backup-description">
            {t("DocumentsModuleDescription")}
          </Text>

          {isCheckedDocuments && (
            <DocumentsModule
              {...commonModulesProps}
              isCheckedDocuments={isCheckedDocuments}
            />
          )}
        </StyledModules>

        <StyledModules isDisabled={isDisabledThirdParty}>
          <RadioButton
            label={t("ThirdPartyResource")}
            name={"isCheckedThirdParty"}
            key={2}
            isChecked={isCheckedThirdParty}
            isDisabled={isDisabledThirdParty || !isMaxProgress}
            {...commonRadioButtonProps}
          />
          <Text className="backup-description">
            {t("ThirdPartyResourceDescription")}
          </Text>
          {isCheckedThirdParty && (
            <ThirdPartyModule
              {...commonModulesProps}
              commonThirdPartyList={this.commonThirdPartyList}
            />
          )}
        </StyledModules>

        <StyledModules>
          <RadioButton
            label={t("ThirdPartyStorage")}
            name={"isCheckedThirdPartyStorage"}
            key={3}
            isChecked={isCheckedThirdPartyStorage}
            isDisabled={!isMaxProgress}
            {...commonRadioButtonProps}
          />
          <Text className="backup-description">
            {t("ThirdPartyStorageDescription")}
          </Text>
          {isCheckedThirdPartyStorage && (
            <ThirdPartyStorageModule {...commonModulesProps} />
          )}
        </StyledModules>

        {downloadingProgress > 0 && downloadingProgress !== 100 && (
          <FloatingButton
            className="layout-progress-bar"
            icon="file"
            alert={false}
            percent={downloadingProgress}
          />
        )}
      </StyledManualBackup>
    );
  }
}

export default withTranslation("Settings")(ManualBackup);
