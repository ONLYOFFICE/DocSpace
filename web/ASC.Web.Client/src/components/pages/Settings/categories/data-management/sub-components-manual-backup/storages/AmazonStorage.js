import React from "react";
import { withTranslation } from "react-i18next";
import Button from "@appserver/components/button";
import AmazonSettings from "../../consumer-storage-settings/AmazonSettings";
class AmazonStorage extends React.Component {
  constructor(props) {
    super(props);
    const { t, availableStorage, selectedId } = this.props;

    this.state = {
      formSettings: {
        bucket: "",
        forcePathStyle: "",
        region: "",
        serviceUrl: "",
        sse: "",
        useHttp: "",
      },
      formErrors: {},
      isChangedInput: false,
    };
    this.namesArray = AmazonSettings.formNames();

    this.isDisabled =
      availableStorage[selectedId] && !availableStorage[selectedId].isSet;

    this.defaultBucketPlaceholder =
      availableStorage[selectedId] &&
      availableStorage[selectedId].properties[0].title;

    this.defaultForcePathStylePlaceholder = t("ForcePathStyle");

    this.defaultRegionPlaceholder =
      availableStorage[selectedId] &&
      availableStorage[selectedId].properties[2].title;

    this.defaultServiceUrlPlaceholder = t("ServiceUrl");

    this.defaultSSEPlaceholder = t("SSE");

    this.defaultUseHttpPlaceholder = t("UseHttp");
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
    const { bucket, region } = formSettings;
    const { onMakeCopyIntoStorage, isInvalidForm } = this.props;

    const isInvalid = isInvalidForm({
      bucket,
      region,
    });

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
      isLoading,
      maxProgress,
      availableStorage,
      selectedId,
    } = this.props;

    return (
      <>
        <AmazonSettings
          formSettings={formSettings}
          onChange={this.onChange}
          isLoading={isLoading}
          isLoadingData={isLoadingData}
          isError={formErrors}
          availableStorage={availableStorage}
          selectedId={selectedId}
          t={t}
        />

        <div className="manual-backup_buttons">
          <Button
            label={t("MakeCopy")}
            onClick={this.onMakeCopy}
            primary
            isDisabled={!maxProgress || this.isDisabled}
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
export default withTranslation("Settings")(AmazonStorage);
