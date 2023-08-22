import React, { useEffect, useRef } from "react";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";

const ButtonContainer = ({
  isLoadingData,
  buttonSize,
  onSaveModuleSettings,
  onCancelModuleSettings,
  isChanged,
  isThirdStorageChanged,
  t,
}) => {
  const prevChange = useRef();

  useEffect(() => {
    prevChange.current = isChanged;
  }, [isChanged]);

  return (
    (isThirdStorageChanged || isChanged) && (
      <div className="auto-backup_buttons">
        <Button
          label={t("Common:SaveButton")}
          onClick={onSaveModuleSettings}
          primary
          isDisabled={isLoadingData}
          size={buttonSize}
          className="save-button"
        />

        <Button
          className="cancel-button"
          label={t("Common:CancelButton")}
          isDisabled={isLoadingData}
          onClick={onCancelModuleSettings}
          size={buttonSize}
        />
      </div>
    )
  );
};

export default inject(({ backup }) => {
  const { isChanged, isThirdStorageChanged } = backup;

  return {
    isChanged,
    isThirdStorageChanged,
  };
})(observer(ButtonContainer));
