import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import AmazonSettings from "../../../consumer-storage-settings/AmazonSettings";

class AmazonStorage extends React.PureComponent {
  constructor(props) {
    super(props);
    const { setCompletedFormFields, storageRegions } = this.props;

    setCompletedFormFields({
      ...AmazonSettings.formNames(storageRegions[0].systemName),
      filePath: "",
    });
  }

  render() {
    const { t, availableStorage, selectedId } = this.props;

    return (
      <>
        <AmazonSettings
          selectedStorage={availableStorage[selectedId]}
          t={t}
          isNeedFilePath
        />
      </>
    );
  }
}

export default inject(({ backup }) => {
  const { storageRegions, setCompletedFormFields } = backup;

  return {
    storageRegions,
    setCompletedFormFields,
  };
})(observer(withTranslation("Settings")(AmazonStorage)));
