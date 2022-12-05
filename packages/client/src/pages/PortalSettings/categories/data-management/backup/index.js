import React from "react";
import { withTranslation, Trans } from "react-i18next";
import Submenu from "@docspace/components/submenu";
import Link from "@docspace/components/link";
import HelpButton from "@docspace/components/help-button";
import { AppServerConfig } from "@docspace/common/constants";
import { combineUrl } from "@docspace/common/utils";
import { inject, observer } from "mobx-react";
import AutoBackup from "./auto-backup";
import ManualBackup from "./manual-backup";
import config from "PACKAGE_FILE";

const Backup = ({
  helpUrlCreatingBackup,
  buttonSize,
  t,
  history,
  isNotPaidPeriod,
  currentColorScheme,
}) => {
  const renderTooltip = (helpInfo) => {
    return (
      <>
        <HelpButton
          iconName={"/static/images/help.react.svg"}
          tooltipContent={
            <>
              <Trans t={t} i18nKey={`${helpInfo}`} ns="Settings">
                {helpInfo}
              </Trans>
              <div>
                <Link
                  as="a"
                  href={helpUrlCreatingBackup}
                  target="_blank"
                  color={currentColorScheme.main.accent}
                  isBold
                  isHovered
                >
                  {t("Common:LearnMore")}
                </Link>
              </div>
            </>
          }
        />
      </>
    );
  };

  const data = [
    {
      id: "data-backup",
      name: t("DataBackup"),
      content: (
        <ManualBackup buttonSize={buttonSize} renderTooltip={renderTooltip} />
      ),
    },
    {
      id: "auto-backup",
      name: t("AutoBackup"),
      content: (
        <AutoBackup buttonSize={buttonSize} renderTooltip={renderTooltip} />
      ),
    },
  ];

  const onSelect = (e) => {
    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/portal-settings/backup/${e.id}`
      )
    );
  };

  return isNotPaidPeriod ? (
    <ManualBackup buttonSize={buttonSize} renderTooltip={renderTooltip} />
  ) : (
    <Submenu data={data} startSelect={data[0]} onSelect={(e) => onSelect(e)} />
  );
};

export default inject(({ auth }) => {
  const { settingsStore, currentTariffStatusStore } = auth;
  const { isNotPaidPeriod } = currentTariffStatusStore;

  const {
    helpUrlCreatingBackup,
    isTabletView,
    currentColorScheme,
  } = settingsStore;

  const buttonSize = isTabletView ? "normal" : "small";

  return {
    helpUrlCreatingBackup,
    buttonSize,
    isNotPaidPeriod,
    currentColorScheme,
  };
})(observer(withTranslation(["Settings", "Common"])(Backup)));
