import React from "react";

import { withTranslation } from "react-i18next";

import Button from "@appserver/components/button";
import Loader from "@appserver/components/loader";
import Checkbox from "@appserver/components/checkbox";
import Text from "@appserver/components/text";
import RadioButton from "@appserver/components/radio-button";

import SelectFolderDialog from "files/SelectFolderDialog";

import { StyledComponent } from "./styled-backup";
import BackupListModalDialog from "./sub-components-restore-backup/backupListModalDialog";
import Documents from "./sub-components-restore-backup/documents";
import ThirdPartyResources from "./sub-components-restore-backup/thirdPartyResources";
import ThirdPartyStorages from "./sub-components-restore-backup/thirdPartyStorages";
import { StyledRestoreModules } from "./styled-backup";

const ICON_URL = "/images/icons/24/file_archive.svg";
const FILTER_TYPE = 10;
const FILTER_VALUE = "gz";
const WITH_SUBFOLDERS = true;
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
    };
  }

  componentDidMount() {
    this.setState(
      {
        isLoading: true,
      },
      function () {
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
    const { t } = this.props;
    const {
      isChecked,
      isLoading,
      isNotify,
      isVisibleDialog,
      isPanelVisible,
      isCheckedDocuments,
      isCheckedThirdParty,
      isCheckedThirdPartyStorage,
    } = this.state;

    return isLoading ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <StyledComponent>
        <Text className="category-item-description">
          {t("DataRestoreDescription")}
        </Text>
        <Text className="category-item-description restore-source">
          {t("Source")}
        </Text>

        <StyledRestoreModules>
          <RadioButton
            fontSize="13px"
            fontWeight="400"
            label={t("DocumentsModule")}
            name={"1"}
            key={1}
            onClick={this.onClickShowStorage}
            isChecked={isCheckedDocuments}
            isDisabled={false}
            value="value"
            className="backup_radio-button"
          />
        </StyledRestoreModules>

        <StyledRestoreModules
          isDisabled={
            this.commonThirdPartyList && this.commonThirdPartyList.length === 0
          }
        >
          <RadioButton
            fontSize="13px"
            fontWeight="400"
            label={t("ThirdPartyResource")}
            name={"2"}
            key={1}
            onClick={this.onClickShowStorage}
            isChecked={isCheckedThirdParty}
            isDisabled={
              this.commonThirdPartyList &&
              this.commonThirdPartyList.length === 0
            }
            value="value"
            className="backup_radio-button"
          />
        </StyledRestoreModules>

        <StyledRestoreModules>
          <RadioButton
            fontSize="13px"
            fontWeight="400"
            label={t("ThirdPartyStorage")}
            name={"3"}
            key={1}
            onClick={this.onClickShowStorage}
            isChecked={isCheckedThirdPartyStorage}
            isDisabled={false}
            value="value"
            className="backup_radio-button"
          />
        </StyledRestoreModules>

        {isCheckedDocuments && (
          <Documents
            isPanelVisible={isPanelVisible}
            onClose={this.onPanelClose}
            onClickInput={this.onClickInput}
            iconUrl={ICON_URL}
            filterType={FILTER_TYPE}
            filterValue={FILTER_VALUE}
            withSubfolders={WITH_SUBFOLDERS}
          />
        )}
        {isCheckedThirdParty && (
          <ThirdPartyResources
            isPanelVisible={isPanelVisible}
            onClose={this.onPanelClose}
            onClickInput={this.onClickInput}
            iconUrl={ICON_URL}
            filterType={FILTER_TYPE}
            filterValue={FILTER_VALUE}
            withSubfolders={WITH_SUBFOLDERS}
          />
        )}
        {isCheckedThirdPartyStorage && <ThirdPartyStorages />}

        <Text className="restore-backup_list" onClick={this.onClickBackupList}>
          {t("BackupList")}
        </Text>
        {isVisibleDialog && (
          <BackupListModalDialog
            t={t}
            isVisibleDialog={isVisibleDialog}
            isLoading={isLoading}
            onModalClose={this.onModalClose}
          />
        )}
        <Checkbox
          truncate
          className="restore-backup-checkbox_notification"
          onChange={this.onChangeCheckboxNotify}
          isChecked={isNotify}
          label={t("UserNotification")}
        />

        <Text className="category-item-description restore-source restore-warning">
          {t("Common:Warning")}
          {"!"}
        </Text>
        <Text className="category-item-description ">
          {t("RestoreSettingsWarning")}
        </Text>
        <Text className="category-item-description restore-warning_link">
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
          onClick={() => console.log("click")}
          primary
          isDisabled={!isChecked}
          size="medium"
          tabIndex={10}
        />
      </StyledComponent>
    );
  }
}

export default withTranslation(["Settings", "Common"])(RestoreBackup);
