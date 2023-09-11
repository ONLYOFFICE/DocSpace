import React from "react";

import PropTypes from "prop-types";
import Button from "../button";
import Text from "../text";
import StyledSaveCancelButtons from "./styled-save-cancel-buttons";

const ButtonKeys = Object.freeze({
  enter: 13,
  esc: 27,
});

class SaveCancelButtons extends React.Component {
  componentDidMount() {
    document.addEventListener("keydown", this.onKeydown, false);
  }

  componentWillUnmount() {
    document.removeEventListener("keydown", this.onKeydown, false);
  }

  onKeydown = (e) => {
    const { onSaveClick, onCancelClick, displaySettings } = this.props;

    if (displaySettings) return;

    switch (e.keyCode) {
      case ButtonKeys.enter:
        onSaveClick();
        break;
      case ButtonKeys.esc:
        onCancelClick();
        break;
      default:
        break;
    }
  };

  render() {
    const {
      onSaveClick,
      onCancelClick,
      displaySettings,
      showReminder,
      reminderTest,
      saveButtonLabel,
      cancelButtonLabel,
      hasScroll,
      disableRestoreToDefault,
      className,
      id,
      isSaving,
      cancelEnable,
      tabIndex,
      saveButtonDisabled,
      additionalClassSaveButton,
      additionalClassCancelButton,
    } = this.props;

    const cancelButtonDisabled = cancelEnable
      ? false
      : typeof disableRestoreToDefault === "boolean"
      ? disableRestoreToDefault
      : !showReminder;

    const tabIndexSaveButton = tabIndex ? tabIndex : -1;
    const tabIndexCancelButton = tabIndex ? tabIndex + 1 : -1;

    const classNameSave = additionalClassSaveButton
      ? `save-button ` + additionalClassSaveButton
      : `save-button`;

    const classNameCancel = additionalClassCancelButton
      ? `cancel-button ` + additionalClassCancelButton
      : `cancel-button`;

    return (
      <StyledSaveCancelButtons
        className={className}
        id={id}
        displaySettings={displaySettings}
        showReminder={showReminder}
        hasScroll={hasScroll}
      >
        <div className="buttons-flex">
          <Button
            tabIndex={tabIndexSaveButton}
            className={classNameSave}
            size="normal"
            isDisabled={!showReminder || saveButtonDisabled}
            primary
            onClick={onSaveClick}
            label={saveButtonLabel}
            minwidth={displaySettings && "auto"}
            isLoading={isSaving}
          />
          <Button
            tabIndex={tabIndexCancelButton}
            className={classNameCancel}
            size="normal"
            isDisabled={cancelButtonDisabled || isSaving}
            onClick={onCancelClick}
            label={cancelButtonLabel}
            minwidth={displaySettings && "auto"}
          />
        </div>
        {showReminder && reminderTest && (
          <Text className="unsaved-changes">{reminderTest}</Text>
        )}
      </StyledSaveCancelButtons>
    );
  }
}

SaveCancelButtons.propTypes = {
  /** Accepts css id */
  id: PropTypes.string,
  /** Accepts css class */
  className: PropTypes.string,
  /** Message text that notifies of the unsaved changes */
  reminderTest: PropTypes.string,
  /** Save button label */
  saveButtonLabel: PropTypes.string,
  /** Cancel button label  */
  cancelButtonLabel: PropTypes.string,
  /** Sets a callback function that is triggered when the save button is clicked */
  onSaveClick: PropTypes.func,
  /** Sets a callback function that is triggered when the cancel button is clicked */
  onCancelClick: PropTypes.func,
  /** Reminder message that notifies of the unsaved changes (Only shown on desktops) */
  showReminder: PropTypes.bool,
  /** Sets save and cancel buttons block to 'position: static' instead of absolute */
  displaySettings: PropTypes.bool,
  /** Displays the scrollbar */
  hasScroll: PropTypes.bool,
  /** Sets the min width of the button */
  minwidth: PropTypes.string,
  /** Sets the Cancel button disabled by default */
  disableRestoreToDefault: PropTypes.bool,
  /** Sets the button to present a disabled state while executing an operation after clicking the save button */
  isSaving: PropTypes.bool,
  /** Activates the disabled button */
  cancelEnable: PropTypes.bool,
  /** Accepts css tab-index */
  tabIndex: PropTypes.number,
};

SaveCancelButtons.defaultProps = {
  saveButtonLabel: "Save",
  cancelButtonLabel: "Cancel",
};

export default SaveCancelButtons;
