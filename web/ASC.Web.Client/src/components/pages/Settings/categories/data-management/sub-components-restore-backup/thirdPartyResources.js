import React from "react";
import SelectFileInput from "files/SelectFileInput";

class ThirdPartyResources extends React.Component {
  render() {
    const { onClose, isPanelVisible, onClickInput, onSelectFile } = this.props;
    return (
      <SelectFileInput
        onClickInput={onClickInput}
        onSelectFile={onSelectFile}
        onClose={onClose}
        isPanelVisible={isPanelVisible}
        foldersType="third-party"
        searchParam=".gz"
        isArchiveOnly
      />
    );
  }
}

export default ThirdPartyResources;
