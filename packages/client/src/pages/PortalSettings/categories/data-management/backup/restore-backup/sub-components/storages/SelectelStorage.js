import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import SelectelSettings from "../../../consumer-storage-settings/SelectelSettings";

class SelectelStorage extends React.Component {
  constructor(props) {
    super(props);
    const { setCompletedFormFields } = this.props;

    setCompletedFormFields({
      ...SelectelSettings.formNames(),
      filePath: "",
    });
  }

  render() {
    const { t, selectedStorage } = this.props;

    return (
      <>
        <SelectelSettings
          t={t}
          isNeedFilePath
          selectedStorage={selectedStorage}
        />
      </>
    );
  }
}

export default inject(({ backup }) => {
  const { setCompletedFormFields } = backup;

  return {
    setCompletedFormFields,
  };
})(observer(withTranslation("Settings")(SelectelStorage)));
