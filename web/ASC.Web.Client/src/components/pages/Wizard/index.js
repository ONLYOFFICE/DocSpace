import React, { Component } from 'react';
import { withRouter } from 'react-router';
import styled from "styled-components";
import i18n from './i18n';
import { withTranslation } from 'react-i18next';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';

import { PageLayout, history } from "asc-web-common";
import { 
  Heading, Text, 
  EmailInput, PasswordInput, 
  FileInput, Checkbox, Link,
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

const { tablet } = utils.device;

const HeaderContent = styled.div`
  position: absolute;
  height: 56px;
  background: #0F4071;
  width: 100%;
  left: 0px;

  .header-logo {
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

    @media ${tablet} {
      width: 100%;
      max-width: 480px;
    }

    @media(max-width: 415px) {
      width: 311px;
      margin: 32px auto 0 auto;
    }
  }

  .header-box {
    width: 100%;

    .wizard-title {
      text-align: center;
      font-weight: 600;
      font-size: 32px;
      margin: 0 12px;
    }

    .wizard-desc {
      text-align: center;
      margin: 10px 12px;
    }

    @media ${tablet} {
      .wizard-title, .wizard-desc  {
        margin: 10px 0;
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
    margin: 32px auto 0 auto;

    .wizard-pass-box { 
      width: 360px;
      margin-top: 16px;

      @media ${tablet} {
        width: 100%;
      }
    }

    .input-file {
      width: 100%;
      margin: 16px auto;
    }

    .generate-pass-link {
      display: block;
      margin: 30px 0 32px 0;
    }

    .checkbox-container {
      width: 100%;
      margin: 16px auto 0 auto;
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
    }

    @media ${tablet} {
      width: 100%;
      margin-top: 32px;
    }
  }

  .settings-box {
    width: 311px;
    margin: 32px auto 0 auto;
    display: flex;
    flex-direction: row;

    .settings-title{
      margin-bottom: 12px;
    }

    .settings-values {
      padding: 0;
      margin: 0;
      margin-left: 16px;
    } 
    .settings-value {
      display: block;
      margin-bottom: 11px;
    }

    .drop-down {
      font-size: 13px;
    }

    .language-value {
      margin: 0;
    }

    .timezone-value {
      margin-left: 0;
      margin-top: 11px; 
    }

    @media ${tablet} {
      width: 480px;
      margin-top: 32px;
    }

    @media(max-width: 415px) {
      width: 311px;
    }
  }

  .wizard-button {
    width: 311px;
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
      changeEmail: '',
      license: false,
      languages: null,
      timezones: null,
      selectLanguage: null,
      selectTimezone: null,

      isRequiredLicense: true,
      emailNeeded: false
    }

   // this.inputRef = React.createRef();
    this.refPassInput = React.createRef();
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
    } else {
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

  isValidPassHandler = val => this.setState({ isValidPass: val });
  
  onChangePassword = e => this.setState({ password: e.target.value });

  onClickChangeEmail = () => this.setState({ visibleModal: true });

  onEmailHandler = result => {
    this.setState({ emailValid: result.isValid});

    if(result.isValid) {
      this.setState({ email: result.value });
    }
  }

  onChangeFile = e => this.setState({ path: e.target.value});

  onChangeLicense = () => this.setState({ license: !this.state.license });

  onContinueHandler = () => {
    const valid = this.checkingValid();

    console.log(this.state.file)

    if (valid) { 
      const { setPortalOwner, wizardToken } = this.props;
      const { password, email,
        selectLanguage, selectTimezone,
        isRequiredLicense
      } = this.state;

      let licenseFile;

      this.setState({ sendingComplete: true })

      //if(isRequiredLicense) licenseFile = this.inputRef.current.files[0];

      const emailTrim = email.trim();
      const analytics = true;
   
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
    const { email, changeEmail, emailValid } = this.state;
    if( emailValid && changeEmail ) {
      this.setState({ email: changeEmail})
    }
    this.setState({ visibleModal: false })
  }

  onCloseModal = () => {
    this.setState({ 
      visibleModal: false, 
      errorLoading: false, 
      errorMessage: null 
    });
  }

  onSelectTimezoneHandler = el => this.setState({ selectTimezone: el });

  onSelectLanguageHandler = lang => {
    this.setState({ 
      selectLanguage: {
        key: lang.key,
        label: lang.label 
      }});
  }

  onInputFileHandler = file => {
    this.setState({
      file: file
    })
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

    } else if( visibleModal ) { //( visibleModal && isOwner )
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
        <Text className="wizard-desc" fontSize="13px">
          {t('desc')}
        </Text>
      </Box>
    )
  }

  renderInputBox = () => {
    const { t, settingsPassword } = this.props;
    const { isRequiredLicense, emailNeeded } = this.state;

    const tooltipPassTitle = t('tooltipPasswordTitle');
    const tooltipPassLength = `${settingsPassword.minLength} ${t('tooltipPasswordLength')}`;
    const tooltipPassDigits = settingsPassword.digits ? `${t('tooltipPasswordDigits')}` : null;
    const tooltipPassCapital = settingsPassword.upperCase ? `${t('tooltipPasswordCapital')}` : null;
    const tooltipPassSpecial = settingsPassword.specSymbols ? `${t('tooltipPasswordSpecial')}` : null;

    const inputEmail = emailNeeded 
      ? <EmailInput
          name="wizard-email"
          tabIndex={1}
          size="large"
          scale={true}
          placeholder={t('email')}
          emailSettings={settings}
          onValidateInput={this.onEmailHandler}
        />
      : null;

    const inputLicenseFile = isRequiredLicense 
      ? <Box className="input-file">
          <FileInput
            tabIndex={3}
            placeholder={t('placeholderLicense')}
            size="large"
            scale={true}
            onInput={this.onInputFileHandler}
          />
        </Box>
      : null; 

    return (
      <Box className="input-box">
        {inputEmail}
        <Box className="wizard-pass-box" >
        <PasswordInput
          ref={this.refPassInput}
          className="wizard-pass" 
          emailInputName="wizard-email"
          tabIndex={2}
          inputName="firstPass"
          size="large"
          scale={true}
          inputValue={this.state.password}
          passwordSettings={settingsPassword}
          isDisabled={false}
          placeholder={t('placeholderPass')}
          tooltipPasswordTitle={tooltipPassTitle}
          tooltipPasswordLength={tooltipPassLength}
          tooltipPasswordDigits={tooltipPassDigits}
          tooltipPasswordCapital={tooltipPassCapital}
          tooltipPasswordSpecial={tooltipPassSpecial}
          onChange={this.onChangePassword}
          onValidateInput={this.isValidPassHandler}
        />
        </Box>
        { inputLicenseFile }
        {!isRequiredLicense 
          ? <Link 
              className='generate-pass-link'
              type="action"
              fontWeight="normal"
              onClick={() => this.refPassInput.current.onGeneratePassword()}>
                {t('generatePassword')}
            </Link>
          : null
        }
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
            fontSize="13px"
            href="https://gnu.org/licenses/gpl-3.0.html" 
            isBold={false}
          >{t('licenseLink')}</Link>
        </Box>
      </Box>
    );
  }

  renderSettingsBox = () => {
    const { selectLanguage, selectTimezone, languages, timezones, emailNeeded, email } = this.state;
    const { t, machineName } = this.props;
    const fakeEmail = 'fake@mail.com';
    
    const titleEmail = !emailNeeded 
      ? <Text className="settings-title">{t('email')}</Text>
      : null
    
    const contentEmail = !emailNeeded 
      ? <Link 
          className="settings-value" 
          type="action" 
          fontSize="13px" 
          fontWeight="600" 
          onClick={this.onClickChangeEmail}
        >
          {email ? email : fakeEmail}
        </Link>
      : null

    return (
      <Box className="settings-box">
        <Box>
          <Text className="settings-title" fontSize="13px">{t('domain')}</Text>
          {titleEmail}
          <Text className="settings-title" fontSize="13px">{t('language')}</Text>
          <Text className="settings-title" fontSize="13px">{t('timezone')}</Text>
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
                  onClick={() => this.onSelectLanguageHandler(el)}
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
      <Box className="wizard-button">
        <Button
          size="large"
          scale={true}
          primary
          label={labelButton}           
          onClick={this.onContinueHandler}
          isDisabled={sendingComplete}
        />
      </Box>
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
        <form className="form-container">
          { modalDialog }
          { headerBox }
          { inputBox }
          { settingsBox }
          { buttonBox }
        </form>
      </WizardContainer>
    }
    return <Loader className="pageLoader" type="rombs" size='40px' />;
  }
}

Body.propTypes = {
  language: PropTypes.string,
  i18n: PropTypes.object,
  isOwner: PropTypes.bool,
  ownerEmail: PropTypes.string,
  isWizardLoaded: PropTypes.bool.isRequired,
  machineName: PropTypes.string.isRequired,
  isComplete: PropTypes.bool.isRequired,
  wizardToken: PropTypes.string,
  settingsPassword: PropTypes.object,
  cultures: PropTypes.array.isRequired,
  portalCulture: PropTypes.string.isRequired,
  timezones: PropTypes.array.isRequired,
  portalTimezone: PropTypes.string.isRequired
}

const WizardWrapper = withTranslation()(Body);

const WizardPage = props => {
  const { language, isLoaded } = props;

  i18n.changeLanguage(language);

  return (
    <>
      { isLoaded && <PageLayout 
          sectionBodyContent={<WizardWrapper i18n={i18n} {...props} />} 
          sectionHeaderContent={sectionHeaderContent}/>
      }
    </>
  );
}

WizardPage.propTypes = {
  language: PropTypes.string,
  isLoaded: PropTypes.bool
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
    
    isLoaded: state.auth.isLoaded,
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
})(withRouter(WizardPage)); 