import React from "react";
import { withTranslation } from "react-i18next";
import TextInput from "@docspace/components/text-input";
import AmazonSettings from "../../../consumer-storage-settings/AmazonSettings";

class AmazonStorage extends React.PureComponent {
  constructor(props) {
    super(props);
    const { onSetRequiredFormNames } = this.props;

    this.namesArray = AmazonSettings.formNames();

    const formSettings = {};

    this.namesArray = AmazonSettings.formNames();
    this.namesArray.forEach((elem) => (formSettings[elem] = ""));

    this.requiredFields = AmazonSettings.requiredFormsName();

    onSetRequiredFormNames([...this.requiredFields, "filePath"]);

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
        <AmazonSettings
          formSettings={formSettings}
          onChange={this.onChange}
          isError={isErrors}
          selectedStorage={availableStorage[selectedId]}
          t={t}
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
export default withTranslation("Settings")(AmazonStorage);
