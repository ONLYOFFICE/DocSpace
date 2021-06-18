import React, { useEffect } from "react";
import { ArticleHeaderContent, ArticleBodyContent } from "./Article";
import { SectionHeaderContent, SectionPagingContent } from "./Section";
import { inject, observer } from "mobx-react";
import PageLayout from "@appserver/common/components/PageLayout";
const Layout = ({
  currentProductId,
  setCurrentProductId,
  language,
  children,
  addUsers,
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
      {addUsers && (
        <PageLayout.SectionPaging>
          <SectionPagingContent />
        </PageLayout.SectionPaging>
      )}
    </PageLayout>
  );
};

export default inject(({ auth, setup }) => {
  const { language, settingsStore } = auth;
  const { addUsers } = setup.headerAction;
  return {
    language,
    setCurrentProductId: settingsStore.setCurrentProductId,
    addUsers,
  };
})(observer(Layout));
