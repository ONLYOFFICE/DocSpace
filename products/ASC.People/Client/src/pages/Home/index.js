import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import Filter from "@appserver/common/api/people/filter";
import Section from "@appserver/common/components/Section";
import { showLoader, hideLoader, isAdmin } from "@appserver/common/utils";

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

  return (
    <>
      <Section
        withBodyScroll
        withBodyAutoFocus={!isMobile}
        isLoading={isLoading}
        firstLoad={firstLoad}
        viewAs={viewAs}
      >
        <Section.SectionHeader>
          <SectionHeaderContent />
        </Section.SectionHeader>
        <Section.SectionFilter>
          <SectionFilterContent />
        </Section.SectionFilter>
        <Section.SectionBody>
          <SectionBodyContent />
        </Section.SectionBody>
        <Section.SectionPaging>
          <SectionPagingContent tReady={tReady} />
        </Section.SectionPaging>
      </Section>

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
