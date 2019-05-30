import React from 'react';
import { Redirect, Route } from 'react-router-dom';
import { AUTH_KEY } from './constants';
import Cookies from 'universal-cookie';

export const PrivateRoute = ({ component: Component, ...rest }) => {
    return (
        <Route
            {...rest}
            render={props =>
                (new Cookies()).get(AUTH_KEY) ? (
                    <Component {...props} />
                ) : (
                        <Redirect
                            to={{
                                pathname: "/login",
                                state: { from: props.location }
                            }}
                        />
                    )
            }
        />
    )
};