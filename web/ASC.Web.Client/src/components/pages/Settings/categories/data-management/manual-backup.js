import React from "react";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";

import Button from "@appserver/components/button";
import { getBackupProgress, startBackup } from "@appserver/common/api/portal";
import toastr from "@appserver/components/toast/toastr";
import ThirdPartyModule from "./sub-components-manual-backup/thirdPartyModule";
import DocumentsModule from "./sub-components-manual-backup/documentsModule";
import ThirdPartyStorageModule from "./sub-components/thirdPartyStorageModule";
import FloatingButton from "@appserver/common/components/FloatingButton";
import RadioButton from "@appserver/components/radio-button";
import { StyledModules, StyledComponent } from "./styled-backup";
import SelectedFolder from "files/SelectedFolder";
import Loader from "@appserver/components/loader";
import { saveToSessionStorage, getFromSessionStorage } from "../../utils";

let selectedManualBackupFromSessionStorage = "";

class ManualBackup extends React.Component {
  constructor(props) {
    super(props);
    this.manualBackup = true;
    selectedManualBackupFromSessionStorage = getFromSessionStorage(
      "selectedManualStorageType"
    );

    const checkedDocuments = selectedManualBackupFromSessionStorage
      ? selectedManualBackupFromSessionStorage === "documents"
      : false;

    const checkedTemporary = selectedManualBackupFromSessionStorage
      ? selectedManualBackupFromSessionStorage === "temporary"
      : true;

    const checkedThirdPartyResource = selectedManualBackupFromSessionStorage
      ? selectedManualBackupFromSessionStorage === "thirdPartyResource"
      : false;

    const checkedThirdPartyStorage = selectedManualBackupFromSessionStorage
      ? selectedManualBackupFromSessionStorage === "thirdPartyStorage"
      : false;

    this.state = {
      isVisiblePanel: false,
      downloadingProgress: 100,
      link: "",
      selectedFolder: "",
      isPanelVisible: false,
      isLoading: true,
      isLoadingData: false,

      isCheckedTemporaryStorage: checkedTemporary,
      isCheckedDocuments: checkedDocuments,
      isCheckedThirdParty: checkedThirdPartyResource,
      isCheckedThirdPartyStorage: checkedThirdPartyStorage,
    };
    this._isMounted = false;
    this.timerId = null;
  }
  componentDidMount() {
    this._isMounted = true;

    this.setState(
      {
        isLoading: true,
      },
      function () {
        SelectedFolder.getCommonThirdPartyList()
          .then(
            (thirdPartyArray) => (this.commonThirdPartyList = thirdPartyArray)
          )
          .then(() => getBackupProgress())
          .then((res) => {
            if (res) {
              this.setState({
                downloadingProgress: res.progress,
                link: res.link,
              });
              if (res.progress !== 100) {
                saveToSessionStorage("selectedManualStorageType", "");
                this._isMounted &&
                  this.setState({
                    isLoadingData: true,
                  });
                this.timerId = setInterval(() => this.getProgress(), 5000);
              }
            }
          })
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

  onClickButton = () => {
    saveToSessionStorage("selectedManualStorageType", "temporary");
    const storageParams = null;
    startBackup("4", storageParams);
    this.setState({
      downloadingProgress: 1,
      isLoadingData: true,
    });

    this.timerId = setInterval(() => this.getProgress(), 5000);
  };

  getProgress = () => {
    const { downloadingProgress, isLoadingData } = this.state;
    const { t } = this.props;
    console.log("downloadingProgress", downloadingProgress);

    getBackupProgress()
      .then((res) => {
        if (res.error.length > 0 && res.progress !== 100) {
          saveToSessionStorage("selectedManualStorageType", "");
          clearInterval(this.timerId);
          this.timerId && toastr.error(`${res.error}`);
          console.log("error", res.error);
          this.timerId = null;
          this.setState({
            downloadingProgress: 100,
            isLoadingData: false,
          });
          return;
        }

        if (res.progress === 100) {
          //debugger;
          saveToSessionStorage("selectedManualStorageType", "");
          clearInterval(this.timerId);

          if (this._isMounted) {
            this.setState({
              link: res.link,
            });
            this.setState({
              isLoadingData: false,
            });
          }

          this.timerId && toastr.success(`${t("SuccessCopied")}`);
          this.timerId = null;
        }
        if (this._isMounted) {
          this.setState({
            downloadingProgress: res.progress,
          });
        }
      })
      .catch((err) => {
        clearInterval(this.timerId);
        this.timerId && toastr.error(err);
        this.timerId = null;
        if (this._isMounted) {
          this.setState({
            downloadingProgress: 100,
          });
        }
      });
  };

  setInterval = () => {
    this.setState({
      downloadingProgress: 1,
      isLoadingData: true,
    });
    this.timerId = setInterval(() => this.getProgress(), 5000);
  };

  onClickDownload = () => {
    const { link } = this.state;
    const url = window.location.origin;
    const downloadUrl = `${url}` + `${link}`;
    window.open(downloadUrl, "_blank");
  };

  onClickShowStorage = (e) => {
    const {
      isCheckedTemporaryStorage,
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
    } = this.state;
    const name = e.target.name;

    switch (+name) {
      case 0:
        if (isCheckedDocuments) {
          this.setState({
            isCheckedDocuments: false,
            isCheckedTemporaryStorage: true,
          });
        }
        if (isCheckedThirdParty) {
          this.setState({
            isCheckedThirdParty: false,
            isCheckedTemporaryStorage: true,
          });
        }
        if (isCheckedThirdPartyStorage) {
          this.setState({
            isCheckedThirdPartyStorage: false,
            isCheckedTemporaryStorage: true,
          });
        }
        break;
      case 1:
        if (isCheckedTemporaryStorage) {
          this.setState({
            isCheckedTemporaryStorage: false,
            isCheckedDocuments: true,
          });
        }
        if (isCheckedThirdParty) {
          this.setState({
            isCheckedThirdParty: false,
            isCheckedDocuments: true,
          });
        }
        if (isCheckedThirdPartyStorage) {
          this.setState({
            isCheckedThirdPartyStorage: false,
            isCheckedDocuments: true,
          });
        }
        break;

      case 2:
        if (isCheckedTemporaryStorage) {
          this.setState({
            isCheckedTemporaryStorage: false,
            isCheckedThirdParty: true,
          });
        }
        if (isCheckedDocuments) {
          this.setState({
            isCheckedDocuments: false,
            isCheckedThirdParty: true,
          });
        }
        if (isCheckedThirdPartyStorage) {
          this.setState({
            isCheckedThirdPartyStorage: false,
            isCheckedThirdParty: true,
          });
        }
        break;

      default:
        if (isCheckedTemporaryStorage) {
          this.setState({
            isCheckedTemporaryStorage: false,
            isCheckedThirdPartyStorage: true,
          });
        }
        if (isCheckedDocuments) {
          this.setState({
            isCheckedDocuments: false,
            isCheckedThirdPartyStorage: true,
          });
        }
        if (isCheckedThirdParty) {
          this.setState({
            isCheckedThirdParty: false,
            isCheckedThirdPartyStorage: true,
          });
        }
        break;
    }
  };
  onSetLoadingData = (isLoading) => {
    this._isMounted &&
      this.setState({
        isLoadingData: isLoading,
      });
  };
  render() {
    const { t, commonThirdPartyList } = this.props;
    const {
      downloadingProgress,
      link,
      isLoading,
      isCheckedTemporaryStorage,
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
      isLoadingData,
    } = this.state;
    const maxProgress = downloadingProgress === 100;

    return isLoading ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledComponent>
        <StyledModules>
          <RadioButton
            fontSize="13px"
            fontWeight="400"
            label={t("TemporaryStorage")}
            name={"0"}
            key={0}
            onClick={this.onClickShowStorage}
            isChecked={isCheckedTemporaryStorage}
            isDisabled={isLoadingData}
            value="value"
            className="automatic-backup_radio-button"
          />
          <Text className="category-item-description">
            {t("TemporaryStorageDescription")}
          </Text>
          {isCheckedTemporaryStorage && (
            <div className="category-item-wrapper temporary-storage">
              <div className="manual-backup_buttons">
                <Button
                  label={t("MakeCopy")}
                  onClick={this.onClickButton}
                  primary
                  isDisabled={!maxProgress}
                  size="medium"
                  tabIndex={10}
                />
                {link.length > 0 && maxProgress && (
                  <Button
                    label={t("DownloadBackup")}
                    onClick={this.onClickDownload}
                    isDisabled={false}
                    size="medium"
                    style={{ marginLeft: "8px" }}
                    tabIndex={11}
                  />
                )}
                {!maxProgress && (
                  <Button
                    label={t("Copying")}
                    onClick={() => console.log("click")}
                    isDisabled={true}
                    size="medium"
                    style={{ marginLeft: "8px" }}
                    tabIndex={11}
                  />
                )}
              </div>
            </div>
          )}
        </StyledModules>

        <StyledModules>
          <RadioButton
            fontSize="13px"
            fontWeight="400"
            label={t("DocumentsModule")}
            name={"1"}
            key={1}
            onClick={this.onClickShowStorage}
            isChecked={isCheckedDocuments}
            isDisabled={isLoadingData}
            value="value"
            className="automatic-backup_radio-button"
          />

          <Text className="category-item-description">
            {t("DocumentsModuleDescription")}
          </Text>

          {isCheckedDocuments && (
            <DocumentsModule
              maxProgress={maxProgress}
              setInterval={this.setInterval}
              isCheckedDocuments={isCheckedDocuments}
              isCopyingLocal={isLoadingData}
            />
          )}
        </StyledModules>

        <StyledModules
          isDisabled={
            this.commonThirdPartyList && this.commonThirdPartyList.length === 0
          }
        >
          <RadioButton
            fontSize="13px"
            fontWeight="400"
            label={t("ThirdPartyResource")}
            name={"2"}
            key={2}
            onClick={this.onClickShowStorage}
            isChecked={isCheckedThirdParty}
            isDisabled={
              (this.commonThirdPartyList &&
                this.commonThirdPartyList.length === 0) ||
              isLoadingData
            }
            value="value"
            className="automatic-backup_radio-button"
          />
          <Text className="category-item-description">
            {t("ThirdPartyResourceDescription")}
          </Text>
          <Text className="category-item-description note_description">
            {t("ThirdPartyResourceNoteDescription")}
          </Text>

          {isCheckedThirdParty && (
            <ThirdPartyModule
              maxProgress={maxProgress}
              commonThirdPartyList={this.commonThirdPartyList}
              setInterval={this.setInterval}
              isCopyingLocal={isLoadingData}
            />
          )}
        </StyledModules>

        <StyledModules>
          <RadioButton
            fontSize="13px"
            fontWeight="400"
            label={t("ThirdPartyStorage")}
            name={"3"}
            key={3}
            onClick={this.onClickShowStorage}
            isChecked={isCheckedThirdPartyStorage}
            isDisabled={isLoadingData}
            value="value"
            className="automatic-backup_radio-button"
          />
          <Text className="category-item-description">
            {t("ThirdPartyStorageDescription")}
          </Text>
          <Text className="category-item-description note_description">
            {t("ThirdPartyStorageNoteDescription")}
          </Text>
          {isCheckedThirdPartyStorage && (
            <ThirdPartyStorageModule
              maxProgress={maxProgress}
              isManualBackup
              setInterval={this.setInterval}
            />
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
      </StyledComponent>
    );
  }
}

export default withTranslation("Settings")(ManualBackup);
