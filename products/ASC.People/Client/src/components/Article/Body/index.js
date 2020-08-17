import React from "react";
import { connect } from "react-redux";
import { utils, TreeMenu, TreeNode, Icons, Link } from "asc-web-components";
import { history } from "asc-web-common";
import { selectGroup } from "../../../store/people/actions";
import { getSelectedGroup } from "../../../store/people/selectors";
import { withTranslation, I18nextProvider } from "react-i18next";
import { utils as commonUtils } from "asc-web-common";

import { createI18N } from "../../../helpers/i18n";
const i18n = createI18N({
  page: "Article",
  localesPath: "Article"
});

const { changeLanguage } = commonUtils;

const getItems = data => {
  return data.map(item => {
    if (item.children && item.children.length) {
      return (
        <TreeNode
          title={item.title}
          key={item.key}
          icon={
            item.root ? (
              <Icons.CatalogDepartmentsIcon
                size="scale"
                isfill={true}
                color="#657077"
              />
            ) : (
              ""
            )
          }
        >
          {getItems(item.children)}
        </TreeNode>
      );
    }
    return <TreeNode key={item.key} title={item.title} />;
  });
};

class ArticleBodyContent extends React.Component {
  shouldComponentUpdate(nextProps) {
    if (
      !utils.array.isArrayEqual(nextProps.selectedKeys, this.props.selectedKeys)
    ) {
      return true;
    }

    if (!utils.array.isArrayEqual(nextProps.data, this.props.data)) {
      return true;
    }

    return false;
  }

  onSelect = data => {
    const { selectGroup, groups, t } = this.props;

    const currentGroup = getSelectedGroup(groups, data[0]);
    document.title = currentGroup
      ? `${currentGroup.name} – ${t("People")}`
      : `${t("People")} – ${t("OrganizationName")}`;
    selectGroup(
      data && data.length === 1 && data[0] !== "root" ? data[0] : null
    );
  };

  switcherIcon = obj => {
    if (obj.isLeaf) {
      return null;
    }
    if (obj.expanded) {
      return (
        <Icons.ExpanderDownIcon size="scale" isfill={true} color="dimgray" />
      );
    } else {
      return (
        <Icons.ExpanderRightIcon size="scale" isfill={true} color="dimgray" />
      );
    }
  };

  render() {
    const { data, selectedKeys } = this.props;

    //console.log("PeopleTreeMenu", this.props);
    return (
      <TreeMenu
        className="people-tree-menu"
        checkable={false}
        draggable={false}
        disabled={false}
        multiple={false}
        showIcon={true}
        defaultExpandAll={true}
        switcherIcon={this.switcherIcon}
        onSelect={this.onSelect}
        selectedKeys={selectedKeys}
      >
        {getItems(data)}
      </TreeMenu>
    );
  }
}

const getTreeGroups = (groups, departments) => {
  const linkProps = { fontSize: "14px", fontWeight: 600, noHover: true };
  const link = history.location.search.slice(1);
  let newLink = link.split("&");
  const index = newLink.findIndex(x => x.includes("group"));
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
        groups.map(g => {
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
            root: false
          };
        }) || []
    }
  ];

  return treeData;
};

const ArticleBodyContentWrapper = withTranslation()(ArticleBodyContent);

const BodyContent = props => {
  changeLanguage(i18n);
  return (
    <I18nextProvider i18n={i18n}>
      <ArticleBodyContentWrapper {...props} />
    </I18nextProvider>
  );
};

function mapStateToProps(state) {
  const groups = state.people.groups;

  return {
    data: getTreeGroups(groups, state.auth.settings.customNames.groupsCaption),
    selectedKeys: state.people.selectedGroup
      ? [state.people.selectedGroup]
      : ["root"],
    groups
  };
}

export default connect(
  mapStateToProps,
  { selectGroup }
)(BodyContent);
