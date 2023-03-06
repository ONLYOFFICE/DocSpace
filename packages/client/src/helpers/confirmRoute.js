import React from "react";
import { Route } from "react-router-dom";
import { ValidationResult } from "./../helpers/constants";
import { withRouter } from "react-router";
import Loader from "@docspace/components/loader";
import Section from "@docspace/common/components/Section";
import { checkConfirmLink } from "@docspace/common/api/user"; //TODO: Move AuthStore
import { combineUrl, getObjectByLocation } from "@docspace/common/utils";
import { inject, observer } from "mobx-react";

class ConfirmRoute extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      linkData: {},
      isLoaded: false,
    };
  }

  componentDidMount() {
    const { forUnauthorized, isAuthenticated } = this.props;

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
            const confirmHeader = search.slice(1);
            const linkData = {
              ...confirmLinkData,
              confirmHeader,
            };

            console.log("checkConfirmLink", {
              confirmLinkData,
              validationResult,
              linkData,
            });

            this.setState({
              isLoaded: true,
              linkData,
            });
            break;
          case ValidationResult.Invalid:
            console.error("invlid link", { confirmLinkData, validationResult });
            window.location.href = combineUrl(
              window.DocSpaceConfig?.proxy?.url,
              path,
              "/error"
            );
            break;
          case ValidationResult.Expired:
            console.error("expired link", {
              confirmLinkData,
              validationResult,
            });
            window.location.href = combineUrl(
              window.DocSpaceConfig?.proxy?.url,
              path,
              "/error"
            );
            break;
          default:
            console.error("unknown link", {
              confirmLinkData,
              validationResult,
            });
            window.location.href = combineUrl(
              window.DocSpaceConfig?.proxy?.url,
              path,
              "/error"
            );
            break;
        }
      })
      .catch((error) => {
        console.error("FAILED checkConfirmLink", { error, confirmLinkData });
        window.location.href = combineUrl(
          window.DocSpaceConfig?.proxy?.url,
          path,
          "/error"
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
