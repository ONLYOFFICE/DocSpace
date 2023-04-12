import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import Submenu from "@docspace/components/submenu";
import { inject, observer } from "mobx-react";
import PortalDeactivationSection from "./portalDeactivation";
import PortalDeletionSection from "./portalDeletion";
import DeleteDataLoader from "./DeleteDataLoader";
import { combineUrl } from "@docspace/common/utils";
import config from "../../../../../package.json";

const DeleteData = (props) => {
  const { t, isNotPaidPeriod, tReady } = props;

  const navigate = useNavigate();

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
    navigate(
      combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        config.homepage,
        `/portal-settings/delete-data/${e.id}`
      )
    );
  };

  if (!isLoading || !tReady) return <DeleteDataLoader />;
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
})(observer(withTranslation("Settings")(DeleteData)));
