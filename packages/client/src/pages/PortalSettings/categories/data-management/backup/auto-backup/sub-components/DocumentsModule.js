import React from "react";
import { inject, observer } from "mobx-react";
import { BackupStorageType } from "@docspace/common/constants";
import SelectFolderInput from "client/SelectFolderInput";
import ScheduleComponent from "./ScheduleComponent";

class DocumentsModule extends React.PureComponent {
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
            foldersType="common"
            withoutProvider
            isDisabled={isLoadingData}
            id={passedId}
            isReset={isReset}
            isSuccessSave={isSuccessSave}
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
    defaultFolderId,
  } = backup;

  const isDocumentsDefault =
    defaultStorageType === `${BackupStorageType.DocumentModuleType}`;

  const passedId = isDocumentsDefault ? defaultFolderId : "";

  return {
    setSelectedFolder,
    passedId,
  };
})(observer(DocumentsModule));
