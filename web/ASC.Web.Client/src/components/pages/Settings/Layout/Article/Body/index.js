import React from 'react';
import { utils } from 'asc-web-components';
import { connect } from 'react-redux';
import {
  TreeMenu,
  TreeNode,
  Icons,
  Link
} from "asc-web-components";
import { setNewSelectedNode } from '../../../../../../store/auth/actions';
import { withRouter } from "react-router";
import { settingsTree } from '../../../../../../helpers/constants';
import styled from 'styled-components';
import { withTranslation } from 'react-i18next';
import { getKeyByLink } from '../../../utils';

const StyledTreeMenu = styled(TreeMenu)`
  .inherit-title-link {
    & > span {
      font-size: inherit;
      font-weight: inherit;
    }
  }
`;

const getItems = (data, path, t) => {
  return data.map(item => {
    if (item.children && item.children.length) {
      const link = path + getSelectedLinkByKey(item.key);
      return (
        <TreeNode
          title={<Link className='inherit-title-link' href={link}>{t(`Settings_${item.link}`)}</Link>}
          key={item.key}
          icon={item.icon && React.createElement(Icons[item.icon], {
            size: 'scale',
            isfill: true,
            color: 'dimgray',
          })}
        >
          {getItems(item.children, path, t)}
        </TreeNode>
      );
    };
    const link = path + getSelectedLinkByKey(item.key);
    return (
      <TreeNode
        key={item.key}
        title={<Link className='inherit-title-link' href={link}>{t(`Settings_${item.link}`)}</Link>}
        icon={item.icon && React.createElement(Icons[item.icon], {
          size: 'scale',
          isfill: true,
          color: 'dimgray',
        })}
      />
    );
  });
};

const getSelectedLinkByKey = key => {
  const length = key.length;
  if (length === 1) {
    return '/' + settingsTree[key].link;
  }
  else if (length === 3) {
    return '/' + settingsTree[key[0]].link + '/' + settingsTree[key[0]].children[key[2]].link;
  }
}

class ArticleBodyContent extends React.Component {

  constructor(props) {
    super(props);

    const { match, history, setNewSelectedNode, i18n, language } = props;
    const fullSettingsUrl = props.match.url;
    const locationPathname = props.location.pathname;
    i18n.changeLanguage(language);

    const fullSettingsUrlLength = fullSettingsUrl.length;

    const resultPath = locationPathname.slice(fullSettingsUrlLength + 1);
    const arrayOfParams = resultPath.split('/');

    const key = getKeyByLink(settingsTree, arrayOfParams);
    const link = getSelectedLinkByKey(key);

    setNewSelectedNode([key]);
    const path = match.path + link;
    history.push(path);

  }

  componentDidUpdate() {
    const { selectedKeys, match, history } = this.props;
    const settingsPath = getSelectedLinkByKey(selectedKeys[0]);
    const newPath = match.path + settingsPath;
    history.push(newPath);
  }

  shouldComponentUpdate(nextProps) {
    if (!utils.array.isArrayEqual(nextProps.selectedKeys, this.props.selectedKeys)) {
      return true;
    }

    return false;
  }

  onSelect = value => {
    const { selectedKeys, setNewSelectedNode } = this.props;

    if (value) {
      if (utils.array.isArrayEqual(value, selectedKeys)) {

        return;
      }
      const selectedKey = value[0];
      if (selectedKey.length === 3) {
        setNewSelectedNode(value);
      }
      else if (selectedKey.length === 1 && (selectedKey.toString() !== selectedKeys.toString()[0] || selectedKeys.toString()[2] !== '0')) {
        const selectedKeys = settingsTree[value].children ? [`${value.toString()}-0`] : value;
        setNewSelectedNode(selectedKeys);
      }
    }
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
    const { selectedKeys, match, t } = this.props;

    console.log("SettingsTreeMenu", this.props);

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
      >
        {getItems(settingsTree, match.path, t)}
      </StyledTreeMenu>
    );
  };
};

function mapStateToProps(state) {
  return {
    selectedKeys: state.auth.settings.settingsTree.selectedKey,
    language: state.auth.user.cultureName,
    ownerId: state.auth.settings.ownerId,
  };
}

export default connect(mapStateToProps, { setNewSelectedNode })(withRouter(withTranslation()(ArticleBodyContent)));