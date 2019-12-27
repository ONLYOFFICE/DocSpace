/* eslint-disable react/prop-types */
import React, { useCallback } from 'react';
import { Redirect, Route } from 'react-router-dom';
import { AUTH_KEY } from "../../constants";

export const PublicRoute = ({ component: Component, ...rest }) => {
    const token = localStorage.getItem(AUTH_KEY);

    const renderComponent = useCallback(
        props => {
            if(token) {
                return (
                    <Redirect
                        to={{
                            pathname: "/",
                            state: { from: props.location }
                        }}
                    />
                );
            }

            return <Component {...props} />;
        }, [token, Component]);

    return (
        <Route
            {...rest}
            render={renderComponent}
        />
    )
};
export default PublicRoute;