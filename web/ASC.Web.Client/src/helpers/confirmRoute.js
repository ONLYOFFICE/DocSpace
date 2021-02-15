import React from "react";
import { Route } from "react-router-dom";
import { ValidationResult } from "./../helpers/constants";
import { Loader } from "asc-web-components";
import { withRouter } from "react-router";
import { api, utils, PageLayout } from "asc-web-common";
import { inject, observer } from "mobx-react";
const { checkConfirmLink } = api.user; //TODO: Move AuthStore
const { getObjectByLocation } = utils;

class ConfirmRoute extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      linkData: {},
      isLoaded: false,
    };
  }

  componentDidMount() {
    const { forUnauthorized, history, isAuthenticated } = this.props;

    if (forUnauthorized && isAuthenticated) {
      this.props.logout(true);
    }

    const { location } = this.props;
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
      .then((validationResult) => {
        switch (validationResult) {
          case ValidationResult.Ok:
            const confirmHeader = `type=${confirmLinkData.type}&${search.slice(
              1
            )}`;
            const linkData = {
              ...confirmLinkData,
              confirmHeader,
            };
            this.setState({
              isLoaded: true,
              linkData,
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
      .catch((error) => {
        history.push(`${path}/error=${error}`);
      });
  }

  render() {
    const { component: Component, ...rest } = this.props;

    console.log(`ConfirmRoute render`, this.props, this.state);

    return (
      <Route
        {...rest}
        render={(props) =>
          !this.state.isLoaded ? (
            <PageLayout>
              <PageLayout.SectionBody>
                <Loader className="pageLoader" type="rombs" size="40px" />
              </PageLayout.SectionBody>
            </PageLayout>
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

export default inject(({ auth }) => {
  const { isAuthenticated, logout } = auth;
  return {
    isAuthenticated,
    logout,
  };
})(observer(withRouter(ConfirmRoute)));
