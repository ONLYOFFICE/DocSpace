import React from 'react';
import { connect } from 'react-redux';
import styled from 'styled-components';

import { 
  Box,
  ToggleButton 
} from 'asc-web-components';

const StyledSettings = styled(Box)`
  .toggle-btn-container {
    display: block;
  }
`;

class Settings extends React.Component {
  constructor(props) {
    super(props)

    this.state = {
      intermediateVersion: false,
      thirdParty: false
    }
  }

  isCheckedIntermediate = () => {
    this.setState({
      intermediateVersion: !this.state.intermediateVersion
    })
  }

  isCheckedThirdParty = () => {
    this.setState({
      thirdParty: !this.state.thirdParty
    })
  }
 
  renderAdminSettings = () => {
    const { 
      intermediateVersion, 
      thirdParty 
    } = this.state;
    return (
      <StyledSettings>
        <Box className="toggle-btn-container">
        <ToggleButton 
          label="Keep all saved intermediate versions"
          onChange={this.isCheckedIntermediate}
          isChecked={intermediateVersion}
        />
        </Box>
        <br />
        <Box className="toggle-btn-container">
        <ToggleButton
          label="Allow users to connect third-party storages"
          onChange={this.isCheckedThirdParty}
          isChecked={thirdParty}
        />
        </Box>
      </StyledSettings>
    )
  }

  renderCommonSettings = () => {

  }

  renderClouds = () => {

  }

  renderSettings = setting => {
    switch (setting) {
      case "common-settings":
        return this.renderCommonSettings();
      case "admin-settings":
        return this.renderAdminSettings();
      case "connected-clouds":
        return this.renderClouds();
      default:
        return this.renderCommonSettings();
    }
  }

  render() {
    const { settingsPath, match } = this.props;
    const { setting } = match.params;
    const content = this.renderSettings(setting);

    return (
      <>
      {content}
      </>
    );
  }
} 

function mapStateToProps(state) {
  const { settingsPath } = state.files;
  return { settingsPath };
}

export default connect(mapStateToProps)(Settings);