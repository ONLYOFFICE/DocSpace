import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { BackupStorageType } from "@appserver/common/constants";
import SelectFolderInput from "files/SelectFolderInput";
import ScheduleComponent from "./ScheduleComponent";
import ComboBox from "@appserver/components/combobox";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";

class ThirdPartyModule extends React.PureComponent {
  constructor(props) {
    super(props);
    const { setSelectedFolder, isResourcesDefault, isDocSpace } = props;

    if (isDocSpace) {
      this.accounts = [
        {
          key: "0",
          label: "Google Drive",
        },
        {
          key: "1",
          label: "OneDrive ",
        },
        {
          key: "2",
          label: "Dropbox ",
        },
        {
          key: "3",
          label: "Box.com",
        },
      ];
    }

    this.state = {
      isPanelVisible: false,
      ...(isDocSpace && { selectedAccount: this.accounts[0] }),
      isConnected: false,
    };
    !isResourcesDefault && setSelectedFolder("");
  }

  onClickInput = () => {
    this.setState({
      isPanelVisible: true,
    });
  };

  onClose = () => {
    this.setState({
      isPanelVisible: false,
    });
  };

  onSelectFolder = (id) => {
    const { setSelectedFolder } = this.props;
    setSelectedFolder(`${id}`);
  };

  onConnect = () => {
    const { isConnected } = this.state;

    this.setState({ isConnected: !isConnected });
  };

  onCopyingDirectly = () => {
    console.log("copy");
  };
  onSelectAccount = (options) => {
    const key = options.key;
    const label = options.label;

    this.setState({
      selectedAccount: { key, label },
    });
  };

  render() {
    const { isPanelVisible, selectedAccount, isConnected } = this.state;
    const {
      isError,
      isLoadingData,
      isReset,
      isSuccessSave,
      passedId,
      commonThirdPartyList,
      isDocSpace,
      isResourcesDefault,

      t,
      ...rest
    } = this.props;
    console.log(isLoadingData, !isConnected);
    return (
      <>
        {!isDocSpace ? (
          <div className="auto-backup_folder-input">
            <SelectFolderInput
              onSelectFolder={this.onSelectFolder}
              onClose={this.onClose}
              onClickInput={this.onClickInput}
              isPanelVisible={isPanelVisible}
              isError={isError}
              foldersType="third-party"
              isDisabled={commonThirdPartyList.length === 0 || isLoadingData}
              id={passedId}
              isReset={isReset}
              isSuccessSave={isSuccessSave}
              foldersList={commonThirdPartyList}
              withoutBasicSelection={isResourcesDefault ? false : true}
            />
          </div>
        ) : (
          <div className="auto-backup_third-party-module">
            <div className="auto-backup_connection">
              <ComboBox
                className="auto-backup_third-party-combo"
                options={this.accounts}
                selectedOption={{
                  key: 0,
                  label: selectedAccount.label,
                }}
                onSelect={this.onSelectAccount}
                noBorder={false}
                scaledOptions
                dropDownMaxHeight={300}
                tabIndex={1}
              />

              <Button
                label={t("Common:Connect")}
                onClick={this.onConnect}
                isDisabled={isLoadingData}
                size={"small"}
              />
            </div>
            <Text fontWeight={"600"}>{"Folder name:"}</Text>

            <SelectFolderInput
              onSelectFolder={this.onSelectFolder}
              onClose={this.onClose}
              onClickInput={this.onClickInput}
              isPanelVisible={isPanelVisible}
              isError={isError}
              foldersType="third-party"
              isDisabled={isLoadingData || !isConnected}
              id={passedId}
              isReset={isReset}
              isSuccessSave={isSuccessSave}
              foldersList={commonThirdPartyList}
              withoutBasicSelection={isResourcesDefault ? false : true}
            />
          </div>
        )}
        <ScheduleComponent isLoadingData={isLoadingData} {...rest} />
      </>
    );
  }
}

export default inject(({ backup }) => {
  const {
    setSelectedFolder,
    //selectedFolderId,
    defaultStorageType,
    commonThirdPartyList,
    defaultFolderId,
  } = backup;

  const isResourcesDefault =
    defaultStorageType === `${BackupStorageType.ResourcesModuleType}`;

  const passedId = isResourcesDefault ? defaultFolderId : "";
  const isDocSpace = true;
  return {
    setSelectedFolder,
    passedId,
    commonThirdPartyList,
    isResourcesDefault,
    isDocSpace,
  };
})(withTranslation(["Settings", "Common"])(observer(ThirdPartyModule)));
