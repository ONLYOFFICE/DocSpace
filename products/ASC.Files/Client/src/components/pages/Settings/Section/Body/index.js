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
  storeForceSave
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

  componentDidUpdate() {
    const { setting, t } = this.props;
    document.title = t(`${setting}`);
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
    const { thirdParty, setThirdParty } = this.props;
    setThirdParty(!thirdParty);
  }

  renderAdminSettings = () => {
    const { intermediateVersion } = this.state;

    const {
      thirdParty,
      t
    } = this.props;

    return (
      <StyledSettings>
        <ToggleButton 
          isDisabled={false}
          className="toggle-btn"
          label={t('intermediateVersion')}
          onChange={this.onChangeStoreForceSave}
          isChecked={intermediateVersion}
        />
        <ToggleButton
          isDisabled={false}
          className="toggle-btn"
          label={t('thirdPartyBtn')}
          onChange={this.onChangeThirdParty}
          isChecked={thirdParty}
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
      keepIntermediate,
      t
    } = this.props;

    const { 
      originalCopy,
      updateExist,
      displayNotification
    } = this.state;

    return (
      <StyledSettings>
        <ToggleButton
          isDisabled={false}
          className="toggle-btn"
          label={t('originalCopy')}
          onChange={this.onChangeOriginalCopy}
          isChecked={originalCopy}
        />
        <ToggleButton
          isDisabled={false}
          className="toggle-btn"
          label={t('displayNotification')}
          onChange={this.onChangeDeleteConfirm}
          isChecked={displayNotification}
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
          isDisabled={false}
          className="toggle-btn"
          label={t('updateOrCreate')}
          onChange={this.onChangeUpdateIfExist}
          isChecked={updateExist}
        />
        <ToggleButton
          isDisabled={true}
          className="toggle-btn"
          label={t('keepIntermediateVersion')}
          onChange={(e)=>console.log(e)}
          isChecked={keepIntermediate}
        />
      </StyledSettings>
    );
  }

  renderClouds = () => {
    return (<></>)
  }

  render() {
    const { setting, thirdParty, isAdmin } = this.props;
    let content;

    if(setting === 'admin' && isAdmin)
      content = this.renderAdminSettings();
    if(setting === 'common') 
      content = this.renderCommonSettings();
    if(setting === 'thirdParty' && thirdParty )
      content = this.renderClouds();
    return content; 
  }
}

function mapStateToProps(state) {
  const { settingsTree } = state.files;
  const { isAdmin } = state.auth.user;
  const { thirdParty } = settingsTree;

  return { 
    thirdParty,
    isAdmin
  }
}

export default connect(
  mapStateToProps, { 
    updateIfExist, 
    storeOriginal, 
    setThirdParty,
    changeDeleteConfirm,
    storeForceSave
  })
  (SectionBodyContent);