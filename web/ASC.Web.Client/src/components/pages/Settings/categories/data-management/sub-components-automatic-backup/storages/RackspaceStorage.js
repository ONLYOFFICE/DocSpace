import React from "react";
import { withTranslation } from "react-i18next";
import RackspaceSettings from "../../consumer-storage-settings/RackspaceSettings";
import Button from "@appserver/components/button";
import ScheduleComponent from "../../sub-components-automatic-backup/ScheduleComponent";
import { StyledStoragesModule } from "../../StyledBackup";
class RackspaceStorage extends React.Component {
  constructor(props) {
    super(props);
    const { selectedStorage } = this.props;

    this.defaultPrivateValue = selectedStorage.properties[0].value;

    this.defaultPublicValue = selectedStorage.properties[1].value;

    this.defaultRegion = selectedStorage.properties[2].value;

    this.state = {
      formSettings: {
        private_container: this.defaultPrivateValue,
        public_container: this.defaultPublicValue,
        region: this.defaultRegion,
      },
      formErrors: {},
      isError: false,
      isChangedInput: false,
    };
    this.isDisabled = !selectedStorage.isSet;
  }

  onChange = (event) => {
    const { formSettings } = this.state;
    const { target } = event;
    const value = target.value;
    const name = target.name;

    this.setState({
      isChangedInput: true,
      formSettings: { ...formSettings, [name]: value },
    });
  };

  onSaveSettings = () => {
    const { convertSettings, isInvalidForm } = this.props;
    const { formSettings } = this.state;
    const { private_container, public_container, region } = formSettings;

    const isInvalid = isInvalidForm({
      private_container,
      region,
      public_container,
    });

    const hasError = isInvalid[0];
    const errors = isInvalid[1];

    if (hasError) {
      this.setState({ formErrors: errors });
      return;
    }

    const valuesArray = [private_container, public_container, region];

    this.setState({
      isChangedInput: false,
      formErrors: {},
    });
    convertSettings(valuesArray.length, valuesArray);
  };

  onCancelSettings = () => {
    const { onCancelModuleSettings } = this.props;

    onCancelModuleSettings();

    this.setState({
      formSettings: {
        private_container: this.defaultPrivateValue,
        public_container: this.defaultPublicValue,
        region: this.defaultRegion,
      },
      formErrors: {},
      isChangedInput: false,
    });
  };

  render() {
    const { isChangedInput, formSettings, formErrors } = this.state;
    const {
      t,
      isLoadingData,

      isCopyingToLocal,
      isChanged,

      selectedStorage,

      selectedPeriodLabel,
      selectedWeekdayLabel,
      selectedMonthDay,
      selectedHour,
      selectedMaxCopies,
      monthNumbersArray,
      hoursArray,
      maxNumberCopiesArray,
      periodsObject,
      weekdaysLabelArray,
      onSelectPeriod,
      onSelectWeekDay,
      onSelectMonthNumber,
      onSelectTime,
      onSelectMaxCopies,
      weeklySchedule,
      monthlySchedule,

      isChangedThirdParty,
    } = this.props;

    return (
      <StyledStoragesModule>
        <RackspaceSettings
          formSettings={formSettings}
          onChange={this.onChange}
          isLoadingData={isLoadingData}
          isError={formErrors}
          selectedStorage={selectedStorage}
          t={t}
        />

        <ScheduleComponent
          isLoadingData={isLoadingData}
          selectedPeriodLabel={selectedPeriodLabel}
          selectedWeekdayLabel={selectedWeekdayLabel}
          selectedMonthDay={selectedMonthDay}
          selectedHour={selectedHour}
          selectedMaxCopies={selectedMaxCopies}
          monthNumbersArray={monthNumbersArray}
          hoursArray={hoursArray}
          maxNumberCopiesArray={maxNumberCopiesArray}
          periodsObject={periodsObject}
          weekdaysLabelArray={weekdaysLabelArray}
          onSelectPeriod={onSelectPeriod}
          onSelectWeekDay={onSelectWeekDay}
          onSelectMonthNumber={onSelectMonthNumber}
          onSelectTime={onSelectTime}
          onSelectMaxCopies={onSelectMaxCopies}
          weeklySchedule={weeklySchedule}
          monthlySchedule={monthlySchedule}
        />

        {(isChanged || isChangedThirdParty || isChangedInput) && (
          //isChanged - from auto backup, monitor  period, time and etc. options;
          //isChangedThirdParty - from storages module, monitors selection storage changes
          //isChangedInput - monitors inputs changes
          <div className="backup_storages-buttons">
            <Button
              label={t("Common:Save")}
              onClick={this.onSaveSettings}
              primary
              isDisabled={isCopyingToLocal || this.isDisabled}
              size="medium"
              tabIndex={10}
              className="save-button"
            />

            <Button
              label={t("Common: Cancel")}
              onClick={this.onCancelSettings}
              isDisabled={isCopyingToLocal}
              size="medium"
              tabIndex={10}
            />
          </div>
        )}
      </StyledStoragesModule>
    );
  }
}
export default withTranslation(["Settings", "Common"])(RackspaceStorage);
