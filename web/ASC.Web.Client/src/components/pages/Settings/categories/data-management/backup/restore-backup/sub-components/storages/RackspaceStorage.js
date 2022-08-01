import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import RackspaceSettings from "../../../consumer-storage-settings/RackspaceSettings";

class RackspaceStorage extends React.Component {
  constructor(props) {
    super(props);
    const { setCompletedFormFields } = this.props;

    setCompletedFormFields({
      ...RackspaceSettings.formNames(),
      filePath: "",
    });
  }

  render() {
    const { t, selectedStorage } = this.props;

    return (
      <>
        <RackspaceSettings
          t={t}
          selectedStorage={selectedStorage}
          isNeedFilePath
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
})(observer(withTranslation("Settings")(RackspaceStorage)));
