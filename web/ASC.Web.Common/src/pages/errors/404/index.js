import React, { useEffect } from 'react';
import { connect } from "react-redux";
import { ErrorContainer } from 'asc-web-components';
import { useTranslation } from 'react-i18next';
import i18n from './i18n';

const Error404Container = ({language}) => {
  const { t } = useTranslation('translation', { i18n });

  useEffect(() => {
    i18n.changeLanguage(language);
  }, [language]);

  return <ErrorContainer>{t("Error404Text")}</ErrorContainer>;
};

function mapStateToProps(state) {
  return {
      language: state.auth.user.cultureName || state.auth.settings.culture,
  };
}

const Error404 = connect(mapStateToProps)(Error404Container);

export default Error404;

