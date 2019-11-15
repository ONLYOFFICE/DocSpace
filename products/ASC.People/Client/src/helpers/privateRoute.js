import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { Route } from "react-router-dom";
import ExternalRedirect from '../helpers/externalRedirect';
import { PageLayout, Loader } from "asc-web-components";
import { Error404 } from "../components/pages/Error";
import { store } from 'asc-web-common';
const { isAdmin, isMe } = store.auth.selectors;

const PrivateRoute = ({ component: Component, ...rest }) => {
  const { isAuthenticated, isLoaded, isAdmin, restricted, allowForMe, currentUser, computedMatch } = rest;
  const { userId } = computedMatch.params;

  console.log("PrivateRoute render", rest);
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
          restricted 
            ? (isAdmin || (allowForMe && userId && isMe(currentUser, userId))
                ? <Component {...props} /> 
                : <Error404 />
              ) 
            : <Component {...props} />
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
    isLoaded: state.auth.isLoaded,
    isAdmin: isAdmin(state.auth.user),
    currentUser: state.auth.user
  };
}

export default connect(mapStateToProps)(PrivateRoute);
