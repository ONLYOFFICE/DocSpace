import React from "react";
import styled, { css } from "styled-components";
import { withTranslation, I18nextProvider } from "react-i18next";

import { isArrayEqual } from "@appserver/components/utils/array";
import TreeMenu from "@appserver/components/tree-menu";
import TreeNode from "@appserver/components/tree-menu/sub-components/tree-node";
import Link from "@appserver/components/link";
// import { withTranslation } from "react-i18next";
import history from "@appserver/common/history";
import Loaders from "@appserver/common/components/Loaders";
import CatalogFolderIcon from "../../../../../../../public/images/catalog.folder.react.svg";
import DepartmentsGroupIcon from "../../../../public/images/departments.group.react.svg";
import ExpanderDownIcon from "../../../../../../../public/images/expander-down.react.svg";
import ExpanderRightIcon from "../../../../../../../public/images/expander-right.react.svg";
// import { createI18N } from "../../../helpers/i18n";

// import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";
import { getSelectedGroup } from "../../../helpers/people-helpers";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";

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
    fill: ${(props) => props.color};
  }
`;
const StyledDepartmentsGroupIcon = styled(DepartmentsGroupIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.color};
  }
`;
const StyledExpanderDownIcon = styled(ExpanderDownIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.color};
  }
`;
const StyledExpanderRightIcon = styled(ExpanderRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.color};
  }
`;
const getItems = (data) => {
  return data.map((item) => {
    if (item.children) {
      return (
        <TreeNode
          className="root-folder"
          title={item.title}
          key={item.key}
          icon={
            item.root ? (
              <StyledDepartmentsGroupIcon size="scale" color="#657077" /* isfill={true} */ /> // TODO: Add isFill prop if need
              // <DepartmentsGroupIcon
              //   size="scale"
              //   isfill={true}
              //   color="#657077"
              // />
            ) : (
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
        icon={<StyledCatalogFolderIcon size="scale" color="#657077" />}
      />
    );
  });
};

class ArticleBodyContent extends React.Component {
  componentDidMount() {
    this.changeTitleDocument();
  }

  componentDidUpdate(prevProps) {
    if (prevProps.selectedKeys[0] !== this.props.selectedKeys[0]) {
      this.changeTitleDocument();
    }
  }

  changeTitleDocument(data = null) {
    const { groups, selectedKeys, setDocumentTitle } = this.props;

    const currentGroup = getSelectedGroup(
      groups,
      data ? data[0] : selectedKeys[0]
    );
    currentGroup ? setDocumentTitle(currentGroup.name) : setDocumentTitle();
  }
  shouldComponentUpdate(nextProps) {
    if (!isArrayEqual(nextProps.selectedKeys, this.props.selectedKeys)) {
      return true;
    }

    if (!isArrayEqual(nextProps.data, this.props.data)) {
      return true;
    }

    return false;
  }
  onSelectHandler = (data) => {
    const { isEdit, setIsVisibleDataLossDialog } = this.props;

    if (isEdit) {
      setIsVisibleDataLossDialog(true, this.onSelect(data));
    } else {
      this.onSelect(data)();
    }
  };
  onSelect = (data) => {
    const { setIsLoading } = this.props;
    return () => {
      const { selectGroup } = this.props;
      const groupId =
        data && data.length === 1 && data[0] !== "root" ? data[0] : null;
      setIsLoading(true);
      this.changeTitleDocument(data);
      selectGroup(groupId);
      setIsLoading(false);
    };
  };
  switcherIcon = (obj) => {
    if (obj.isLeaf) {
      return null;
    }
    if (obj.expanded) {
      return <StyledExpanderDownIcon size="scale" color="dimgray" />;
    } else {
      return <StyledExpanderRightIcon size="scale" color="dimgray" />;
    }
  };

  render() {
    const { isLoaded, data, selectedKeys, isAdmin } = this.props;

    //console.log("PeopleTreeMenu", this.props);
    return !isLoaded ? (
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
    );
  }
}

const getTreeGroups = (groups, departments) => {
  const linkProps = { fontSize: "14px", fontWeight: 600, noHover: true };
  const link = history.location.search.slice(1);
  let newLink = link.split("&");
  const index = newLink.findIndex((x) => x.includes("group"));
  index && newLink.splice(1, 1);
  newLink = newLink.join("&");

  const onTitleClick = () => {
    history.push("/products/people/");
  };

  const treeData = [
    {
      key: "root",
      title: (
        <Link
          {...linkProps}
          onClick={onTitleClick}
          href={`${history.location.pathname}`}
        >
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

const BodyContent = withTranslation("Article")(ArticleBodyContent);

export default inject(({ auth, peopleStore }) => {
  const groups = peopleStore.groupsStore.groups;
  const { groupsCaption } = auth.settingsStore.customNames;
  const data = getTreeGroups(groups, groupsCaption);
  const selectedKeys = peopleStore.selectedGroupStore.selectedGroup
    ? [peopleStore.selectedGroupStore.selectedGroup]
    : ["root"];
  return {
    setDocumentTitle: auth.setDocumentTitle,
    isLoaded: auth.isLoaded,
    isAdmin: auth.isAdmin,
    groups,
    data,
    selectedKeys,
    selectGroup: peopleStore.selectedGroupStore.selectGroup,
    isEdit: peopleStore.editingFormStore.isEdit,
    setIsVisibleDataLossDialog:
      peopleStore.editingFormStore.setIsVisibleDataLossDialog,
    setIsLoading: peopleStore.setIsLoading,
  };
})(observer(BodyContent));
