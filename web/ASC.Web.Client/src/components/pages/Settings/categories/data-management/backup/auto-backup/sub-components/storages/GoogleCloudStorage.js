import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import GoogleCloudSettings from "../../../consumer-storage-settings/GoogleCloudSettings";
import ScheduleComponent from "../ScheduleComponent";
import { StyledStoragesModule } from "../../../StyledBackup";
class GoogleCloudStorage extends React.Component {
  constructor(props) {
    super(props);
    const { selectedStorage, setCompletedFormFields } = this.props;

    setCompletedFormFields(GoogleCloudSettings.formNames(), "googlecloud");

    this.isDisabled = !selectedStorage?.isSet;
  }

  render() {
    const { t, isLoadingData, selectedStorage, ...rest } = this.props;

    return (
      <StyledStoragesModule>
        <GoogleCloudSettings selectedStorage={selectedStorage} />

        <ScheduleComponent isLoadingData={isLoadingData} {...rest} />
      </StyledStoragesModule>
    );
  }
}

export default inject(({ backup }) => {
  const { setCompletedFormFields } = backup;

  return {
    setCompletedFormFields,
  };
})(observer(withTranslation(["Settings", "Common"])(GoogleCloudStorage)));
