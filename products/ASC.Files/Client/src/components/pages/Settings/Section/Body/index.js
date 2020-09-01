import React from  'react';
import styled from 'styled-components';
import { connect } from 'react-redux';
import { 
  Heading,
  ToggleButton 
} from 'asc-web-components';

import { 
  setUpdateIfExist, 
  setStoreOriginal, 
  setEnableThirdParty,
  setConfirmDelete, 
  setStoreForceSave,
  setSelectedNode,
  setForceSave
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
      displayNotification: true
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
    const { storeForceSave, setStoreForceSave } = this.props;
    setStoreForceSave( !storeForceSave, "storeForceSave" );
  }

  onChangeThirdParty = () => {
    const { enableThirdParty, setEnableThirdParty } = this.props;
    setEnableThirdParty(!enableThirdParty, "enableThirdParty");
  }

  renderAdminSettings = () => {

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
    const { storeOriginalFiles, setStoreOriginal } = this.props;
    setStoreOriginal( !storeOriginalFiles, "storeOriginalFiles" );
  }

  onChangeDeleteConfirm = () => {
    const { confirmDelete, setConfirmDelete } = this.props;
    setConfirmDelete( !confirmDelete, "confirmDelete" );
  }

  onChangeUpdateIfExist = () => {
    const { updateIfExist, setUpdateIfExist } = this.props;
    setUpdateIfExist( !updateIfExist, "updateIfExist" );
  }

  onChangeForceSave = () => {
    const { forceSave, setForceSave } = this.props;
    setForceSave( !forceSave, "forceSave" );
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
          onChange={this.onChangeForceSave}
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
    setUpdateIfExist, 
    setStoreOriginal, 
    setEnableThirdParty,
    setConfirmDelete,
    setStoreForceSave,
    setSelectedNode,
    setForceSave
  })
  (SectionBodyContent);