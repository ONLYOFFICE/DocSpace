import React from 'react';
import { Text } from 'asc-web-components';
import { useTranslation } from 'react-i18next';

const ArticleHeaderContent = () => {
  const { t } = useTranslation();
  return <Text.MenuHeader>{t('Settings')}</Text.MenuHeader>;
}

export default ArticleHeaderContent;