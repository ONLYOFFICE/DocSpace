import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { BackupStorageType } from "@docspace/common/constants";
import SelectFolderInput from "client/SelectFolderInput";
import ScheduleComponent from "./ScheduleComponent";

class ThirdPartyModule extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      isPanelVisible: false,
    };
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

  render() {
    const { isPanelVisible } = this.state;
    const {
      isError,
      isLoadingData,
      isReset,
      isSuccessSave,
      passedId,
      commonThirdPartyList,
      ...rest
    } = this.props;

    return (
      <>
        <div className="auto-backup_folder-input">
          <SelectFolderInput
            onSelectFolder={this.onSelectFolder}
            onClose={this.onClose}
            onClickInput={this.onClickInput}
            isPanelVisible={isPanelVisible}
            isError={isError}
            foldersType="third-party"
            isDisabled={isLoadingData}
            id={passedId}
            isReset={isReset}
            isSuccessSave={isSuccessSave}
            foldersList={commonThirdPartyList}
          />
        </div>
        <ScheduleComponent isLoadingData={isLoadingData} {...rest} />
      </>
    );
  }
}

export default inject(({ backup }) => {
  const {
    setSelectedFolder,
    selectedFolderId,
    defaultStorageType,
    commonThirdPartyList,
    defaultFolderId,
  } = backup;

  const isResourcesDefault =
    defaultStorageType === `${BackupStorageType.ResourcesModuleType}`;

  const passedId = isResourcesDefault ? defaultFolderId : "";

  return {
    setSelectedFolder,
    passedId,
    commonThirdPartyList,
  };
})(withTranslation("Settings")(observer(ThirdPartyModule)));
