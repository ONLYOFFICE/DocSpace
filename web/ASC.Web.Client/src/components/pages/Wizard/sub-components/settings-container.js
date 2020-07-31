import React from 'react';
import PropTypes from "prop-types";
import styled from 'styled-components';

import { 
  Box, 
  GroupButton, 
  DropDownItem, 
  Text, 
  Link, 
  utils 
} from 'asc-web-components';

const { tablet } = utils.device;

const StyledContainer = styled(Box)`
  width: 311px;
  margin: 32px auto 0 auto;
  display: flex;
  flex-direction: row;

  .settings-title{
    margin-bottom: 12px;
  }

  .timezone-title {
    margin-bottom: 0;
  }

  .settings-values {
    padding: 0;
    margin: 0;
    margin-left: 16px;
  } 

  .settings-value {
    display: block;
    margin-bottom: 12px;
  }

  .drop-down {
    font-size: 13px;
  }

  .language-value {
    margin: 0;
  }

  .timezone-value {
    margin: 0;
    margin-top: 11px; 
  }

  @media ${tablet} {
    width: 480px;
    margin-top: 32px;
  }

  @media(max-width: 520px) {
    width: 311px;
    margin: 32px 0px 0px 0px;
  }
`;

const SettingsContainer = ({
  selectLanguage, 
  selectTimezone, 
  languages, 
  timezones, 
  emailNeeded, 
  email, 
  emailOwner,
  t, 
  machineName, 
  onClickChangeEmail, 
  onSelectLanguageHandler, 
  onSelectTimezoneHandler
}) => {
  
  const titleEmail = !emailNeeded 
    ? <Text className="settings-title">{t('email')}</Text>
    : null
  
  const contentEmail = !emailNeeded 
    ? <Link 
        className="settings-value" 
        type="action" 
        fontSize="13px" 
        fontWeight="600" 
        onClick={onClickChangeEmail}
      >
        {email ? email : emailOwner}
      </Link>
    : null

  return (
    <StyledContainer>
      <Box>
        <Text className="settings-title" fontSize="13px">{t('domain')}</Text>
        {titleEmail}
        <Text className="settings-title" fontSize="13px">{t('language')}</Text>
        <Text className="settings-title timezone-title" fontSize="13px">{t('timezone')}</Text>
      </Box>
      <Box className="settings-values">
        <Text className="settings-value" fontSize="13px" fontWeight="600">{machineName}</Text>
        {contentEmail}
        <GroupButton 
          className="drop-down settings-value language-value" 
          label={selectLanguage.label} 
          isDropdown={true}
          dropDownMaxHeight={300}>
          {
            languages.map(el => (
              <DropDownItem 
                key={el.key} 
                label={el.label}
                onClick={() => onSelectLanguageHandler(el)}
              />
            )) 
          }
        </GroupButton>
        
        <GroupButton 
          className="drop-down settings-value timezone-value" 
          label={selectTimezone.label} 
          isDropdown={true}
          dropDownMaxHeight={300} >
          {
            timezones.map(el => (
              <DropDownItem 
                key={el.key} 
                label={el.label}
                onClick={() => onSelectTimezoneHandler(el)}
              />
            ))
          }
        </GroupButton>
        
      </Box>
    </StyledContainer>
  );
}

SettingsContainer.propTypes = {
  selectLanguage: PropTypes.object.isRequired,
  selectTimezone: PropTypes.object.isRequired,
  languages: PropTypes.array.isRequired,
  timezones: PropTypes.array.isRequired,
  emailNeeded: PropTypes.bool.isRequired,
  emailOwner: PropTypes.string,
  t: PropTypes.func.isRequired,
  machineName: PropTypes.string.isRequired,
  email: PropTypes.string,
  onClickChangeEmail: PropTypes.func.isRequired,
  onSelectLanguageHandler: PropTypes.func.isRequired,
  onSelectTimezoneHandler: PropTypes.func.isRequired
}

export default SettingsContainer;