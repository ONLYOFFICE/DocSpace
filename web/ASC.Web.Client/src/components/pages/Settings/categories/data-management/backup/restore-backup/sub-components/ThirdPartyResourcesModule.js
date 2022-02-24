import React from "react";
import SelectFileInput from "files/SelectFileInput";

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

export default ThirdPartyResources;
