import React, { useEffect } from "react";
import Loader from "@docspace/components/loader";
import Section from "@docspace/common/components/Section";
import { loginWithConfirmKey } from "@docspace/common/api/user";
import toastr from "@docspace/components/toast/toastr";

const Auth = (props) => {
  const { linkData } = props;
  console.log("Auth render", { linkData });

  const optionalKeys = {};

  if (linkData.hasOwnProperty("first")) {
    optionalKeys.First = linkData.first === "true";
  }

  if (linkData.hasOwnProperty("sms")) {
    optionalKeys.Sms = linkData.sms === "true";
  }

  if (linkData.hasOwnProperty("module")) {
    optionalKeys.Module = linkData.module;
  }

  useEffect(() => {
    loginWithConfirmKey({
      ConfirmData: {
        Email: linkData.email,
        Key: linkData.key,
        ...optionalKeys,
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
