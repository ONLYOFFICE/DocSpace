/* eslint-disable react/prop-types */
import React from "react";
import { Redirect, Route } from "react-router-dom";
import { connect } from "react-redux";
//import { Loader } from "asc-web-components";
import PageLayout from "../PageLayout";
import { getCurrentUser, getIsLoaded, isAdmin, isAuthenticated, isMe } from "../../store/auth/selectors.js";
import { Error401, Error404 } from "../../pages/errors";
import RectangleLoader from "../Loaders/RectangleLoader/RectangleLoader";
//import isEmpty from "lodash/isEmpty";

const PrivateRoute = ({ component: Component, ...rest }) => {
  const {
    isAdmin,
    isAuthenticated,
    isLoaded,
    restricted,
    allowForMe,
    user,
    computedMatch,
  } = rest;
  const { userId } = computedMatch.params;

  const renderComponent = (props) => {
    if (isLoaded && !isAuthenticated) {
      console.log("PrivateRoute render Redirect to login", rest);
      return (
        <Redirect
          to={{
            pathname: "/login",
            state: { from: props.location },
          }}
        />
      );
    }

    if(!isLoaded) {
      return (
            <PageLayout>
              <PageLayout.SectionBody>
                <RectangleLoader  height="90vh"/>
              </PageLayout.SectionBody>
            </PageLayout>
          );
    }

    // const userLoaded = !isEmpty(user);
    // if (!userLoaded) {
    //   return <Component {...props} />;
    // }

    // if (!userLoaded) {
    //   console.log("PrivateRoute render Loader", rest);
    //   return (
    //     <PageLayout>
    //       <PageLayout.SectionBody>
    //         <Loader className="pageLoader" type="rombs" size="40px" />
    //       </PageLayout.SectionBody>
    //     </PageLayout>
    //   );
    // }

    if (
      !restricted ||
      isAdmin ||
      (allowForMe && userId && isMe(user, userId))
    ) {
      console.log(
        "PrivateRoute render Component",
        rest,
        Component.name || Component.displayName
      );
      return <Component {...props} />;
    }

    if (restricted) {
      console.log("PrivateRoute render Error401", rest);
      return <Error401 />;
    }

    console.log("PrivateRoute render Error404", rest);
    return <Error404 />;
  };

  //console.log("PrivateRoute render", rest);
  return <Route {...rest} render={renderComponent} />;
};

function mapStateToProps(state) {
  return {
    isAdmin: isAdmin(state),
    user: getCurrentUser(state),
    isAuthenticated: isAuthenticated(state),
    isLoaded: getIsLoaded(state)
  };
}

export default connect(mapStateToProps)(PrivateRoute);
