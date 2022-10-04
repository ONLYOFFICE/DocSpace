import React, { useEffect, useState } from "react";
import { inject, observer } from "mobx-react";
import difference from "lodash/difference";
import { withTranslation } from "react-i18next";

import { ADS_TIMEOUT } from "@docspace/client/src/helpers/filesConstants";

import { getBannerAttribute } from "@docspace/components/utils/banner";

import SnackBar from "@docspace/components/snackbar";
import { EmployeeActivationStatus } from "@docspace/common/constants";
import QuotasBar from "./QuotasBar";

const Bar = (props) => {
  const {
    t,
    firstLoad,
    isAdmin,
    setMaintenanceExist,
    withActivationBar,
    sendActivationLink,
    setHideActivationBar,
  } = props;

  const [htmlLink, setHtmlLink] = useState();
  const [campaigns, setCampaigns] = useState();

  const { loadLanguagePath } = getBannerAttribute();

  const bar = (localStorage.getItem("bar") || "")
    .split(",")
    .filter((bar) => bar.length > 0);

  const updateBanner = async () => {
    const closed = JSON.parse(localStorage.getItem("barClose"));
    const banner = difference(bar, closed);
    let index = Number(localStorage.getItem("barIndex") || 0);

    if (banner.length < 1 || index + 1 >= banner.length) {
      index = 0;
    } else {
      index++;
    }

    try {
      const [htmlUrl, campaigns] = await loadLanguagePath();
      setHtmlLink(htmlUrl);
      setCampaigns(campaigns);
    } catch (e) {
      updateBanner();
    }

    localStorage.setItem("barIndex", index);
    return;
  };

  useEffect(() => {
    const updateTimeout = setTimeout(() => updateBanner(), 1000);
    const updateInterval = setInterval(updateBanner, ADS_TIMEOUT);
    return () => {
      clearTimeout(updateTimeout);
      clearInterval(updateInterval);
    };
  }, []);

  const sendActivationLinkAction = () => {
    sendActivationLink && sendActivationLink(t);
  };

  const onCloseActivationBar = () => {
    setHideActivationBar && setHideActivationBar(true);
  };

  const onClose = () => {
    setMaintenanceExist(false);
    const closeItems = JSON.parse(localStorage.getItem("barClose")) || [];
    const closed =
      closeItems.length > 0 ? [...closeItems, campaigns] : [campaigns];
    localStorage.setItem("barClose", JSON.stringify(closed));
    setHtmlLink(null);
  };

  const onLoad = () => {
    setMaintenanceExist(true);
  };

  return isAdmin && false ? (
    <></>
  ) : withActivationBar ? (
    <QuotasBar
      type={"confirm-email"}
      onClick={sendActivationLinkAction}
      onClose={onCloseActivationBar}
    />
  ) : htmlLink && !firstLoad ? (
    <SnackBar
      onLoad={onLoad}
      clickAction={onClose}
      isCampaigns={true}
      htmlContent={htmlLink}
    />
  ) : null;
};

export default inject(({ auth }) => {
  const {
    user,
    withActivationBar,
    sendActivationLink,
    setHideActivationBar,
  } = auth.userStore;

  const { isAdmin } = user;

  return {
    isAdmin,
    withActivationBar,
    sendActivationLink,
    setHideActivationBar,
  };
})(withTranslation(["Profile"])(observer(Bar)));
