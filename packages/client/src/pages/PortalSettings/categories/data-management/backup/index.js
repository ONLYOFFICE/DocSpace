import HelpReactSvgUrl from "PUBLIC_DIR/images/help.react.svg?url";
import React, { useEffect } from "react";
import { withTranslation, Trans } from "react-i18next";
import Submenu from "@docspace/components/submenu";
import Link from "@docspace/components/link";
import HelpButton from "@docspace/components/help-button";
import { combineUrl } from "@docspace/common/utils";
import { inject, observer } from "mobx-react";
import AutoBackup from "./auto-backup";
import ManualBackup from "./manual-backup";
import config from "PACKAGE_FILE";

const Backup = ({
  automaticBackupUrl,
  buttonSize,
  t,
  history,
  isNotPaidPeriod,
}) => {
  const renderTooltip = (helpInfo) => {
    return (
      <>
        <HelpButton
          place="bottom"
          iconName={HelpReactSvgUrl}
          tooltipContent={
            <>
              <Trans t={t} i18nKey={`${helpInfo}`} ns="Settings">
                {helpInfo}
              </Trans>
              <div>
                <Link
                  href={automaticBackupUrl}
                  target="_blank"
                  fontSize="13px"
                  color="#333333"
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
        window.DocSpaceConfig?.proxy?.url,
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
    automaticBackupUrl,
    isTabletView,
    currentColorScheme,
  } = settingsStore;

  const buttonSize = isTabletView ? "normal" : "small";

  return {
    automaticBackupUrl,
    buttonSize,
    isNotPaidPeriod,
    currentColorScheme,
    toDefault,
  };
})(observer(withTranslation(["Settings", "Common"])(Backup)));
