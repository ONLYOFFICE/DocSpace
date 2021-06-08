import React from "react";
import { withRouter } from "react-router";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import TreeMenu from "@appserver/components/tree-menu";
import TreeNode from "@appserver/components/tree-menu/sub-components/tree-node";
import ExpanderDownIcon from "../../../../../../../../../public/images/expander-down.react.svg";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import { isArrayEqual } from "@appserver/components/utils/array";
import ExpanderRightIcon from "../../../../../../../../../public/images/expander-right.react.svg";
import {
  //getKeyByLink,
  settingsTree,
  getSelectedLinkByKey,
  //selectKeyOfTreeElement,
  getCurrentSettingsCategory,
} from "../../../utils";
import { ReactSVG } from "react-svg";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import { clickBackdrop } from "@appserver/common/utils";
import { tablet } from "@appserver/components/utils/device";

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

  span.rc-tree-node-content-wrapper:not(.rc-tree-node-content-wrapper-open) {
    width: unset;
    padding: 0 4px 0 4px !important;
    max-width: 98%;
    @media ${tablet} {
      margin-left: 24px;
    }
  }

  .tree_icon {
    display: inline;
    path {
      fill: dimgray;
    }
  }
  svg {
    &:not(:root) {
      width: 100%;
      height: 100%;
    }
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
const getTreeItems = (data, path, t) => {
  const maptKeys = (tKey) => {
    switch (tKey) {
      case "AccessRights":
        return t("AccessRights");
      case "ManagementCategoryCommon":
        return t("ManagementCategoryCommon");
      case "Customization":
        return t("Customization");
      case "StudioTimeLanguageSettings":
        return t("StudioTimeLanguageSettings");
      case "CustomTitles":
        return t("CustomTitles");
      case "TeamTemplate":
        return t("TeamTemplate");
      case "ManagementCategorySecurity":
        return t("ManagementCategorySecurity");
      case "PortalAccess":
        return t("PortalAccess");
      case "TwoFactorAuth":
        return t("TwoFactorAuth");
      case "ManagementCategoryIntegration":
        return t("ManagementCategoryIntegration");
      case "ThirdPartyAuthorization":
        return t("ThirdPartyAuthorization");
      case "Migration":
        return t("Migration");
      case "Backup":
        return t("Backup");
      default:
        throw new Error("Unexpected translation key");
    }
  };
  return data.map((item) => {
    if (item.children && item.children.length && !item.isCategory) {
      return (
        <TreeNode
          title={
            <Text className="inherit-title-link header">
              {maptKeys(item.tKey)}
            </Text>
          }
          key={item.key}
          icon={item.icon && <ReactSVG className="tree_icon" src={item.icon} />}
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
            {maptKeys(item.tKey)}
          </Link>
        }
        icon={item.icon && <ReactSVG src={item.icon} className="tree_icon" />}
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
    if (!isArrayEqual(prevState.selectedKeys, this.state.selectedKeys)) {
      const { selectedKeys } = this.state;
      const { match, history } = this.props;
      const settingsPath = getSelectedLinkByKey(selectedKeys[0], settingsTree);
      const newPath = match.path + settingsPath;
      history.push(newPath);
    }
  }

  onSelect = (value) => {
    const { selectedKeys } = this.state;

    if (isArrayEqual(value, selectedKeys)) {
      return;
    }

    this.setState({ selectedKeys: value });
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

export default withRouter(withTranslation("Settings")(ArticleBodyContent));
