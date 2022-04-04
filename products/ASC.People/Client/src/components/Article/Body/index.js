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

const departmentsIcon = "images/departments.group.react.svg";
const groupIcon = "/static/images/catalog.folder.react.svg";

const ArticleBodyContent = ({
  selectedKey,
  groups,
  toggleArticleOpen,
  showText,
  groupsCaption,
  history,
  filter,
  selectGroup,
  isVisitor,
  isAdmin,
  setDocumentTitle,
  isArticleLoading,
}) => {
  const [groupItems, setGroupItems] = React.useState(null);

  const changeTitleDocument = React.useCallback(
    (id) => {
      const currentGroup = getSelectedGroup(
        groups,
        id === "root" ? selectedKey : id
      );
      currentGroup ? setDocumentTitle(currentGroup.name) : setDocumentTitle();
    },
    [getSelectedGroup, groups, selectedKey, setDocumentTitle]
  );

  const isActive = React.useCallback(
    (group) => {
      if (group === selectedKey) return true;
      return false;
    },
    [selectedKey]
  );

  const onClick = React.useCallback(
    (data) => {
      const isRoot = data === "departments";
      const groupId = isRoot ? "root" : data;

      changeTitleDocument(groupId);

      if (window.location.pathname.indexOf("/people/filter") > 0) {
        selectGroup(groupId);
        if (isMobileOnly || isMobile()) toggleArticleOpen();
      } else {
        const newFilter = isRoot ? Filter.getDefault() : filter.clone();

        if (!isRoot) newFilter.group = groupId;

        const urlFilter = newFilter.toUrlParams();
        const url = combineUrl(
          AppServerConfig.proxyURL,
          config.homepage,
          `/filter?${urlFilter}`
        );
        history.push(url);
        if (isMobileOnly || isMobile()) toggleArticleOpen();
      }
    },
    [changeTitleDocument, selectGroup, toggleArticleOpen, filter]
  );

  const getItems = React.useCallback(
    (data) => {
      const items = data.map((group) => {
        const active = isActive(group.id);
        return (
          <CatalogItem
            key={group.id}
            id={group.id}
            icon={groupIcon}
            text={group.name}
            showText={showText}
            showInitial={true}
            onClick={onClick}
            isActive={active}
          />
        );
      });
      setGroupItems(items);
    },
    [onClick, isActive, showText]
  );

  React.useEffect(() => {
    getItems(groups);
  }, [groups, getItems]);

  return (
    <>
      {!isVisitor &&
        (isArticleLoading ? (
          <Loaders.ArticleGroup />
        ) : (
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
            {groupItems}
          </div>
        ))}
    </>
  );
};

const BodyContent = withTranslation("Article")(withRouter(ArticleBodyContent));

export default inject(({ auth, peopleStore }) => {
  const { settingsStore, setDocumentTitle, isAdmin } = auth;
  const { customNames, showText, toggleArticleOpen } = settingsStore;
  const {
    groupsStore,
    selectedGroupStore,
    filterStore,
    loadingStore,
  } = peopleStore;
  const { filter } = filterStore;
  const { groups } = groupsStore;
  const { groupsCaption } = customNames;
  const { selectedGroup, selectGroup } = selectedGroupStore;
  const selectedKey = selectedGroup ? selectedGroup : "root";

  const { isLoading, isLoaded, firstLoad } = loadingStore;

  const isArticleLoading = (isLoading || !isLoaded) && firstLoad;
  return {
    setDocumentTitle,
    isArticleLoading,
    isVisitor: auth.userStore.user.isVisitor,
    isAdmin,
    groups,
    groupsCaption,
    selectedKey,
    selectGroup,

    filter,

    showText,
    toggleArticleOpen,
  };
})(observer(BodyContent));
