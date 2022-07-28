import React from "react";
import { withTranslation } from "react-i18next";
import TextInput from "@docspace/components/text-input";
import SelectelSettings from "../../../consumer-storage-settings/SelectelSettings";

class RackspaceStorage extends React.Component {
  constructor(props) {
    super(props);
    const { onSetRequiredFormNames } = this.props;

    const formSettings = {};
    this.namesArray = SelectelSettings.formNames();
    this.namesArray.forEach((elem) => (formSettings[elem] = ""));

    onSetRequiredFormNames([...this.namesArray, "filePath"]);

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
    const { t, isErrors, availableStorage, selectedId } = this.props;

    const { formSettings } = this.state;

    return (
      <>
        <SelectelSettings
          formSettings={formSettings}
          onChange={this.onChange}
          isError={isErrors}
          selectedStorage={availableStorage[selectedId]}
        />

        <TextInput
          name="filePath"
          className="backup_text-input"
          scale={true}
          value={formSettings.filePath}
          onChange={this.onChange}
          isDisabled={!availableStorage[selectedId]?.isSet}
          placeholder={t("Path")}
          tabIndex={this.namesArray.length}
          hasError={isErrors?.filePath}
        />
      </>
    );
  }
}
export default withTranslation("Settings")(RackspaceStorage);
