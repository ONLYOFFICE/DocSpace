import React, { useEffect } from "react";
import { observer } from "mobx-react";
import Article from "@docspace/common/components/Article";
import { ArticleHeaderContent, ArticleBodyContent } from "./Article";
import { SectionHeaderContent } from "./Section";
import Section from "@docspace/common/components/Section";

const ArticleSettings = React.memo(() => {
  return (
    <Article hideProfileBlock hideAppsBlock>
      <Article.Header>
        <ArticleHeaderContent />
      </Article.Header>

      <Article.Body>
        <ArticleBodyContent />
      </Article.Body>
    </Article>
  );
});

const Layout = ({ children }) => {
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
