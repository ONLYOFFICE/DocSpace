import React from "react";
import PropTypes from "prop-types";

import FileInput from "@appserver/components/file-input";
import SelectFileInput from "files/SelectFileInput";
class ThirdPartyResources extends React.Component {
  render() {
    const {
      onClose,
      isPanelVisible,
      onClickInput,
      iconUrl,
      filterType,
      filterValue,
      withSubfolders,
      onSelectFile,
    } = this.props;
    return (
      <SelectFileInput
        isPanelVisible={isPanelVisible}
        onClose={onClose}
        onClickInput={onClickInput}
        iconUrl={iconUrl}
        filterValue={filterValue}
        withSubfolders={withSubfolders}
        filterType={filterType}
        onSelectFile={onSelectFile}
        foldersType="third-party"
      />
    );
  }
}

export default ThirdPartyResources;
