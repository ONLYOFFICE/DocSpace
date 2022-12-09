import React, { useEffect } from "react";
import { withRouter } from "react-router";
import Loader from "@docspace/components/loader";
import Section from "@docspace/common/components/Section";
import { loginWithConfirmKey } from "@docspace/common/api/user";

const Auth = (props) => {
  console.log("Auth render");
  const { linkData } = props;

  useEffect(() => {
    loginWithConfirmKey({
      ConfirmData: {
        Email: linkData.email,
        Key: linkData.confirmHeader,
      },
    });
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
