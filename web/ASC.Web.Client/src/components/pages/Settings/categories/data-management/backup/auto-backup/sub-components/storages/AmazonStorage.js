import React from "react";
import { withTranslation } from "react-i18next";
import AmazonSettings from "../../../consumer-storage-settings/AmazonSettings";
import ScheduleComponent from "../ScheduleComponent";
import { StyledStoragesModule } from "../../../StyledBackup";

import { inject, observer } from "mobx-react";
class AmazonStorage extends React.Component {
  constructor(props) {
    super(props);
    const { selectedStorage, setCompletedFormFields } = this.props;

    setCompletedFormFields(
      AmazonSettings.formNames(),
      selectedStorage.properties
    );

    this.isDisabled = !selectedStorage?.isSet;
  }

  render() {
    const { t, isLoadingData, selectedStorage, ...rest } = this.props;

    return (
      <StyledStoragesModule>
        <AmazonSettings
          isLoadingData={isLoadingData}
          selectedStorage={selectedStorage}
          t={t}
        />

        <ScheduleComponent isLoadingData={isLoadingData} {...rest} />
      </StyledStoragesModule>
    );
  }
}

export default inject(({ backup }) => {
  const {
    setCompletedFormFields,
    updateDefaultSettings,
    resetNewFormSettings,
  } = backup;

  return {
    setCompletedFormFields,
    updateDefaultSettings,
    resetNewFormSettings,
  };
})(observer(withTranslation(["Settings", "Common"])(AmazonStorage)));
