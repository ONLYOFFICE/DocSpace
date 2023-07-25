import React from "react";
import { Trans, useTranslation } from "react-i18next";
import { observer, inject } from "mobx-react";

import Text from "@docspace/components/text";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

import StyledComponent from "../StyledComponent";

const OfficialDocumentation = ({ dataBackupUrl }) => {
  const { t } = useTranslation("PaymentsEnterprise");

  const dockerLink =
    "https://helpcenter.onlyoffice.com/installation/docspace-enterprise-install-docker.aspx";
  const linuxDocker =
    "https://helpcenter.onlyoffice.com/installation/docspace-enterprise-install-linux.aspx";
  const windowsDocker =
    "https://helpcenter.onlyoffice.com/installation/docspace-enterprise-install-windows.aspx";

  return (
    <StyledComponent>
      <div className="official-documentation">
        {"—"}
        <Text fontWeight={600}>
          {t("UpgradeToProBannerInstructionItemDocker")}{" "}
          <ColorTheme
            tag="a"
            themeId={ThemeType.Link}
            fontSize="13px"
            fontWeight="600"
            href={dockerLink}
            target="_blank"
          >
            {t("UpgradeToProBannerInstructionReadNow")}
          </ColorTheme>
        </Text>

        {"—"}
        <Text fontWeight={600}>
          {t("UpgradeToProBannerInstructionItemLinux")}{" "}
          <ColorTheme
            tag="a"
            themeId={ThemeType.Link}
            fontSize="13px"
            fontWeight="600"
            href={linuxDocker}
            target="_blank"
          >
            {t("UpgradeToProBannerInstructionReadNow")}
          </ColorTheme>
        </Text>

        {"—"}
        <Text fontWeight={600}>
          {t("UpgradeToProBannerInstructionItemWindows")}{" "}
          <ColorTheme
            tag="a"
            themeId={ThemeType.Link}
            fontSize="13px"
            fontWeight="600"
            href={windowsDocker}
            target="_blank"
          >
            {t("UpgradeToProBannerInstructionReadNow")}
          </ColorTheme>
        </Text>
      </div>

      <Text className="upgrade-info">
        <Trans
          i18nKey="UpgradeToProBannerInstructionNote"
          ns="PaymentsEnterprise"
          t={t}
        >
          Please note that the editors will be unavailable during the upgrade.
          We also recommend to
          <ColorTheme
            tag="a"
            themeId={ThemeType.Link}
            fontWeight="600"
            href={dataBackupUrl}
            target="_blank"
          >
            backup your data
          </ColorTheme>
          before you start.
        </Trans>
      </Text>
    </StyledComponent>
  );
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const { dataBackupUrl } = settingsStore;

  return { dataBackupUrl };
})(observer(OfficialDocumentation));
