/* eslint-disable react/prop-types */
import React from "react";
import { Redirect, Route } from "react-router-dom";
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
      return <Redirect to={"/"} />;
    }

    if (isAuthenticated && isPortalRestoring && !isPreparationPortalUrl) {
      return (
        <Redirect
          to={combineUrl(
            window.DocSpaceConfig?.proxy?.url,
            "/preparation-portal"
          )}
        />
      );
    }

    if (!wizardCompleted && props.location.pathname !== "/wizard") {
      return <Redirect to={"/wizard"} />;
    }

    if (
      !isAuthenticated &&
      isPortalRestoring &&
      wizardCompleted &&
      !isPreparationPortalUrl
    ) {
      return (
        <Redirect
          to={combineUrl(
            window.DocSpaceConfig?.proxy?.url,
            "/preparation-portal"
          )}
        />
      );
    }

    if (wizardCompleted && !isAuthenticated && !isPortalRestoring)
      return <Redirect to={"/login"} />;

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
