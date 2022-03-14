import React from "react";
import { withTranslation } from "react-i18next";
import Filter from "@appserver/common/api/people/filter";
import Loaders from "@appserver/common/components/Loaders";
import { inject, observer } from "mobx-react";
import { getSelectedGroup } from "../../../helpers/people-helpers";
import { withRouter } from "react-router";
import { isMobile } from "@appserver/components/utils/device";
import { isMobileOnly } from "react-device-detect";
import config from "../../../../package.json";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import CatalogItem from "@appserver/components/catalog-item";
import withLoader from "../../../HOCs/withLoader";

const departmentsIcon = "images/departments.group.react.svg";
const groupIcon = "/static/images/catalog.folder.react.svg";

const CatalogBodyContent = ({
  selectedKeys,
  groups,
  setFirstLoad,
  toggleCatalogOpen,
  showText,
  groupsCaption,
  history,
  filter,
  selectGroup,
  isVisitor,
  isAdmin,
  isLoaded,
  setDocumentTitle,
}) => {
  React.useEffect(() => {
    changeTitleDocument();
  }, []);

  React.useEffect(() => {
    changeTitleDocument();
  }, [selectedKeys[0]]);

  const changeTitleDocument = (id) => {
    const currentGroup = getSelectedGroup(
      groups,
      id === "root" ? selectedKeys[0] : id
    );
    currentGroup ? setDocumentTitle(currentGroup.name) : setDocumentTitle();
  };

  const isActive = (group) => {
    if (group === selectedKeys[0]) return true;
    return false;
  };
  const onClick = (data) => {
    const isRoot = data === "departments";
    const groupId = isRoot ? "root" : data;
    changeTitleDocument(groupId);

    if (window.location.pathname.indexOf("/people/filter") > 0) {
      selectGroup(groupId);
      if (isMobileOnly || isMobile()) toggleCatalogOpen();
    } else {
      setFirstLoad(true);
      const newFilter = isRoot ? Filter.getDefault() : filter.clone();

      if (!isRoot) newFilter.group = groupId;

      const urlFilter = newFilter.toUrlParams();
      const url = combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/filter?${urlFilter}`
      );
      history.push(url);
      if (isMobileOnly || isMobile()) toggleCatalogOpen();
    }
  };

  const getItems = (data) => {
    const items = data.map((group) => {
      return (
        <CatalogItem
          key={group.id}
          id={group.id}
          icon={groupIcon}
          text={group.name}
          showText={showText}
          showInitial={true}
          onClick={onClick}
          isActive={isActive(group.id)}
        />
      );
    });
    return items;
  };

  return (
    <>
      {!isVisitor && (
        <div style={!isAdmin && isMobileOnly ? { marginTop: "16px" } : null}>
          <CatalogItem
            key={"root"}
            id={"departments"}
            icon={departmentsIcon}
            onClick={onClick}
            text={groupsCaption}
            showText={showText}
            isActive={isActive("root")}
          />
          {getItems(groups)}
        </div>
      )}
    </>
  );
};

const BodyContent = withTranslation("Article")(
  withRouter(withLoader(CatalogBodyContent)(<Loaders.PeopleCatalogLoader />))
);

export default inject(({ auth, peopleStore }) => {
  const { settingsStore, setDocumentTitle, isAdmin } = auth;
  const { customNames, showText, toggleCatalogOpen } = settingsStore;
  const {
    groupsStore,
    selectedGroupStore,
    editingFormStore,
    filterStore,
    loadingStore,
  } = peopleStore;
  const { filter } = filterStore;
  const { groups } = groupsStore;
  const { groupsCaption } = customNames;
  const { isEdit, setIsVisibleDataLossDialog } = editingFormStore;
  const { selectedGroup, selectGroup } = selectedGroupStore;
  const selectedKeys = selectedGroup ? [selectedGroup] : ["root"];
  const { setFirstLoad, isLoading, setIsLoading, isLoaded } = loadingStore;
  return {
    setDocumentTitle,
    isLoaded,
    isVisitor: auth.userStore.user.isVisitor,
    isAdmin,
    groups,
    groups,
    groupsCaption,
    selectedKeys,
    selectGroup,
    isEdit,
    setIsVisibleDataLossDialog,
    isLoading,
    setIsLoading,
    filter,
    setFirstLoad,
    showText,
    toggleCatalogOpen,
  };
})(observer(BodyContent));
