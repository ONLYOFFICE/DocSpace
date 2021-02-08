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
      showReminder,
      reminderTest,
      saveButtonLabel,
      cancelButtonLabel,
      className,
      id,
    } = this.props;
    return (
      <StyledSaveCancelButtons className={className} id={id}>
        <div>
          <Button
            className="save-button"
            size="big"
            isDisabled={false}
            primary
            onClick={onSaveClick}
            label={saveButtonLabel}
          />
          <Button
            size="big"
            isDisabled={false}
            onClick={onCancelClick}
            label={cancelButtonLabel}
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
  id: PropTypes.string,
  className: PropTypes.string,
  reminderTest: PropTypes.string,
  saveButtonLabel: PropTypes.string,
  cancelButtonLabel: PropTypes.string,
  onSaveClick: PropTypes.func,
  onCancelClick: PropTypes.func,
  showReminder: PropTypes.bool,
};

SaveCancelButtons.defaultProps = {
  saveButtonLabel: "Save",
  cancelButtonLabel: "Cancel",
};

export default SaveCancelButtons;
