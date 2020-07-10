import React, { Component } from 'react';
import { withRouter } from 'react-router';
import styled from "styled-components";
import i18n from './i18n';
import { withTranslation } from 'react-i18next';
import { connect } from 'react-redux';
import { Redirect } from 'react-router-dom'

import { PageLayout, history } from "asc-web-common";
import { 
  Heading, Text, 
  EmailInput, PasswordInput, 
  InputBlock, Checkbox, Link,
  GroupButton, DropDownItem, 
  Button, Box, Loader, Toast, toastr, 
  ModalDialog, utils 
} from 'asc-web-components';

import { 
  getPortalPasswordSettings, getPortalTimezones, 
  getPortalCultures, setIsWizardLoaded, 
  getMachineName, setPortalOwner 
} from '../../../store/wizard/actions';

const { EmailSettings } = utils.email;
const settings = new EmailSettings();
settings.allowDomainPunycode = true;

const { tablet, mobile } = utils.device;

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

    @media ${tablet} {
      left: 144px;
    }

    @media(max-width: 415px) {
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
    margin: 0 auto;
    margin-top: 80px; 
    padding: 0;

    @media ${tablet} {
      width: 100%;
      max-width: 480px;
    }

    @media(max-width: 415px) {
      width: 100%;
      margin-top: 32px;
    }
  }

  .header-box {
    width: 100%;
    font-family: 'Open Sans';
    font-style: normal;

    .wizard-title {
      text-align: center;
      font-weight: 600;
      font-size: 32px;
      line-height: 36px;
      margin: 0px 12px;
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

    @media ${tablet} {
      .wizard-title, .wizard-desc  {
        margin: 10px 0px;
        text-align: left;
      }
    }

    @media(max-width: 415px) {
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
      width: 100%;
      margin-top: 16px;
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

    @media ${tablet} {
      width: 100%;
      margin: 32px 0 0 0;
    }

    @media(max-width: 415px) {
      width: 100%;
    }
  }

  .settings-box {
    width: 311px;
    margin: 32px auto 0 auto;
    display: flex;
    flex-direction: row;
    padding: 0;

    .settings-title {
      width: 66px;
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
      width: 100%;
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

    .language-value{
      margin: 0;
    }

    .timezone-value {
      margin-top: 16px; 
    }

    @media ${tablet} {
      width: 480px;
      margin: 32px 0 0 0;
    }

    @media(max-width: 415px) {
      width: 311px;
    }
  }

  .wizard-button {
    display: block;
    width: 311px;
    height: 44px;
    margin: 32px auto 0 auto;

    @media ${tablet} {
      width: 100%;
    }
  }

  .modal-change-email {
    height: 32px;
    width: 528px;

    @media ${tablet} {
      width: 293px;
    }
  }
  
  .modal-button-save {
    height: 36px;
    width: 100px;

    @media ${tablet} {
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

    const { t } = props;

    this.state = {
      password: '',
      isValidPass: false,
      errorLoading: false,
      errorMessage: null,
      sendingComplete: false,
      visibleModal: false,
      path: '',
      emailValid: false,
      email: '',
      newEmail: '',
      license: false,
      languages: null,
      timezones: null,
      selectLanguage: null,
      selectTimezone: null
    }

    this.inputRef = React.createRef();
    document.title = t('wizardTitle');
  }

  async componentDidMount() {
    const { 
      t, wizardToken, 
      getPortalPasswordSettings, getPortalCultures, 
      getPortalTimezones, setIsWizardLoaded, 
      getMachineName, history
    } = this.props;

    window.addEventListener("keyup", this.onKeyPressHandler);

    if(!wizardToken) { 
      history.push('/');
    }

    try {
      await Promise.all([
        getPortalPasswordSettings(wizardToken),
        getMachineName(wizardToken),
        getPortalTimezones(wizardToken)
          .then(() => {
            const { timezones, portalTimezone } = this.props;
            const zones = this.mapTimezonesToArray(timezones);
            const select = zones.filter(zone => zone.key === portalTimezone);
            this.setState({
              timezones: zones,
              selectTimezone: {
                key: select[0].key,
                label: select[0].label
              }
            });
          }),
        getPortalCultures()
          .then(() => {
            const { cultures, portalCulture } = this.props;
            const languages = this.mapCulturesToArray(cultures, t);
            const select = languages.filter(lang => lang.key === portalCulture);
            this.setState({ 
              languages: languages, 
              selectLanguage: { 
                key: select[0].key, 
                label: select[0].label 
              }
            })
          })
      ])
      .then(() => setIsWizardLoaded(true)); 
    } catch(e) {
      this.setState({
        errorLoading: true,
        errorMessage: e 
      })
    }
  }

  shouldComponentUpdate(nextProps) {
    if(nextProps.isWizardLoaded === true) {
      return true;
    } else {
      return false;
    }
  }

  componentWillUnmount() {
    window.removeEventListener("keyup", this.onKeyPressHandler);
  }

  mapTimezonesToArray = (timezones) => {
    return timezones.map((timezone) => {
      return { key: timezone.id, label: timezone.displayName.substring(0, 11) };
    });
  };
  
  mapCulturesToArray = (cultures, t) => {
    return cultures.map((culture) => {
       return { key: culture, label: t(`Culture_${culture}`) };
    });
  };

  onKeyPressHandler = e => {
    if (e.key === "Enter") this.onContinueHandler();
  }

  isValidPassHandler = val => {
    this.setState({ isValidPass: val });
  }

  onChangePassword = e => {
    console.log('onchange password', e.target.value);
    this.setState({ password: e.target.value });
  }
  
  onIconFileClick = (e) => {
    console.log('input file click');
    e.target.blur()
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

    // const valid = false;
    // this.setState({ sendingComplete: true })
    // setTimeout(() => this.setState({ sendingComplete: false }), 3000)

    const { setPortalOwner } = this.props;
    if (valid) { 
      console.log('valid params');

      this.setState({ sendingComplete: true })

      const licenseFile = this.inputRef.current.files[0];
      const {
        password, email,
        selectLanguage, selectTimezone
      } = this.state;

      const { wizardToken } = this.props;

      const emailTrim = email.trim();
      const analytics = true
   
      setPortalOwner(emailTrim, password, selectLanguage.key, wizardToken, analytics)
        .then(() => history.push(`/`))
        .catch((e) => {
          this.setState({
            errorLoading: true,
            sendingComplete: false,
            errorMessage: e 
          })
        }) 
    }
  }

  checkingValid = () => {
    const { t } = this.props;
    const { isValidPass, emailValid, path, license } = this.state;

    if(!isValidPass) toastr.error(t('errorPassword'));
  
    if(!emailValid) toastr.error(t('errorEmail'));

    if(!license) toastr.error(t('errorLicenseRead'));
    
    if( isValidPass && emailValid && license ) return true;

    return false; 
  }

  onSaveEmailHandler = () => {
    console.log('save email', this.state.email);
    this.setState({ visibleModal: false });
  }

  onCloseModal = () => {
    console.log('onClose modal');
    this.setState({ visibleModal: false, errorLoading: false, errorMessage: null });
  }

  onSelectTimezoneHandler = el => {
    console.log('on select timezone');
    this.setState({ selectTimezone: el });
  }

  onSelectLanguageHandler = lang => {
    console.log('on select lang', lang);
    this.setState({ 
      selectLanguage: {
        key: lang.key,
        label: lang.label 
      }});
  }

  renderModalDialog = () => {
    const { errorLoading, visibleModal, errorMessage } = this.state;
    const { t, isOwner, ownerEmail } = this.props;

    let header, content, footer;

    const visible = errorLoading ? errorLoading : visibleModal;

    if(errorLoading) {
      header = t('errorLicenseTitle');
      content = <span 
        className="modal-error-content">
          {errorMessage ? errorMessage: t('errorLicenseBody')}
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
        value={ownerEmail}
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
          {t('welcomeTitle')}
        </Heading>
        <Text className="wizard-desc">
          {t('desc')}
        </Text>
      </Box>
    )
  }

  renderInputBox = () => {
    const { t, isOwner, settingsPassword } = this.props;

    const tooltipPassTitle = t('tooltipPasswordTitle');
    const tooltipPassLength = `${settingsPassword.minLength} ${t('tooltipPasswordLength')}`;
    const tooltipPassDigits = settingsPassword.digits ? `${t('tooltipPasswordDigits')}` : null;
    const tooltipPassCapital = settingsPassword.upperCase ? `${t('tooltipPasswordCapital')}` : null;
    const tooltipPassSpecial = settingsPassword.specSymbols ? `${t('tooltipPasswordSpecial')}` : null;
    
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
          inputWidth="100%"
          NewPasswordButtonVisible={false}
          tooltipPasswordTitle={tooltipPassTitle}
          tooltipPasswordLength={tooltipPassLength}
          tooltipPasswordDigits={tooltipPassDigits}
          tooltipPasswordCapital={tooltipPassCapital}
          tooltipPasswordSpecial={tooltipPassSpecial}
          inputValue={this.state.password}
          passwordSettings={settingsPassword}
          isDisabled={false}
          placeholder={t('placeholderPass')}
          onChange={this.onChangePassword}
          onValidateInput={this.isValidPassHandler}
        />
        <InputBlock
          tabIndex={3}
          value={this.state.path}
          className="input-block"
          iconName={"CatalogFolderIcon"}
          placeholder={t('placeholderLicense')}
          onIconClick={this.onIconFileClick}
          onChange={this.onChangeFile}
          onFocus={this.onIconFileClick}
        >
          <input 
            type="file" 
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
    const { selectLanguage, selectTimezone, languages, timezones } = this.state;
    const { isOwner, t, ownerEmail, machineName } = this.props;
    
    const titleEmail = isOwner 
      ? <Text className="settings-title">{t('email')}</Text>
      : null
    
    const contentEmail = isOwner 
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
          <Text className="text value">{machineName}</Text>
          {contentEmail}
          <GroupButton 
            className="drop-down value language-value" 
            label={selectLanguage.label} 
            isDropdown={true}
            dropDownMaxHeight={300}>
            {
              languages.map(el => (
                <DropDownItem 
                  key={el.key} 
                  label={el.label}
                  onClick={() => this.onSelectLanguageHandler(el)}
                />
              )) 
            }
          </GroupButton>
          
          <GroupButton 
            className="drop-down value timezone-value" 
            label={selectTimezone.label} 
            isDropdown={true}
            dropDownMaxHeight={300} >
            {
              timezones.map(el => (
                <DropDownItem 
                  key={el.key} 
                  label={el.label}
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
    const { sendingComplete } = this.state;
    const labelButton = sendingComplete ? t('buttonLoading') : t('buttonContinue')

    return (
      <Button
        className="wizard-button"
        primary
        label={labelButton}           
        size="big"
        onClick={this.onContinueHandler}
        isDisabled={sendingComplete}
      />
    );
  }

  render() {
    const { isWizardLoaded } = this.props;
  
    console.log('wizard render');

    if (isWizardLoaded) {
      const headerBox = this.renderHeaderBox();
      const inputBox = this.renderInputBox();
      const settingsBox = this.renderSettingsBox();
      const buttonBox = this.renderButtonBox();
      const modalDialog = this.renderModalDialog();

      return <WizardContainer>
        <Toast/>
        <Box className="form-container">
        { modalDialog }
          {headerBox}
          {inputBox}
          {settingsBox}
          {buttonBox}
        </Box>
      </WizardContainer>
    }
    return <Loader className="pageLoader" type="rombs" size='40px' />;
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
    isOwner: state.wizard.isOwner,
    ownerEmail: state.wizard.ownerEmail,

    isWizardLoaded: state.wizard.isWizardLoaded, 
    machineName: state.wizard.machineName,
    isComplete: state.wizard.isComplete,

    wizardToken: state.auth.settings.wizardToken,
    settingsPassword: state.auth.settings.passwordSettings,
  
    cultures: state.auth.settings.cultures,
    portalCulture: state.auth.settings.culture,
    timezones: state.auth.settings.timezones,
    portalTimezone: state.auth.settings.timezone
  };
}

export default connect(mapStateToProps, {  
  getPortalPasswordSettings, getPortalCultures, 
  getPortalTimezones, setIsWizardLoaded, 
  getMachineName, setPortalOwner 
})(withRouter(Wizard)); 