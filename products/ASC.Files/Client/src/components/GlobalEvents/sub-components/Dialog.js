import React from "react";
import { inject, observer } from "mobx-react";

import toastr from "@appserver/components/toast/toastr";
import ModalDialog from "@appserver/components/modal-dialog";
import TextInput from "@appserver/components/text-input";
import Button from "@appserver/components/button";

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
    <ModalDialog
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
        <Button
          key="SendBtn"
          label={t("Common:SaveButton")}
          size="normal"
          scale
          primary
          onClick={onSaveAction}
        />
        <Button
          key="CloseBtn"
          label={t("Common:CancelButton")}
          size="normal"
          scale
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
