import React from "react";
import styled, { css } from "styled-components";
import { withTranslation } from "react-i18next";
import Filter from "@appserver/common/api/people/filter";
import TreeMenu from "@appserver/components/tree-menu";
import TreeNode from "@appserver/components/tree-menu/sub-components/tree-node";
import Link from "@appserver/components/link";

import Loaders from "@appserver/common/components/Loaders";
import CatalogFolderIcon from "../../../../../../../public/images/catalog.folder.react.svg";
import DepartmentsGroupIcon from "../../../../public/images/departments.group.react.svg";
import ExpanderDownIcon from "../../../../../../../public/images/expander-down.react.svg";
import ExpanderRightIcon from "../../../../../../../public/images/expander-right.react.svg";
import { inject, observer } from "mobx-react";
import { getSelectedGroup } from "../../../helpers/people-helpers";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import { withRouter } from "react-router";
import config from "../../../../package.json";
import { clickBackdrop, combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import Base from "@appserver/components/themes/base";

const StyledTreeMenu = styled(TreeMenu)`
  ${(props) =>
    props.isAdmin &&
    css`
      margin-top: 19px;
    `}
`;

const StyledCatalogFolderIcon = styled(CatalogFolderIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.peopleArticleBody.iconColor};
  }
`;
StyledCatalogFolderIcon.defaultProps = { theme: Base };

const StyledDepartmentsGroupIcon = styled(DepartmentsGroupIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.peopleArticleBody.iconColor};
  }
`;
StyledDepartmentsGroupIcon.defaultProps = { theme: Base };

const StyledExpanderDownIcon = styled(ExpanderDownIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.peopleArticleBody.expanderColor};
  }
`;
StyledExpanderDownIcon.defaultProps = { theme: Base };

const StyledExpanderRightIcon = styled(ExpanderRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.peopleArticleBody.expanderColor};
  }
`;
StyledExpanderRightIcon.defaultProps = { theme: Base };
const getItems = (data) => {
  return data.map((item) => {
    if (item.children) {
      return (
        <TreeNode
          className="tree-root-folder"
          title={item.title}
          key={item.key}
          icon={
            item.root ? (
              <StyledDepartmentsGroupIcon size="scale" /* isfill={true} */ /> // TODO: Add isFill prop if need
            ) : (
              // <DepartmentsGroupIcon
              //   size="scale"
              //   isfill={true}
              //   color="#657077"
              // />
              ""
            )
          }
        >
          {getItems(item.children)}
        </TreeNode>
      );
    }
    return (
      <TreeNode
        className="inner-folder"
        key={item.key}
        title={item.title}
        icon={<StyledCatalogFolderIcon size="scale" />}
      />
    );
  });
};

class ArticleBodyContent extends React.Component {
  componentDidMount() {
    this.changeTitleDocument();
    this.props.setFirstLoad(false);
  }

  getTreeGroups = (groups, departments) => {
    const linkProps = { fontSize: "14px", fontWeight: 600, noHover: true };
    const { history } = this.props;
    const link = history.location.search.slice(1);
    let newLink = link.split("&");
    const index = newLink.findIndex((x) => x.includes("group"));
    index && newLink.splice(1, 1);
    newLink = newLink.join("&");

    const treeData = [
      {
        key: "root",
        title: (
          <Link {...linkProps} href={`${history.location.pathname}`}>
            {departments}
          </Link>
        ),
        root: true,
        children:
          (groups &&
            groups.map((g) => {
              return {
                key: g.id,
                title: (
                  <Link
                    {...linkProps}
                    data-id={g.id}
                    href={`${history.location.pathname}?group=${g.id}&${newLink}`}
                  >
                    {g.name}
                  </Link>
                ),
                root: false,
              };
            })) ||
          [],
      },
    ];

    return treeData;
  };

  componentDidUpdate(prevProps) {
    if (prevProps.selectedKeys[0] !== this.props.selectedKeys[0]) {
      this.changeTitleDocument();
    }
  }

  changeTitleDocument(id) {
    const { groups, selectedKeys, setDocumentTitle } = this.props;

    const currentGroup = getSelectedGroup(
      groups,
      id === "root" ? selectedKeys[0] : id
    );
    currentGroup ? setDocumentTitle(currentGroup.name) : setDocumentTitle();
  }

  onSelectHandler = (data) => {
    const { isEdit, setIsVisibleDataLossDialog } = this.props;

    if (isEdit) {
      setIsVisibleDataLossDialog(true, () => this.onSelect(data));
    } else {
      this.onSelect(data);
    }
  };
  onSelect = (data) => {
    //const { selectGroup } = this.props;
    const groupId = data[0];
    const isRoot = groupId === "root";
    //data && data.length === 1 && data[0] !== "root" ? data[0] : null;

    const { history, selectGroup } = this.props;

    this.changeTitleDocument(groupId);

    if (window.location.pathname.indexOf("/people/filter") > 0) {
      selectGroup(groupId);
    } else {
      const { filter } = this.props;
      const newFilter = isRoot ? Filter.getDefault() : filter.clone();

      if (!isRoot) newFilter.group = groupId;

      const urlFilter = newFilter.toUrlParams();
      const url = combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/filter?${urlFilter}`
      );
      history.push(url);
    }
  };

  switcherIcon = (obj) => {
    if (obj.isLeaf) {
      return null;
    }
    if (obj.expanded) {
      return <StyledExpanderDownIcon size="scale" />;
    } else {
      return <StyledExpanderRightIcon size="scale" />;
    }
  };

  render() {
    const {
      isLoaded,
      groups,
      groupsCaption,
      selectedKeys,
      isAdmin,
      isVisitor,
    } = this.props;

    const data = this.getTreeGroups(groups, groupsCaption);

    //console.log("PeopleTreeMenu", this.props);
    return (
      !isVisitor &&
      (!isLoaded ? (
        <Loaders.TreeFolders />
      ) : (
        <StyledTreeMenu
          className="people-tree-menu"
          checkable={false}
          draggable={false}
          disabled={false}
          multiple={false}
          showIcon={true}
          defaultExpandAll={true}
          switcherIcon={this.switcherIcon}
          onSelect={this.onSelectHandler}
          selectedKeys={selectedKeys}
          isFullFillSelection={false}
          gapBetweenNodes="22"
          gapBetweenNodesTablet="26"
          isEmptyRootNode={getItems(data).length > 0}
          isAdmin={isAdmin}
        >
          {getItems(data)}
        </StyledTreeMenu>
      ))
    );
  }
}

const BodyContent = withTranslation("Article")(withRouter(ArticleBodyContent));

export default inject(({ auth, peopleStore }) => {
  const { settingsStore, isLoaded, setDocumentTitle, isAdmin } = auth;
  const { customNames } = settingsStore;
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
  const { setFirstLoad, isLoading } = loadingStore;
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
    filter,
    setFirstLoad,
  };
})(observer(BodyContent));
