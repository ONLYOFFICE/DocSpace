import React, { useEffect } from "react";
import Article from "@docspace/common/components/Article";
import { ArticleHeaderContent, ArticleBodyContent } from "./Article";
import { SectionHeaderContent, SectionPagingContent } from "./Section";
import { inject, observer } from "mobx-react";
import Section from "@docspace/common/components/Section";
import withLoading from "SRC_DIR/HOCs/withLoading";
//import commonIconsStyles from "@docspace/components/utils/common-icons-style";
const ArticleSettings = React.memo(() => {
  return (
    <Article>
      <Article.Header>
        <ArticleHeaderContent />
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
  isTitleVisible,
}) => {
  useEffect(() => {
    currentProductId !== "settings" && setCurrentProductId("settings");
  }, [language, currentProductId, setCurrentProductId]);

  return (
    <>
      <ArticleSettings />
      <Section withBodyScroll={true} settingsStudio={true}>
        {/* <Section.SectionHeader>{isTitleVisible && <SectionHeaderContent />}</Section.SectionHeader> */}
        {isTitleVisible && (
          <Section.SectionHeader>
            <SectionHeaderContent />
          </Section.SectionHeader>
        )}

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

export default inject(({ auth, setup, webhooksStore }) => {
  const { language, settingsStore } = auth;
  const { addUsers } = setup.headerAction;
  const { isTitleVisible } = webhooksStore;

  return {
    language,
    setCurrentProductId: settingsStore.setCurrentProductId,
    addUsers,
    isTitleVisible,
  };
})(withLoading(observer(Layout)));
