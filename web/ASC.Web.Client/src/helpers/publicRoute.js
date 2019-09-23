import React from 'react';
import { Redirect, Route } from 'react-router-dom';
import { AUTH_KEY } from './constants';
import Cookies from 'universal-cookie';

export const PublicRoute = ({ component: Component, ...rest }) => {
    return (
        <Route
            {...rest}
            render={props =>
                (new Cookies()).get(AUTH_KEY) ? (
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