import React from 'react';
import { utils } from 'asc-web-components';
import { connect } from 'react-redux';
import {
  TreeMenu,
  TreeNode,
  Icons,
  Link
} from "asc-web-components";
import { withRouter } from "react-router";
import styled from 'styled-components';
import { withTranslation } from 'react-i18next';
import { getKeyByLink, settingsTree } from '../../../utils';

const StyledTreeMenu = styled(TreeMenu)`
  .inherit-title-link {
    & > span {
      font-size: inherit;
      font-weight: inherit;
    }
  }
`;

const getTreeItems = (data, path, t) => {
  return data.map(item => {
    if (item.children && item.children.length) {
      const link = path + getSelectedLinkByKey(item.key);
      const tName = `Settings_${item.link}`;
      return (
        <TreeNode
          title={<Link className='inherit-title-link' href={link}>{t(tName)}</Link>}
          key={item.key}
          icon={item.icon && React.createElement(Icons[item.icon], {
            size: 'scale',
            isfill: true,
            color: 'dimgray',
          })}
        >
          {getTreeItems(item.children, path, t)}
        </TreeNode>
      );
    };
    const link = path + getSelectedLinkByKey(item.key);
    const tName = `Settings_${item.link}`;
    return (
      <TreeNode
        key={item.key}
        title={<Link className='inherit-title-link' href={link}>{t(tName)}</Link>}
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

    const { match, history,  i18n, language  } = props;
    const fullSettingsUrl = props.match.url;
    const locationPathname = props.location.pathname;
    i18n.changeLanguage(language);

    if (locationPathname === fullSettingsUrl) {
      const newPath = match.path + getSelectedLinkByKey('0-0');
      history.push(newPath);
      return;
    }

    const fullSettingsUrlLength = fullSettingsUrl.length;

    const resultPath = locationPathname.slice(fullSettingsUrlLength + 1);
    const arrayOfParams = resultPath.split('/');

    const key = getKeyByLink(settingsTree, arrayOfParams);
    const link = getSelectedLinkByKey(key);

    const path = match.path + link;
    history.push(path);

    this.state = {
      selectedKeys: [key]
    };

  }

  componentDidUpdate() {
    const {selectedKeys } =  this.state;
    const { match, history } = this.props;
    const settingsPath = getSelectedLinkByKey(selectedKeys[0]);
    const newPath = match.path + settingsPath;
    history.push(newPath);
  }

  shouldComponentUpdate(nextProps, nextState) {
    if (!utils.array.isArrayEqual(nextState.selectedKeys, this.state.selectedKeys)) {
      return true;
    }

    return false;
  }

  onSelect = value => {
    const {selectedKeys } =  this.state;

    if (value) {
      if (utils.array.isArrayEqual(value, selectedKeys)) {

        return;
      }
      const selectedKey = value[0];
      if (selectedKey.length === 3) {
        this.setState({selectedKeys: value});
      }
      else if (selectedKey.length === 1 && (selectedKey.toString() !== selectedKeys.toString()[0] || selectedKeys.toString()[2] !== '0')) {
        const selectedKeys = settingsTree[value].children ? [`${value.toString()}-0`] : value;
        this.setState({selectedKeys});
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
    const {selectedKeys } =  this.state;
    const { match, t } = this.props;

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
        {getTreeItems(settingsTree, match.path, t)}
      </StyledTreeMenu>
    );
  };
};

function mapStateToProps(state) {
  return {
    language: state.auth.user.cultureName,
  };
}

export default connect(mapStateToProps)(withRouter(withTranslation()(ArticleBodyContent)));