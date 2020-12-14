import React, { useEffect } from "react";
import { connect } from "react-redux";
import { utils, TreeMenu, TreeNode, Icons, Link } from "asc-web-components";
import {
  selectGroup,
  setIsVisibleDataLossDialog,
  setIsLoading,
} from "../../../store/people/actions";
import { getSelectedGroup } from "../../../store/people/selectors";
import { withTranslation, I18nextProvider } from "react-i18next";
import {
  history,
  utils as commonUtils,
  store as initStore,
  Loaders,
} from "asc-web-common";
import { createI18N } from "../../../helpers/i18n";
import styled, { css } from "styled-components";
import { setDocumentTitle } from "../../../helpers/utils";

const i18n = createI18N({
  page: "Article",
  localesPath: "Article",
});

const { changeLanguage } = commonUtils;
const { isAdmin } = initStore.auth.selectors;

const StyledTreeMenu = styled(TreeMenu)`
  ${(props) =>
    props.isAdmin &&
    css`
      margin-top: 19px;
    `}
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
              <Icons.DepartmentsGroupIcon
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
    return (
      <TreeNode
        className="inner-folder"
        key={item.key}
        title={item.title}
        icon={
          <Icons.CatalogFolderIcon size="scale" isfill={true} color="#657077" />
        }
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
    const { groups, selectedKeys } = this.props;

    const currentGroup = getSelectedGroup(
      groups,
      data ? data[0] : selectedKeys[0]
    );
    currentGroup ? setDocumentTitle(currentGroup.name) : setDocumentTitle();
  }
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
  onSelectHandler = (data) => {
    const { editingForm, setIsVisibleDataLossDialog } = this.props;

    if (editingForm.isEdit) {
      setIsVisibleDataLossDialog(true, this.onSelect(data));
    } else {
      this.onSelect(data)();
    }
  };
  onSelect = (data) => {
    const { setIsLoading } = this.props
    return () => {
      const { selectGroup } = this.props;
      setIsLoading(true);
      this.changeTitleDocument(data);
      selectGroup(
        data && data.length === 1 && data[0] !== "root" ? data[0] : null
      ).finally(() => setIsLoading(false))
    };
  };
  switcherIcon = (obj) => {
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
        }) || [],
    },
  ];

  return treeData;
};

const ArticleBodyContentWrapper = withTranslation()(ArticleBodyContent);

const BodyContent = (props) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  return (
    <I18nextProvider i18n={i18n}>
      <ArticleBodyContentWrapper {...props} />
    </I18nextProvider>
  );
};

function mapStateToProps(state) {
  const groups = state.people.groups;
  const { isLoaded, settings } = state.auth;
  const { customNames } = settings;
  const { groupsCaption } = customNames;
  const { editingForm } = state.people;

  return {
    data: getTreeGroups(groups, groupsCaption),
    selectedKeys: state.people.selectedGroup
      ? [state.people.selectedGroup]
      : ["root"],
    groups,
    isAdmin: isAdmin(state),
    isLoaded,
    editingForm,
  };
}

export default connect(mapStateToProps, {
  selectGroup,
  setIsVisibleDataLossDialog,
  setIsLoading,
})(BodyContent);
