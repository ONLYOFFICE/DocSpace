import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import Link from "@docspace/components/link";

import {
  ResetApplicationDialog,
  BackupCodesDialog,
} from "SRC_DIR/components/dialogs";

import { StyledWrapper } from "./styled-login-settings";

const LoginSettings = (props) => {
  const { t } = useTranslation(["Profile", "Common"]);

  const {
    profile,
    resetTfaApp,
    getNewBackupCodes,
    backupCodes,
    backupCodesCount,
    setBackupCodes,
  } = props;
  const [resetAppDialogVisible, setResetAppDialogVisible] = useState(false);
  const [backupCodesDialogVisible, setBackupCodesDialogVisible] = useState(
    false
  );

  return (
    <StyledWrapper>
      <div className="header">
        <Text fontSize="16px" fontWeight={700}>
          {t("TfaLoginSettings")}
        </Text>
        <Text color="#A3A9AE">{t("TwoFactorDescription")}</Text>
      </div>
      <div className="actions">
        <Button
          className="button"
          label={t("ShowBackupCodes")}
          onClick={() => setBackupCodesDialogVisible(true)}
          size="small"
        />
        <Link
          fontWeight="600"
          isHovered
          type="action"
          onClick={() => setResetAppDialogVisible(true)}
        >
          {t("Common:ResetApplication")}
        </Link>
      </div>

      {resetAppDialogVisible && (
        <ResetApplicationDialog
          visible={resetAppDialogVisible}
          onClose={() => setResetAppDialogVisible(false)}
          resetTfaApp={resetTfaApp}
          id={profile.id}
        />
      )}
      {backupCodesDialogVisible && (
        <BackupCodesDialog
          visible={backupCodesDialogVisible}
          onClose={() => setBackupCodesDialogVisible(false)}
          getNewBackupCodes={getNewBackupCodes}
          backupCodes={backupCodes}
          backupCodesCount={backupCodesCount}
          setBackupCodes={setBackupCodes}
        />
      )}
    </StyledWrapper>
  );
};

export default inject(({ auth, peopleStore }) => {
  const { tfaStore } = auth;
  const { targetUserStore } = peopleStore;

  const { targetUser: profile } = targetUserStore;

  const {
    getNewBackupCodes,
    unlinkApp: resetTfaApp,
    backupCodes,
    setBackupCodes,
  } = tfaStore;

  return {
    profile,
    resetTfaApp,
    getNewBackupCodes,
    backupCodes,
    setBackupCodes,
  };
})(observer(LoginSettings));
