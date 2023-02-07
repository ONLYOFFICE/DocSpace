import React from "react";
import { inject, observer } from "mobx-react";
import DirectThirdPartyConnection from "../../common-container/DirectThirdPartyConnection";

const ThirdPartyResources = (props) => {
  return (
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
