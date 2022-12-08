import React, { useEffect } from "react";
import { withRouter } from "react-router";
import Loader from "@docspace/components/loader";
import Section from "@docspace/common/components/Section";
import { combineUrl } from "@docspace/common/utils";
import tryRedirectTo from "@docspace/common/utils/tryRedirectTo";
import { AppServerConfig } from "@docspace/common/constants";

const Auth = (props) => {
  console.log("Auth render");

  useEffect(() => {
    tryRedirectTo(combineUrl(AppServerConfig.proxyURL, `/login`));
  });

  return <Loader className="pageLoader" type="rombs" size="40px" />;
};

const AuthPage = (props) => (
  <Section>
    <Section.SectionBody>
      <Auth {...props} />
    </Section.SectionBody>
  </Section>
);

export default withRouter(AuthPage);
