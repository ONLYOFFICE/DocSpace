import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import SaveCancelButtons from "@docspace/components/save-cancel-buttons";

import ResetConfirmationModal from "./sub-components/ResetConfirmationModal";

const StyledWrapper = styled.div`
  display: flex;
  flex-direction: row;
  align-items: center;

  .save-button {
    margin-right: 8px;
  }
`;

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
    <StyledWrapper>
      <SaveCancelButtons
        className="save-cancel-buttons"
        onSaveClick={() => saveSsoSettings(t)}
        onCancelClick={isSsoEnabled ? openResetModal : resetForm}
        showReminder={true}
        saveButtonLabel={t("Common:SaveButton")}
        cancelButtonLabel={t("Settings:RestoreDefaultButton")}
        displaySettings={true}
        hasScroll={false}
        isSaving={isSubmitLoading}
        saveButtonDisabled={hasErrors || !hasChanges || isLoadingXml}
        cancelEnable={!(isSubmitLoading || isLoadingXml)}
        additionalClassSaveButton="save-button"
        additionalClassCancelButton="restore-button"
      />
      {confirmationResetModal && <ResetConfirmationModal />}
    </StyledWrapper>
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
