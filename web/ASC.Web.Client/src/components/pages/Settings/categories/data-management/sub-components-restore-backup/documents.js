import React from "react";
import SelectFileInput from "files/SelectFileInput";

class Documents extends React.Component {
  render() {
    const { onClose, isPanelVisible, onClickInput, onSelectFile } = this.props;
    return (
      <SelectFileInput
        onClickInput={onClickInput}
        onSelectFile={onSelectFile}
        onClose={onClose}
        isPanelVisible={isPanelVisible}
        foldersType="common"
        withoutProvider
        isArchiveOnly
        searchParam=".gz"
      />
    );
  }
}

export default Documents;
