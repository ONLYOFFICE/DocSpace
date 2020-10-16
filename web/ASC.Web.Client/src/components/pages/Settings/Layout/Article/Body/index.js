import React from "react";
import {
  TreeMenu,
  TreeNode,
  Icons,
  Link,
  Text,
  utils,
} from "asc-web-components";
import { withRouter } from "react-router";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import {
  getKeyByLink,
  settingsTree,
  getSelectedLinkByKey,
  selectKeyOfTreeElement,
  getCurrentSettingsCategory,
} from "../../../utils";

const StyledTreeMenu = styled(TreeMenu)`
  .inherit-title-link {
    font-size: inherit;
    font-weight: inherit;
    color: #657077;

    &.header {
      font-weight: bold;
      text-transform: uppercase;
      pointer-events: none;
    }
  }

  .rc-tree-node-content-wrapper-open {
    pointer-events: none;
  }
`;

const getTreeItems = (data, path, t) => {
  return data.map((item) => {
    if (item.children && item.children.length && !item.isCategory) {
      return (
        <TreeNode
          title={
            <Text className="inherit-title-link header">{t(item.tKey)}</Text>
          }
          key={item.key}
          icon={
            item.icon &&
            React.createElement(Icons[item.icon], {
              size: "scale",
              isfill: true,
              color: "dimgray",
            })
          }
          disableSwitch={true}
        >
          {getTreeItems(item.children, path, t)}
        </TreeNode>
      );
    }
    const link = path + getSelectedLinkByKey(item.key, settingsTree);
    return (
      <TreeNode
        key={item.key}
        title={
          <Link className="inherit-title-link" href={link}>
            {t(item.tKey)}
          </Link>
        }
        icon={
          item.icon &&
          React.createElement(Icons[item.icon], {
            size: "scale",
            isfill: true,
            color: "dimgray",
          })
        }
        disableSwitch={true}
      />
    );
  });
};

class ArticleBodyContent extends React.Component {
  constructor(props) {
    super(props);

    const fullSettingsUrl = props.match.url;
    const locationPathname = props.location.pathname;

    const fullSettingsUrlLength = fullSettingsUrl.length;
    const resultPath = locationPathname.slice(fullSettingsUrlLength + 1);
    const arrayOfParams = resultPath.split("/");

    let link = "";
    const selectedItem = arrayOfParams[arrayOfParams.length - 1];
    if (
      selectedItem === "owner" ||
      selectedItem === "admins" ||
      selectedItem === "modules"
    ) {
      link = `/${resultPath}`;
    } else if (selectedItem === "accessrights") {
      link = `/${resultPath}/owner`;
    }
    const CurrentSettingsCategoryKey = getCurrentSettingsCategory(
      arrayOfParams,
      settingsTree
    );

    if (link === "") {
      link = getSelectedLinkByKey(CurrentSettingsCategoryKey, settingsTree);
    }

    this.state = {
      selectedKeys: [CurrentSettingsCategoryKey],
    };
  }

  componentDidUpdate(prevProps, prevState) {
    if (
      !utils.array.isArrayEqual(prevState.selectedKeys, this.state.selectedKeys)
    ) {
      const { selectedKeys } = this.state;
      const { match, history } = this.props;
      const settingsPath = getSelectedLinkByKey(selectedKeys[0], settingsTree);
      const newPath = match.path + settingsPath;
      history.push(newPath);
    }
  }

  onSelect = (value) => {
    const { selectedKeys } = this.state;

    if (utils.array.isArrayEqual(value, selectedKeys)) {
      return;
    }

    this.setState({ selectedKeys: value });
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
    const { selectedKeys } = this.state;
    const { match, t } = this.props;

    return (
      <StyledTreeMenu
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
        disableSwitch={true}
      >
        {getTreeItems(settingsTree, match.path, t)}
      </StyledTreeMenu>
    );
  }
}

export default withRouter(withTranslation()(ArticleBodyContent));
