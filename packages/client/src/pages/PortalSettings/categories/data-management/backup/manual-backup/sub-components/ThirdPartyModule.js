import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import Button from "@docspace/components/button";
import { getFromLocalStorage } from "../../../../../utils";
import { BackupStorageType } from "@docspace/common/constants";
import DirectThirdPartyConnection from "../../common-container/DirectThirdPartyConnection";

let folder = "";
const ThirdPartyResource = "ThirdPartyResource";

class ThirdPartyModule extends React.Component {
  constructor(props) {
    super(props);

    folder = getFromLocalStorage("LocalCopyFolder");
    const moduleType = getFromLocalStorage("LocalCopyStorageType");

    const selectedFolder = moduleType === ThirdPartyResource ? folder : "";

    this.state = {
      isStartCopy: false,
      selectedFolder: selectedFolder,
      isError: false,
      isLoading: false,
    };

    this._isMount = false;
  }

  componentDidMount() {
    this._isMount = true;
  }
  componentWillUnmount() {
    this._isMount = false;
  }

  onSelectFolder = (folderId) => {
    this._isMount &&
      this.setState({
        selectedFolder: folderId,
      });
  };

  isInvalidForm = () => {
    const { selectedFolder } = this.state;

    if (selectedFolder) return false;

    this.setState({
      isError: true,
    });

    return true;
  };

  onMakeCopy = async () => {
    const { onMakeCopy } = this.props;
    const { selectedFolder, isError } = this.state;
    const { ResourcesModuleType } = BackupStorageType;
    if (this.isInvalidForm()) return;

    isError &&
      this.setState({
        isError: false,
      });

    this.setState({
      isStartCopy: true,
    });

    await onMakeCopy(
      selectedFolder,
      ThirdPartyResource,
      `${ResourcesModuleType}`
    );

    this.setState({
      isStartCopy: false,
    });
  };

  render() {
    const {
      isMaxProgress,
      t,
      buttonSize,
      connectedThirdPartyAccount,
      isTheSameThirdPartyAccount,
    } = this.props;
    const { isError, isStartCopy, selectedFolder } = this.state;

    const isModuleDisabled = !isMaxProgress || isStartCopy;

    return (
      <div className="manual-backup_third-party-module">
        <DirectThirdPartyConnection
          t={t}
          onSelectFolder={this.onSelectFolder}
          isDisabled={isModuleDisabled}
          {...(selectedFolder && { id: selectedFolder })}
          withoutInitPath={!selectedFolder}
          isError={isError}
          buttonSize={buttonSize}
        />

        {connectedThirdPartyAccount?.id && isTheSameThirdPartyAccount && (
          <Button
            label={t("Common:CreateCopy")}
            onClick={this.onMakeCopy}
            primary
            isDisabled={isModuleDisabled || selectedFolder === ""}
            size={buttonSize}
          />
        )}
      </div>
    );
  }
}
export default inject(({ backup }) => {
  const { connectedThirdPartyAccount, isTheSameThirdPartyAccount } = backup;

  return {
    connectedThirdPartyAccount,
    isTheSameThirdPartyAccount,
  };
})(withTranslation(["Settings", "Common"])(observer(ThirdPartyModule)));
