import React from "react";
import { withTranslation } from "react-i18next";
import TextInput from "@appserver/components/text-input";
import RackspaceSettings from "../../consumer-storage-settings/RackspaceSettings";

class RackspaceStorage extends React.Component {
  constructor(props) {
    super(props);
    const { onSetFormNames } = this.props;

    this.namesArray = RackspaceSettings.formNames();
    onSetFormNames([...this.namesArray, "path"]);
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
    console.log("isErrors", isErrors);
    return (
      <>
        <RackspaceSettings
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
          tabIndex={this.namesArray.length}
          hasError={isErrors?.path}
        />
      </>
    );
  }
}
export default withTranslation("Settings")(RackspaceStorage);
