import React, { useEffect, useState } from "react";
import Submenu from "@docspace/components/submenu";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { AppServerConfig } from "@docspace/common/constants";
import { combineUrl } from "@docspace/common/utils";
import config from "../../../../../../package.json";

import Backup from "./backup";
import AppLoader from "@docspace/common/components/AppLoader";

const DataManagementWrapper = (props) => {
  const { t, history } = props;
  const [currentTab, setCurrentTab] = useState(0);
  const [isLoading, setIsLoading] = useState(false);

  const data = [
    {
      id: "backup",
      name: t("Backup"),
      content: <Backup />,
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
        AppServerConfig.proxyURL,
        config.homepage,
        `/settings/datamanagement/${e.id}`
      )
    );
  };

  if (!isLoading) return <AppLoader />;
  return (
    <Submenu
      data={data}
      startSelect={currentTab}
      onSelect={(e) => onSelect(e)}
    />
  );
};

export default inject(({ setup }) => {
  const { initSettings } = setup;

  return {
    loadBaseInfo: async () => {
      await initSettings();
    },
  };
})(withTranslation("Settings")(withRouter(observer(DataManagementWrapper))));
