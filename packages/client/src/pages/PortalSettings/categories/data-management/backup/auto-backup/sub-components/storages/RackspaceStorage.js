import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import RackspaceSettings from "../../../consumer-storage-settings/RackspaceSettings";
import ScheduleComponent from "../ScheduleComponent";
import { StyledStoragesModule } from "../../../StyledBackup";
class RackspaceStorage extends React.Component {
  constructor(props) {
    super(props);
    const { selectedStorage, setCompletedFormFields } = this.props;

    setCompletedFormFields(RackspaceSettings.formNames(), "rackspace");
    this.isDisabled = !selectedStorage?.isSet;
  }

  render() {
    const { t, isLoadingData, selectedStorage, ...rest } = this.props;

    return (
      <StyledStoragesModule>
        <RackspaceSettings
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
  const { setCompletedFormFields } = backup;

  return {
    setCompletedFormFields,
  };
})(observer(withTranslation(["Settings", "Common"])(RackspaceStorage)));
