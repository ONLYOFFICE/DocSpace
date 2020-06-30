import React, { Component } from 'react';
import { withRouter } from 'react-router';
import styled from "styled-components";
import i18n from './i18n';
import { withTranslation } from 'react-i18next';
import { connect } from 'react-redux';

import { PageLayout } from "asc-web-common";
import { 
  Heading, Text, 
  EmailInput, PasswordInput, 
  InputBlock, Checkbox, Link,
  GroupButton, DropDownItem, 
  Button, Box, Loader, 
  ModalDialog, utils } from 'asc-web-components';

import { getParams, setOwnerToSrv, saveNewEmail } from '../../../store/wizard/actions';

const { EmailSettings } = utils.email;
const settings = new EmailSettings();
settings.allowDomainPunycode = true;

const settingsPassword = {
  minLength: 2,
  upperCase: true,
  digits: true,
  specSymbols: true
};

const HeaderContent = styled.div`
  display: flex;
  flex-direction: column;
  position: absolute;
  height: 56px;
  background: #0F4071;
  width: 100%;
  left: 0px;

  .header-logo {
    padding: 0;
    margin: 0;
    position: absolute;
    left: 240px;
    top: 14.5px;

    @media(max-width: 768px) {
      left: 144px;
    }

    @media(max-width: 375px) {
      left: 32px;
    }
  }
`;

const sectionHeaderContent = <HeaderContent>
  <a className="header-wizard" href="/wizard">
    <img
      className="header-logo"
      src="images/onlyoffice_logo/light_small_general.svg"
      alt="Logo"
    />
  </a>
</HeaderContent>;

const WizardContainer = styled.div`
  .form-container {
    width: 960px;
    height: 496px;
    margin: 0 auto;
    margin-top: 80px;
    padding: 0;

    @media(max-width: 768px) {
      width: 480px
    }

    @media(max-width: 375px) {
      width: 311px;
      margin-top: 32px;
    }
  }

  .header-box {
    height: 64px;
    width: 960px;
    font-family: 'Open Sans';
    font-style: normal;

    .wizard-title {
      text-align: center;
      font-weight: 600;
      font-size: 32px;
      line-height: 36px;
      margin: 0px 12px;
      flex: none;
      order: 0;
    }

    .wizard-desc {
      text-align: center;
      font-weight: normal;
      font-size: 13px;
      line-height: 20px;
      margin: 10px 12px;
      flex: none;
      order: 1;
    }

    @media(max-width: 768px) {
      .wizard-title, .wizard-desc  {
        margin: 10px 0px;
        text-align: left;
      }
    }

    @media(max-width: 375px) {
      width: 311px;
      
      .wizard-title {
        font-size: 23px;
        line-height: 28px;
      }
    }
  }

  .input-box {
    width: 311px;
    font-family: 'Open Sans';
    font-style: normal;
    font-weight: normal;
    margin: 32px auto 0 auto;

    input {
      font-size: 16px;
      line-height: 22px;
      padding-left: 15px;
    }

    .wizard-input-email {
      width: 100%;
      height: 44px;
      font-size: 16px;
      line-height: 22px;
      padding-left: 16px;
    }

    .wizard-pass { 
      width: 360px;
      margin-top: 16px;

      .input-relative {
        width: 311px;

        @media(max-width: 768px) {
          width: 480px;
        }
        @media(max-width: 375px) {
          width: 311px;
        }
      }
    }

    .wizard-pass input {
      height: 44px;
    }

    .password-tooltip {
      height: 14px;
      text-align: left;
      padding: 0;
      margin: 0 auto;
      font-size: 10px;
      line-height: 14px;
      color: #A3A9AE;
    }

    .input-block {
      width: 100%;
      height: 44px;
      margin: 16px auto;
    }

    .input-file {
      display: none;
    }

    .checkbox-container {
      width: 100%;
      margin: 17px auto 0 auto;
    }

    .wizard-checkbox {
      display: inline-block;
    }

    .wizard-checkbox span {
      margin-right: 0.3em;
      vertical-align: middle;
    }

    .link {
      vertical-align: middle;
      font-size: 13px;
      line-height: 18px;
    }

    @media(max-width: 768px) {
      width: 480px;
      margin: 32px 0 0 0;
    }

    @media(max-width: 375px) {
      width: 311px;
    }
  }

  .settings-box {
    width: 311px;
    margin: 32px auto 0 auto;
    display: flex;
    flex-direction: row;
    padding: 0;

    .settings-title {
      font-family: Open Sans;
      font-style: normal;
      font-weight: normal;
      font-size: 13px;
      line-height: 20px;
      margin: 16px 0px;
    }

    .values {
      margin: 0;
      padding: 0;
      margin-left: 16px;
    }

    .text, .value {
      font-style: normal;
      font-weight: 600;
      font-size: 13px;
      line-height: 20px;
    } 

    .link {
      display: inline-block;
      margin-bottom: 16px;
    }

    .text {
      margin: 16px 0;
    }

    .drop-down {
      display: block;
      margin: 0 0 16px 0;
    }

    .drop-down .value {
      display: block;
      width: 100%;
      text-align: left;
    }

    @media(max-width: 768px) {
      width: 480px;
      margin: 32px 0 0 0;
    }

    @media(max-width: 768px) {
      width: 311px;
    }
  }

  .wizard-button {
    display: block;
    width: 311px;
    height: 44px;
    margin: 32px auto 0 auto;

    @media(max-width: 768px) {
      width: 100%;
    }
  }

  .modal-change-email {
    height: 32px;
    width: 528px;

    @media(max-width: 768px) {
      width: 293px;
    }
  }
  
  .modal-button-save {
    height: 36px;
    width: 100px;

    @media(max-width: 768px) {
      width: 293px;
      height: 32px;
    }
  }

  .modal-error-content {
    font-family: 'Open Sans';
    font-style: normal;
    font-weight: normal;
    font-size: 13px;
    line-height: 20px;
  }
`;

