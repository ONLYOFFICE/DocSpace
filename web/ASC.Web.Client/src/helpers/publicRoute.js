import React from 'react';
import { Redirect, Route } from 'react-router-dom';
import { constants } from 'asc-web-common';
const { AUTH_KEY } = constants;

export const PublicRoute = ({ component: Component, ...rest }) => {
    const token = localStorage.getItem(AUTH_KEY);
    return (
        <Route
            {...rest}
            render={props =>
                token ? (
                    <Redirect
                        to={{
                            pathname: "/",
                            state: { from: props.location }
                        }}
                    />
                ) : (
                        <Component {...props} />
                    )
            }
        />
    )
};
export default PublicRoute;