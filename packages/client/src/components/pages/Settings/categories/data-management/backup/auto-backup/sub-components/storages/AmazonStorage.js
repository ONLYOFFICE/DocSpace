import React from "react";
import { withTranslation } from "react-i18next";
import AmazonSettings from "../../../consumer-storage-settings/AmazonSettings";
import ScheduleComponent from "../ScheduleComponent";
import { StyledStoragesModule } from "../../../StyledBackup";
class AmazonStorage extends React.Component {
  constructor(props) {
    super(props);
    const {
      selectedStorage,
      onSetRequiredFormNames,
      onSetFormSettings,
    } = this.props;

    this.defaultFormSettings = {};

    this.namesArray = AmazonSettings.formNames();
    this.requiredFields = AmazonSettings.requiredFormsName();

    onSetRequiredFormNames([...this.requiredFields]);

    const isCorrectFields =
      this.namesArray.length === selectedStorage.properties.length;

    if (isCorrectFields) {
      for (let i = 0; i < selectedStorage.properties.length; i++) {
        const elem = selectedStorage.properties[i].name;
        const value = selectedStorage.properties[i].value;

        this.defaultFormSettings[elem] = value;
      }
    } else {
      this.namesArray.forEach((elem) => (this.defaultFormSettings[elem] = ""));
    }

    onSetFormSettings(null, null, this.defaultFormSettings);

    this.state = {
      formSettings: { ...this.defaultFormSettings },
    };
    this.isDisabled = !selectedStorage?.isSet;
  }

  onChange = (event) => {
    const { formSettings } = this.state;
    const { onSetFormSettings, onSetIsChanged } = this.props;

    const { target } = event;
    const value = target.value;
    const name = target.name;

    onSetFormSettings(name, value);
    onSetIsChanged(true);

    this.setState({
      formSettings: { ...formSettings, [name]: value },
    });
  };

  componentDidUpdate(prevProps) {
    const { isReset, isSuccessSave, onSetFormSettings } = this.props;

    if (isReset && isReset !== prevProps.isReset) {
      onSetFormSettings(null, null, this.defaultFormSettings);
      this.setState({
        formSettings: {
          ...this.defaultFormSettings,
        },
      });
    }

    if (isSuccessSave && isSuccessSave !== prevProps.isSuccessSave) {
      this.defaultFormSettings = this.state.formSettings;
    }
  }

  render() {
    const { formSettings } = this.state;
    const {
      t,
      isLoadingData,
      selectedStorage,
      formErrors,
      ...rest
    } = this.props;

    return (
      <StyledStoragesModule>
        <AmazonSettings
          formSettings={formSettings}
          onChange={this.onChange}
          isLoadingData={isLoadingData}
          isError={formErrors}
          selectedStorage={selectedStorage}
          t={t}
        />

        <ScheduleComponent isLoadingData={isLoadingData} {...rest} />
      </StyledStoragesModule>
    );
  }
}
export default withTranslation(["Settings", "Common"])(AmazonStorage);
