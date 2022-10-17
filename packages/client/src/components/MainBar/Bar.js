import React, { useEffect, useState } from "react";
import { inject, observer } from "mobx-react";
import difference from "lodash/difference";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";

import { ADS_TIMEOUT } from "@docspace/client/src/helpers/filesConstants";

import { getConvertedSize } from "@docspace/common/utils";

import { getBannerAttribute } from "@docspace/components/utils/banner";
import SnackBar from "@docspace/components/snackbar";

import QuotasBar from "./QuotasBar";
import ConfirmEmailBar from "./ConfirmEmailBar";

const CONFIRM_EMAIL = "confirm-email";
const ROOM_QUOTA = "room-quota";
const STORAGE_QUOTA = "storage-quota";

const Bar = (props) => {
  const {
    t,
    firstLoad,
    history,

    isAdmin,
    setMaintenanceExist,
    withActivationBar,
    sendActivationLink,

    onPaymentsClick,

    maxCountRoomsByQuota,
    usedRoomsCount,

    maxTotalSizeByQuota,
    usedTotalStorageSizeCount,

    showRoomQuotaBar,
    showStorageQuotaBar,
  } = props;

  const [barVisible, setBarVisible] = useState({
    roomQuota: true,
    storageQuota: true,
    confirmEmail: true,
  });

  const [htmlLink, setHtmlLink] = useState();
  const [campaigns, setCampaigns] = useState();

  const { loadLanguagePath } = getBannerAttribute();

  const updateBanner = async () => {
    const bar = (localStorage.getItem("bar") || "")
      .split(",")
      .filter((bar) => bar.length > 0);

    const closed = JSON.parse(localStorage.getItem("barClose"));

    const banner = difference(bar, closed);

    let index = Number(localStorage.getItem("barIndex") || 0);

    if (banner.length < 1 || index + 1 >= banner.length) {
      index = 0;
    } else {
      index++;
    }

    if (!isAdmin) {
      setBarVisible((value) => ({
        ...value,
        roomQuota: false,
        storageQuota: false,
      }));
    }

    if (closed.includes(ROOM_QUOTA) && barVisible.roomQuota) {
      setBarVisible((value) => ({ ...value, roomQuota: false }));
    }

    if (closed.includes(STORAGE_QUOTA) && barVisible.storageQuota) {
      setBarVisible((value) => ({ ...value, storageQuota: false }));
    }

    if (closed.includes(CONFIRM_EMAIL) && barVisible.confirmEmail) {
      setBarVisible((value) => ({ ...value, confirmEmail: false }));
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
    if (sendActivationLink) {
      sendActivationLink(t).finally(() => {
        return onCloseActivationBar();
      });
    } else {
      onCloseActivationBar();
    }
  };

  const onCloseActivationBar = () => {
    const closeItems = JSON.parse(localStorage.getItem("barClose")) || [];

    const closed =
      closeItems.length > 0 ? [...closeItems, CONFIRM_EMAIL] : [CONFIRM_EMAIL];

    localStorage.setItem("barClose", JSON.stringify(closed));

    setBarVisible((value) => ({ ...value, confirmEmail: false }));
    setMaintenanceExist(false);
  };

  const onClickQuota = (isRoomQuota) => {
    onPaymentsClick && onPaymentsClick();

    onCloseQuota(isRoomQuota);
  };

  const onCloseQuota = (isRoomQuota) => {
    const currentBar = isRoomQuota ? ROOM_QUOTA : STORAGE_QUOTA;
    const closeItems = JSON.parse(localStorage.getItem("barClose")) || [];

    const closed =
      closeItems.length > 0 ? [...closeItems, currentBar] : [currentBar];

    localStorage.setItem("barClose", JSON.stringify(closed));

    setBarVisible((value) =>
      isRoomQuota
        ? { ...value, roomQuota: false }
        : { ...value, storageQuota: false }
    );
    setMaintenanceExist(false);
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

  const isRoomQuota = showRoomQuotaBar && barVisible.roomQuota;
  const isStorageQuota = showStorageQuotaBar && barVisible.storageQuota;

  const quotasValue = {
    maxValue: isRoomQuota
      ? maxCountRoomsByQuota
      : getConvertedSize(t, maxTotalSizeByQuota),
    currentValue: isRoomQuota
      ? usedRoomsCount
      : getConvertedSize(t, usedTotalStorageSizeCount),
  };

  return isRoomQuota || isStorageQuota ? (
    <QuotasBar
      isRoomQuota={isRoomQuota}
      {...quotasValue}
      onClick={onClickQuota}
      onClose={onClickQuota}
      onLoad={onLoad}
    />
  ) : withActivationBar && barVisible.confirmEmail ? (
    <ConfirmEmailBar
      onLoad={onLoad}
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

export default inject(({ auth, profileActionsStore }) => {
  const { user, withActivationBar, sendActivationLink } = auth.userStore;

  const { onPaymentsClick } = profileActionsStore;

  const {
    maxCountRoomsByQuota,
    usedRoomsCount,

    maxTotalSizeByQuota,
    usedTotalStorageSizeCount,

    showRoomQuotaBar,
    showStorageQuotaBar,
  } = auth.currentQuotaStore;

  const { isAdmin } = user;

  return {
    isAdmin,
    withActivationBar,
    sendActivationLink,

    onPaymentsClick,

    maxCountRoomsByQuota,
    usedRoomsCount,

    maxTotalSizeByQuota,
    usedTotalStorageSizeCount,

    showRoomQuotaBar,
    showStorageQuotaBar,
  };
})(withTranslation(["Profile", "Common"])(withRouter(observer(Bar))));