class Body extends Component {
  constructor(props) {
    super(props);

    this.state = {
      password: '',
      isValidPass: false,
      errorLoading: false,
      visibleModal: false,
      path: '',
      emailValid: false,
      email: '',
      license: false,
      selectLanguage: "",
      selectTimezone: "",
      valid: false
    }

    this.inputRef = React.createRef();
  }

  componentDidMount() {
    const { t, getParams } = this.props;
    document.title = t('title');
    getParams();
  }

  isValidPassHandler = val => {
    this.setState({ isValidPass: val}, () => console.log(this.state.isValidPass));
  }

  onChangePassword = e => {
    console.log('onchange password', e.target.value);
    this.setState({ password: e.target.value });
  }
  
  onIconFileClick = () => {
    console.log('input file click');
    this.inputRef.current.click();
  }

  onClickChangeEmail = () => {
    console.log("change mail click")
    this.setState({ visibleModal: true })
  }

  onEmailHandler = result => {
    this.setState({ emailValid: result.isValid});

    if(result.isValid) {
      console.log('email is valid')
      this.setState({ email: result.value });
    }
  }

  onChangeFile = e => {
    console.log('select file', e.target.value)
    this.setState({ path: e.target.value}) 
  }

  onInputFile = () => {
    console.log('on input file inner input')
    this.setState({path: this.inputRef.current.value});
  }

  onChangeLicense = () => {
    console.log('onchange License');
    this.setState({ license: !this.state.license });
  }

  onContinueHandler = () => {
    console.log('continue btn click');
    const valid = this.checkingValid();
    if (valid) { 
      console.log('valid params');
      const { setOwnerToSrv } = this.props;
      const owner = {
        password: this.state.password,
        email: this.state.email,
        language: this.state.selectLanguage,
        timezone: this.state.selectTimezone
      }
      setOwnerToSrv(owner);
    }
    else {
      console.log('not valid params');
    }
  }

  checkingValid = () => {
    const { isValidPass, emailValid, path, license } = this.state;
    
    if( isValidPass && emailValid && path && license ) {
      return true;
    }

    return false;
  }

  onSaveEmailHandler = () => {
    const { saveNewEmail } = this.props;
    console.log('save email', this.state.email);
    saveNewEmail(this.state.email)
    this.setState({ visibleModal: false });
  }

  onCloseModal = () => {
    console.log('onClose modal');
    this.setState({ visibleModal: false, errorLoading: false});
  }

  onSelectTimezoneHandler = el => {
    console.log('on select timezone');
    this.setState({ selectTimezone: el });
  }

  onSelectLanguageHandler = lang => {
    console.log('on select lang');
    this.setState({ selectLanguage: lang })
  }

  renderModalDialog = () => {
    const { errorLoading, visibleModal } = this.state;
    const { t, isOwner } = this.props;

    let header, content, footer;

    const visible = errorLoading ? errorLoading : visibleModal;

    if(errorLoading) {
      header = t('errorLicenseTitle');
      content = <span 
        className="modal-error-content">
          {t('errorLicenseBody')}
      </span>;

    } else if( visibleModal && isOwner ) {
      header = t('changeEmailTitle');

      content = <EmailInput
        className="modal-change-email"
        tabIndex={1}
        id="change-email"
        name="email-wizard"
        placeholder={t('placeholderEmail')}
        emailSettings={settings}
        value={this.state.email}
        onValidateInput={this.onEmailHandler}
      />;

      footer = <Button
        className="modal-button-save"
        key="saveBtn"
        label={t('changeEmailBtn')}
        primary={true}
        size="medium"
        onClick={this.onSaveEmailHandler}
      />;
    }

    return <ModalDialog
        visible={visible}
        scale={false}
        displayType="auto"
        zIndex={310}
        headerContent={header}
        bodyContent={content}
        footerContent={footer}
        onClose={this.onCloseModal}
      />
  }

  renderHeaderBox = () => {
    const { t } = this.props;
    return (
      <Box className="header-box">
        <Heading level={1} title="Wizard" className="wizard-title">
          {t('welcome')}
        </Heading>
        <Text className="wizard-desc">
          {t('desc')}
        </Text>
      </Box>
    )
  }

