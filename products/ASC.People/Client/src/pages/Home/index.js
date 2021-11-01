import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import Filter from "@appserver/common/api/people/filter";
import PageLayout from "@appserver/common/components/PageLayout";
import { showLoader, hideLoader } from "@appserver/common/utils";
import {
  ArticleHeaderContent,
  ArticleBodyContent,
  ArticleMainButtonContent,
} from "../../components/Article";
import {
  SectionHeaderContent,
  SectionBodyContent,
  SectionFilterContent,
  SectionPagingContent,
} from "./Section";
import { inject, observer } from "mobx-react";
import { isMobile } from "react-device-detect";
import Dialogs from "./Section/Body/Dialogs"; //TODO: Move dialogs to another folder

const Home = ({
  isLoading,
  history,
  getUsersList,
  setIsLoading,
  setIsRefresh,
  selectedGroup,
}) => {
  const { location } = history;
  const { pathname } = location;
  console.log("People Home render");

  useEffect(() => {
    if (pathname.indexOf("/people/filter") > -1) {
      setIsLoading(true);
      setIsRefresh(true);
      const newFilter = Filter.getFilter(location);
      console.log("PEOPLE URL changed", pathname, newFilter);
      getUsersList(newFilter).finally(() => {
        setIsLoading(false);
        setIsRefresh(false);
      });
    }
  }, [pathname, location]);
  useEffect(() => {
    if (isMobile) {
      const customScrollElm = document.querySelector(
        "#customScrollBar > .scroll-body"
      );
      customScrollElm && customScrollElm.scrollTo(0, 0);
    }
  }, [selectedGroup]);

  useEffect(() => {
    isLoading ? showLoader() : hideLoader();
  }, [isLoading]);

  return (
    <>
      <PageLayout
        withBodyScroll
        withBodyAutoFocus={!isMobile}
        isLoading={isLoading}
      >
        <PageLayout.ArticleHeader>
          <ArticleHeaderContent />
        </PageLayout.ArticleHeader>

        <PageLayout.ArticleMainButton>
          <ArticleMainButtonContent />
        </PageLayout.ArticleMainButton>

        <PageLayout.ArticleBody>
          <ArticleBodyContent />
        </PageLayout.ArticleBody>

        <PageLayout.SectionHeader>
          <SectionHeaderContent />
        </PageLayout.SectionHeader>

        <PageLayout.SectionFilter>
          <SectionFilterContent />
        </PageLayout.SectionFilter>

        <PageLayout.SectionBody>
          <SectionBodyContent />
        </PageLayout.SectionBody>

        <PageLayout.SectionPaging>
          <SectionPagingContent />
        </PageLayout.SectionPaging>
      </PageLayout>

      <Dialogs />
    </>
  );
};

Home.propTypes = {
  isLoading: PropTypes.bool,
};

export default inject(({ peopleStore }) => {
  const { usersStore, selectedGroupStore, loadingStore } = peopleStore;
  const { getUsersList } = usersStore;
  const { selectedGroup } = selectedGroupStore;
  const { isLoading, setIsLoading, setIsRefresh } = loadingStore;

  return {
    isLoading,
    getUsersList,
    setIsLoading,
    setIsRefresh,
    selectedGroup,
  };
})(observer(withRouter(Home)));
