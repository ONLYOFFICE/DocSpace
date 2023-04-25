import React from "react";
import { useLocation } from "react-router-dom";
import { ValidationResult } from "./../helpers/constants";
import Loader from "@docspace/components/loader";
import Section from "@docspace/common/components/Section";
import { checkConfirmLink } from "@docspace/common/api/user"; //TODO: Move AuthStore
import { combineUrl, getObjectByLocation } from "@docspace/common/utils";
import { inject, observer } from "mobx-react";

const ConfirmRoute = (props) => {
  const [state, setState] = React.useState({
    linkData: {},
    isLoaded: false,
  });

  const location = useLocation();

  React.useEffect(() => {
    const { forUnauthorized, isAuthenticated } = props;

    if (forUnauthorized && isAuthenticated) {
      props.logout();
    }

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

            setState((val) => ({ ...val, isLoaded: true, linkData }));
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
  }, []);

  // console.log(`ConfirmRoute render`, this.props, this.state);

  return !state.isLoaded ? (
    <Section>
      <Section.SectionBody>
        <Loader className="pageLoader" type="rombs" size="40px" />
      </Section.SectionBody>
    </Section>
  ) : (
    React.cloneElement(props.children, {
      linkData: state.linkData,
    })
  );
};

export default inject(({ auth }) => {
  const { isAuthenticated, logout } = auth;
  return {
    isAuthenticated,
    logout,
  };
})(observer(ConfirmRoute));
