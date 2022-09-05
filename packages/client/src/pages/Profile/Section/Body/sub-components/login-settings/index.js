import React, { useState } from "react";
import styled from "styled-components";
import { useTranslation } from "react-i18next";

import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import Link from "@docspace/components/link";

import { hugeMobile } from "@docspace/components/utils/device";

import {
  ResetApplicationDialog,
  BackupCodesDialog,
} from "../../../../../../components/dialogs";

const StyledWrapper = styled.div`
  display: flex;
  flex-direction: column;
  gap: 16px;

  .header {
    display: flex;
    flex-direction: column;
    gap: 4px;
  }

  .actions {
    display: flex;
    gap: 16px;
    align-items: center;

    @media ${hugeMobile} {
      flex-direction: column;
      gap: 12px;
      align-items: flex-start;

      .button {
        width: 100%;
        height: 40px;
        font-size: 14px;
      }
    }
  }
`;

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

export default LoginSettings;
