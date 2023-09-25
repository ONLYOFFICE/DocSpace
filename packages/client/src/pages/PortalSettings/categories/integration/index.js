import React, { useEffect, useState } from "react";
import Submenu from "@docspace/components/submenu";
import { useNavigate } from "react-router-dom";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";

import SSO from "./SingleSignOn";
import ThirdParty from "./ThirdPartyServicesSettings";

import SMTPSettings from "./SMTPSettings";
import DocumentService from "./DocumentService";

const IntegrationWrapper = (props) => {
  const { t, tReady, enablePlugins, toDefault, isSSOAvailable, standalone } =
    props;
  const navigate = useNavigate();

  useEffect(() => {
    return () => {
      isSSOAvailable &&
        !window.location.pathname.includes("single-sign-on") &&
        toDefault();
    };
  }, []);

  const data = [
    {
      id: "third-party-services",
      name: t("Translations:ThirdPartyTitle"),
      content: <ThirdParty />,
    },
    {
      id: "single-sign-on",
      name: t("SingleSignOn"),
      content: <SSO />,
    },
    {
      id: "smtp-settings",
      name: t("SMTPSettings"),
      content: <SMTPSettings />,
    },
  ];

  if (standalone) {
    const documentServiceData = {
      id: "document-service",
      name: t("DocumentService"),
      content: <DocumentService />,
    };

    data.push(documentServiceData);
  }

  const getCurrentTab = () => {
    const path = location.pathname;
    const currentTab = data.findIndex((item) => path.includes(item.id));
    return currentTab !== -1 ? currentTab : 0;
  };

  const currentTab = getCurrentTab();

  const onSelect = (e) => {
    navigate(
      combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        config.homepage,
        `/portal-settings/integration/${e.id}`
      )
    );
  };

  return <Submenu data={data} startSelect={currentTab} onSelect={onSelect} />;
};

export default inject(({ auth, ssoStore }) => {
  const { standalone } = auth.settingsStore;
  const { load: toDefault } = ssoStore;
  const { enablePlugins } = auth.settingsStore;
  const { isSSOAvailable } = auth.currentQuotaStore;

  return {
    enablePlugins,
    toDefault,
    isSSOAvailable,
    standalone,
  };
})(
  withTranslation(["Settings", "SingleSignOn", "Translations"])(
    observer(IntegrationWrapper)
  )
);
