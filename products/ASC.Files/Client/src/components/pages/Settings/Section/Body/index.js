import React from  'react';
import styled from 'styled-components';
import { 
  Box,
  ToggleButton 
} from 'asc-web-components';

const StyledSettings = styled.div`

  .setting-container {
    display: flex;
    flex-direction: row;
  }

  .toggle-btn {

  }
`;

const SectionBodyContent = ({ 
  setting,
  intermediateVersion,
  thirdParty,
  isCheckedThirdParty,
  isCheckedIntermediate
}) => {
  let content;

  const renderAdminSettings = () => {
    return (
      <StyledSettings displayProp="flex" className="setting-container">
        <ToggleButton 
          isDisabled={true}
          className="toggle-btn"
          label="Keep all saved intermediate versions"
          onChange={isCheckedIntermediate}
          isChecked={intermediateVersion}
        />
        <br />
        <ToggleButton
          isDisabled={true}
          className="toggle-btn"
          label="Allow users to connect third-party storages"
          onChange={isCheckedThirdParty}
          isChecked={thirdParty}
        />
      </StyledSettings>
    )
  }

  const renderCommonSettings = () => {

  }

  const renderClouds = () => {

  }

  if(setting === 'admin-settings')
    return renderAdminSettings();
  if(setting === 'common-settings') 
    return renderAdminSettings();
  if(setting === 'connected-clouds')
    return renderAdminSettings();
}

export default SectionBodyContent;