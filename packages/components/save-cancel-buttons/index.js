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
    const { onSaveClick, onCancelClick } = this.props;

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
    } = this.props;

    const cancelButtonDisabled = cancelEnable
      ? false
      : typeof disableRestoreToDefault === "boolean"
      ? disableRestoreToDefault
      : !showReminder;

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
            className="save-button"
            size="normal"
            isDisabled={!showReminder}
            primary
            onClick={onSaveClick}
            label={saveButtonLabel}
            minwidth={displaySettings && "auto"}
            isLoading={isSaving}
          />
          <Button
            className="cancel-button"
            size="normal"
            isDisabled={cancelButtonDisabled || isSaving}
            onClick={onCancelClick}
            label={cancelButtonLabel}
            minwidth={displaySettings && "auto"}
          />
        </div>
        {showReminder && (
          <Text className="unsaved-changes"> {reminderTest} </Text>
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
  /** Text reminding of unsaved changes */
  reminderTest: PropTypes.string,
  /** Save button label */
  saveButtonLabel: PropTypes.string,
  /** Cancel button label  */
  cancelButtonLabel: PropTypes.string,
  /** What the save button will trigger when clicked */
  onSaveClick: PropTypes.func,
  /** What the cancel button will trigger when clicked */
  onCancelClick: PropTypes.func,
  /** Show message about unsaved changes (Only shown on desktops) */
  showReminder: PropTypes.bool,
  /** Tells when the button should present a disabled state */
  displaySettings: PropTypes.bool,
  hasScroll: PropTypes.bool,
  minwidth: PropTypes.string,
  disableRestoreToDefault: PropTypes.bool,
  isSaving: PropTypes.bool,
  cancelEnable: PropTypes.bool,
};

SaveCancelButtons.defaultProps = {
  saveButtonLabel: "Save",
  cancelButtonLabel: "Cancel",
};

export default SaveCancelButtons;
