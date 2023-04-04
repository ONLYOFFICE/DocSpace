import React, { useEffect } from "react";
import Loader from "@docspace/components/loader";
import Section from "@docspace/common/components/Section";
import { loginWithConfirmKey } from "@docspace/common/api/user";
import toastr from "@docspace/components/toast/toastr";

const Auth = (props) => {
  console.log("Auth render");
  const { linkData } = props;

  useEffect(() => {
    loginWithConfirmKey({
      ConfirmData: {
        Email: linkData.email,
        Key: linkData.key,
      },
    })
      .then((res) => {
        console.log("Login with confirm key success", res);
        if (typeof res === "string") window.location.replace(res);
        else window.location.replace("/");
      })
      .catch((error) => toastr.error(error));
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

export default AuthPage;
