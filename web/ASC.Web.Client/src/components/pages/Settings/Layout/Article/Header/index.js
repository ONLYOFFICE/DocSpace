import React from 'react';
import { Header } from 'asc-web-components';
import { useTranslation } from 'react-i18next';

const ArticleHeaderContent = () => {
  const { t } = useTranslation();
  return <Header type="menu">{t('Settings')}</Header>;
}

export default ArticleHeaderContent;