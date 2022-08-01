import React from "react";
import SelectFileInput from "client/SelectFileInput";

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
        filesListTitle={t("SelectFileInGZFormat")}
        withoutResetFolderTree
        ignoreSelectedFolderTree
        maxFolderInputWidth="446px"
      />
    );
  }
}

export default Documents;
