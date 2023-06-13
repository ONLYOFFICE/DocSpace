import React, { useEffect } from "react";
import Article from "@docspace/common/components/Article";
import { ArticleHeaderContent, ArticleBodyContent } from "./Article";
import { SectionHeaderContent, SectionPagingContent } from "./Section";
import { inject, observer } from "mobx-react";
import Section from "@docspace/common/components/Section";
import withLoading from "SRC_DIR/HOCs/withLoading";
//import commonIconsStyles from "@docspace/components/utils/common-icons-style";

import { useParams } from "react-router-dom";
import HistoryHeader from "../categories/developer-tools/Webhooks/WebhookHistory/sub-components/HistoryHeader";
import DetailsNavigationHeader from "../categories/developer-tools/Webhooks/WebhookEventDetails/sub-components/DetailsNavigationHeader";

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
  isGeneralPage,
}) => {
  useEffect(() => {
    currentProductId !== "settings" && setCurrentProductId("settings");
  }, [language, currentProductId, setCurrentProductId]);

  const { id, eventId } = useParams();

  const webhookHistoryPath = `/portal-settings/developer-tools/webhooks/${id}`;
  const webhookDetailsPath = `/portal-settings/developer-tools/webhooks/${id}/${eventId}`;
  const currentPath = window.location.pathname;

  return (
    <>
      <ArticleSettings />
      {!isGeneralPage && (
        <Section withBodyScroll={true} settingsStudio={true}>
          <Section.SectionHeader>
            {currentPath === webhookHistoryPath ? (
              <HistoryHeader />
            ) : currentPath === webhookDetailsPath ? (
              <DetailsNavigationHeader />
            ) : (
              <SectionHeaderContent />
            )}
          </Section.SectionHeader>

          <Section.SectionBody>{children}</Section.SectionBody>
          {addUsers && (
            <Section.SectionPaging>
              <SectionPagingContent />
            </Section.SectionPaging>
          )}
        </Section>
      )}
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
