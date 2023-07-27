import React from "react";
import { observer } from "mobx-react";
import Article from "@docspace/common/components/Article";
import ArticleWrapper from "@docspace/common/components/Article/ArticleWrapper";
import { ArticleHeaderContent, ArticleBodyContent } from "./Article";
import { SectionHeaderContent } from "./Section";
import Section from "@docspace/common/components/Section";

type TLayoutProps = {
  children: React.ReactNode;
};

const ArticleSettings = React.memo(() => {
  return (
    <ArticleWrapper hideProfileBlock hideAppsBlock withCustomArticleHeader>
      <Article.Header>
        <ArticleHeaderContent />
      </Article.Header>

      <Article.Body>
        <ArticleBodyContent />
      </Article.Body>
    </ArticleWrapper>
  );
});

const Layout = ({ children }: TLayoutProps) => {
  return (
    <>
      <ArticleSettings />
      <Section withBodyScroll settingsStudio>
        <Section.SectionHeader>
          <SectionHeaderContent />
        </Section.SectionHeader>

        <Section.SectionBody>{children}</Section.SectionBody>
      </Section>
    </>
  );
};

export default observer(Layout);
