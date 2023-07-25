import React, { useEffect, useCallback, useState } from "react";
import { inject, observer } from "mobx-react";

import toastr from "@docspace/components/toast/toastr";
import ModalDialog from "@docspace/components/modal-dialog";
import TextInput from "@docspace/components/text-input";
import Button from "@docspace/components/button";
import ComboBox from "@docspace/components/combobox";
import Checkbox from "@docspace/components/checkbox";
import Box from "@docspace/components/box";

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
  isCreateDialog,
  extension,
  keepNewFileName,
  setKeepNewFileName,
}) => {
  const [value, setValue] = useState("");
  const [isDisabled, setIsDisabled] = useState(false);
  const [isChecked, setIsChecked] = useState(false);
  const [isChanged, setIsChanged] = useState(false);

  useEffect(() => {
    keepNewFileName && isCreateDialog && setIsChecked(keepNewFileName);
  }, [isCreateDialog, keepNewFileName]);

  useEffect(() => {
    let input = document?.getElementById("create-text-input");
    if (input && value === startValue && !isChanged) input.select();
  }, [visible, value]);

  useEffect(() => {
    if (startValue) setValue(startValue);
  }, [startValue]);

  const onKeyUpHandler = useCallback(
    (e) => {
      if (e.keyCode === 27) onCancelAction(e);
      if (e.keyCode === 13) onSaveAction(e);
    },
    [value]
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
      toastr.warning(t("Files:ContainsSpecCharacter"));
    }

    newValue = newValue.replace(folderFormValidation, "_");

    setValue(newValue);
    setIsChanged(true);
  }, []);

  const onFocus = useCallback((e) => {
    e.target.select();
  }, []);

  const onSaveAction = useCallback(
    (e) => {
      setIsDisabled(true);
      isCreateDialog && setKeepNewFileName(isChecked);
      onSave && onSave(e, value);
    },
    [onSave, isCreateDialog, value, isChecked]
  );

  const onCancelAction = useCallback((e) => {
    if (isChecked) {
      setKeepNewFileName(false);
    }
    onCancel && onCancel(e);
  }, []);

  const onCloseAction = useCallback(
    (e) => {
      if (!isDisabled && isChecked) {
        setKeepNewFileName(false);
      }
      onClose && onClose(e);
    },
    [isDisabled]
  );

  const onChangeCheckbox = () => {
    isCreateDialog && setIsChecked((val) => !val);
  };

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
          id="create-text-input"
          name="create"
          type="text"
          scale={true}
          value={value}
          isAutoFocussed={true}
          tabIndex={1}
          onChange={onChange}
          onFocus={onFocus}
          isDisabled={isDisabled}
          maxLength={165}
        />
        {isCreateDialog && extension && (
          <Box displayProp="flex" alignItems="center" paddingProp="16px 0 0">
            <Checkbox
              className="dont-ask-again"
              label={t("Common:DontAskAgain")}
              isChecked={isChecked}
              onChange={onChangeCheckbox}
            />
          </Box>
        )}

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
          className="submit"
          key="GlobalSendBtn"
          label={isCreateDialog ? t("Common:Create") : t("Common:SaveButton")}
          size="normal"
          scale
          primary
          isLoading={isDisabled}
          isDisabled={isDisabled}
          onClick={onSaveAction}
        />
        <Button
          className="cancel-button"
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

export default inject(({ auth, settingsStore }) => {
  const { folderFormValidation } = auth.settingsStore;
  const { keepNewFileName, setKeepNewFileName } = settingsStore;

  return { folderFormValidation, keepNewFileName, setKeepNewFileName };
})(observer(Dialog));
