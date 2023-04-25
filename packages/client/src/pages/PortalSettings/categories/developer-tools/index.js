import React, { useEffect, useState } from "react";
import Submenu from "@docspace/components/submenu";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";

import JavascriptSDK from "./JavascriptSDK";
import Webhooks from "./Webhooks";

import AppLoader from "@docspace/common/components/AppLoader";
import SSOLoader from "./sub-components/ssoLoader";

const DeveloperToolsWrapper = (props) => {
  const { t, tReady, history, loadBaseInfo, setDocumentTitle } = props;
  const [currentTab, setCurrentTab] = useState(0);
  const [isLoading, setIsLoading] = useState(false);

  setDocumentTitle("Developer Tools");

  const data = [
    {
      id: "javascript-sdk",
      name: t("Javascript sdk"),
      content: <JavascriptSDK />,
    },
    {
      id: "webhooks",
      name: t("Webhooks"),
      content: <Webhooks />,
    },
  ];

  const load = async () => {
    const path = location.pathname;
    const currentTab = data.findIndex((item) => path.includes(item.id));
    if (currentTab !== -1) setCurrentTab(currentTab);

    await loadBaseInfo();
    setIsLoading(true);
  };

  useEffect(() => {
    load();
  }, []);

  const onSelect = (e) => {
    history.push(
      combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        config.homepage,
        `/portal-settings/developer/tools/${e.id}`,
      ),
    );
  };

  if (!isLoading && !tReady) return currentTab === 0 ? <SSOLoader /> : <AppLoader />;

  return <Submenu data={data} startSelect={currentTab} onSelect={onSelect} />;
};

export default inject(({ setup, auth }) => {
  const { initSettings } = setup;
  const { setDocumentTitle } = auth;

  return {
    loadBaseInfo: async () => {
      await initSettings();
    },
    setDocumentTitle,
  };
})(
  withTranslation(["Settings", "SingleSignOn", "Translations"])(
    withRouter(observer(DeveloperToolsWrapper)),
  ),
);
