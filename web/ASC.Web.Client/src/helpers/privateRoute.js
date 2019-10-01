import React from 'react';
import { Redirect, Route } from 'react-router-dom';
import { AUTH_KEY } from './constants';
//import Cookies from 'universal-cookie';

export const PrivateRoute = ({ component: Component, ...rest }) => {
    const token = localStorage.getItem(AUTH_KEY);
    return (
        <Route
            {...rest}
            render={props =>
                token ? (
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