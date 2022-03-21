import React from "react";
import SelectFileInput from "files/SelectFileInput";

class Documents extends React.Component {
  render() {
    return (
      <SelectFileInput
        {...this.props}
        foldersType="common"
        withoutProvider
        isArchiveOnly
        searchParam=".gz"
      />
    );
  }
}

export default Documents;
