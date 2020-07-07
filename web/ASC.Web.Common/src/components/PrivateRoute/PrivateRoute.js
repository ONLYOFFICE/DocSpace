/* eslint-disable react/prop-types */
import React, { useCallback } from "react";
import { Redirect, Route } from "react-router-dom";
import { connect } from "react-redux";
import { Loader } from "asc-web-components";
import PageLayout from "../PageLayout";
import { isAdmin, isMe } from "../../store/auth/selectors.js";
import { AUTH_KEY } from "../../constants";
import { Error401, Error404 } from "../../pages/errors";

const PrivateRoute = ({ component: Component, ...rest }) => {
  const {
    isAuthenticated,
    isLoaded,
    isAdmin,
    restricted,
    allowForMe,
    currentUser,
    computedMatch,
    wizardToken
  } = rest;

  const { userId } = computedMatch.params;
  const token = localStorage.getItem(AUTH_KEY);

  const renderComponent = useCallback(
    props => {
      if (!isLoaded) {
        return (
          <PageLayout
            sectionBodyContent={
              <Loader className="pageLoader" type="rombs" size='40px' />
            }
          />
        );
      }

      if (!token || (isLoaded && !isAuthenticated)) {
        if(wizardToken) {
          return (
            <Redirect 
              to={{
                pathname: '/wizard',
                state: {from: props.location}
              }}
            />
          )
        }         
        return (
          <Redirect
            to={{
              pathname: "/login",
              state: { from: props.location }
            }}
          />
        );
      }

      if (
        !restricted ||
        isAdmin ||
        (allowForMe && userId && isMe(currentUser, userId))
      )
        return <Component {...props} />;

      if (restricted) {
        return <Error401 />;
      }

      return <Error404 />;
    },
    [
      token,
      isAuthenticated,
      isLoaded,
      isAdmin,
      restricted,
      allowForMe,
      currentUser,
      userId,
      wizardToken,
      Component
    ]
  );

   console.log("PrivateRoute render", rest);
  return <Route {...rest} render={renderComponent} />;
};

function mapStateToProps(state) {
  return {
    isAuthenticated: state.auth.isAuthenticated,
    isLoaded: state.auth.isLoaded,
    isAdmin: isAdmin(state.auth.user),
    currentUser: state.auth.user,
    wizardToken: state.auth.settings.wizardToken
  };
}

export default connect(mapStateToProps)(PrivateRoute);
