import React from "react";
import { useTranslation } from "react-i18next";

import Text from "@docspace/components/text";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

import { StyledComponent } from "../StyledComponent";

const OfficialDocumentation = () => {
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
    </StyledComponent>
  );
};

export default OfficialDocumentation;
