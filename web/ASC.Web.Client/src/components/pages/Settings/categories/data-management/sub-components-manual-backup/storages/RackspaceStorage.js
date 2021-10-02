import React from "react";
import { withTranslation } from "react-i18next";
import Button from "@appserver/components/button";
import RackspaceSettings from "../../consumer-storage-settings/RackspaceSettings";

class RackspaceStorage extends React.Component {
  constructor(props) {
    super(props);
    const { availableStorage, selectedId } = this.props;

    this.namesArray = RackspaceSettings.formNames();

    this.state = {
      formSettings: {
        private_container: "",
        public_container: "",
        region: "",
      },
      formErrors: {},
    };
    this.isDisabled =
      availableStorage[selectedId] && !availableStorage[selectedId].isSet;

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
    const { private_container, public_container, region } = formSettings;
    const { onMakeCopyIntoStorage, isInvalidForm } = this.props;

    const isInvalid = isInvalidForm({
      private_container,
      public_container,
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
      selectedId,
      availableStorage,
    } = this.props;

    return (
      <>
        <RackspaceSettings
          formSettings={formSettings}
          onChange={this.onChange}
          isLoading={isLoading}
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
export default withTranslation("Settings")(RackspaceStorage);
