/* eslint-disable react/prop-types */
import React from "react";
import { Redirect, Route } from "react-router-dom";
import AppLoader from "../AppLoader";
import { inject, observer } from "mobx-react";
import { TenantStatus } from "../../constants";

export const PublicRoute = ({ component: Component, ...rest }) => {
  const {
    wizardCompleted,
    isAuthenticated,
    isLoaded,
    personal,
    tenantStatus,
  } = rest;
  const renderComponent = (props) => {
    const isPreparationPortalUrl =
      props.location.pathname === "/preparation-portal";
    const isPortalRestoring = tenantStatus === TenantStatus.PortalRestore;

    if (!isLoaded) {
      return <AppLoader />;
    }

    if (personal) {
      return (
        <Redirect
          to={{
            pathname: "/",
            state: { from: props.location },
          }}
        />
      );
    }

    if (isAuthenticated && !isPortalRestoring) {
      return (
        <Redirect
          to={{
            pathname: "/",
            state: { from: props.location },
          }}
        />
      );
    }

    if (isAuthenticated && isPortalRestoring && !isPreparationPortalUrl) {
      return (
        <Redirect
          to={{
            pathname: combineUrl(
              window.DocSpaceConfig?.proxy?.url,
              "/preparation-portal"
            ),
            state: { from: props.location },
          }}
        />
      );
    }

    if (!wizardCompleted && props.location.pathname !== "/wizard") {
      return (
        <Redirect
          to={{
            pathname: "/wizard",
            state: { from: props.location },
          }}
        />
      );
    }

    if (
      !isAuthenticated &&
      isPortalRestoring &&
      wizardCompleted &&
      !isPreparationPortalUrl
    ) {
      return (
        <Redirect
          to={{
            pathname: combineUrl(
              window.DocSpaceConfig?.proxy?.url,
              "/preparation-portal"
            ),
            state: { from: props.location },
          }}
        />
      );
    }

    if (wizardCompleted && !isAuthenticated && !isPortalRestoring)
      return window.location.replace("/login");

    return <Component {...props} {...rest} />;
  };
  return <Route {...rest} render={renderComponent} />;
};

export default inject(({ auth }) => {
  const { settingsStore, isAuthenticated, isLoaded } = auth;
  const { wizardCompleted, personal, tenantStatus } = settingsStore;

  return {
    tenantStatus,
    wizardCompleted,
    isAuthenticated,
    isLoaded,
    personal,
  };
})(observer(PublicRoute));
