import React, { Suspense, useEffect } from "react";
import { connect } from 'react-redux';
import { Loader, PageLayout } from "asc-web-components";
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";
import {
  ArticleHeaderContent,
  ArticleBodyContent
} from "./Article";
import { SectionHeaderContent, SectionBodyContent } from './Section';
import { setCurrentProductId } from '../../../store/auth/actions';

const Settings = (props) => {

  useEffect(() => {
    const { currentProductId, setCurrentProductId } = props;
    currentProductId !== 'settings' && setCurrentProductId('settings');
  }, [props]);
  console.log("Settings render");
  return (
    <I18nextProvider i18n={i18n}>
      <Suspense
        fallback={<Loader className="pageLoader" type="rombs" size={40} />}
      >
        <PageLayout
          withBodyScroll={false}
          articleHeaderContent={<ArticleHeaderContent />}
          articleBodyContent={<ArticleBodyContent />}
          sectionHeaderContent={<SectionHeaderContent />}
          sectionBodyContent={<SectionBodyContent />}
        />

      </Suspense>
    </I18nextProvider >
  );
};

export default connect(null, { setCurrentProductId })(Settings);
