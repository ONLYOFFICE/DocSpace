import React from "react";
import { withTranslation } from "react-i18next";
import TextInput from "@appserver/components/text-input";
import RackspaceSettings from "../../consumer-storage-settings/RackspaceSettings";

class RackspaceStorage extends React.Component {
  constructor(props) {
    super(props);
    const { onSetRequiredFormNames } = this.props;

    let formSettings = {};

    this.namesArray = RackspaceSettings.formNames();
    this.namesArray.forEach((elem) => (formSettings[elem] = ""));

    onSetRequiredFormNames([...this.namesArray, "path"]);

    this.state = {
      formSettings,
    };
  }

  componentWillUnmount() {
    this.props.onResetFormSettings();
  }

  onChange = (event) => {
    const { target } = event;
    const value = target.value;
    const name = target.name;
    const { formSettings } = this.state;
    const { onSetFormSettings } = this.props;

    onSetFormSettings(name, value);

    this.setState({ formSettings: { ...formSettings, ...{ [name]: value } } });
  };

  render() {
    const {
      t,
      isInitialLoading,
      isErrors,
      availableStorage,
      selectedId,
    } = this.props;

    const { formSettings } = this.state;

    return (
      <>
        <RackspaceSettings
          formSettings={formSettings}
          onChange={this.onChange}
          isLoading={isInitialLoading}
          isError={isErrors}
          selectedStorage={availableStorage[selectedId]}
        />

        <TextInput
          name="path"
          className="backup_text-input"
          scale={true}
          value={formSettings.path}
          onChange={this.onChange}
          isDisabled={isInitialLoading || !availableStorage[selectedId]?.isSet}
          placeholder={t("Path")}
          tabIndex={this.namesArray.length}
          hasError={isErrors?.path}
        />
      </>
    );
  }
}
export default withTranslation("Settings")(RackspaceStorage);
