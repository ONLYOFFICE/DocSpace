import React from "react";
import { inject, observer } from "mobx-react";
import DirectThirdPartyConnection from "../../common-container/DirectThirdPartyConnection";

const ThirdPartyResources = (props) => {
  return (
    // return !isDocSpace ? (
    //   <SelectFileInput
    //     {...props}
    //     foldersType="third-party"
    //     searchParam=".gz"
    //     filesListTitle={t("SelectFileInGZFormat")}
    //     withoutResetFolderTree
    //     ignoreSelectedFolderTree
    //     isArchiveOnly
    //     maxFolderInputWidth="446px"
    //   />
    // ) : (
    <div className={"restore-backup_third-party-module"}>
      <DirectThirdPartyConnection
        {...props}
        withoutBasicSelection
        isFileSelection
      />
    </div>
  );
};

export default inject(({ backup }) => {
  const { commonThirdPartyList } = backup;

  return {
    commonThirdPartyList,
  };
})(observer(ThirdPartyResources));
