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
  } = rest;

  const { params, path } = computedMatch;
  const { userId } = params;

  const renderComponent = (props) => {
    const isPortalUrl = props.location.pathname === "/preparation-portal";

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

    if (tenantStatus !== TenantStatus.PortalRestore && isPortalUrl) {
      return window.location.replace("/");
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
  const { userStore, isAuthenticated, isLoaded, isAdmin, settingsStore } = auth;
  const { user } = userStore;

  const {
    setModuleInfo,
    wizardCompleted,
    personal,
    tenantStatus,
  } = settingsStore;

  return {
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
