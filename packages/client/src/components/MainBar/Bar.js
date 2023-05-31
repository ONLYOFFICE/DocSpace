import React, { useEffect, useState } from "react";
import { inject, observer } from "mobx-react";
import difference from "lodash/difference";
import { withTranslation } from "react-i18next";

import { ADS_TIMEOUT } from "@docspace/client/src/helpers/filesConstants";

import { getConvertedSize } from "@docspace/common/utils";

import { getBannerAttribute } from "@docspace/components/utils/banner";
import SnackBar from "@docspace/components/snackbar";
import { QuotaBarTypes } from "SRC_DIR/helpers/constants";

import QuotasBar from "./QuotasBar";
import ConfirmEmailBar from "./ConfirmEmailBar";
import { showEmailActivationToast } from "SRC_DIR/helpers/people-helpers";

const Bar = (props) => {
  const {
    t,
    tReady,
    firstLoad,

    isAdmin,
    userEmail,
    setMaintenanceExist,
    withActivationBar,
    sendActivationLink,

    onPaymentsClick,

    maxCountRoomsByQuota,
    usedRoomsCount,

    maxTotalSizeByQuota,
    usedTotalStorageSizeCount,

    maxCountManagersByQuota,
    addedManagersCount,

    showRoomQuotaBar,
    showStorageQuotaBar,
    showUserQuotaBar,

    currentColorScheme,

    setMainBarVisible,
  } = props;

  const [barVisible, setBarVisible] = useState({
    roomQuota: false,
    storageQuota: false,
    userQuota: false,
    storageAndUserQuota: false,
    storageAndRoomQuota: false,
    confirmEmail: false,
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

    if (closed) {
      if (isAdmin) {
        setBarVisible((value) => ({
          ...value,
          roomQuota: !closed.includes(QuotaBarTypes.RoomQuota),
          storageQuota: !closed.includes(QuotaBarTypes.StorageQuota),
          userQuota: !closed.includes(QuotaBarTypes.UserQuota),
          storageAndRoomQuota: !closed.includes(
            QuotaBarTypes.UserAndStorageQuota
          ),
          storageAndUserQuota: !closed.includes(
            QuotaBarTypes.RoomAndStorageQuota
          ),
        }));
      }

      if (!closed.includes(QuotaBarTypes.ConfirmEmail)) {
        setBarVisible((value) => ({ ...value, confirmEmail: true }));
      }
    } else {
      setBarVisible({
        roomQuota: isAdmin,
        storageQuota: isAdmin,
        userQuota: isAdmin,
        storageAndUserQuota: isAdmin,
        storageAndRoomQuota: isAdmin,
        confirmEmail: true,
      });
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
    sendActivationLink && sendActivationLink().then(showEmailActivationToast);
  };

  const onCloseActivationBar = () => {
    const closeItems = JSON.parse(localStorage.getItem("barClose")) || [];

    const closed =
      closeItems.length > 0
        ? [...closeItems, QuotaBarTypes.ConfirmEmail]
        : [QuotaBarTypes.ConfirmEmail];

    localStorage.setItem("barClose", JSON.stringify(closed));

    setBarVisible((value) => ({ ...value, confirmEmail: false }));
    setMaintenanceExist(false);
  };

  const onClickQuota = (isRoomQuota) => {
    onPaymentsClick && onPaymentsClick();

    onCloseQuota(isRoomQuota);
  };

  const onCloseQuota = (currentBar) => {
    const closeItems = JSON.parse(localStorage.getItem("barClose")) || [];

    const closed =
      closeItems.length > 0 ? [...closeItems, currentBar] : [currentBar];

    localStorage.setItem("barClose", JSON.stringify(closed));

    switch (currentBar) {
      case QuotaBarTypes.RoomQuota:
        setBarVisible((value) => ({ ...value, roomQuota: false }));
        break;
      case QuotaBarTypes.StorageQuota:
        setBarVisible((value) => ({ ...value, storageQuota: false }));
        break;
      case QuotaBarTypes.UserQuota:
        setBarVisible((value) => ({ ...value, userQuota: false }));
        break;
      case QuotaBarTypes.UserAndStorageQuota:
        setBarVisible((value) => ({ ...value, storageAndUserQuota: false }));
        break;
      case QuotaBarTypes.RoomAndStorageQuota:
        setBarVisible((value) => ({ ...value, storageAndRoomQuota: false }));
        break;
    }

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

  const getCurrentBar = () => {
    if (
      showRoomQuotaBar &&
      showStorageQuotaBar &&
      barVisible.storageAndRoomQuota
    ) {
      return {
        type: QuotaBarTypes.RoomAndStorageQuota,
        maxValue: null,
        currentValue: null,
      };
    }
    if (
      showUserQuotaBar &&
      showStorageQuotaBar &&
      barVisible.storageAndUserQuota
    ) {
      return {
        type: QuotaBarTypes.UserAndStorageQuota,
        maxValue: null,
        currentValue: null,
      };
    }

    if (showRoomQuotaBar && barVisible.roomQuota) {
      return {
        type: QuotaBarTypes.RoomQuota,
        maxValue: maxCountRoomsByQuota,
        currentValue: usedRoomsCount,
      };
    }
    if (showStorageQuotaBar && barVisible.storageQuota) {
      return {
        type: QuotaBarTypes.StorageQuota,
        maxValue: getConvertedSize(t, maxTotalSizeByQuota),
        currentValue: getConvertedSize(t, usedTotalStorageSizeCount),
      };
    }
    if (showUserQuotaBar && barVisible.userQuota) {
      return {
        type: QuotaBarTypes.UserQuota,
        maxValue: maxCountManagersByQuota,
        currentValue: addedManagersCount,
      };
    }
    return null;
  };

  const currentBar = getCurrentBar();

  const showQuotasBar = !!currentBar && tReady;

  React.useEffect(() => {
    const newValue =
      showQuotasBar ||
      (withActivationBar && barVisible.confirmEmail && tReady) ||
      (htmlLink && !firstLoad && tReady);

    setMainBarVisible(newValue);

    return () => {
      setMainBarVisible(false);
    };
  }, [
    showQuotasBar,
    withActivationBar,
    barVisible.confirmEmail,
    tReady,
    htmlLink,
    firstLoad,
  ]);

  return showQuotasBar ? (
    <QuotasBar
      currentColorScheme={currentColorScheme}
      {...currentBar}
      onClick={onClickQuota}
      onClose={onCloseQuota}
      onLoad={onLoad}
    />
  ) : withActivationBar && barVisible.confirmEmail && tReady ? (
    <ConfirmEmailBar
      userEmail={userEmail}
      currentColorScheme={currentColorScheme}
      onLoad={onLoad}
      onClick={sendActivationLinkAction}
      onClose={onCloseActivationBar}
    />
  ) : htmlLink && !firstLoad && tReady ? (
    <SnackBar
      onLoad={onLoad}
      onAction={onClose}
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

    maxCountManagersByQuota,
    addedManagersCount,

    showRoomQuotaBar,
    showStorageQuotaBar,
    showUserQuotaBar,
  } = auth.currentQuotaStore;

  const { currentColorScheme, setMainBarVisible } = auth.settingsStore;

  return {
    isAdmin: user?.isAdmin,
    userEmail: user?.email,
    withActivationBar,
    sendActivationLink,

    onPaymentsClick,

    maxCountRoomsByQuota,
    usedRoomsCount,

    maxTotalSizeByQuota,
    usedTotalStorageSizeCount,

    maxCountManagersByQuota,
    addedManagersCount,

    showRoomQuotaBar,
    showStorageQuotaBar,
    showUserQuotaBar,

    currentColorScheme,
    setMainBarVisible,
  };
})(withTranslation(["Profile", "Common"])(observer(Bar)));
