import React from "react";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import { I18nextProvider, useTranslation, Trans } from "react-i18next";
import { AppServerConfig } from "@docspace/common/constants";
import config from "PACKAGE_FILE";
import { combineUrl } from "@docspace/common/utils";

import i18n from "./i18n";

const InvalidError = () => {
  const { proxyURL } = AppServerConfig;
  const homepage = config.homepage;

  const PROXY_HOMEPAGE_URL = combineUrl(proxyURL, homepage);

  const { t } = useTranslation("Errors");

  setTimeout(() => (location.href = PROXY_HOMEPAGE_URL), 10000);

  return (
    <ErrorContainer headerText={t("ErrorInvalidHeader")}>
      <Text fontSize="13px" fontWeight="600">
        <Trans t={t} i18nKey="ErrorInvalidText">
          In 10 seconds you will be redirected to the
          <Link
            color="#2DA7DB"
            fontSize="13px"
            fontWeight="600"
            href={PROXY_HOMEPAGE_URL}
          >
            login page
          </Link>
        </Trans>
      </Text>
    </ErrorContainer>
  );
};

export default () => (
  <I18nextProvider i18n={i18n}>
    <InvalidError />
  </I18nextProvider>
);
