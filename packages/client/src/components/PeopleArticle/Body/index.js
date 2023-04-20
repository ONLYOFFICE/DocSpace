import DepartmentsGroupReactSvgUrl from "PUBLIC_DIR/images/departments.group.react.svg?url";
import CatalogFolderReactSvgUrl from "PUBLIC_DIR/images/catalog.folder.react.svg?url";
import React from "react";
import { withTranslation } from "react-i18next";
import Filter from "@docspace/common/api/people/filter";
import Loaders from "@docspace/common/components/Loaders";
import { inject, observer } from "mobx-react";
import { getSelectedGroup } from "../../../helpers/people-helpers";
import { useNavigate } from "react-router-dom";
import { isMobile } from "@docspace/components/utils/device";
import { isMobileOnly } from "react-device-detect";
import config from "PACKAGE_FILE";
import { combineUrl } from "@docspace/common/utils";
import CatalogItem from "@docspace/components/catalog-item";
import withLoader from "../../../HOCs/withLoader";

const departmentsIcon = DepartmentsGroupReactSvgUrl;
const groupIcon = CatalogFolderReactSvgUrl;

const ArticleBodyContent = ({
  selectedKey,
  groups,
  toggleArticleOpen,
  showText,
  groupsCaption,

  filter,
  selectGroup,
  isVisitor,
  isAdmin,
  setDocumentTitle,
}) => {
  const [groupItems, setGroupItems] = React.useState(null);

  const navigate = useNavigate();

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

      if (window.location.pathname.indexOf("/accounts/filter") > 0) {
        selectGroup(groupId);
        if (isMobileOnly || isMobile()) toggleArticleOpen();
      } else {
        const newFilter = isRoot ? Filter.getDefault() : filter.clone();

        if (!isRoot) newFilter.group = groupId;

        const urlFilter = newFilter.toUrlParams();
        const url = combineUrl(
          window.DocSpaceConfig?.proxy?.url,
          config.homepage,
          `/accounts/filter?${urlFilter}`
        );
        navigate(url);
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
          {groupItems}
        </div>
      )}
    </>
  );
};

const BodyContent = withTranslation(["Article"])(
  withLoader(ArticleBodyContent)(<Loaders.ArticleFolder />)
);

export default inject(({ auth, peopleStore }) => {
  const { settingsStore, setDocumentTitle, isAdmin } = auth;
  const { customNames, showText, toggleArticleOpen } = settingsStore;
  const { groupsStore, selectedGroupStore, filterStore } = peopleStore;
  const { filter } = filterStore;
  const { groups } = groupsStore;
  const { groupsCaption } = customNames;
  const { selectedGroup, selectGroup } = selectedGroupStore;
  const selectedKey = selectedGroup ? selectedGroup : "root";

  return {
    setDocumentTitle,

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
