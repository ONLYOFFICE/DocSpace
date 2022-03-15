import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import Filter from "@appserver/common/api/people/filter";
import PageLayout from "@appserver/common/components/PageLayout";
import { showLoader, hideLoader, isAdmin } from "@appserver/common/utils";
import {
  ArticleHeaderContent,
  ArticleBodyContent,
  ArticleMainButtonContent,
} from "../../components/Article";
import {
  CatalogHeaderContent,
  CatalogMainButtonContent,
  CatalogBodyContent,
} from "../../components/Catalog";
import {
  SectionHeaderContent,
  SectionBodyContent,
  SectionFilterContent,
  SectionPagingContent,
} from "./Section";
import { inject, observer } from "mobx-react";
import { isMobile } from "react-device-detect";
import { withTranslation } from "react-i18next";
import Dialogs from "./Section/Body/Dialogs"; //TODO: Move dialogs to another folder

const PureHome = ({
  isAdmin,
  isLoading,
  history,
  getUsersList,
  setIsLoading,
  setIsRefresh,
  selectedGroup,
  tReady,
  showCatalog,
  firstLoad,
  setFirstLoad,
  viewAs,
}) => {
  const { location } = history;
  const { pathname } = location;
  //console.log("People Home render");

  useEffect(() => {
    if (pathname.indexOf("/people/filter") > -1) {
      setIsLoading(true);
      setIsRefresh(true);
      const newFilter = Filter.getFilter(location);
      //console.log("PEOPLE URL changed", pathname, newFilter);
      getUsersList(newFilter).finally(() => {
        setFirstLoad(false);
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

  useEffect(() => {});

  return (
    <>
      <PageLayout
        withBodyScroll
        withBodyAutoFocus={!isMobile}
        isLoading={isLoading}
        firstLoad={firstLoad}
        viewAs={viewAs}
      >
        {showCatalog && (
          <PageLayout.CatalogHeader>
            <CatalogHeaderContent />
          </PageLayout.CatalogHeader>
        )}
        {showCatalog && isAdmin && (
          <PageLayout.CatalogMainButton>
            <CatalogMainButtonContent />
          </PageLayout.CatalogMainButton>
        )}
        {showCatalog && (
          <PageLayout.CatalogBody>
            <CatalogBodyContent />
          </PageLayout.CatalogBody>
        )}
        {!showCatalog && (
          <PageLayout.ArticleHeader>
            <ArticleHeaderContent />
          </PageLayout.ArticleHeader>
        )}
        {!showCatalog && (
          <PageLayout.ArticleMainButton>
            <ArticleMainButtonContent />
          </PageLayout.ArticleMainButton>
        )}
        {!showCatalog && (
          <PageLayout.ArticleBody>
            <ArticleBodyContent />
          </PageLayout.ArticleBody>
        )}
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
          <SectionPagingContent tReady={tReady} />
        </PageLayout.SectionPaging>
      </PageLayout>

      <Dialogs />
    </>
  );
};

PureHome.propTypes = {
  isLoading: PropTypes.bool,
};

const Home = withTranslation("Home")(PureHome);

export default inject(({ auth, peopleStore }) => {
  const { settingsStore } = auth;
  const { showCatalog } = settingsStore;
  const { usersStore, selectedGroupStore, loadingStore, viewAs } = peopleStore;
  const { getUsersList } = usersStore;
  const { selectedGroup } = selectedGroupStore;
  const {
    isLoading,
    setIsLoading,
    setIsRefresh,
    firstLoad,
    setFirstLoad,
  } = loadingStore;

  return {
    isAdmin: auth.isAdmin,
    isLoading,
    getUsersList,
    setIsLoading,
    setIsRefresh,
    selectedGroup,
    showCatalog,
    firstLoad,
    setFirstLoad,
    viewAs,
  };
})(observer(withRouter(Home)));
