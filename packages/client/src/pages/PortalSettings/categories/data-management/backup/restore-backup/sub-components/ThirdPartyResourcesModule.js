import React from "react";
import SelectFileInput from "client/SelectFileInput";
import { inject, observer } from "mobx-react";

class ThirdPartyResources extends React.Component {
  render() {
    const { t } = this.props;
    return (
      <SelectFileInput
        {...this.props}
        foldersType="third-party"
        searchParam=".gz"
        filesListTitle={t("SelectFileInGZFormat")}
        withoutResetFolderTree
        ignoreSelectedFolderTree
        isArchiveOnly
        maxFolderInputWidth="446px"
      />
    );
  }
}

export default inject(({ backup }) => {
  const { commonThirdPartyList } = backup;

  return {
    commonThirdPartyList,
  };
})(observer(ThirdPartyResources));
