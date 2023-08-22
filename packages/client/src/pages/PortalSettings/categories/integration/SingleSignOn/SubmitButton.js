import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@docspace/components/box";
import Button from "@docspace/components/button";

import ResetConfirmationModal from "./sub-components/ResetConfirmationModal";

const SubmitResetButtons = (props) => {
  const { t } = useTranslation(["SingleSignOn", "Settings", "Common"]);
  const {
    saveSsoSettings,
    isSsoEnabled,
    openResetModal,
    resetForm,
    confirmationResetModal,
    isSubmitLoading,
    hasErrors,
    hasChanges,
    isLoadingXml,
  } = props;

  return (
    <Box alignItems="center" displayProp="flex" flexDirection="row">
      <Button
        className="save-button"
        label={t("Common:SaveButton")}
        onClick={() => saveSsoSettings(t)}
        primary
        size="small"
        tabIndex={23}
        isDisabled={hasErrors || !hasChanges || isLoadingXml}
        isLoading={isSubmitLoading}
      />
      <Button
        className="restore-button"
        label={t("Settings:RestoreDefaultButton")}
        onClick={isSsoEnabled ? openResetModal : resetForm}
        size="small"
        tabIndex={24}
        isDisabled={isSubmitLoading || isLoadingXml}
      />
      {confirmationResetModal && <ResetConfirmationModal />}
    </Box>
  );
};

export default inject(({ ssoStore }) => {
  const {
    saveSsoSettings,
    isSsoEnabled,
    openResetModal,
    resetForm,
    confirmationResetModal,
    isSubmitLoading,
    hasErrors,
    hasChanges,
    isLoadingXml,
  } = ssoStore;

  return {
    saveSsoSettings,
    isSsoEnabled,
    openResetModal,
    resetForm,
    confirmationResetModal,
    isSubmitLoading,
    hasErrors,
    hasChanges,
    isLoadingXml,
  };
})(observer(SubmitResetButtons));
