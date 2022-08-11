import React from "react";
import SelectFileInput from "files/SelectFileInput";

class Documents extends React.Component {
  render() {
    const { t } = this.props;
    return (
      <SelectFileInput
        {...this.props}
        foldersType="rooms"
        withoutProvider
        isArchiveOnly
        searchParam=".gz"
        filesListTitle={t("SelectFileInGZFormat")}
        withoutResetFolderTree
        ignoreSelectedFolderTree
        maxFolderInputWidth="446px"
        withoutBasicSelection
      />
    );
  }
}

export default Documents;
