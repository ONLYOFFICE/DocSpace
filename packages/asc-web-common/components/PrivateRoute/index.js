/* eslint-disable react/prop-types */
import React, { useEffect } from "react";
import { Redirect, Route } from "react-router-dom";
//import Loader from "@appserver/components/loader";
//import Section from "../Section";
// import Error401 from "studio/Error401";
// import Error404 from "studio/Error404";
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
  const isPortal = window.location.pathname === "/preparation-portal";
  const { params, path } = computedMatch;
  const { userId } = params;

  const renderComponent = (props) => {
    if (isLoaded && !isAuthenticated) {
      if (personal) {
        window.location.replace("/");
        return <></>;
      }

      console.log("PrivateRoute render Redirect to login", rest);
      return (
        <Redirect
          to={{
            pathname: combineUrl(
              AppServerConfig.proxyURL,
              wizardCompleted ? "/login" : "/wizard"
            ),
            state: { from: props.location },
          }}
        />
      );
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
      !isPortal
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

  useEffect(() => {
    if (!isLoaded) return;

    let currentModule;

    if (path === "" || path === "/") {
      currentModule = {
        id: "home",
        origLink: "/",
      };
    } else if (path.startsWith("/my")) {
      currentModule = {
        id: "f4d98afd-d336-4332-8778-3c6945c81ea0",
        origLink: "/products/people",
      };
    } else {
      currentModule = modules.find((m) => m.link.startsWith(path));
    }

    if (!currentModule) return;

    const { id, origLink, link } = currentModule;

    setModuleInfo(origLink || link, id);
  }, [path, modules, isLoaded]);

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
    moduleStore,
  } = auth;
  const { user } = userStore;
  const { modules } = moduleStore;
  const {
    setModuleInfo,
    wizardCompleted,
    personal,
    tenantStatus,
  } = settingsStore;

  return {
    modules,
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
