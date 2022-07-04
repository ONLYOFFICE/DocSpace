import React, { useState, useEffect } from "react";
import Submenu from "@appserver/components/submenu";
import { withTranslation, Trans } from "react-i18next";
import { enableAutoBackup } from "@appserver/common/api/portal";
import Link from "@appserver/components/link";
import { inject, observer } from "mobx-react";

import AutoBackup from "./auto-backup";
import ManualBackup from "./manual-backup";
import toastr from "@appserver/components/toast/toastr";
import { getBackupStorage } from "@appserver/common/api/settings";
import HelpButton from "@appserver/components/help-button";

const Backup = ({
  setThirdPartyStorage,
  helpUrlCreatingBackup,
  getProgress,
  buttonSize,
  t,
}) => {
  const [isInitialLoading, setIsInitialLoading] = useState(true);
  const [enableAuto, setEnableAuto] = useState(false);

  useEffect(() => {
    setBasicSettings();
  }, []);
  const setBasicSettings = async () => {
    const requests = [enableAutoBackup(), getBackupStorage()];

    try {
      getProgress();

      const [enableAuto, backupStorage] = await Promise.all(requests);

      setThirdPartyStorage(backupStorage);

      setIsInitialLoading(false);
      setEnableAuto(enableAuto);
    } catch (error) {
      toastr.error(error);
      setIsInitialLoading(false);
    }
  };

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
                  color="#555F65"
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

  const onSelect = () => {};

  return isInitialLoading ? (
    <></>
  ) : (
    <Submenu data={data} startSelect={data[0]} onSelect={(e) => onSelect(e)} />
  );
};

export default inject(({ auth, backup }) => {
  const { language, settingsStore } = auth;
  const {
    helpUrlCreatingBackup,
    organizationName,
    isTabletView,
  } = settingsStore;
  const {
    setThirdPartyStorage,
    setBackupSchedule,
    setCommonThirdPartyList,
    getProgress,
    clearProgressInterval,
    downloadingProgress,
  } = backup;

  const buttonSize = isTabletView ? "normal" : "small";
  return {
    helpUrlCreatingBackup,
    language,
    setThirdPartyStorage,
    setBackupSchedule,
    setCommonThirdPartyList,
    getProgress,
    clearProgressInterval,
    downloadingProgress,
    organizationName,
    buttonSize,
  };
})(observer(withTranslation(["Settings", "Common"])(Backup)));
