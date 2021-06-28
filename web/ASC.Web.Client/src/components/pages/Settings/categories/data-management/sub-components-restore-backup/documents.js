import React from "react";
import PropTypes from "prop-types";

import SelectFileInput from "files/SelectFileInput";
class Documents extends React.Component {
  render() {
    const {
      onClose,
      isPanelVisible,
      onClickInput,
      iconUrl,
      filterType,
      filterValue,
      withSubfolders,
    } = this.props;
    return (
      <SelectFileInput
        isPanelVisible={isPanelVisible}
        onClose={onClose}
        onClickInput={onClickInput}
        foldersType="common"
        isCommonWithoutProvider
        iconUrl={iconUrl}
        filterValue={filterValue}
        withSubfolders={withSubfolders}
        filterType={filterType}
      />
    );
  }
}

export default Documents;
