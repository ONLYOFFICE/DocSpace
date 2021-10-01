import React from "react";
import FileInput from "@appserver/components/file-input";

const LocalFile = ({ onSelectLocalFile }) => {
  const onClickInput = (file) => {
    let data = new FormData();
    data.append("files", file);

    onSelectLocalFile(data);
  };
  return (
    <FileInput onInput={onClickInput} scale className="restore-backup_input" />
  );
};

export default LocalFile;
