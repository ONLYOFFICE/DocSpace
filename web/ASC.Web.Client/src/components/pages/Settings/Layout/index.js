import React, { useEffect } from "react";
import { PageLayout } from "asc-web-common";
import { I18nextProvider } from "react-i18next";
import { ArticleHeaderContent, ArticleBodyContent } from "./Article";
import { SectionHeaderContent } from "./Section";
import { utils } from "asc-web-common";

import { createI18N } from "../../../../helpers/i18n";
import { inject, observer } from "mobx-react";

const i18n = createI18N({
  page: "Settings",
  localesPath: "pages/Settings",
});

const { changeLanguage } = utils;

const Layout = ({
  currentProductId,
  setCurrentProductId,
  language,
  children,
}) => {
  useEffect(() => {
    currentProductId !== "settings" && setCurrentProductId("settings");
    changeLanguage(i18n);
  }, [language, currentProductId, setCurrentProductId]);

  return (
    <I18nextProvider i18n={i18n}>
      <PageLayout withBodyScroll={true}>
        <PageLayout.ArticleHeader>
          <ArticleHeaderContent />
        </PageLayout.ArticleHeader>

        <PageLayout.ArticleBody>
          <ArticleBodyContent />
        </PageLayout.ArticleBody>

        <PageLayout.SectionHeader>
          <SectionHeaderContent />
        </PageLayout.SectionHeader>

        <PageLayout.SectionBody>{children}</PageLayout.SectionBody>
      </PageLayout>
    </I18nextProvider>
  );
};

export default inject(({ auth }) => {
  const { language, settingsStore } = auth;
  return {
    language,
    setCurrentProductId: settingsStore.setCurrentProductId,
  };
})(observer(Layout));
