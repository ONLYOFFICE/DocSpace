import React from 'react'
import { withRouter } from 'react-router'
import { connect } from 'react-redux'
import { Avatar, Button, Textarea, Text, toastr } from 'asc-web-components'
import { withTranslation } from 'react-i18next';
import { toEmployeeWrapper, getUserRole, profileEqual, updateProfile } from '../../../../../store/profile/actions';
import { MainContainer, AvatarContainer, MainFieldsContainer, TextField, PasswordField, DateField, RadioField, DepartmentField } from './userFormFields'

class UpdateUserForm extends React.Component {

  constructor(props) {
    super(props);

    this.state = this.mapPropsToState(props);

    this.validate = this.validate.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.onTextChange = this.onTextChange.bind(this);
    this.onBirthdayDateChange = this.onBirthdayDateChange.bind(this);
    this.onWorkFromDateChange = this.onWorkFromDateChange.bind(this);
    this.onGroupClose = this.onGroupClose.bind(this);
    this.onCancel = this.onCancel.bind(this);
  }

  componentDidUpdate(prevProps, prevState) {
    if (!profileEqual(this.props.profile, prevProps.profile)) {
      this.setState(this.mapPropsToState(this.props));
    }
  }

  mapPropsToState = (props) => {
    return {
      isLoading: false,
      errors: {
        firstName: false,
        lastName: false,
        email: false,
        password: false,
      },
      profile: { 
        ...{ passwordType: "link" },
        ...toEmployeeWrapper(props.profile)
      }
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

  validate() {
    const errors = {
      firstName: !this.state.profile.firstName,
      lastName: !this.state.profile.lastName,
      email: !this.state.profile.email,
      password: this.state.profile.passwordType === "temp" && !this.state.profile.password
    };
    const hasError = errors.firstName || errors.lastName || errors.email || errors.password;
    this.setState({errors: errors});
    return !hasError;
  }

  handleSubmit() {
    if(!this.validate())
      return false;

    this.setState({isLoading: true});

    this.props.updateProfile(this.state.profile)
      .then(() => {
        toastr.success("Success");
        this.props.history.goBack();
      })
      .catch((error) => {
        toastr.error(error.message)
        this.setState({isLoading: false})
      });
  }

  onCancel() {
    this.props.history.goBack();
  }

  render() {
    return (
      <>
        <MainContainer>
          <AvatarContainer>
            <Avatar
              size="max"
              role={getUserRole(this.state.profile)}
              source={this.state.profile.avatarMax}
              userName={this.state.profile.displayName}
              editing={true}
              editLabel={this.props.t("Resource:EditPhoto")}
            />
          </AvatarContainer>
          <MainFieldsContainer>
            <TextField
              isRequired={true}
              hasError={this.state.errors.firstName}
              labelText={`${this.props.t("Resource:FirstName")}:`}
              inputName="firstName"
              inputValue={this.state.profile.firstName}
              isDisabled={this.state.isLoading}
              onChange={this.onTextChange}
            />
            <TextField
              isRequired={true}
              hasError={this.state.errors.lastName}
              labelText={`${this.props.t("Resource:LastName")}:`}
              inputName="lastName"
              inputValue={this.state.profile.lastName}
              isDisabled={this.state.isLoading}
              onChange={this.onTextChange}
            />
            <TextField
              isRequired={true}
              hasError={this.state.errors.email}
              labelText={`${this.props.t("Resource:Email")}:`}
              inputName="email"
              inputValue={this.state.profile.email}
              isDisabled={this.state.isLoading}
              onChange={this.onTextChange}
            />
            <PasswordField
              isRequired={true}
              hasError={this.state.errors.password}
              labelText={`${this.props.t("Resource:Password")}:`}
              radioName="passwordType"
              radioValue={this.state.profile.passwordType}
              radioOptions={[
                { value: 'link', label: this.props.t("Resource:ActivationLink")},
                { value: 'temp', label: this.props.t("Resource:TemporaryPassword")}
              ]}
              radioIsDisabled={this.state.isLoading}
              radioOnChange={this.onTextChange}
              inputName="password"
              inputValue={this.state.profile.password}
              inputIsDisabled={this.state.isLoading || this.state.profile.passwordType === "link"}
              inputOnChange={this.onTextChange}
            />
            <DateField
              labelText={`${this.props.t("Resource:Birthdate")}:`}
              inputName="birthday"
              inputValue={this.state.profile.birthday ? new Date(this.state.profile.birthday) : undefined}
              inputIsDisabled={this.state.isLoading}
              inputOnChange={this.onBirthdayDateChange}
            />
            <RadioField
              labelText={`${this.props.t("Resource:Sex")}:`}
              radioName="sex"
              radioValue={this.state.profile.sex}
              radioOptions={[
                { value: 'male', label: this.props.t("Resource:SexMale")},
                { value: 'female', label: this.props.t("Resource:SexFemale")}
              ]}
              radioIsDisabled={this.state.isLoading}
              radioOnChange={this.onTextChange}
            />
            <DateField
              labelText={`${this.props.t("Resource:EmployedSinceDate")}:`}
              inputName="workFrom"
              inputValue={this.state.profile.workFrom ? new Date(this.state.profile.workFrom) : undefined}
              inputIsDisabled={this.state.isLoading}
              inputOnChange={this.onWorkFromDateChange}
            />
            <TextField
              labelText={`${this.props.t("Resource:Location")}:`}
              inputName="location"
              inputValue={this.state.profile.location}
              isDisabled={this.state.isLoading}
              onChange={this.onTextChange}
            />
            <TextField
              labelText={`${this.props.t("Resource:Position")}:`}
              inputName="title"
              inputValue={this.state.profile.title}
              isDisabled={this.state.isLoading}
              onChange={this.onTextChange}
            />
            <DepartmentField
              labelText={`${this.props.t("Resource:Departments")}:`}
              departments={this.state.profile.groups}
              onClose={this.onGroupClose}
            />
          </MainFieldsContainer>
        </MainContainer>
        <div>
          <Text.ContentHeader>{this.props.t("Resource:Comments")}</Text.ContentHeader>
          <Textarea name="notes" value={this.state.profile.notes} isDisabled={this.state.isLoading} onChange={this.onTextChange}/> 
        </div>
        <div style={{marginTop: "60px"}}>
          <Button label={this.props.t("UserControlsCommonResource:SaveButton")} onClick={this.handleSubmit} primary isDisabled={this.state.isLoading} size="big"/>
          <Button label={this.props.t("UserControlsCommonResource:CancelButton")} onClick={this.onCancel} isDisabled={this.state.isLoading} size="big" style={{ marginLeft: "8px" }}/>
        </div>
      </>
    );
  };
}

const mapStateToProps = (state) => {
  return {
    profile: state.profile.targetUser
  }
};

export default connect(
  mapStateToProps,
  {
    updateProfile
  }
)(withRouter(withTranslation()(UpdateUserForm)));