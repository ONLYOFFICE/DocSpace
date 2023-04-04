import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router-dom";
import Submenu from "@docspace/components/submenu";
import { inject, observer } from "mobx-react";
import PortalDeactivationSection from "./portalDeactivation";
import PortalDeletionSection from "./portalDeletion";
import DeleteDataLoader from "./DeleteDataLoader";
import { combineUrl } from "@docspace/common/utils";
import config from "../../../../../package.json";

const DeleteData = (props) => {
  const { t, history, isNotPaidPeriod } = props;

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
        window.DocSpaceConfig?.proxy?.url,
        config.homepage,
        `/portal-settings/delete-data/${e.id}`
      )
    );
  };

  if (!isLoading) return <DeleteDataLoader />;
  return isNotPaidPeriod ? (
    <PortalDeletionSection />
  ) : (
    <Submenu
      data={data}
      startSelect={currentTab}
      onSelect={(e) => onSelect(e)}
    />
  );
};

export default inject(({ auth }) => {
  const { currentTariffStatusStore } = auth;
  const { isNotPaidPeriod } = currentTariffStatusStore;

  return {
    isNotPaidPeriod,
  };
})(observer(withTranslation("Settings")(withRouter(DeleteData))));
