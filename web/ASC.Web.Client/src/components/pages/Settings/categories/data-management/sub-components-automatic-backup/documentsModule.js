import React from "react";
import { withTranslation } from "react-i18next";
import SelectFolderInput from "files/SelectFolderInput";
import ScheduleComponent from "./scheduleComponent";

class DocumentsModule extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isPanelVisible: false,
    };
  }

  onClickInput = () => {
    this.setState({
      isPanelVisible: true,
    });
  };

  onClose = () => {
    this.setState({
      isPanelVisible: false,
    });
  };

  render() {
    const { isPanelVisible, isLoading } = this.state;
    const {
      isLoadingData,
      onSetLoadingData,
      onSelectFolder,

      weeklySchedule,
      monthlySchedule,

      selectedPeriodLabel,
      selectedWeekdayLabel,
      selectedHour,
      selectedMonthDay,
      selectedMaxCopies,

      onSelectMaxCopies,
      onSelectPeriod,
      onSelectWeekDay,
      onSelectMonthNumber,
      onSelectTime,

      periodsObject,
      weekdaysLabelArray,
      monthNumbersArray,
      hoursArray,
      maxNumberCopiesArray,
      defaultSelectedFolder,

      isReset,
      resourcesModule,
      isError,
    } = this.props;

    return (
      <>
        <SelectFolderInput
          onSelectFolder={onSelectFolder}
          onClose={this.onClose}
          onClickInput={this.onClickInput}
          isPanelVisible={isPanelVisible}
          isError={isError}
          onSetLoadingData={onSetLoadingData}
          foldersType="common"
          withoutProvider
          isSavingProcess={isLoadingData}
          id={!resourcesModule ? defaultSelectedFolder : ""}
          isReset={isReset}
          onSetLoadingData={onSetLoadingData}
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
      </>
    );
  }
}
export default withTranslation(["Settings", "Common"])(DocumentsModule);
