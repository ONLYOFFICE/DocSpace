import React from 'react';
import { Heading } from 'asc-web-common';
import { useTranslation } from 'react-i18next';

const ArticleHeaderContent = () => {
  const { t } = useTranslation();
  return <Heading type="menu">{t('Settings')}</Heading>;
}

export default ArticleHeaderContent;