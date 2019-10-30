import React from 'react';
import { Redirect, Route } from 'react-router-dom';
import { AUTH_KEY } from './constants';
import { connect } from "react-redux";
import { isAdmin } from "../store/auth/selectors";
import { Error404 } from "../components/pages/Error";
import { PageLayout, Loader } from "asc-web-components";
//import Cookies from 'universal-cookie';

const PrivateRoute = ({ component: Component, ...rest }) => {
    const { isAuthenticated, isLoaded, isAdmin, restricted } = rest;

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
                            ? (isAdmin
                                ? <Component {...props} />
                                : <Error404 />
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