/* eslint-disable react/prop-types */
import React from "react";
import { Redirect, Route } from "react-router-dom";
import AppLoader from "../AppLoader";
import { inject, observer } from "mobx-react";

export const PublicRoute = ({ component: Component, ...rest }) => {
  const { wizardCompleted, isAuthenticated, isLoaded, personal } = rest;
  const renderComponent = (props) => {
    if (!isLoaded) {
      return <AppLoader />;
    }

    if (isAuthenticated || personal) {
      return (
        <Redirect
          to={{
            pathname: "/",
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

    return <Component {...props} {...rest} />;
  };
  return <Route {...rest} render={renderComponent} />;
};

export default inject(({ auth }) => {
  const { settingsStore, isAuthenticated, isLoaded } = auth;
  const { wizardCompleted, personal } = settingsStore;

  return {
    wizardCompleted,
    isAuthenticated,
    isLoaded,
    personal,
  };
})(observer(PublicRoute));
