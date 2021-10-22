import React, { useEffect } from "react";
import { withRouter } from "react-router";
import TreeMenu from "@appserver/components/tree-menu";
import TreeNode from "@appserver/components/tree-menu/sub-components/tree-node";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import SettingsIcon from "../../../../../../../public/images/settings.react.svg";
import ExpanderDownIcon from "../../../../../../../public/images/expander-down.react.svg";
import ExpanderRightIcon from "../../../../../../../public/images/expander-right.react.svg";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import config from "../../../../package.json";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import Loaders from "@appserver/common/components/Loaders";
import withLoader from "../../../HOCs/withLoader";

const StyledTreeMenu = styled(TreeMenu)`
  margin-top: 18px !important;
  @media (max-width: 1024px) {
    margin-top: 14px !important;
  }

  .rc-tree-node-content-wrapper {
    width: 98% !important;
  }

  .rc-tree-node-selected {
    background: #dfe2e3 !important;
  }

  .rc-tree-treenode-disabled > span:not(.rc-tree-switcher),
  .rc-tree-treenode-disabled > a,
  .rc-tree-treenode-disabled > a span {
    cursor: wait;
  }

  .rc-tree-child-tree .rc-tree-node-content-wrapper > .rc-tree-title {
    width: 99% !important;
    padding-left: 4px !important;
  }

  .rc-tree-child-tree span.rc-tree-node-selected {
    max-width: 106%;
  }

  .rc-tree-child-tree {
    margin-left: 22px;
  }

  @media (max-width: 1024px) {
    .settings-node {
      margin-left: 18px !important;
    }
  }
`;
const StyledExpanderDownIcon = styled(ExpanderDownIcon)`
  ${commonIconsStyles}
  path {
    fill: dimgray;
  }
`;
const StyledExpanderRightIcon = styled(ExpanderRightIcon)`
  ${commonIconsStyles}
  path {
    fill: dimgray;
  }
`;
const StyledSettingsIcon = styled(SettingsIcon)`
  ${commonIconsStyles}
  path {
    fill: dimgray;
  }
`;
const PureTreeSettings = ({
  match,
  enableThirdParty,
  isAdmin,
  selectedTreeNode,
  expandedSetting,
  isLoading,
  setSelectedNode,
  setExpandSettingsTree,

  setSelectedFolder,
  //selectedFolder,
  history,
  setIsLoading,
  t,
  isVisitor,
  isDesktop,
}) => {
  const { setting } = match.params;

  useEffect(() => {
    setIsLoading(true);
    setSelectedNode([setting]);
    setIsLoading(false);
  }, [setting, setIsLoading, setSelectedNode]);

  useEffect(() => {
    const { setting } = match.params;
    if (setting && !expandedSetting) setExpandSettingsTree(["settings"]);
  }, [match, expandedSetting, setExpandSettingsTree]);

  const switcherIcon = (obj) => {
    if (obj.isLeaf) {
      return null;
    }
    if (obj.expanded) {
      return <StyledExpanderDownIcon size="scale" />;
    } else {
      return <StyledExpanderRightIcon size="scale" />;
    }
  };

  const onSelect = (section) => {
    const path = section[0];

    //if (selectedFolder) setSelectedFolder({});
    setSelectedFolder(null); //getSelectedTreeNode

    if (path === "settings") {
      setSelectedNode(["common"]);
      if (!expandedSetting || expandedSetting[0] !== "settings")
        setExpandSettingsTree(section);
      return history.push(
        combineUrl(
          AppServerConfig.proxyURL,
          config.homepage,
          "/settings/common"
        )
      );
    }

    if (selectedTreeNode[0] !== path) {
      setSelectedNode(section);
      return history.push(
        combineUrl(
          AppServerConfig.proxyURL,
          config.homepage,
          `/settings/${path}`
        )
      );
    }
  };

  const onExpand = (data) => {
    setExpandSettingsTree(data);
  };

  const renderTreeNode = () => {
    return (
      <TreeNode
        id="settings"
        key="settings"
        title={t("Common:Settings")}
        isLeaf={false}
        icon={<StyledSettingsIcon size="scale" />}
      >
        <TreeNode
          className="settings-node"
          id="common-settings"
          key="common"
          isLeaf={true}
          title={t("CommonSettings")}
        />
        {isAdmin ? (
          <TreeNode
            className="settings-node"
            id="admin-settings"
            key="admin"
            isLeaf={true}
            title={t("Common:AdminSettings")}
          />
        ) : null}
        {enableThirdParty && !isVisitor && !isDesktop ? (
          <TreeNode
            selectable={true}
            className="settings-node"
            id="connected-clouds"
            key="thirdParty"
            isLeaf={true}
            title={t("ThirdPartySettings")}
          />
        ) : null}
      </TreeNode>
    );
  };

  const nodes = renderTreeNode();

  return (
    <StyledTreeMenu
      expandedKeys={expandedSetting}
      selectedKeys={selectedTreeNode}
      defaultExpandParent={false}
      disabled={isLoading}
      className="settings-tree-menu"
      switcherIcon={switcherIcon}
      onSelect={onSelect}
      showIcon={true}
      onExpand={onExpand}
      isFullFillSelection={false}
      gapBetweenNodes="22"
      gapBetweenNodesTablet="26"
    >
      {nodes}
    </StyledTreeMenu>
  );
};

const TreeSettings = withTranslation(["Settings", "Common"])(
  withRouter(withLoader(PureTreeSettings)(<></>))
);

export default inject(
  ({
    auth,
    filesStore,
    settingsStore,
    treeFoldersStore,
    selectedFolderStore,
  }) => {
    const { setIsLoading, isLoading } = filesStore;
    const { setSelectedFolder } = selectedFolderStore;
    const { selectedTreeNode, setSelectedNode } = treeFoldersStore;
    const {
      enableThirdParty,
      expandedSetting,
      setExpandSettingsTree,
    } = settingsStore;

    return {
      isAdmin: auth.isAdmin,
      isVisitor: auth.userStore.user.isVisitor,
      isLoading,
      selectedTreeNode,
      enableThirdParty,
      expandedSetting,

      setIsLoading,
      setSelectedFolder,
      setSelectedNode,
      setExpandSettingsTree,
      isDesktop: auth.settingsStore.isDesktopClient,
    };
  }
)(observer(TreeSettings));
