import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Button from "@docspace/components/button";

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
