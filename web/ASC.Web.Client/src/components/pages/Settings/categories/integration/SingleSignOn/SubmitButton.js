import React from "react";
import { observer } from "mobx-react";

import Box from "@appserver/components/box";
import Button from "@appserver/components/button";

import ResetConfirmationModal from "./sub-components/ResetConfirmationModal";

const SubmitResetButtons = ({ FormStore, t }) => {
  return (
    <Box alignItems="center" displayProp="flex" flexDirection="row">
      <Button
        className="save-button"
        label={t("Common:SaveButton")}
        onClick={FormStore.onSubmit}
        primary
        size="medium"
        tabIndex={23}
      />
      <Button
        label={t("ResetSettings")}
        onClick={
          FormStore.isSsoEnabled
            ? FormStore.openResetModal
            : FormStore.resetForm
        }
        size="medium"
        tabIndex={24}
      />
      {FormStore.confirmationResetModal && (
        <ResetConfirmationModal FormStore={FormStore} t={t} />
      )}
    </Box>
  );
};

export default observer(SubmitResetButtons);
