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

  componentDidUpdate(prevProps) {
    const {
      isReset,
      isSuccessSave,
      resetNewFormSettings,
      updateDefaultSettings,
    } = this.props;

    if (isReset && isReset !== prevProps.isReset) {
      resetNewFormSettings();
    }

    if (isSuccessSave && isSuccessSave !== prevProps.isSuccessSave) {
      updateDefaultSettings();
    }
  }

  render() {
    const {
      t,
      isLoadingData,
      selectedStorage,
      formErrors,
      onSetIsChanged,
      ...rest
    } = this.props;

    return (
      <StyledStoragesModule>
        <AmazonSettings
          onSetIsChanged={onSetIsChanged}
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
