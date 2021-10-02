import React from "react";
import { withTranslation } from "react-i18next";
import TextInput from "@appserver/components/text-input";
import GoogleCloudSettings from "../../consumer-storage-settings/GoogleCloudSettings";

class GoogleCloudStorage extends React.Component {
  constructor(props) {
    super(props);
    const { onSetFormNames } = this.props;

    onSetFormNames(["bucket", "path"]);
  }

  componentWillUnmount() {
    this.props.onResetFormSettings();
  }

  render() {
    const {
      t,
      isLoading,
      formSettings,
      onChange,
      isErrors,
      availableStorage,
      selectedId,
    } = this.props;

    return (
      <>
        <GoogleCloudSettings
          formSettings={formSettings}
          onChange={onChange}
          isLoading={isLoading}
          isError={isErrors}
          selectedStorage={availableStorage[selectedId]}
        />
        <TextInput
          name="path"
          className="backup_text-input"
          scale={true}
          value={formSettings.path}
          onChange={onChange}
          isDisabled={isLoading || !availableStorage[selectedId]?.isSet}
          placeholder={t("Path")}
          tabIndex={2}
          hasError={isErrors?.path}
        />
      </>
    );
  }
}
export default withTranslation("Settings")(GoogleCloudStorage);
