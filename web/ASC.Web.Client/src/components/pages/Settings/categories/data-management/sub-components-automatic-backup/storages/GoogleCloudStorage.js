import React from "react";
import { withTranslation } from "react-i18next";
import GoogleCloudSettings from "../../consumer-storage-settings/GoogleCloudSettings";
import Button from "@appserver/components/button";
import ScheduleComponent from "../../sub-components-automatic-backup/scheduleComponent";
class GoogleCloudStorage extends React.Component {
  constructor(props) {
    super(props);
    const { selectedStorage } = this.props;

    this.defaultBucketValue = selectedStorage.properties[0].value;

    this.state = {
      formSettings: {
        bucket: this.defaultBucketValue,
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
    const { bucket } = formSettings;
    const isInvalid = isInvalidForm({
      bucket,
    });

    const hasError = isInvalid[0];
    const errors = isInvalid[1];

    if (hasError) {
      this.setState({ formErrors: errors });
      return;
    }

    const valuesArray = [bucket];

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
        bucket: this.defaultBucketValue,
      },
      formErrors: {},
      isChangedInput: false,
    });
  };

  render() {
    const { isChangedInput, formErrors, formSettings } = this.state;
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
      <>
        <GoogleCloudSettings
          formSettings={formSettings}
          onChange={this.onChange}
          isError={formErrors}
          selectedStorage={selectedStorage}
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
          <>
            <Button
              label={t("Common:Save")}
              onClick={this.onSaveSettings}
              primary
              isDisabled={isCopyingToLocal || this.isDisabled}
              size="medium"
              tabIndex={10}
            />

            <Button
              label={t("Common: Cancel")}
              onClick={this.onCancelSettings}
              primary
              isDisabled={isCopyingToLocal}
              size="medium"
              tabIndex={10}
            />
          </>
        )}
      </>
    );
  }
}
export default withTranslation(["Settings", "Common"])(GoogleCloudStorage);
