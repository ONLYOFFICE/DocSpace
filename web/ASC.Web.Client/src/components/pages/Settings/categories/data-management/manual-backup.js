import React, { useEffect } from "react";
import Text from "@appserver/components/text";
import { useTranslation } from "react-i18next";
import commonSettingsStyles from "../../utils/commonSettingsStyles";
import styled from "styled-components";
import Button from "@appserver/components/button";
import { getBackupStorage } from "@appserver/common/api/settings";

const StyledComponent = styled.div`
  ${commonSettingsStyles}
  .manual-backup_buttons {
    margin-top: 16px;
  }
`;
const ManualBackup = () => {
  const { t } = useTranslation("Settings");
  useEffect(() => {
    const res = getBackupStorage();
    console.log("res!!!!!!!", res);
  }, []);
  return (
    <StyledComponent>
      <div className="category-item-wrapper temporary-storage">
        <div className="category-item-heading">
          <Text className="inherit-title-link header">
            {t("TemporaryStorage")}
          </Text>
        </div>
        <Text className="category-item-description">
          {t("TemporaryStorageDescription")}
        </Text>
        <div className="manual-backup_buttons">
          <Button
            label={t("MakeCopy")}
            onClick={() => console.log("click")}
            primary
            isDisabled={false}
            size="medium"
            tabIndex={10}
          />
          <Button
            label={t("DownloadBackup")}
            onClick={() => console.log("click")}
            isDisabled={false}
            size="medium"
            style={{ marginLeft: "8px" }}
            tabIndex={11}
          />
        </div>
      </div>
    </StyledComponent>
  );
};

export default ManualBackup;
