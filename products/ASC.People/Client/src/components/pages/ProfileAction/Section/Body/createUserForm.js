import React from 'react'
import { withRouter } from 'react-router'
import { connect } from 'react-redux'
import { Avatar, Button, Textarea, Text, toastr } from 'asc-web-components'
import { withTranslation } from 'react-i18next';
import { toEmployeeWrapper, getUserRole, createProfile } from '../../../../../store/profile/actions';
import { MainContainer, AvatarContainer, MainFieldsContainer } from './FormFields/Form'
import TextField from './FormFields/TextField'
import PasswordField from './FormFields/PasswordField'
import DateField from './FormFields/DateField'
import RadioField from './FormFields/RadioField'
import DepartmentField from './FormFields/DepartmentField'
import { department, position, employedSinceDate } from '../../../../../helpers/customNames';

class CreateUserForm extends React.Component {

  constructor(props) {
    super(props);

    this.state = this.mapPropsToState(props);

    this.validate = this.validate.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.onTextChange = this.onTextChange.bind(this);
    this.onBirthdayDateChange = this.onBirthdayDateChange.bind(this);
    this.onWorkFromDateChange = this.onWorkFromDateChange.bind(this);
    this.onGroupClose = this.onGroupClose.bind(this);
    this.onShowPassword = this.onShowPassword.bind(this);
    this.onCancel = this.onCancel.bind(this);
  }

  componentDidUpdate(prevProps, prevState) {
    if (this.props.match.params.type !== prevProps.match.params.type) {
      this.setState(this.mapPropsToState(this.props));
    }
  }

  mapPropsToState = (props) => {
    return {
      isLoading: false,
      showPassword: false,
      errors: {
        firstName: false,
        lastName: false,
        email: false,
        password: false,
      },
      profile: toEmployeeWrapper({ 
        isVisitor: props.match.params.type === "guest",
        passwordType: "link"
      })
    };
  }

