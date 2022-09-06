import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import GoogleCloudSettings from "../../../consumer-storage-settings/GoogleCloudSettings";

class GoogleCloudStorage extends React.Component {
  constructor(props) {
    super(props);
    const { setCompletedFormFields } = this.props;

    setCompletedFormFields({
      ...GoogleCloudSettings.formNames(),
      filePath: "",
    });
  }

  render() {
    const { t, selectedStorage } = this.props;

    return (
      <>
        <GoogleCloudSettings
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
})(observer(withTranslation("Settings")(GoogleCloudStorage)));
