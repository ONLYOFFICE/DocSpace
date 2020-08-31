import React from  'react';
import styled from 'styled-components';
import { connect } from 'react-redux';
import { 
  Heading,
  ToggleButton 
} from 'asc-web-components';

import { 
  updateIfExist, 
  storeOriginal, 
  setThirdParty,
  changeDeleteConfirm, 
  storeForceSave,
  setSelectedNode
} from '../../../../../store/files/actions';

const StyledSettings = styled.div`
  display: grid;
  grid-gap: 10px;

  .toggle-btn {
    display: block;
    position: relative;
  }

  .heading {
    margin-bottom: 0;
    margin-top: 14px;
  }
`;

class SectionBodyContent extends React.Component {
  constructor(props) {
    super(props);

    this.state = { 
      originalCopy: true,
      updateExist: true,
      displayNotification: true,
      intermediateVersion: true
    }
  }

  componentDidMount() {
    const { setting, onLoading, t } = this.props;
    document.title = t(`${setting}`);
    onLoading(false);
  }

  componentDidUpdate(prevProps) {
    const { setting, t, setSelectedNode, selectedTreeNode } = this.props;
    document.title = t(`${setting}`); 
    if(prevProps.setting !== setting && setting !== selectedTreeNode[0]) {
      setSelectedNode([ setting ])
    }
  }

  componentWillUnmount() {
    document.title = 'ASC.Files';
  }

  onChangeStoreForceSave = () => {
    const { intermediateVersion } = this.state;
    const { storeForceSave } = this.props;

    storeForceSave( !intermediateVersion );
    this.setState({ intermediateVersion: !intermediateVersion });
  }

  onChangeThirdParty = () => {
    const { enableThirdParty, setThirdParty } = this.props;
    setThirdParty(!enableThirdParty);
  }

  renderAdminSettings = () => {
    const { intermediateVersion } = this.state;

    const {
      enableThirdParty,
      storeForceSave,
      t
    } = this.props;

    return (
      <StyledSettings>
        <ToggleButton 
          className="toggle-btn"
          label={t('intermediateVersion')}
          onChange={this.onChangeStoreForceSave}
          isChecked={storeForceSave}
        />
        <ToggleButton
          className="toggle-btn"
          label={t('thirdPartyBtn')}
          onChange={this.onChangeThirdParty}
          isChecked={enableThirdParty}
        />
      </StyledSettings>
    )
  }

  onChangeOriginalCopy = () => {
    const { originalCopy } = this.state;
    const { storeOriginal } = this.props;

    storeOriginal( originalCopy );
    this.setState({ originalCopy: !originalCopy });
  }

  onChangeUpdateIfExist = () => {
    const { updateExist } = this.state;
    const { updateIfExist } = this.props;

    updateIfExist( !updateExist );
    this.setState({ updateExist: !updateExist });
  }

  onChangeDeleteConfirm = () => {
    const { displayNotification } = this.state;
    const { changeDeleteConfirm } = this.props;

    changeDeleteConfirm( !displayNotification );
    this.setState({ displayNotification: !displayNotification });
  }

  renderCommonSettings = () => {
    const {
      recent,
      favorites,
      templates,
      updateIfExist,
      confirmDelete,
      storeOriginalFiles,
      forceSave,
      t
    } = this.props;

    return (
      <StyledSettings>
        <ToggleButton
          className="toggle-btn"
          label={t('originalCopy')}
          onChange={this.onChangeOriginalCopy}
          isChecked={storeOriginalFiles}
        />
        <ToggleButton
          className="toggle-btn"
          label={t('displayNotification')}
          onChange={this.onChangeDeleteConfirm}
          isChecked={confirmDelete}
        />
        <ToggleButton
          isDisabled={true}
          className="toggle-btn"
          label={t('displayRecent')}
          onChange={(e)=>console.log(e)}
          isChecked={recent}
        />
        <ToggleButton
          isDisabled={true}
          className="toggle-btn"
          label={t('displayFavorites')}
          onChange={(e)=>console.log(e)}
          isChecked={favorites}
        />
        <ToggleButton
          isDisabled={true}
          className="toggle-btn"
          label={t('displayTemplates')}
          onChange={(e)=>console.log(e)}
          isChecked={templates}
        />
        <Heading className="heading" level={2} size="small">{t('storingFileVersion')}</Heading>
        <ToggleButton
          className="toggle-btn"
          label={t('updateOrCreate')}
          onChange={this.onChangeUpdateIfExist}
          isChecked={updateIfExist}
        />
        <ToggleButton
          className="toggle-btn"
          label={t('keepIntermediateVersion')}
          onChange={(e)=>console.log(e)}
          isChecked={forceSave}
        />
      </StyledSettings>
    );
  }

  renderClouds = () => {
    return (<></>)
  }

  render() {
    const { setting, enableThirdParty, isAdmin } = this.props;
    let content;

    if(setting === 'admin' && isAdmin)
      content = this.renderAdminSettings();
    if(setting === 'common') 
      content = this.renderCommonSettings();
    if(setting === 'thirdParty' && enableThirdParty )
      content = this.renderClouds();
    return content; 
  }
}

function mapStateToProps(state) {
  const { settingsTree, selectedTreeNode } = state.files;
  const { isAdmin } = state.auth.user;
  const { 
    storeOriginalFiles,
    confirmDelete,
    updateIfExist,
    forceSave,
    storeForceSave,
    enableThirdParty
  } = settingsTree;

  return { 
    isAdmin,
    selectedTreeNode,
    storeOriginalFiles,
    confirmDelete,
    updateIfExist,
    forceSave,
    storeForceSave,
    enableThirdParty
  }
}

export default connect(
  mapStateToProps, { 
    updateIfExist, 
    storeOriginal, 
    setThirdParty,
    changeDeleteConfirm,
    storeForceSave,
    setSelectedNode
  })
  (SectionBodyContent);