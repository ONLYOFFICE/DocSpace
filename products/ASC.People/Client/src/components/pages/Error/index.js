import React from 'react';
import { ErrorContainer } from 'asc-web-components';
import { useTranslation } from 'react-i18next';
import i18n from './i18n';

export const Error404 = () => {
  const { t } = useTranslation('translation', { i18n });
  return <ErrorContainer>{t("Error404Text")}</ErrorContainer>;
};