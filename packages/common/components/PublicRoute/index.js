/* eslint-disable react/prop-types */
import React from "react";
import { Navigate, Route } from "react-router-dom";
import AppLoader from "../AppLoader";
import { inject, observer } from "mobx-react";
import { TenantStatus } from "../../constants";

export const PublicRoute = ({ children, ...rest }) => {
  const { wizardCompleted, isAuthenticated, tenantStatus } = rest;
  const renderComponent = () => {
    const isPreparationPortalUrl =
      props.location.pathname === "/preparation-portal";
    const isPortalRestoring = tenantStatus === TenantStatus.PortalRestore;

    // if (!isLoaded) {
    //   return <AppLoader />;
    // }

    if (isAuthenticated && !isPortalRestoring) {
      return <Navigate replace to={"/"} />;
    }

    if (isAuthenticated && isPortalRestoring && !isPreparationPortalUrl) {
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

    if (!wizardCompleted && props.location.pathname !== "/wizard") {
      return <Navigate replace to={"/wizard"} />;
    }

    if (
      !isAuthenticated &&
      isPortalRestoring &&
      wizardCompleted &&
      !isPreparationPortalUrl
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

    if (wizardCompleted && !isAuthenticated && !isPortalRestoring)
      return <Navigate replace to={"/login"} />;

    return children;
  };

  const component = renderComponent();

  return component;
};

export default inject(({ auth }) => {
  const { settingsStore, isAuthenticated, isLoaded } = auth;
  const { wizardCompleted, tenantStatus } = settingsStore;

  return {
    tenantStatus,
    wizardCompleted,
    isAuthenticated,
    isLoaded,
  };
})(observer(PublicRoute));
