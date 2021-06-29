import React from "react";
import FileInput from "@appserver/components/file-input";

const LocalFile = ({ onSetRestoreParams }) => {
  const onInputFileHandler = (file) => {
    let data = new FormData();
    data.append("files", file);

    const backupId = "";
    const storageType = "3";
    const storageParams = [
      {
        key: "filePath",
        value: data,
      },
    ];

    onSetRestoreParams(backupId, storageType, storageParams);
  };
  return (
    <FileInput
      onInput={onInputFileHandler}
      scale
      className="restore-backup_input"
    />
  );
};

export default LocalFile;
