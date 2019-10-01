import React from 'react';
import { Redirect, Route } from 'react-router-dom';
import { AUTH_KEY } from './constants';
//import Cookies from 'universal-cookie';

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