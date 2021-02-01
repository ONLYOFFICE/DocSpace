import React, { useState, useEffect } from "react";
import { Box } from "@appserver/components/src";
import { useTranslation } from "react-i18next";
import {
  changeLanguage,
  getObjectByLocation,
} from "@appserver/common/src/utils";
import ErrorContainer from "@appserver/common/src/components/ErrorContainer";
import PageLayout from "@appserver/common/src/components/PageLayout";
import { createI18N } from "../../../helpers/i18n";
import { setDocumentTitle } from "../../../helpers/utils";

const i18n = createI18N({
  page: "ThirdParty",
  localesPath: "pages/ThirdParty",
});

const ThirdPartyResponsePage = ({ match }) => {
  const { params } = match;
  const { provider } = params;
  const { t } = useTranslation("translation", { i18n });
  const [error, setError] = useState(null);

  useEffect(() => {
    changeLanguage(i18n);
    const urlParams = getObjectByLocation(window.location);
    const code = urlParams ? urlParams.code || null : null;
    const error = urlParams ? urlParams.error || null : null;
    setDocumentTitle(provider);
    if (error) {
      setError(error);
    } else if (code) {
      localStorage.setItem("code", code);
      window.close();
    } else {
      setError(t("ErrorEmptyResponse"));
    }
  }, [t, provider]);

  return (
    <PageLayout>
      <PageLayout.SectionBody>
        {error ? (
          <ErrorContainer bodyText={error} />
        ) : (
          <Loaders.Rectangle height="96vh" />
        )}
      </PageLayout.SectionBody>
    </PageLayout>
  );
};

export default ThirdPartyResponsePage;
