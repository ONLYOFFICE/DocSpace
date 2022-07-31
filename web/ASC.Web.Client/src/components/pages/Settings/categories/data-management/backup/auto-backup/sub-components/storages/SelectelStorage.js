import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import SelectelSettings from "../../../consumer-storage-settings/SelectelSettings";
import ScheduleComponent from "../ScheduleComponent";
import { StyledStoragesModule } from "../../../StyledBackup";
class SelectelStorage extends React.Component {
  constructor(props) {
    super(props);
    const { selectedStorage, setCompletedFormFields } = this.props;

    setCompletedFormFields(SelectelSettings.formNames(), "selectel");
    this.isDisabled = !selectedStorage.isSet;
  }

  render() {
    const {
      t,
      isLoadingData,
      formErrors,
      selectedStorage,
      ...rest
    } = this.props;

    return (
      <StyledStoragesModule>
        <SelectelSettings
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
})(observer(withTranslation(["Settings", "Common"])(SelectelStorage)));
