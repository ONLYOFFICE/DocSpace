import React from "react";
import { withTranslation } from "react-i18next";
import AmazonSettings from "../consumer-storage-settings/AmazonSettings";
import Button from "@appserver/components/button";
import ScheduleComponent from "../sub-components-automatic-backup/scheduleComponent";
class AmazonStorage extends React.Component {
  constructor(props) {
    super(props);
    const { selectedStorage } = this.props;

    this.defaultBucketValue = selectedStorage.properties[0]?.value;

    this.defaultForcePathStyleValue = selectedStorage.properties[1]?.value;

    this.defaultRegionValue = selectedStorage.properties[2]?.value;

    this.defaultServiceUrlValue = selectedStorage.properties[3]?.value;

    this.defaultSSEValue = selectedStorage.properties[4]?.value;

    this.defaultUseHttpValue = selectedStorage.properties[5]?.value;

    this.state = {
      formSettings: {
        bucket: this.defaultBucketValue,
        forcePathStyle: this.defaultForcePathStyleValue,
        region: this.defaultRegionValue,
        serviceUrl: this.defaultServiceUrlValue,
        sse: this.defaultSSEValue,
        useHttp: this.defaultUseHttpValue,
      },
      formErrors: {},
      isChangedInput: false,
    };
    this.isDisabled = !selectedStorage?.isSet;
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
    const {
      bucket,
      forcePathStyle,
      region,
      serviceUrl,
      sse,
      useHttp,
    } = formSettings;

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

    const valuesArray = [
      bucket,
      forcePathStyle,
      region,
      serviceUrl,
      sse,
      useHttp,
    ];

    this.setState({
      isChangedInput: false,
      formErrors: {},
    });
    convertSettings(valuesArray.length, valuesArray);
    // debugger;
  };

  onCancelSettings = () => {
    const { onCancelModuleSettings } = this.props;

    onCancelModuleSettings();

    this.setState({
      formSettings: {
        bucket: this.defaultBucketValue,
        forcePathStyle: this.defaultForcePathStyleValue,
        region: this.defaultRegionValue,
        serviceUrl: this.defaultServiceUrlValue,
        sse: this.defaultSSEValue,
        useHttp: this.defaultUseHttpValue,
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
      <>
        <AmazonSettings
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
export default withTranslation(["Settings", "Common"])(AmazonStorage);
