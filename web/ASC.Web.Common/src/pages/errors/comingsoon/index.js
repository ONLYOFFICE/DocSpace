import React, { useEffect } from "react";
import { connect } from "react-redux";
import { ErrorContainer } from "asc-web-components";
import { useTranslation } from "react-i18next";
import i18n from "./i18n";

const ComingSoonContainer = ({ language }) => {
  const { t } = useTranslation("translation", { i18n });

  useEffect(() => {
    i18n.changeLanguage(language);
  }, [language]);

  return (
    <ErrorContainer
      headerText={t("ComingSoonHeader")}
      bodyText={t("ComingSoonText")}
      buttonText={t("ComingSoonButtonText")}
      buttonUrl="https://www.onlyoffice.com/blog"
    />
  );
};

function mapStateToProps(state) {
  return {
    language: state.auth.user.cultureName || state.auth.settings.culture
  };
}

const ComingSoon = connect(mapStateToProps)(ComingSoonContainer);

export default ComingSoon;
