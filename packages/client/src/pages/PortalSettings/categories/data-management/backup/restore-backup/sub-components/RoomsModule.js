import React from "react";

import SelectFileInputWrapper from "client/SelectFileInput";
import { FilesSelectorFilterTypes } from "@docspace/common/constants";
class RoomsModule extends React.Component {
  render() {
    const { t } = this.props;

    return (
      <SelectFileInputWrapper
        {...this.props}
        filteredType="exceptSortedByTags"
        withoutProvider
        isArchiveOnly
        filterParam={FilesSelectorFilterTypes.GZ}
        descriptionText={t("SelectFileInGZFormat")}
        withSubfolders={false}
        maxFolderInputWidth="446px"
        withoutBasicSelection
      />
    );
  }
}
export default RoomsModule;
