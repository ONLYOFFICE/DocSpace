import React, { useEffect } from "react";
import FileInput from "@docspace/components/file-input";

const LocalFile = ({ onSelectLocalFile, hasError }) => {
  const onClickInput = (file) => {
    let data = new FormData();
    data.append("file", file);

    onSelectLocalFile(data);
  };

  useEffect(() => {
    return () => {
      onSelectLocalFile("");
    };
  }, []);
  return (
    <FileInput
      hasError={hasError}
      onInput={onClickInput}
      scale
      className="restore-backup_input"
    />
  );
};

export default LocalFile;
