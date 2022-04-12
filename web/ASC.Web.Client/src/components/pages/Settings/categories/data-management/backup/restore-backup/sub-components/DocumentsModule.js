import React from "react";
import SelectFileInput from "files/SelectFileInput";

class Documents extends React.Component {
  render() {
    const { t } = this.props;
    return (
      <SelectFileInput
        {...this.props}
        foldersType="common"
        withoutProvider
        isArchiveOnly
        searchParam=".gz"
        dialogName="Select File"
        filesListTitle={t("SelectFileInGZFormat")}
        withoutResetFolderTree
        ignoreSelectedFolderTree
      />
    );
  }
}

export default Documents;
