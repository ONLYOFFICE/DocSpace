import React, { useEffect, useCallback } from "react";
import { inject, observer } from "mobx-react";

import toastr from "@docspace/components/toast/toastr";
import ModalDialog from "@docspace/components/modal-dialog";
import TextInput from "@docspace/components/text-input";
import Button from "@docspace/components/button";
import ComboBox from "@docspace/components/combobox";
import { isSafari, isTablet } from "react-device-detect";

const Dialog = ({
  t,
  title,
  startValue,
  visible,
  folderFormValidation,
  options,
  selectedOption,
  onSelect,
  onSave,
  onCancel,
  onClose,
}) => {
  const [value, setValue] = React.useState("");
  const [isDisabled, setIsDisabled] = React.useState(false);

  useEffect(() => {
    if (startValue) setValue(startValue);
  }, [startValue]);

  const onKeyUpHandler = useCallback(
    (e) => {
      if (e.keyCode === 27) onCancelAction(e);
      if (e.keyCode === 13) onSaveAction(e);
    },
    [onSaveAction, onCancelAction, value]
  );
  useEffect(() => {
    document.addEventListener("keyup", onKeyUpHandler, false);

    return () => {
      document.removeEventListener("keyup", onKeyUpHandler, false);
    };
  }, [onKeyUpHandler]);

  const onChange = useCallback((e) => {
    let newValue = e.target.value;

    if (newValue.match(folderFormValidation)) {
      toastr.warning(t("ContainsSpecCharacter"));
    }

    newValue = newValue.replace(folderFormValidation, "_");

    setValue(newValue);
  }, []);

  const onFocus = useCallback((e) => {
    e.target.select();
  }, []);

  const returnWindowPositionAfterKeyboard = () => {
    isSafari && isTablet && window.scrollTo(0, 0);
  };

  const onSaveAction = useCallback(
    (e) => {
      returnWindowPositionAfterKeyboard();
      setIsDisabled(true);
      onSave && onSave(e, value);
    },
    [onSave, value]
  );

  const onCancelAction = useCallback((e) => {
    returnWindowPositionAfterKeyboard();
    onCancel && onCancel(e);
  }, []);

  const onCloseAction = useCallback((e) => {
    returnWindowPositionAfterKeyboard();
    onClose && onClose(e);
  }, []);

  return (
    <ModalDialog
      visible={visible}
      displayType={"modal"}
      scale={true}
      onClose={onCloseAction}
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
        {options && (
          <ComboBox
            style={{ marginTop: "16px" }}
            options={options}
            selectedOption={selectedOption}
            onSelect={onSelect}
          />
        )}
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="GlobalSendBtn"
          label={t("Common:SaveButton")}
          size="normal"
          scale
          primary
          isDisabled={isDisabled}
          onClick={onSaveAction}
        />
        <Button
          key="CloseBtn"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          isDisabled={isDisabled}
          onClick={onCancelAction}
        />
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

export default inject(({ auth }) => {
  const { folderFormValidation } = auth.settingsStore;

  return { folderFormValidation };
})(observer(Dialog));
