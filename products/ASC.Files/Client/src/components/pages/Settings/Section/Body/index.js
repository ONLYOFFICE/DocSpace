import React from  'react';
import styled from 'styled-components';
import { 
  Heading,
  ToggleButton 
} from 'asc-web-components';

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
  }

  componentDidMount() {
    this.props.onLoading(false);
  }

  renderAdminSettings = () => {
    const {
      setting,
      intermediateVersion,
      thirdParty,
      t
    } = this.props;

    document.title = t(`${setting}`);

    return (
      <StyledSettings>
        <ToggleButton 
          isDisabled={true}
          className="toggle-btn"
          label={t('intermediateVersion')}
          onChange={(e)=>console.log(e)}
          isChecked={intermediateVersion}
        />
        <ToggleButton
          isDisabled={true}
          className="toggle-btn"
          label={t('thirdPartyBtn')}
          onChange={(e)=>console.log(e)}
          isChecked={thirdParty}
        />
      </StyledSettings>
    )
  }

  renderCommonSettings = () => {
    const {
      originalCopy,
      trash,
      recent,
      favorites,
      templates,
      updateOrCreate,
      keepIntermediate,
      t
    } = this.props;

    return (
      <StyledSettings>
        <ToggleButton
          isDisabled={true}
          className="toggle-btn"
          label={t('originalCopy')}
          onChange={(e)=>console.log(e)}
          isChecked={originalCopy}
        />
        <ToggleButton
          isDisabled={true}
          className="toggle-btn"
          label={t('displayNotification')}
          onChange={(e)=>console.log(e)}
          isChecked={trash}
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
          isDisabled={true}
          className="toggle-btn"
          label={t('updateOrCreate')}
          onChange={(e)=>console.log(e)}
          isChecked={updateOrCreate}
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

  }

  render() {
    const { setting } = this.props;
    let content;

    if(setting === 'admin')
      content = this.renderAdminSettings();
    if(setting === 'common') 
      content = this.renderCommonSettings();
    if(setting === 'thirdParty')
      content = this.renderAdminSettings();

    return content;
  }

  
}

export default SectionBodyContent;