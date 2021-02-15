/* eslint-disable react/prop-types */
import React from "react";
import { Redirect, Route } from "react-router-dom";
import PageLayout from "../PageLayout";
import RectangleLoader from "../Loaders/RectangleLoader/RectangleLoader";
import { inject, observer } from "mobx-react";

export const PublicRoute = ({ component: Component, ...rest }) => {
  const { wizardToken, wizardCompleted, isAuthenticated, isLoaded } = rest;
  const renderComponent = (props) => {
    if (!isLoaded) {
      return (
        <PageLayout>
          <PageLayout.SectionBody>
            <RectangleLoader height="90vh" />
          </PageLayout.SectionBody>
        </PageLayout>
      );
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

    if (wizardToken && !wizardCompleted) {
      return (
        <Redirect
          to={{
            pathname: "/wizard",
            state: { from: props.location },
          }}
        />
      );
    }

    return <Component {...props} />;
  };
  return <Route {...rest} render={renderComponent} />;
};

export default inject(({ auth }) => {
  const { settingsStore, isAuthenticated, isLoaded } = auth;
  const { wizardToken, wizardCompleted } = settingsStore;

  return {
    wizardToken,
    wizardCompleted,
    isAuthenticated,
    isLoaded,
  };
})(observer(PublicRoute));
