import React from "react";
import SelectFileInput from "files/SelectFileInput";
import { inject, observer } from "mobx-react";

class ThirdPartyResources extends React.Component {
  render() {
    return (
      <SelectFileInput
        {...this.props}
        foldersType="third-party"
        searchParam=".gz"
        isArchiveOnly
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
