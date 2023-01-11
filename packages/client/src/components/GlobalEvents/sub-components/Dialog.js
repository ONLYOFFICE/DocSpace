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
  onCreate,
  isChecked,
  setIsChecked,
}) => {
  const [value, setValue] = useState("");
  const [isDisabled, setIsDisabled] = useState(false);

  useEffect(() => {
    let input = document?.getElementById("create-text-input");
    if (input && value === startValue) input.select();
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
      toastr.warning(t("ContainsSpecCharacter"));
    }

    newValue = newValue.replace(folderFormValidation, "_");

    setValue(newValue);
  }, []);

  const onFocus = useCallback((e) => {
    e.target.select();
  }, []);

  const onSaveAction = useCallback(
    (e) => {
      setIsDisabled(true);
      onSave && onSave(e, value);
    },
    [onSave, value]
  );

  const onCancelAction = useCallback((e) => {
    onCancel && onCancel(e);
    setIsChecked(false);
  }, []);

  const onCloseAction = useCallback(
    (e) => {
      if (!isDisabled) {
        onClose && onClose(e);
        setIsChecked(false);
      }
    },
    [isDisabled]
  );

  const onChangeCheckbox = () => {
    setIsChecked(!isChecked);
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
        />
        {onCreate && (
          <Box displayProp="flex" alignItems="center" paddingProp="16px 0 8px">
            <Checkbox isChecked={isChecked} onChange={onChangeCheckbox} />
            {t("Common:DontAskAgain")}
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
          key="GlobalSendBtn"
          label={onCreate ? t("Common:Create") : t("Common:Save")}
          size="normal"
          scale
          primary
          isLoading={isDisabled}
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

export default inject(({ auth, filesStore }) => {
  const { folderFormValidation } = auth.settingsStore;
  const { isChecked, setIsChecked } = filesStore;

  return { folderFormValidation, isChecked, setIsChecked };
})(observer(Dialog));
