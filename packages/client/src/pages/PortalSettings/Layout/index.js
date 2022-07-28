import React, { useEffect } from "react";
import Article from "@docspace/common/components/Article";
import { ArticleHeaderContent, ArticleBodyContent } from "./Article";
import { SectionHeaderContent, SectionPagingContent } from "./Section";
import { inject, observer } from "mobx-react";
import Section from "@docspace/common/components/Section";
import withLoading from "SRC_DIR/HOCs/withLoading";
//import commonIconsStyles from "@docspace/components/utils/common-icons-style";
const ArticleSettings = React.memo(({ isLoadedPage }) => {
  return (
    <Article isLoadedPage={isLoadedPage}>
      <Article.Header>
        <ArticleHeaderContent isLoadedPage={isLoadedPage} />
      </Article.Header>

      <Article.Body>
        <ArticleBodyContent />
      </Article.Body>
    </Article>
  );
});

const Layout = ({
  currentProductId,
  setCurrentProductId,
  language,
  children,
  addUsers,
  isLoadedPage,
}) => {
  useEffect(() => {
    currentProductId !== "settings" && setCurrentProductId("settings");
  }, [language, currentProductId, setCurrentProductId]);

  return (
    <>
      <ArticleSettings isLoadedPage={isLoadedPage} />
      <Section withBodyScroll={true} settingsStudio={true}>
        <Section.SectionHeader>
          <SectionHeaderContent />
        </Section.SectionHeader>

        <Section.SectionBody>{children}</Section.SectionBody>
        {addUsers && (
          <Section.SectionPaging>
            <SectionPagingContent />
          </Section.SectionPaging>
        )}
      </Section>
    </>
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
})(withLoading(observer(Layout)));
