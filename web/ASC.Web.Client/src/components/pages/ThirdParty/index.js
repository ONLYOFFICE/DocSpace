import React, { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import { utils, Loaders, ErrorContainer, PageLayout } from "asc-web-common";
import { setDocumentTitle } from "../../../helpers/utils";

const { getObjectByLocation } = utils;

const ThirdPartyResponsePage = ({ match }) => {
  const { params } = match;
  const { provider } = params;
  const { t } = useTranslation("ThirdParty");
  const [error, setError] = useState(null);

  useEffect(() => {
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
