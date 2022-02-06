import React from "react";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";

import Button from "@appserver/components/button";
import { getBackupProgress, startBackup } from "@appserver/common/api/portal";
import toastr from "@appserver/components/toast/toastr";
import ThirdPartyModule from "./sub-components-manual-backup/ThirdPartyModule";
import DocumentsModule from "./sub-components-manual-backup/DocumentsModule";
import ThirdPartyStorageModule from "./sub-components-manual-backup/ThirdPartyStorageModule";

import FloatingButton from "@appserver/common/components/FloatingButton";
import RadioButton from "@appserver/components/radio-button";
import { StyledModules, StyledManualBackup } from "./StyledBackup";
import SelectFolderDialog from "files/SelectFolderDialog";
import Loader from "@appserver/components/loader";
import { saveToSessionStorage, getFromSessionStorage } from "../../utils";

let selectedManualStorageType = "";

class ManualBackup extends React.Component {
  constructor(props) {
    super(props);

    selectedManualStorageType = getFromSessionStorage(
      "selectedManualStorageType"
    );

    const checkedDocuments = selectedManualStorageType
      ? selectedManualStorageType === "documents"
      : false;

    const checkedTemporary = selectedManualStorageType
      ? selectedManualStorageType === "temporary"
      : true;

    const checkedThirdPartyResource = selectedManualStorageType
      ? selectedManualStorageType === "thirdPartyResource"
      : false;

    const checkedThirdPartyStorage = selectedManualStorageType
      ? selectedManualStorageType === "thirdPartyStorage"
      : false;

    this.state = {
      isVisiblePanel: false,
      downloadingProgress: 100,
      link: "",
      selectedFolder: "",
      isPanelVisible: false,
      isInitialLoading: true,
      isLoadingData: false,

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
    saveToSessionStorage("selectedManualStorageType", "");

    getFromSessionStorage("selectedFolderPath") &&
      saveToSessionStorage("selectedFolderPath", ""); //for documents, third party modules

    getFromSessionStorage("selectedFolder") &&
      saveToSessionStorage("selectedFolder", ""); //for documents, third party modules

    getFromSessionStorage("selectedStorageId") &&
      saveToSessionStorage("selectedStorageId", ""); //for  third party storages module

    getFromSessionStorage("selectedStorage") &&
      saveToSessionStorage("selectedStorage", ""); //for  third party storages module
  };

  checkDownloadingProgress = async () => {
    try {
      let progressResponse;

      [this.commonThirdPartyList, progressResponse] = await Promise.all([
        SelectFolderDialog.getCommonThirdPartyList(),
        getBackupProgress(),
      ]);

      if (progressResponse && !progressResponse.error) {
        progressResponse.link &&
          progressResponse.link.slice(0, 1) === "/" &&
          this.setState({
            link: progressResponse.link,
          });

        this.setState({
          downloadingProgress: progressResponse.progress,
        });

        if (progressResponse.progress !== 100) {
          this._isMounted &&
            this.setState({
              isLoadingData: true,
            });

          this.timerId = setInterval(() => this.getProgress(), 1000);
        } else {
          this.clearSessionStorage();
        }
      } else {
        this.clearSessionStorage();
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

    this.setState(
      {
        isInitialLoading: true,
      },
      function () {
        this.checkDownloadingProgress();
      }
    );
  }

  componentWillUnmount() {
    this._isMounted = false;
    clearInterval(this.timerId);
  }

  onMakeTemporaryBackup = () => {
    saveToSessionStorage("selectedManualStorageType", "temporary");

    startBackup("4", null);

    this.setState({
      downloadingProgress: 1,
      isLoadingData: true,
    });

    this.timerId = setInterval(() => this.getProgress(), 1000);
  };

  getProgress = () => {
    const { downloadingProgress } = this.state;
    const { t } = this.props;

    getBackupProgress()
      .then((response) => {
        if (response.error.length > 0 && response.progress !== 100) {
          console.log("error", response.error);

          this.clearSessionStorage();
          clearInterval(this.timerId);

          this.timerId && toastr.error(`${t("CopyingError")}`);

          this.timerId = null;

          this.setState({
            downloadingProgress: 100,
            isLoadingData: false,
          });
          return;
        }

        if (response.progress === 100) {
          this.clearSessionStorage();

          clearInterval(this.timerId);

          if (this._isMounted) {
            response.link?.slice(0, 1) === "/" &&
              this.setState({
                link: response.link,
              });

            this.setState({
              isLoadingData: false,
            });
          }

          this.timerId && toastr.success(`${t("SuccessCopied")}`);
          this.timerId = null;
        }
        if (this._isMounted) {
          downloadingProgress !== response.progress &&
            this.setState({
              downloadingProgress: response.progress,
            });
        }
      })
      .catch((err) => {
        console.log("error", err);

        this.clearSessionStorage();
        clearInterval(this.timerId);

        this.timerId && toastr.error(`${t("CopyingError")}`);
        this.timerId = null;

        if (this._isMounted) {
          this.setState({
            downloadingProgress: 100,
            isLoadingData: false,
          });
        }
      });
  };

  setInterval = () => {
    this.setState({
      downloadingProgress: 1,
      isLoadingData: true,
    });
    this.timerId = setInterval(() => this.getProgress(), 1000);
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

  onSetLoadingData = (isLoading) => {
    this._isMounted &&
      this.setState({
        isLoadingData: isLoading,
      });
  };

  onMakeCopy = (
    selectedFolder,
    moduleName,
    moduleType,
    key,
    selectedId,
    storageValues,
    selectedStorageId,
    selectedStorage
  ) => {
    const storageParams = [
      {
        key: `${key}`,
        value: selectedFolder ? selectedFolder : selectedId,
      },
    ];

    saveToSessionStorage("selectedManualStorageType", `${moduleName}`);

    if (selectedFolder) {
      saveToSessionStorage("selectedFolder", `${selectedFolder}`);

      SelectFolderDialog.getFolderPath(selectedFolder).then((folderPath) => {
        saveToSessionStorage("selectedFolderPath", `${folderPath}`);
      });
    } else {
      saveToSessionStorage("selectedStorageId", `${selectedStorageId}`);
      saveToSessionStorage("selectedStorage", `${selectedStorage}`);

      for (let i = 0; i < storageValues.length; i++) {
        storageParams.push(storageValues[i]);
      }
    }

    startBackup(moduleType, storageParams);
    this.setInterval();
  };
  render() {
    const { t } = this.props;
    const {
      downloadingProgress,
      link,
      isInitialLoading,
      isCheckedTemporaryStorage,
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
      isLoadingData,
    } = this.state;

    const isMaxProgress = downloadingProgress === 100;
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
      isCopyingLocal: isLoadingData,
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
            isDisabled={isLoadingData}
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
            isDisabled={isDisabledThirdParty || isLoadingData}
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
            isDisabled={isLoadingData}
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