  renderInputBox = () => {
    const { t, isOwner } = this.props;
    const inputEmail = !isOwner 
      ? <EmailInput
          className="wizard-input-email"
          tabIndex={1}
          id="input-email"
          name="email-wizard"
          placeholder={t('email')}
          emailSettings={settings}
          onValidateInput={this.onEmailHandler}
        />
      : null

    return (
      <Box className="input-box">
        {inputEmail}
        <PasswordInput
          className="wizard-pass"
          tabIndex={2}
          id="first"
          inputName="firstPass"
          emailInputName="email-wizard"
          inputWidth="311px"
          inputValue={this.state.password}
          passwordSettings={settingsPassword}
          isDisabled={false}
          placeholder={t('placeholderPass')}
          onChange={this.onChangePassword}
          onValidateInput={this.isValidPassHandler}
        />
        <Text className="password-tooltip">
          {t('tooltipPass')}
        </Text>
        <InputBlock
          value={this.state.path}
          className="input-block"
          iconName={"CatalogFolderIcon"}
          placeholder={t('placeholderLicense')}
          onIconClick={this.onIconFileClick}
          onChange={this.onChangeFile}
        >
          <input type="file" 
            className="input-file" 
            onInput={this.onInputFile}
            ref={this.inputRef}/>
        </InputBlock>
        <Box className="checkbox-container">
          <Checkbox
            className="wizard-checkbox"
            id="license"
            name="confirm"
            label={t('license')}
            isChecked={this.state.license}
            isIndeterminate={false}
            isDisabled={false}
            onChange={this.onChangeLicense}
          />
          <Link 
            className="link"
            type="page" 
            color="#116d9d" 
            href="https://gnu.org/licenses/gpl-3.0.html" 
            isBold={false}
          >{t('licenseLink')}</Link>
        </Box>
      </Box>
    );
  }

  renderSettingsBox = () => {
    const { selectLanguage, selectTimezone } = this.state;
    const { isOwner, t, domain, languages, timezones, ownerEmail } = this.props;

    const titleEmail = isOwner 
      ? <Text className="settings-title">{t('email')}</Text>
      : null
    
    const email = isOwner 
      ? <Link className="link value" type="action" onClick={this.onClickChangeEmail}>{ownerEmail}</Link>
      : null

    return (
      <Box className="settings-box">
        <Box className="setting-title-block">
          <Text className="settings-title">{t('domain')}</Text>
          {titleEmail}
          <Text className="settings-title">{t('language')}</Text>
          <Text className="settings-title">{t('timezone')}</Text>
        </Box>
        <Box className="values">
          <Text className="text value">{domain}</Text>
          {email}
          <GroupButton className="drop-down value" label={selectLanguage ? selectLanguage : languages[0]} isDropdown={true}>
            {
              languages.map(el => (
                <DropDownItem 
                  key={el} 
                  label={el}
                  onClick={() => this.onSelectLanguageHandler(el)}
                />
              ))
            }
          </GroupButton>
          <GroupButton className="drop-down value" label={selectTimezone ? selectTimezone : timezones[0]} isDropdown={true}>
            {
              timezones.map(el => (
                <DropDownItem 
                  key={el} 
                  label={el}
                  onClick={() => this.onSelectTimezoneHandler(el)}
                />
              ))
            }
          </GroupButton>
        </Box>
      </Box>
    );
  }

  renderButtonBox = () => {
    const { t } = this.props;
    return (
      <Button
        className="wizard-button"
        primary
        label={t('buttonContinue')}           
        size="big"
        onClick={this.onContinueHandler}
      />
    );
  }

  render() {
    console.log('wizard render')
    const headerBox = this.renderHeaderBox();
    const inputBox = this.renderInputBox();
    const settingsBox = this.renderSettingsBox();
    const buttonBox = this.renderButtonBox();
    const modalDialog = this.renderModalDialog();

    return (
      <WizardContainer>
        
        <Box className="form-container">
        { modalDialog }
          {headerBox}
          {inputBox}
          {settingsBox}
          {buttonBox}
        </Box>
      </WizardContainer>
    )
  }
}

const WizardPage = props => <PageLayout 
  sectionBodyContent={<Body {...props} />} 
  sectionHeaderContent={sectionHeaderContent}
/>;

const WizardWrapper = withTranslation()(WizardPage);

const Wizard = props => {
  const { language } = props;

  i18n.changeLanguage(language);

  return <WizardWrapper i18n={i18n}  {...props}/>
}

function mapStateToProps(state) {
  return {
    domain: state.wizard.domain,
    language: state.wizard.language,
    languages: state.wizard.languages,
    timezone: state.wizard.timezone,
    timezones: state.wizard.timezones,
    isOwner: state.wizard.isOwner,
    ownerEmail: state.wizard.ownerEmail
  };
}

export default connect(mapStateToProps, { getParams, setOwnerToSrv, saveNewEmail })(withRouter(Wizard)); 