  onTextChange(event) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile[event.target.name] = event.target.value;
    this.setState(stateCopy)
  }

  onBirthdayDateChange(value) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.birthday = value ? value.toJSON() : null;
    this.setState(stateCopy)
  }

  onWorkFromDateChange(value) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.workFrom = value ? value.toJSON() : null;
    this.setState(stateCopy)
  }

  onGroupClose(id) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.groups = this.state.groups.filter((group) => group.id !== id);
    this.setState(stateCopy)
  }

  onShowPassword() {
    this.setState({showPassword: !this.state.showPassword});
  }

  validate() {
    const { profile } = this.state;
    const emailRegex = /.+@.+\..+/;
    const errors = {
      firstName: !profile.firstName,
      lastName: !profile.lastName,
      email: !emailRegex.test(profile.email),
      password: profile.passwordType === "temp" && !profile.password
    };
    const hasError = errors.firstName || errors.lastName || errors.email || errors.password;
    this.setState({errors: errors});
    return !hasError;
  }

  handleSubmit() {
    if(!this.validate())
      return false;

    this.setState({isLoading: true});

    this.props.createProfile(this.state.profile)
      .then((profile) => {
        toastr.success("Success");
        this.props.history.push(`${this.props.settings.homepage}/view/${profile.userName}`);
      })
      .catch((error) => {
        toastr.error(error.message)
        this.setState({isLoading: false})
      });
  }

  onCancel() {
    this.props.history.push(this.props.settings.homepage)
  }

  render() {
    const { isLoading, showPassword, errors, profile } = this.state;
    const { t } = this.props;

    return (
      <>
        <MainContainer>
          <AvatarContainer>
            <Avatar
              size="max"
              role={getUserRole(profile)}
              editing={true}
              editLabel={t("AddPhoto")}
            />
          </AvatarContainer>
          <MainFieldsContainer>
            <TextField
              isRequired={true}
              hasError={errors.firstName}
              labelText={`${t("FirstName")}:`}
              inputName="firstName"
              inputValue={profile.firstName}
              inputIsDisabled={isLoading}
              inputOnChange={this.onTextChange}
              inputAutoFocussed={true}
              inputTabIndex={1}
            />
            <TextField
              isRequired={true}
              hasError={errors.lastName}
              labelText={`${t("LastName")}:`}
              inputName="lastName"
              inputValue={profile.lastName}
              inputIsDisabled={isLoading}
              inputOnChange={this.onTextChange}
              inputTabIndex={2}
            />
            <TextField
              isRequired={true}
              hasError={errors.email}
              labelText={`${t("Email")}:`}
              inputName="email"
              inputValue={profile.email}
              inputIsDisabled={isLoading}
              inputOnChange={this.onTextChange}
              inputTabIndex={3}
            />
            <PasswordField
              isRequired={true}
              hasError={errors.password}
              labelText={`${t("Password")}:`}
              radioName="passwordType"
              radioValue={profile.passwordType}
              radioOptions={[
                { value: 'link', label: t("ActivationLink")},
                { value: 'temp', label: t("TemporaryPassword")}
              ]}
              radioIsDisabled={isLoading}
              radioOnChange={this.onTextChange}
              inputName="password"
              inputValue={profile.password}
              inputIsDisabled={isLoading || profile.passwordType === "link"}
              inputOnChange={this.onTextChange}
              inputIconOnClick={this.onShowPassword}
              inputShowPassword={showPassword}
              refreshIconOnClick={()=>{}}
              copyLinkText={t("CopyEmailAndPassword")}
              copyLinkOnClick={()=>{}}
              inputTabIndex={4}
            />
            <DateField
              labelText={`${t("Birthdate")}:`}
              inputName="birthday"
              inputValue={profile.birthday ? new Date(profile.birthday) : undefined}
              inputIsDisabled={isLoading}
              inputOnChange={this.onBirthdayDateChange}
              inputTabIndex={5}
            />
            <RadioField
              labelText={`${t("Sex")}:`}
              radioName="sex"
              radioValue={profile.sex}
              radioOptions={[
                { value: 'male', label: t("SexMale")},
                { value: 'female', label: t("SexFemale")}
              ]}
              radioIsDisabled={isLoading}
              radioOnChange={this.onTextChange}
            />
            <DateField
              labelText={`${t("CustomEmployedSinceDate", { employedSinceDate })}:`}
              inputName="workFrom"
              inputValue={profile.workFrom ? new Date(profile.workFrom) : undefined}
              inputIsDisabled={isLoading}
              inputOnChange={this.onWorkFromDateChange}
              inputTabIndex={6}
            />
            <TextField
              labelText={`${t("Location")}:`}
              inputName="location"
              inputValue={profile.location}
              inputIsDisabled={isLoading}
              inputOnChange={this.onTextChange}
              inputTabIndex={7}
            />
            <TextField
              labelText={`${t("CustomPosition", { position })}:`}
              inputName="title"
              inputValue={profile.title}
              inputIsDisabled={isLoading}
              inputOnChange={this.onTextChange}
              inputTabIndex={8}
            />
            <DepartmentField
              labelText={`${t("CustomDepartment", { department })}:`}
              departments={profile.groups}
              onRemoveDepartment={this.onGroupClose}
            />
          </MainFieldsContainer>
        </MainContainer>
        <div>
          <Text.ContentHeader>{t("Comments")}</Text.ContentHeader>
          <Textarea name="notes" value={profile.notes} isDisabled={isLoading} onChange={this.onTextChange} tabIndex={9}/> 
        </div>
        <div style={{marginTop: "60px"}}>
          <Button label={t("SaveButton")} onClick={this.handleSubmit} primary isDisabled={isLoading} size="big" tabIndex={10}/>
          <Button label={t("CancelButton")} onClick={this.onCancel} isDisabled={isLoading} size="big" style={{ marginLeft: "8px" }} tabIndex={11}/>
        </div>
      </>
    );
  };
}

const mapStateToProps = (state) => {
  return {
    settings: state.auth.settings
  }
};

export default connect(
  mapStateToProps,
  {
    createProfile
  }
)(withRouter(withTranslation()(CreateUserForm)));