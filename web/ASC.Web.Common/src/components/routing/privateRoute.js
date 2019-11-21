/* eslint-disable react/prop-types */
import React from 'react';
import { Redirect, Route } from 'react-router-dom';
import { connect } from "react-redux";
import { PageLayout, Loader } from "asc-web-components";
import { store, constants } from 'asc-web-common';
const { isAdmin, isMe } = store.auth.selectors;
const { AUTH_KEY } = constants;

const PrivateRoute = ({ component: Component, ...rest }) => {
    const { isAuthenticated, isLoaded, isAdmin, restricted, allowForMe, currentUser, computedMatch } = rest;
    const { userId } = computedMatch.params;

    const token = localStorage.getItem(AUTH_KEY);

    // console.log("PrivateRoute render", rest);
    return (
        <Route
            {...rest}
            render={props =>
                token ? (
                    !isLoaded ? (
                        <PageLayout
                             sectionBodyContent={
                                 <Loader className="pageLoader" type="rombs" size={40} />
                             }
                        />
                    ) : isAuthenticated ? (
                        restricted
                            ? (isAdmin || allowForMe && userId && isMe(currentUser, userId)
                                ? <Component {...props} />
                                : <Redirect
                                        to={{
                                            pathname: "/error=401",
                                            state: { from: props.location }
                                        }}
                                    />
                            )
                            : <Component {...props} />
                    ) : (
                                <Redirect
                                    to={{
                                        pathname: "/login",
                                        state: { from: props.location }
                                    }}
                                />
                            )) : (
                        <Redirect
                            to={{
                                pathname: "/login",
                                state: { from: props.location }
                            }}
                        />
                    )
            }
        />
    );
};

function mapStateToProps(state) {
    return {
        isAuthenticated: state.auth.isAuthenticated,
        isLoaded: state.auth.isLoaded,
        isAdmin: isAdmin(state.auth.user)
    };
}

export default connect(mapStateToProps)(PrivateRoute);