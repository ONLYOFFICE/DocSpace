import React, { useEffect } from "react";
import { connect } from 'react-redux';
import { PageLayout } from "asc-web-common";
import i18n from "../i18n";
import { I18nextProvider } from "react-i18next";
import {
  ArticleHeaderContent,
  ArticleBodyContent
} from "./Article";
import { SectionHeaderContent } from './Section';
import { store } from 'asc-web-common';
const { setCurrentProductId } = store.auth.actions;

const Layout = ({ currentProductId, setCurrentProductId, language, children }) => {

  useEffect(() => {
    currentProductId !== 'settings' && setCurrentProductId('settings');
    i18n.changeLanguage(language);
  }, [language, currentProductId, setCurrentProductId]);

  return (
    <I18nextProvider i18n={i18n}>
      <PageLayout
        withBodyScroll={true}
        articleHeaderContent={<ArticleHeaderContent />}
        articleBodyContent={<ArticleBodyContent />}
        sectionHeaderContent={<SectionHeaderContent />}
        sectionBodyContent={children}
      />
    </I18nextProvider >
  );
};

function mapStateToProps(state) {
  return {
    language: state.auth.user.cultureName
  };
}

export default connect(mapStateToProps, { setCurrentProductId })(Layout);
