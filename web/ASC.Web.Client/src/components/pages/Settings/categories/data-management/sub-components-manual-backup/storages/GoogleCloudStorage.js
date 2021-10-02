import React from "react";
import { withTranslation } from "react-i18next";
import Button from "@appserver/components/button";
import GoogleCloudSettings from "../../consumer-storage-settings/GoogleCloudSettings";

class GoogleCloudStorage extends React.Component {
  constructor(props) {
    super(props);
    const { availableStorage, selectedId } = this.props;

    this.state = {
      formSettings: {
        bucket: "",
      },
      isError: false,
      isChangedInput: false,
      formErrors: {},
    };
    this.isDisabled =
      availableStorage[selectedId] && !availableStorage[selectedId].isSet;

    this.placeholder =
      availableStorage[selectedId] &&
      availableStorage[selectedId].properties[0].title;
    this._isMounted = false;
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
    const { bucket } = formSettings;
    const { onMakeCopyIntoStorage, isInvalidForm } = this.props;

    const isInvalid = isInvalidForm({
      bucket,
    });

    const hasError = isInvalid[0];
    const errors = isInvalid[1];

    if (hasError) {
      this.setState({ formErrors: errors });
      return;
    }

    onMakeCopyIntoStorage([bucket]);
    this.setState({ formErrors: {} });
  };
  render() {
    const { formSettings, formErrors } = this.state;
    const {
      t,
      isLoadingData,
      isLoading,
      maxProgress,
      availableStorage,
      selectedId,
    } = this.props;

    return (
      <>
        <GoogleCloudSettings
          formSettings={formSettings}
          onChange={this.onChange}
          isLoading={isLoading}
          isError={formErrors}
          availableStorage={availableStorage}
          selectedId={selectedId}
          isLoadingData={isLoadingData}
        />

        <div className="manual-backup_buttons">
          <Button
            label={t("MakeCopy")}
            onClick={this.onMakeCopy}
            primary
            isDisabled={!maxProgress}
            size="medium"
            tabIndex={10}
          />
          {!maxProgress && (
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
export default withTranslation("Settings")(GoogleCloudStorage);
