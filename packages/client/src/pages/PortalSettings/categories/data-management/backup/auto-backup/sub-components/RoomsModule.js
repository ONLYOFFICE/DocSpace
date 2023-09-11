import React from "react";
import { inject, observer } from "mobx-react";
import { BackupStorageType } from "@docspace/common/constants";
import ScheduleComponent from "./ScheduleComponent";
import FilesSelectorInput from "SRC_DIR/components/FilesSelectorInput";

class RoomsModule extends React.PureComponent {
  onSelectFolder = (id) => {
    const { setSelectedFolder } = this.props;

    setSelectedFolder(`${id}`);
  };

  render() {
    const {
      isError,
      isLoadingData,
      savingProcess,
      passedId,
      isSavingProcess,
      isResetProcess,
      isDocumentsDefault,
      ...rest
    } = this.props;

    return (
      <>
        <div className="auto-backup_folder-input">
          <FilesSelectorInput
            onSelectFolder={this.onSelectFolder}
            {...(passedId && { id: passedId })}
            withoutInitPath={!isDocumentsDefault}
            isError={isError}
            isDisabled={isLoadingData}
            isRoomsOnly
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
    isSavingProcess,
    isResetProcess,
  } = backup;

  const isDocumentsDefault =
    defaultStorageType === `${BackupStorageType.DocumentModuleType}`;

  const passedId = isDocumentsDefault ? defaultFolderId : "";

  return {
    defaultFolderId,
    selectedFolderId,
    setSelectedFolder,
    passedId,
    isSavingProcess,
    isResetProcess,
    isDocumentsDefault,
  };
})(observer(RoomsModule));
