import React from "react";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import commonSettingsStyles from "../../utils/commonSettingsStyles";
import styled from "styled-components";
import Button from "@appserver/components/button";
import Checkbox from "@appserver/components/checkbox";
import { inject, observer } from "mobx-react";

import { getBackupProgress, startBackup } from "@appserver/common/api/portal";
import toastr from "@appserver/components/toast/toastr";
import { toast } from "react-toastify";
import ThirdPartyModule from "./sub-components-manual-backup/thirdPartyModule";
import DocumentsModule from "./sub-components-manual-backup/documentsModule";
import ThirdPartyStorageModule from "./sub-components/thirdPartyStorageModule";
import FloatingButton from "@appserver/common/components/FloatingButton";
import RadioButton from "@appserver/components/radio-button";
import { StyledModules, StyledComponent } from "./styled-backup";

class ManualBackup extends React.Component {
  constructor(props) {
    super(props);
    this.manualBackup = true;

    // this.state = {
    //   backupMailTemporaryStorage: false,
    //   backupMailDocuments: false,
    //   backupMailThirdParty: false,
    //   backupMailThirdPartyStorage: false,
    // };
    this.state = {
      isVisiblePanel: false,
      downloadingProgress: 100,
      link: "",
      selectedFolder: "",
      isPanelVisible: false,
      isLoading: true,

      isCheckedTemporaryStorage: true,
      isCheckedDocuments: false,
      isCheckedThirdParty: false,
      isCheckedThirdPartyStorage: false,
    };
    this._isMounted = false;
    this.timerId = null;
  }
  componentDidMount() {
    this._isMounted = true;

    const { getCommonThirdPartyList } = this.props;

    this.setState(
      {
        isLoading: true,
      },
      function () {
        getCommonThirdPartyList()
          .then(() => getBackupProgress())
          .then((res) => {
            if (res) {
              this.setState({
                downloadingProgress: res.progress,
                link: res.link,
              });
              if (res.progress !== 100)
                this.timerId = setInterval(() => this.getProgress(), 5000);
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

  // onClickCheckbox = (e) => {
  //   const name = e.target.name;
  //   let change = !this.state[name];
  //   this.setState({ [name]: change });
  // };
  componentWillUnmount() {
    this._isMounted = false;
    clearInterval(this.timerId);
  }

  onClickButton = () => {
    const storageParams = null;
    startBackup("4", storageParams);
    this.setState({
      downloadingProgress: 1,
    });
    this.timerId = setInterval(() => this.getProgress(), 5000);
  };

  getProgress = () => {
    const { downloadingProgress } = this.state;
    const { t } = this.props;
    console.log("downloadingProgress", downloadingProgress);
    getBackupProgress()
      .then((res) => {
        if (res.error.length > 0 && res.progress !== 100) {
          clearInterval(this.timerId);
          this.timerId && toastr.error(`${res.error}`);
          console.log("error", res.error);
          this.timerId = null;
          this.setState({
            downloadingProgress: 100,
          });
          return;
        }

        if (res.progress === 100) {
          clearInterval(this.timerId);

          if (this._isMounted) {
            this.setState({
              link: res.link,
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
  render() {
    const {
      t,
      providers,
      panelVisible,
      folderPath,
      commonThirdPartyList,
    } = this.props;
    const {
      downloadingProgress,
      link,
      isPanelVisible,
      isLoading,
      isCheckedTemporaryStorage,
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
    } = this.state;
    const maxProgress = downloadingProgress === 100;
    // const {
    //   backupMailTemporaryStorage,
    //   backupMailDocuments,
    //   backupMailThirdParty,
    //   backupMailThirdPartyStorage,
    // } = this.state;
    // console.log("isLoading", isLoading);
    // console.log("commonThirdPartyList", commonThirdPartyList);
    return isLoading ? (
      <></>
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
            //isDisabled={isLoadingData}
            value="value"
            className="automatic-backup_radio-button"
          />
          <Text className="category-item-description">
            {t("TemporaryStorageDescription")}
          </Text>
          {isCheckedTemporaryStorage && (
            <div className="category-item-wrapper temporary-storage">
              {/* <div className="backup-include_mail">
            <Checkbox
              name={"backupMailTemporaryStorage"}
              isChecked={backupMailTemporaryStorage}
              label={t("IncludeMail")}
              onChange={this.onClickCheckbox}
            />
          </div> */}
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
            //isDisabled={isLoadingData}
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
            />
          )}
        </StyledModules>

        <StyledModules>
          <RadioButton
            fontSize="13px"
            fontWeight="400"
            label={t("ThirdPartyResource")}
            name={"2"}
            key={2}
            onClick={this.onClickShowStorage}
            isChecked={isCheckedThirdParty}
            //isDisabled={isLoadingData}
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
              commonThirdPartyList={commonThirdPartyList}
              setInterval={this.setInterval}
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
            //isDisabled={isLoadingData}
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
            icon="upload"
            alert={false}
            percent={downloadingProgress}
          />
        )}
      </StyledComponent>
    );
  }
}

export default inject(({ auth, setup }) => {
  const { setPanelVisible, panelVisible } = auth;
  const { folderPath } = auth.settingsStore;
  const { getCommonThirdPartyList } = setup;
  const { commonThirdPartyList } = setup.dataManagement;

  return {
    setPanelVisible,
    panelVisible,
    folderPath,

    commonThirdPartyList,
    getCommonThirdPartyList,
  };
})(withTranslation("Settings")(observer(ManualBackup)));
