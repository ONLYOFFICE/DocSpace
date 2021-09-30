import React, { useEffect } from 'react';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router';
import Filter from '@appserver/common/api/people/filter';
import PageLayout from '@appserver/common/components/PageLayout';
import { showLoader, hideLoader, isAdmin } from '@appserver/common/utils';
import {
  ArticleHeaderContent,
  ArticleBodyContent,
  ArticleMainButtonContent,
} from '../../components/Article';
import {
  CatalogHeaderContent,
  CatalogMainButtonContent,
  CatalogBodyContent,
} from '../../components/Catalog';
import {
  SectionHeaderContent,
  SectionBodyContent,
  SectionFilterContent,
  SectionPagingContent,
} from './Section';
import { inject, observer } from 'mobx-react';
import { isMobile } from 'react-device-detect';
import Dialogs from './Section/Body/Dialogs'; //TODO: Move dialogs to another folder

const Home = ({
  isAdmin,
  isLoading,
  history,
  getUsersList,
  setIsLoading,
  setIsRefresh,
  selectedGroup,
  showCatalog,
}) => {
  const { location } = history;
  const { pathname } = location;
  console.log('People Home render');

  useEffect(() => {
    if (pathname.indexOf('/people/filter') > -1) {
      setIsLoading(true);
      setIsRefresh(true);
      const newFilter = Filter.getFilter(location);
      console.log('PEOPLE URL changed', pathname, newFilter);
      getUsersList(newFilter).finally(() => {
        setIsLoading(false);
        setIsRefresh(false);
      });
    }
  }, [pathname, location]);
  useEffect(() => {
    if (isMobile) {
      const customScrollElm = document.querySelector('#customScrollBar > .scroll-body');
      customScrollElm && customScrollElm.scrollTo(0, 0);
    }
  }, [selectedGroup]);

  useEffect(() => {
    isLoading ? showLoader() : hideLoader();
  }, [isLoading]);

  return (
    <>
      <PageLayout withBodyScroll withBodyAutoFocus={!isMobile} isLoading={isLoading}>
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

export default inject(({ auth, peopleStore }) => {
  const { settingsStore } = auth;
  const { showCatalog, showText, toggleShowText, userShowText } = settingsStore;
  const { usersStore, selectedGroupStore, loadingStore } = peopleStore;
  const { getUsersList } = usersStore;
  const { selectedGroup } = selectedGroupStore;
  const { isLoading, setIsLoading, setIsRefresh } = loadingStore;

  return {
    isAdmin: auth.isAdmin,
    isLoading,
    getUsersList,
    setIsLoading,
    setIsRefresh,
    selectedGroup,
    showCatalog,
  };
})(observer(withRouter(Home)));
