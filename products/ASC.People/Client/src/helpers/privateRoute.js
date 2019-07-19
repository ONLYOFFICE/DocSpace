import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { Route } from "react-router-dom";
import ExternalRedirect from '../helpers/externalRedirect';
import { PageLayout, Loader } from "asc-web-components";

const PrivateRoute = ({ component: Component, ...rest }) => {
  const { isAuthenticated, isLoaded } = rest;
  return (
    <Route
      {...rest}
      render={props =>
        !isLoaded ? (
          <PageLayout
            sectionBodyContent={
              <Loader className="pageLoader" type="rombs" size={40} />
            }
          />
        ) : isAuthenticated ? (
          <Component {...props} />
        ) : (
          <ExternalRedirect to="/login" />
        )
      }
    />
  );
};

PrivateRoute.propTypes = {
  isAuthenticated: PropTypes.bool.isRequired
};

function mapStateToProps(state) {
  return {
    isAuthenticated: state.auth.isAuthenticated,
    isLoaded: state.auth.isLoaded
  };
}

export default connect(mapStateToProps)(PrivateRoute);
