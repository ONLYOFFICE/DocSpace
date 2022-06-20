import React from "react";

import styled from "styled-components";

import ModalDialog from "@appserver/components/modal-dialog";
import TextInput from "@appserver/components/text-input";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";

const StyledSaveCancelButtons = styled(SaveCancelButtons)`
  position: relative !important;

  padding: 8px 0 0;

  .buttons-flex {
    width: 100%;

    display: grid;
    align-items: center;
    grid-template-columns: 1fr 1fr;
    grid-gap: 8px;
  }

  button {
    width: 100%;
  }
`;

const Dialog = ({ title, startValue, visible, onSave, onCancel, onClose }) => {
  const [value, setValue] = React.useState("");
  const [isDisabled, setIsDisabled] = React.useState(false);

  React.useEffect(() => {
    if (startValue) setValue(startValue);
  }, [startValue]);

  const onChange = React.useCallback((e) => {
    const newValue = e.target.value;

    setValue(newValue);
  }, []);

  const onFocus = React.useCallback((e) => {
    e.target.select();
  }, []);

  const onSaveAction = React.useCallback(
    (e) => {
      setIsDisabled(true);
      onSave && onSave(e, value);
    },
    [onSave, value]
  );

  const onCancelAction = React.useCallback((e) => {
    onCancel && onCancel(e);
  }, []);

  return (
    <ModalDialog
      visible={visible}
      displayType={"modal"}
      width={"400px"}
      scale={true}
      onClose={onClose}
    >
      <ModalDialog.Header>{title}</ModalDialog.Header>
      <ModalDialog.Body>
        <TextInput
          name={"create"}
          type={"text"}
          scale={true}
          value={value}
          isAutoFocussed={true}
          tabIndex={1}
          onChange={onChange}
          onFocus={onFocus}
          isDisabled={isDisabled}
        />
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <StyledSaveCancelButtons
          saveButtonLabel={"Save"}
          cancelButtonLabel={"Cancel"}
          onSaveClick={onSaveAction}
          onCancelClick={onCancelAction}
          showReminder={!isDisabled}
        />
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

export default React.memo(Dialog);
