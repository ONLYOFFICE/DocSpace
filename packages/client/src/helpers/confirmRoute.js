import React from "react";
import { Route } from "react-router-dom";
import { ValidationResult } from "./../helpers/constants";
import { withRouter } from "react-router";
import Loader from "@docspace/components/loader";
import Section from "@docspace/common/components/Section";
import { checkConfirmLink } from "@docspace/common/api/user"; //TODO: Move AuthStore
import { combineUrl, getObjectByLocation } from "@docspace/common/utils";
import { inject, observer } from "mobx-react";
import { AppServerConfig } from "@docspace/common/constants";

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
      this.props.logout();
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
            const confirmHeader = `${confirmLinkData}&${search.slice(1)}`;
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
            history.push(
              combineUrl(AppServerConfig.proxyURL, path, "/error=Invalid link")
            );
            break;
          case ValidationResult.Expired:
            history.push(
              combineUrl(AppServerConfig.proxyURL, path, "/error=Expired link")
            );
            break;
          default:
            history.push(
              combineUrl(AppServerConfig.proxyURL, path, "/error=Unknown error")
            );
            break;
        }
      })
      .catch((error) => {
        let errorMessage = "";
        if (typeof error === "object") {
          errorMessage =
            error?.response?.data?.error?.message ||
            error?.statusText ||
            error?.message ||
            "";
        } else {
          errorMessage = error;
        }
        history.push(
          combineUrl(AppServerConfig.proxyURL, path, `/error=${errorMessage}`)
        );
      });
  }

  render() {
    const { component: Component, ...rest } = this.props;

    // console.log(`ConfirmRoute render`, this.props, this.state);

    return (
      <Route
        {...rest}
        render={(props) =>
          !this.state.isLoaded ? (
            <Section>
              <Section.SectionBody>
                <Loader className="pageLoader" type="rombs" size="40px" />
              </Section.SectionBody>
            </Section>
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
