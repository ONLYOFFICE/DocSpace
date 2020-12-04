/* eslint-disable react/prop-types */
import React from "react";
import { Redirect, Route } from "react-router-dom";
import { connect } from "react-redux";

export const PublicRoute = ({ component: Component, ...rest }) => {
  const { wizardToken, wizardCompleted, isAuthenticated } = rest;

  const renderComponent = (props) => {
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
  return {
    isAuthenticated: state.auth.isAuthenticated,
    wizardToken: state.auth.settings.wizardToken,
    wizardCompleted: state.auth.settings.wizardCompleted,
  };
}

export default connect(mapStateToProps)(PublicRoute);
