import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import Submenu from "@docspace/components/submenu";

import PortalDeactivationSection from "./portalDeactivation";
import PortalDeletionSection from "./portalDeletion";
import DeleteDataLoader from "./DeleteDataLoader";

import { AppServerConfig } from "@docspace/common/constants";
import { combineUrl } from "@docspace/common/utils";
import config from "../../../../../package.json";

const DeleteData = (props) => {
  const { t, history } = props;

  const [currentTab, setCurrentTab] = useState(0);
  const [isLoading, setIsLoading] = useState(false);

  const data = [
    {
      id: "deletion",
      name: t("DeleteDocSpace"),
      content: <PortalDeletionSection />,
    },
    {
      id: "deactivation",
      name: t("PortalDeactivation"),
      content: <PortalDeactivationSection />,
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
        `/portal-settings/delete-data/${e.id}`
      )
    );
  };

  if (!isLoading) return <DeleteDataLoader />;
  return (
    <Submenu
      data={data}
      startSelect={currentTab}
      onSelect={(e) => onSelect(e)}
    />
  );
};

export default withTranslation("Settings")(withRouter(DeleteData));
