import React from "react";
import { withTranslation } from "react-i18next";
import Button from "@appserver/components/button";
import AmazonSettings from "../../consumer-storage-settings/AmazonSettings";
class AmazonStorage extends React.Component {
  constructor(props) {
    super(props);
    const { availableStorage, selectedId } = this.props;

    const formSettings = {};

    this.namesArray = AmazonSettings.formNames();
    this.namesArray.forEach((elem) => (formSettings[elem] = ""));

    this.requiredFields = AmazonSettings.requiredFormsName();

    this.state = {
      formSettings,
      formErrors: {},
    };

    this.isDisabled =
      availableStorage[selectedId] && !availableStorage[selectedId].isSet;
  }

  onChange = (event) => {
    const { formSettings } = this.state;
    const { target } = event;
    const value = target.value;
    const name = target.name;

    this.setState({ formSettings: { ...formSettings, [name]: value } });
  };

  onMakeCopy = () => {
    const { formSettings } = this.state;
    const { onMakeCopyIntoStorage, isInvalidForm } = this.props;

    const requiredSettings = {};
    this.requiredFields.forEach(
      (el) => (requiredSettings[el] = formSettings[el])
    );

    const isInvalid = isInvalidForm(requiredSettings);

    const hasError = isInvalid[0];
    const errors = isInvalid[1];

    if (hasError) {
      this.setState({ formErrors: errors });
      return;
    }

    onMakeCopyIntoStorage(this.namesArray);
    this.setState({ formErrors: {} });
  };
  render() {
    const { formSettings, formErrors } = this.state;
    const {
      t,
      isLoadingData,
      isMaxProgress,
      availableStorage,
      selectedId,
    } = this.props;

    return (
      <>
        <AmazonSettings
          formSettings={formSettings}
          onChange={this.onChange}
          isLoadingData={isLoadingData}
          isError={formErrors}
          selectedStorage={availableStorage[selectedId]}
          t={t}
        />

        <div className="manual-backup_buttons">
          <Button
            label={t("MakeCopy")}
            onClick={this.onMakeCopy}
            primary
            isDisabled={!isMaxProgress || this.isDisabled}
            size="medium"
            tabIndex={10}
          />
          {!isMaxProgress && (
            <Button
              label={t("Copying")}
              onClick={() => console.log("click")}
              isDisabled={true}
              size="medium"
              style={{ marginLeft: "8px" }}
              tabIndex={11}
            />
          )}
        </div>
      </>
    );
  }
}
export default withTranslation("Settings")(AmazonStorage);
