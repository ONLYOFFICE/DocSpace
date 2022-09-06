import React, { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import { getObjectByLocation } from "@docspace/common/utils";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import Section from "@docspace/common/components/Section";
import Loaders from "@docspace/common/components/Loaders";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";

const ThirdPartyResponsePage = ({ match }) => {
  const { params } = match;
  const { provider } = params;
  const { t } = useTranslation("Errors");
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
    <Section>
      <Section.SectionBody>
        {error ? (
          <ErrorContainer bodyText={error} />
        ) : (
          <Loaders.Rectangle height="96vh" />
        )}
      </Section.SectionBody>
    </Section>
  );
};

export default ThirdPartyResponsePage;
