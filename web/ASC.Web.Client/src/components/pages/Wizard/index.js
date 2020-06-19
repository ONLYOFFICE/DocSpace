import React, { Component } from 'react';
import { withRouter } from 'react-router';
import styled from "styled-components";
import { connect } from 'react-redux'
import i18n from './i18n';
import { withTranslation } from 'react-i18next';

/*
import {
  utils
} from "asc-web-common";
*/
import { 
  ButtonBox, CheckboxBox, HeaderBox, 
  LanguageAndTimezoneBox, PasswordBox,
  RegistrationSettingsBox, SettingsWrapper,
  WelcomeBox, WizardForm } from './sub-components';

// const { changeLanguage } = utils;

const StyledWizardWrapper = styled.div`
  margin: 0;
  padding: 0;
  width: 100%;
`;

class WizardPage extends Component { 
  constructor(props) {
    super(props);

    this.state = { 
      firstPassword: '',
      secondPassword: '',
      domain: 'SomeDomain',
      languages: [
        'English','Russia'
      ],
      selectLanguage: '',
      timeZones: [
        '(UTC+03:00) Москва, Санкт-Петербург',
        '(UTC+01:00) Амстердам, Берлин, Берн, Вена, Рим, Стокгольм'
      ],
      selectedTimeZone: '',
      isSendData: true,
      isAcceptLicense: false
    };
  }

  componentDidMount() {
    document.title = this.props.t('title');
  }

  acceptLicenseHandler = () => {
    this.setState({isAcceptLicense: !this.state.isAcceptLicense});
  }

  sendDataHandler = () => {
    this.setState({isSendData: !this.state.isSendData});
  }

  render() {
    const { 
      firstPassword, secondPassword,
      domain, languages, timeZones,
      isSendData, isAcceptLicense 
    } = this.state;

    const { t } = this.props;

    return (
      <StyledWizardWrapper>
        <HeaderBox />
        <WizardForm>
          <WelcomeBox t={t}/>
          <SettingsWrapper>
            <PasswordBox 
              firstPass={firstPassword} 
              secondPass={secondPassword}
              t={t} />
            <div className="float-right">
              <RegistrationSettingsBox domain={domain} t={t}/>
              <LanguageAndTimezoneBox 
                languages={languages} 
                timeZones={timeZones} 
                t={t} />
            </div>
          </SettingsWrapper>
          <CheckboxBox 
            isSendData={isSendData}
            isAcceptLicense={isAcceptLicense}
            sendDataHandler={this.sendDataHandler}
            acceptLicenseHandler={this.acceptLicenseHandler}
            t={t}
          />
          <ButtonBox label={t('buttonBox.buttonName')}/>
        </WizardForm>
      </StyledWizardWrapper>
    );
  }
}

const WizardWrapper = withTranslation()(WizardPage);

const Wizard = props => {
  const { language } = props;

  i18n.changeLanguage(language);

  return <WizardWrapper i18n={i18n}  {...props}/>
}

function mapStateToProps(state) {
  return {
    language: state.auth.user.cultureName || state.auth.settings.culture,
  };
}

export default connect(mapStateToProps)(withRouter(Wizard));  