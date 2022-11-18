/* eslint-disable react/prop-types */
import React, { useEffect } from "react";
import { Redirect, Route } from "react-router-dom";
//import Loader from "@docspace/components/loader";
//import Section from "../Section";
// import Error401 from "client/Error401";
// import Error404 from "client/Error404";
import AppLoader from "../AppLoader";
import { inject, observer } from "mobx-react";
import { isMe } from "../../utils";
import combineUrl from "../../utils/combineUrl";
import { AppServerConfig, TenantStatus } from "../../constants";
import CurrentTariffStatus from "../../store/CurrentTariffStatusStore";

const PrivateRoute = ({ component: Component, ...rest }) => {
  const {
    isAdmin,
    isAuthenticated,
    isLoaded,
    restricted,
    allowForMe,
    user,
    computedMatch,
    setModuleInfo,
    modules,
    currentProductId,
    wizardCompleted,
    personal,
    location,
    tenantStatus,
    isNotPaidPeriod,
    withManager,
  } = rest;

  const { params, path } = computedMatch;
  const { userId } = params;

  const renderComponent = (props) => {
    const isPortalUrl = props.location.pathname === "/preparation-portal";
    const isPaymentsUrl =
      props.location.pathname === "/portal-settings/payments/portal-payments";
    const isBackupUrl =
      props.location.pathname === "/portal-settings/backup/data-backup";
    const isPortalUnavailableUrl =
      props.location.pathname === "/portal-unavailable";

    const isPortalDeletionUrl =
      props.location.pathname === "/portal-settings/delete-data/deletion" ||
      props.location.pathname === "/portal-settings/delete-data/deactivation";

    if (isLoaded && !isAuthenticated) {
      if (personal) {
        window.location.replace("/");
        return <></>;
      }

      console.log("PrivateRoute render Redirect to login", rest);
      const redirectPath = wizardCompleted ? "/login" : "/wizard";
      return window.location.replace(redirectPath);
      // return (
      //   <Redirect
      //     to={{
      //       pathname: combineUrl(
      //         AppServerConfig.proxyURL,
      //         wizardCompleted ? "/login" : "/wizard"
      //       ),
      //       state: { from: props.location },
      //     }}
      //   />
      // );
    }

    if (
      isLoaded &&
      ((!isNotPaidPeriod && isPortalUnavailableUrl) ||
        (!user.isOwner && isPortalDeletionUrl))
    ) {
      return window.location.replace("/");
    }

    if (location.pathname === "/" && personal) {
      return (
        <Redirect
          to={{
            pathname: "/products/files",
            state: { from: props.location },
          }}
        />
      );
    }

    if (
      isLoaded &&
      isAuthenticated &&
      tenantStatus === TenantStatus.PortalRestore &&
      !isPortalUrl
    ) {
      return (
        <Redirect
          to={{
            pathname: combineUrl(
              AppServerConfig.proxyURL,
              "/preparation-portal"
            ),
            state: { from: props.location },
          }}
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
        <Redirect
          to={{
            pathname: combineUrl(
              AppServerConfig.proxyURL,
              "/portal-settings/payments/portal-payments"
            ),
            state: { from: props.location },
          }}
        />
      );
    }

    if (tenantStatus !== TenantStatus.PortalRestore && isPortalUrl) {
      return window.location.replace("/");
    }

    if (
      isNotPaidPeriod &&
      isLoaded &&
      !user.isOwner &&
      !user.isAdmin &&
      isPortalUnavailableUrl
    ) {
      return (
        <Redirect
          to={{
            pathname: combineUrl(
              AppServerConfig.proxyURL,
              "/portal-unavailable"
            ),
            state: { from: props.location },
          }}
        />
      );
    }

    if (!isLoaded) {
      return <AppLoader />;
    }

    // const userLoaded = !isEmpty(user);
    // if (!userLoaded) {
    //   return <Component {...props} />;
    // }

    // if (!userLoaded) {
    //   console.log("PrivateRoute render Loader", rest);
    //   return (
    //     <Section>
    //       <Section.SectionBody>
    //         <Loader className="pageLoader" type="rombs" size="40px" />
    //       </Section.SectionBody>
    //     </Section>
    //   );
    // }

    if (
      !restricted ||
      isAdmin ||
      (withManager && !user.isVisitor) ||
      (allowForMe && userId && isMe(user, userId))
    ) {
      // console.log(
      //   "PrivateRoute render Component",
      //   rest,
      //   Component.name || Component.displayName
      // );
      return <Component {...props} {...rest} />;
    }

    if (restricted) {
      console.log("PrivateRoute render Error401", rest);
      return (
        <Redirect
          to={{
            pathname: "/error401",
            state: { from: props.location },
          }}
        />
      );
    }

    console.log("PrivateRoute render Error404", rest);
    return (
      <Redirect
        to={{
          pathname: "/error404",
          state: { from: props.location },
        }}
      />
    );
  };

  //console.log("PrivateRoute render", rest);
  return <Route {...rest} render={renderComponent} />;
};

export default inject(({ auth }) => {
  const {
    userStore,
    isAuthenticated,
    isLoaded,
    isAdmin,
    settingsStore,
    currentTariffStatusStore,
  } = auth;
  const { isNotPaidPeriod } = currentTariffStatusStore;
  const { user } = userStore;

  const {
    setModuleInfo,
    wizardCompleted,
    personal,
    tenantStatus,
  } = settingsStore;

  return {
    isNotPaidPeriod,
    user,
    isAuthenticated,
    isAdmin,
    isLoaded,
    setModuleInfo,
    wizardCompleted,
    tenantStatus,
    personal,
  };
})(observer(PrivateRoute));
