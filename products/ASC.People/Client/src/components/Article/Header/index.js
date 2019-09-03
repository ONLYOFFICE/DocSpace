import React from 'react';
import { Text } from 'asc-web-components';
import { useTranslation } from 'react-i18next';
import i18n from '../i18n';

const ArticleHeaderContent = () => {
  const { t } = useTranslation('translation', { i18n });
  return <Text.MenuHeader>{t('People')}</Text.MenuHeader>;
}

export default ArticleHeaderContent;