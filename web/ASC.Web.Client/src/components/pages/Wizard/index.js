import React, { Component } from 'react';
import { withRouter } from 'react-router';
import styled from "styled-components";

import { PageLayout } from "asc-web-common";
import { 
  Heading, Text, 
  EmailInput, PasswordInput, 
  InputBlock, Checkbox, Link,
  DropDown, GroupButton, 
  DropDownItem, Button, 
  Box, Loader, ModalDialog,
  utils } from 'asc-web-components';

const { EmailSettings } = utils.email;
const settings = new EmailSettings();
settings.allowDomainPunycode = true;

const settingsPassword = {
  minLength: 6,
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
      font-size: 16px;
      line-height: 22px;
      padding-left: 15px;
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
`;

class Body extends Component {
  constructor(props) {
    super(props);

    this.state = {
      password: '',
      isValidPass: false,
      isOwner: true,
      errorLoading: false
    }

    this.inputRef = React.createRef();
  }

  isValidPassHandler = (val) => {
    this.state({ isValidPass: val});
  }

  onIconFileClick = () => {
    console.log('click')
    this.inputRef.current.click();
    console.log(this.inputRef.current.value);
  }

  onClickChangeEmail = () => {
    this.renderModal();
  }

  renderModal = () => {
    const { isOwner, errorLoading } = this.state;

    if( !isOwner ) {
      const header = "Change e-mail"
      const content = "111";

      return <ModalDialog
        visible={false}
        scale={false}
        displayType="auto"
        zIndex={310}
        headerContent={header}
        bodyContent={content}
      />
    } else {
      return null
    }
  }

  renderHeaderBox = () => {
    return (
      <Box className="header-box">
        <Heading level={1} title="Wizard" className="wizard-title">
          Welcome to your portal!
        </Heading>
        <Text className="wizard-desc">
          Please setup the portal registration data.
        </Text>
      </Box>
    )
  }

  renderInputBox = () => {
    const { isOwner } = this.state;

    const inputEmail = isOwner 
      ? <EmailInput
          className="wizard-input-email"
          tabIndex={1}
          id="input-email"
          name="email-wizard"
          placeholder={'E-mail'}
          emailSettings={settings}
          onValidateInput={() => console.log('email')}
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
          placeholder={'Password'}
          onChange={() => console.log('pass onChange')}
          onValidateInput={this.isValidPassHandler}
        />
        <Text className="password-tooltip">
          2-30 characters
        </Text>
        <InputBlock
          className="input-block"
          iconName={"CatalogFolderIcon"}
          onIconClick={this.onIconFileClick}
          onChange={() => console.log('change')}
        >
          <input type="file" className="input-file" ref={this.inputRef}/>
        </InputBlock>
        <Box className="checkbox-container">
          <Checkbox
            className="wizard-checkbox"
            id="license"
            name="confirm"
            label={'Accept the terms of the'}
            isChecked={false}
            isIndeterminate={false}
            isDisabled={false}
            onChange={() => {}}
          />
          <Link 
            className="link"
            type="page" 
            color="#116d9d" 
            href="https://gnu.org/licenses/gpl-3.0.html" 
            isBold={false}
          >License agreements</Link>
        </Box>
      </Box>
    );
  }

  renderSettingsBox = () => {
    const { isOwner } = this.state;

    const titleEmail = !isOwner 
      ? <Text className="settings-title">Email</Text>
      : null
    
    const email = !isOwner 
      ? <Link className="link value" type="action" onClick={this.onClickChangeEmail}>portaldomainname@mail.com</Link>
      : null

    return (
      <Box className="settings-box">
        <Box className="setting-title-block">
          <Text className="settings-title">Domain:</Text>
          {titleEmail}
          <Text className="settings-title">Language:</Text>
          <Text className="settings-title">Time zone:</Text>
        </Box>
        <Box className="values">
          <Text className="text value">portaldomainname.com</Text>
          {email}
          <GroupButton className="drop-down value" label="English (United States)" isDropdown={true}>
            <DropDownItem 
              label="English (United States)"
              onClick={() => console.log('English click')}
            />
            <DropDownItem 
              label="Русский (Российская Федерация)"
              onClick={() => console.log('Russia click')}
            />
          </GroupButton>
          <GroupButton className="drop-down value" label="UTC" isDropdown={true}>
            <DropDownItem 
              label="UTC"
              onClick={() => console.log('UTC')}
            />
            <DropDownItem 
              label="Not UTC"
              onClick={() => console.log('Not UTC')}
            />
          </GroupButton>
        </Box>
      </Box>
    );
  }

  renderButtonBox = () => {
    return (
      <Button
        className="wizard-button"
        primary
        label={"Continue"}           
        size="big"
        onClick={() => console.log('click btn')}
      />
    );
  }

  render() {
    const headerBox = this.renderHeaderBox();
    const inputBox = this.renderInputBox();
    const settingsBox = this.renderSettingsBox();
    const buttonBox = this.renderButtonBox();

    return (
      <WizardContainer>
        <Box className="form-container">
          {headerBox}
          {inputBox}
          {settingsBox}
          {buttonBox}
        </Box>
      </WizardContainer>
    )
  }
}

const Wizard = props => <PageLayout 
  sectionBodyContent={<Body {...props} />} 
  sectionHeaderContent={sectionHeaderContent}
/>;

export default withRouter(Wizard);