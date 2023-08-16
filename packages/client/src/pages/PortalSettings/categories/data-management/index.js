import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { withTranslation, Trans } from "react-i18next";
import { inject, observer } from "mobx-react";

import HelpReactSvgUrl from "PUBLIC_DIR/images/help.react.svg?url";

import Submenu from "@docspace/components/submenu";
import Link from "@docspace/components/link";
import Text from "@docspace/components/text";
import Box from "@docspace/components/box";
import HelpButton from "@docspace/components/help-button";
import { combineUrl } from "@docspace/common/utils";
import AppLoader from "@docspace/common/components/AppLoader";
import { removeLocalStorage } from "../../utils";
import config from "../../../../../package.json";
import ManualBackup from "./backup/manual-backup";
import AutoBackup from "./backup/auto-backup";

const DataManagementWrapper = (props) => {
  const {
    dataBackupUrl,
    automaticBackupUrl,
    buttonSize,
    t,
    history,
    isNotPaidPeriod,
    toDefault,
  } = props;

  const [currentTab, setCurrentTab] = useState(0);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    return () => {
      removeLocalStorage("LocalCopyStorageType");
      toDefault();
    };
  }, []);

  const renderTooltip = (helpInfo) => {
    const isAutoBackupPage = window.location.pathname.includes(
      "portal-settings/backup/auto-backup"
    );
    return (
      <>
        <HelpButton
          size={12}
          offsetRight={5}
          place="right"
          iconName={HelpReactSvgUrl}
          tooltipContent={
            <Text fontSize="12px">
              <Trans t={t} i18nKey={`${helpInfo}`} ns="Settings">
                {helpInfo}
              </Trans>
              <Box marginProp="10px 0 0">
                <Link
                  color="#333333"
                  fontSize="13px"
                  href={isAutoBackupPage ? automaticBackupUrl : dataBackupUrl}
                  target="_blank"
                  isBold
                  isHovered
                >
                  {t("Common:LearnMore")}
                </Link>
              </Box>
            </Text>
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

  useEffect(() => {
    const path = location.pathname;

    const currentTab = data.findIndex((item) => path.includes(item.id));
    if (currentTab !== -1) setCurrentTab(currentTab);

    setIsLoading(true);
  }, []);

  const onSelect = (e) => {
    history.push(
      combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        config.homepage,
        `/portal-settings/backup/${e.id}`
      )
    );
  };

  if (!isLoading) return <AppLoader />;

  return isNotPaidPeriod ? (
    <ManualBackup buttonSize={buttonSize} renderTooltip={renderTooltip} />
  ) : (
    <Submenu
      data={data}
      startSelect={currentTab}
      onSelect={(e) => onSelect(e)}
    />
  );
};

export default inject(({ auth, setup, backup }) => {
  const { initSettings } = setup;
  const { settingsStore, currentTariffStatusStore } = auth;
  const { isNotPaidPeriod } = currentTariffStatusStore;
  const { toDefault } = backup;
  const {
    dataBackupUrl,
    automaticBackupUrl,
    isTabletView,
    currentColorScheme,
  } = settingsStore;

  const buttonSize = isTabletView ? "normal" : "small";
  return {
    loadBaseInfo: async () => {
      await initSettings();
    },
    dataBackupUrl,
    automaticBackupUrl,
    buttonSize,
    isNotPaidPeriod,
    currentColorScheme,
    toDefault,
  };
})(
  withTranslation(["Settings", "Common"])(
    withRouter(observer(DataManagementWrapper))
  )
);
