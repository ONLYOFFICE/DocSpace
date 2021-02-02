/* eslint-disable react/prop-types */
import React from "react";
import { Redirect, Route } from "react-router-dom";
import { connect } from "react-redux";
import { getIsLoaded, isAuthenticated } from "../../store/auth/selectors";
import PageLayout from "../PageLayout";
import RectangleLoader from "../Loaders/RectangleLoader/RectangleLoader";
import store from "../../store";
import { observer } from "mobx-react";

const { settingsStore } = store;

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

function mapStateToProps(state) {
  //const { settings } = state.auth;
  //const {wizardToken, wizardCompleted} = settings;
  return {
    isAuthenticated: isAuthenticated(state),
    isLoaded: getIsLoaded(state),

    //wizardToken,
    //wizardCompleted,
  };
}

const PublicRouteWrapper = observer((props) => {
  return (
    <PublicRoute
      wizardToken={settingsStore.settings.wizardToken}
      wizardCompleted={settingsStore.settings.wizardCompleted}
      {...props}
    />
  );
});

export default connect(mapStateToProps)(PublicRouteWrapper);
