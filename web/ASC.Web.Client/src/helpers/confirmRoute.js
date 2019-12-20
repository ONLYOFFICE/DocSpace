import React from "react";
import { Route } from "react-router-dom";
import { ValidationResult } from "./../helpers/constants";
import { Loader } from "asc-web-components";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { api, constants, utils, PageLayout } from "asc-web-common";
const { checkConfirmLink } = api.user;
const { AUTH_KEY } = constants;
const { getObjectByLocation } = utils;

class ConfirmRoute extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      linkData: {},
      isLoaded: false
    };
  }

  componentDidMount() {
    const { forUnauthorized, history } = this.props;

    if (forUnauthorized && localStorage.getItem(AUTH_KEY))
      return history.push(`/error=Access error. You should be unauthorized for performing this action`);

    const { location, isAuthenticated } = this.props;
    const { search } = location;

    const queryParams = getObjectByLocation(location);
    const url = location.pathname;
    const posSeparator = url.lastIndexOf("/");
    const type = url.slice(posSeparator + 1);
    const confirmLinkData = Object.assign({ type }, queryParams);

    let path = "";
    if (!isAuthenticated) {
      path = "/login";
    }

    checkConfirmLink(confirmLinkData)
      .then(validationResult => {
        switch (validationResult) {
          case ValidationResult.Ok:
            const confirmHeader = `type=${confirmLinkData.type}&${search.slice(1)}`;
            const linkData = {
              ...confirmLinkData,
              confirmHeader
            };
            this.setState({
              isLoaded: true,
              linkData
            });
            break;
          case ValidationResult.Invalid:
            history.push(`${path}/error=Invalid link`);
            break;
          case ValidationResult.Expired:
            history.push(`${path}/error=Expired link`);
            break;
          default:
            history.push(`${path}/error=Unknown error`);
            break;
        }
      })
      .catch(error => {
        history.push(`${path}/error=${error}`);
      });
  }

  render() {
    const { component: Component, ...rest } = this.props;

    console.log(`ConfirmRoute render`, this.props, this.state);

    return (
      <Route
        {...rest}
        render={props =>
          !this.state.isLoaded ? (
            <PageLayout
              sectionBodyContent={
                <Loader className="pageLoader" type="rombs" size='40px' />
              }
            />
          ) : (
            <Component
              {...(props = { ...props, linkData: this.state.linkData })}
            />
          )
        }
      />
    );
  }
}

function mapStateToProps(state) {
  return {
    isAuthenticated: state.auth.isAuthenticated
  };
}

export default connect(
  mapStateToProps,
  { checkConfirmLink }
)(withRouter(ConfirmRoute));
