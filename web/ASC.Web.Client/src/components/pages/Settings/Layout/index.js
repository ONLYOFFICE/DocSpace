import React, { useEffect } from "react";
import { PageLayout } from "asc-web-common";
import { ArticleHeaderContent, ArticleBodyContent } from "./Article";
import { SectionHeaderContent } from "./Section";
import { inject, observer } from "mobx-react";

const Layout = ({
  currentProductId,
  setCurrentProductId,
  language,
  children,
}) => {
  useEffect(() => {
    currentProductId !== "settings" && setCurrentProductId("settings");
  }, [language, currentProductId, setCurrentProductId]);

  return (
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
  );
};

export default inject(({ auth }) => {
  const { language, settingsStore } = auth;
  return {
    language,
    setCurrentProductId: settingsStore.setCurrentProductId,
  };
})(observer(Layout));
