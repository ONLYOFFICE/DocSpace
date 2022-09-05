import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { isMobile } from "react-device-detect";

import Filter from "@docspace/common/api/people/filter";

import Section from "@docspace/common/components/Section";

import { showLoader, hideLoader } from "@docspace/common/utils";

import { InfoPanelHeaderContent, InfoPanelBodyContent } from "./InfoPanel";

import {
  SectionHeaderContent,
  SectionBodyContent,
  SectionFilterContent,
  SectionPagingContent,
  Bar,
} from "./Section";

import Dialogs from "./Section/Body/Dialogs"; //TODO: Move dialogs to another folder

const PureHome = ({
  isLoading,
  history,
  getUsersList,
  setIsLoading,
  setIsRefresh,
  selectedGroup,
  tReady,

  firstLoad,
  setFirstLoad,
  viewAs,
  checkedMaintenance,
  snackbarExist,
  setMaintenanceExist,

  setSelectedNode,
}) => {
  const { location } = history;
  const { pathname } = location;
  //console.log("People Home render");

  useEffect(() => {
    if (pathname.indexOf("/accounts/filter") > -1) {
      setSelectedNode(["accounts", "filter"]);
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
  }, [pathname, location, setSelectedNode]);

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
        <Section.SectionBar>
          {checkedMaintenance && !snackbarExist && (
            <Bar setMaintenanceExist={setMaintenanceExist} />
          )}
        </Section.SectionBar>
        <Section.SectionFilter>
          <SectionFilterContent />
        </Section.SectionFilter>
        <Section.SectionBody>
          <SectionBodyContent />
        </Section.SectionBody>
        <Section.InfoPanelHeader>
          <InfoPanelHeaderContent />
        </Section.InfoPanelHeader>
        <Section.InfoPanelBody>
          <InfoPanelBodyContent />
        </Section.InfoPanelBody>
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

const Home = withTranslation("People")(PureHome);

export default inject(({ auth, peopleStore, treeFoldersStore }) => {
  const { settingsStore } = auth;
  const { showCatalog } = settingsStore;
  const { usersStore, selectedGroupStore, loadingStore, viewAs } = peopleStore;
  const { getUsersList } = usersStore;
  const { selectedGroup } = selectedGroupStore;
  const { setSelectedNode } = treeFoldersStore;
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
    setSelectedNode,
    checkedMaintenance: auth.settingsStore.checkedMaintenance,
    setMaintenanceExist: auth.settingsStore.setMaintenanceExist,
    snackbarExist: auth.settingsStore.snackbarExist,
  };
})(observer(withRouter(Home)));
