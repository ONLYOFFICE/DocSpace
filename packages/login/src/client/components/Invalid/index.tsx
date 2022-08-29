import React from "react";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import { useTranslation, Trans } from "react-i18next";
import { AppServerConfig } from "@docspace/common/constants";
import { combineUrl } from "@docspace/common/utils";

const homepage = "/login";
const InvalidError = () => {
  const [proxyHomepageUrl, setProxyHomepageUrl] = React.useState("");
  const { t } = useTranslation("Login");

  React.useEffect(() => {
    const { proxyURL } = AppServerConfig;
    const url = combineUrl(proxyURL, homepage);
    setProxyHomepageUrl(url);
    setTimeout(() => (location.href = url), 10000);
  }, []);

  return (
    <ErrorContainer headerText={t("ErrorInvalidHeader")}>
      <Text fontSize="13px" fontWeight="600">
        <Trans t={t} i18nKey="ErrorInvalidText">
          In 10 seconds you will be redirected to the
          <Link
            color="#2DA7DB"
            fontSize="13px"
            fontWeight="600"
            href={proxyHomepageUrl}
          >
            login page
          </Link>
        </Trans>
      </Text>
    </ErrorContainer>
  );
};

export default InvalidError;
