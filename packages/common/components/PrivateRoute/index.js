/* eslint-disable react/prop-types */
import React from "react";
import { Navigate, Route } from "react-router-dom";
import { inject, observer } from "mobx-react";

import AppLoader from "../AppLoader";

import combineUrl from "../../utils/combineUrl";
import { TenantStatus } from "../../constants";

const PrivateRoute = ({ children, ...rest }) => {
  const {
    isAdmin,
    isAuthenticated,
    isLoaded,
    restricted,

    user,

    wizardCompleted,
    location,
    tenantStatus,
    isNotPaidPeriod,
    withManager,
    withCollaborator,
    isLogout,
  } = rest;

  const renderComponent = () => {
    const isPortalUrl = location.pathname === "/preparation-portal";

    const isPaymentsUrl =
      location.pathname === "/portal-settings/payments/portal-payments";
    const isBackupUrl =
      location.pathname === "/portal-settings/backup/data-backup";

    const isPortalUnavailableUrl = location.pathname === "/portal-unavailable";

    const isPortalDeletionUrl =
      location.pathname === "/portal-settings/delete-data/deletion" ||
      location.pathname === "/portal-settings/delete-data/deactivation";

    if (isLoaded && !isAuthenticated) {
      // console.log("PrivateRoute render Redirect to login", rest);
      const redirectPath = wizardCompleted ? "/login" : "/wizard";

      const isHomeUrl = location.pathname === "/";

      if (wizardCompleted && !isHomeUrl && !isLogout) {
        sessionStorage.setItem("referenceUrl", window.location.href);
      }

      return <Navigate replace to={redirectPath} />;
    }

    if (
      isLoaded &&
      ((!isNotPaidPeriod && isPortalUnavailableUrl) ||
        (!user.isOwner && isPortalDeletionUrl))
    ) {
      return <Navigate replace to={"/"} />;
    }

    if (
      isLoaded &&
      isAuthenticated &&
      tenantStatus === TenantStatus.PortalRestore &&
      !isPortalUrl
    ) {
      return (
        <Navigate
          replace
          to={combineUrl(
            window.DocSpaceConfig?.proxy?.url,
            "/preparation-portal"
          )}
        />
      );
    }

    if (
      isNotPaidPeriod &&
      isLoaded &&
      (user.isOwner || user.isAdmin) &&
      !isPaymentsUrl &&
      !isBackupUrl &&
      !isPortalDeletionUrl
    ) {
      return (
        <Navigate
          replace
          to={combineUrl(
            window.DocSpaceConfig?.proxy?.url,
            "/portal-settings/payments/portal-payments"
          )}
        />
      );
    }

    if (
      isNotPaidPeriod &&
      isLoaded &&
      !user.isOwner &&
      !user.isAdmin &&
      !isPortalUnavailableUrl
    ) {
      return (
        <Navigate
          replace
          to={combineUrl(
            window.DocSpaceConfig?.proxy?.url,
            "/portal-unavailable"
          )}
        />
      );
    }

    // if (!isLoaded) {
    //   return <AppLoader />;
    // }

    if (
      !restricted ||
      isAdmin ||
      (withManager && !user.isVisitor && !user.isCollaborator) ||
      (withCollaborator && !user.isVisitor)
    ) {
      return children;
    }

    if (restricted) {
      return <Navigate replace to={"/error401"} />;
    }

    return <Navigate replace to={"/error404"} />;
  };

  const component = renderComponent();

  return component;
};

export default inject(({ auth }) => {
  const {
    userStore,
    isAuthenticated,
    isLoaded,
    isAdmin,
    settingsStore,
    currentTariffStatusStore,
    isLogout,
  } = auth;
  const { isNotPaidPeriod } = currentTariffStatusStore;
  const { user } = userStore;

  const { wizardCompleted, tenantStatus } = settingsStore;

  return {
    isNotPaidPeriod,
    user,
    isAuthenticated,
    isAdmin,
    isLoaded,

    wizardCompleted,
    tenantStatus,

    isLogout,
  };
})(observer(PrivateRoute));
