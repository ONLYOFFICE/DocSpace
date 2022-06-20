import React from "react";
import { inject, observer } from "mobx-react";
import styled from "styled-components";

import toastr from "@appserver/components/toast/toastr";
import ModalDialog from "@appserver/components/modal-dialog";
import TextInput from "@appserver/components/text-input";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";

const StyledModalDialog = styled(ModalDialog)`
  width: 400px;

  @media (max-width: 400px) {
    width: 100%;
  }
`;

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

const Dialog = ({
  t,
  title,
  startValue,
  visible,
  folderFormValidation,
  onSave,
  onCancel,
  onClose,
}) => {
  const [value, setValue] = React.useState("");
  const [isDisabled, setIsDisabled] = React.useState(false);

  React.useEffect(() => {
    if (startValue) setValue(startValue);
  }, [startValue]);

  const onChange = React.useCallback((e) => {
    let newValue = e.target.value;

    if (newValue.match(folderFormValidation)) {
      toastr.warning(t("ContainsSpecCharacter"));
    }

    newValue = newValue.replace(folderFormValidation, "_");

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
    <StyledModalDialog
      visible={visible}
      displayType={"modal"}
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
    </StyledModalDialog>
  );
};

export default inject(({ auth }) => {
  const { folderFormValidation } = auth.settingsStore;

  return { folderFormValidation };
})(observer(Dialog));
