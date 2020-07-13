/* eslint-disable react/prop-types */
import React, { useCallback } from 'react';
import { Redirect, Route } from 'react-router-dom';
import { AUTH_KEY } from "../../constants";
import { connect } from "react-redux";

export const PublicRoute = ({ component: Component, ...rest }) => {
    const token = localStorage.getItem(AUTH_KEY);

    const {wizardToken} = rest;

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

            if(wizardToken) {
                return (
                    <Redirect
                        to={{
                            pathname: "/wizard",
                            state: { from: props.location }
                        }}
                    />
                );
            }

            return <Component {...props} />;
        }, [token, wizardToken, Component]); 
    return (
        <Route
            {...rest}
            render={renderComponent}
        />
    )
};

function mapStateToProps(state) {
    return {
      wizardToken: state.auth.settings.wizardToken
    };
  }

export default connect(mapStateToProps)(PublicRoute);