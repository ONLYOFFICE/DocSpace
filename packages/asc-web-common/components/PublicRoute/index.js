/* eslint-disable react/prop-types */
import React from "react";
import { Redirect, Route } from "react-router-dom";
import AppLoader from "../AppLoader";
import { inject, observer } from "mobx-react";

export const PublicRoute = ({ component: Component, ...rest }) => {
  const { wizardCompleted, isAuthenticated, isLoaded } = rest;
  const renderComponent = (props) => {
    if (!isLoaded) {
      return <AppLoader />;
    }

    if (isAuthenticated) {
      return (
        <Redirect
          to={{
            pathname: "/",
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
  const { wizardCompleted } = settingsStore;

  return {
    wizardCompleted,
    isAuthenticated,
    isLoaded,
  };
})(observer(PublicRoute));
